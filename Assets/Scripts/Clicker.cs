using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class Clicker : MonoBehaviour
{
    private string url = "https://sid-restapi.onrender.com";
    private string Token;
    
    [SerializeField] private TMP_Text textScore;
    public TMP_Text maintext;
    public TMP_Text timerText;

    public GameObject btonVolver;
    public GameObject botonContador; // Referencia al botón
    
    private bool isGameOver = false;
    
    private float tiempo = 10f;
    
    private int score = 0;
    private int suma = 1;
    
    [SerializeField]
    AuthHandler ath;

    public void Again()
    {
        isGameOver = false;
        tiempo = 10f;
        score = 0;

    }
    public void AddClicks()
    {
        score+=suma;
        textScore.text = "score: "+score.ToString();
        
        UserModel credentials = new UserModel(); // Se instancia una sola vez
        credentials.username = PlayerPrefs.GetString("username");
        credentials.data.score = score;
        
        string postData = JsonUtility.ToJson(credentials);
        StartCoroutine("PatchUser", postData);
    }
    IEnumerator PatchUser(string postData)  
    {
        Token = PlayerPrefs.GetString("token");
        
        string path = "/api/usuarios";
        
        UnityWebRequest www = UnityWebRequest.Put(url + path, postData);
        www.method = "PATCH";
        www.SetRequestHeader("Content-Type", "application/json");
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
            }
            else
            {
                string mensaje = "status:"+www.responseCode;
                mensaje += "\nError: " + www.downloadHandler.text;
                Debug.Log(mensaje);
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
        timerText.text = tiempo.ToString("F1");
        textScore.text = "score: "+score.ToString();

        btonVolver.SetActive(true);
        maintext.GameObject().SetActive(true);
        // Guarda la puntuación sin crear un nuevo objeto en cada frame

        if (botonContador != null) botonContador.SetActive(false); // Desactiva el botón sin GameObject.Find

        if (maintext != null) maintext.text = "Se acabó el tiempo!";

    }

    public void ChangeMainText()
    {
        maintext.text = "Bienvenid@ de nuevo";
    }
}