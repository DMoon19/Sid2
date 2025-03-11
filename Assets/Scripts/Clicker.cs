using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Clicker : MonoBehaviour
{
    private string url = "https://sid-restapi.onrender.com";
    string Token;
    
    public TMP_Text text;
    public TMP_Text maintext;
    public TMP_Text timerText;
    public GameObject botonContador; // Referencia al botón
    private int totalClicks = 0;
    private float tiempo = 10f;
    private bool isGameOver = false;
    private UserModel user; // Crear una sola vez

    [SerializeField]
    AuthHandler ath;

    public void AddClicks()
    {
        totalClicks++;
        text.text = totalClicks.ToString();
        UserModel credentials = new UserModel(); // Se instancia una sola vez
        credentials.username = PlayerPrefs.GetString("username");
        credentials.data.score = PlayerPrefs.GetInt("score");
        
        string postData = JsonUtility.ToJson(credentials);
        StartCoroutine("PatchUser", postData);
    }
    IEnumerator PatchUser(string postData)
    {
        string path = "/api/usuarios";
        Token = PlayerPrefs.GetString("Token");
        UnityWebRequest www = UnityWebRequest.Get(url + path);
        www.method = "PATCH";
        www.SetRequestHeader("x-token", Token);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                string json = www.downloadHandler.text;
                UserModel response = JsonUtility.FromJson<UserModel>(json);
                StartCoroutine("GetUsers");
            }
            else
            {
                Debug.Log("Token Vencido... redirecionar a Login");
            }
        }
    }

    private void Start()
    {
        if (timerText != null)
            timerText.text = tiempo.ToString("F1");
    }

    private void Update()
    {
        if (isGameOver) return; // Evita ejecutar código innecesario

        tiempo -= Time.deltaTime;

        if (tiempo > 0)
        {
            // Solo actualiza el texto si cambia el valor visualmente
            float tiempoMostrado = Mathf.Ceil(tiempo * 10) / 10; // Redondeo a 1 decimal
            if (timerText != null && timerText.text != tiempoMostrado.ToString("F1"))
                timerText.text = tiempoMostrado.ToString("F1");
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        tiempo = 0;

        // Guarda la puntuación sin crear un nuevo objeto en cada frame
        user.data.score = totalClicks;
        Debug.Log(user.data.score);

        if (botonContador != null) botonContador.SetActive(false); // Desactiva el botón sin GameObject.Find

        if (maintext != null) maintext.text = "Se acabó el tiempo!";

        Debug.Log("¡Tiempo agotado! Fin del juego.");
    }
}