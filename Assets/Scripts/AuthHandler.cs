using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

public class AuthHandler : MonoBehaviour
{
    private string url = "https://sid-restapi.onrender.com";
    [SerializeField]
    private string Token;
    private string Username;

    [SerializeField] 
    private TMP_Text _text;    
    [SerializeField]
    GameObject authPanel;
    [SerializeField]
    private GameObject Manager;
    private void Start()
    {
       Token = PlayerPrefs.GetString("token");
       Username = PlayerPrefs.GetString("username");
       
      

       if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Username))
       {
           Debug.Log("No hay token");
       }
       else
       {
           StartCoroutine("GetProfile");
       }
    }

  

    public void Login()
    {
        Credentials credentials = new Credentials();
        credentials.username = GameObject.Find("InputField Username")
            .GetComponent<TMP_InputField>().text;
        credentials.password = GameObject.Find("InputField Password")
            .GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(credentials);
        StartCoroutine(LoginPost(postData));

    }

    public void Register()
    {
        Credentials credentials = new Credentials();
        credentials.username = GameObject.Find("InputField Username")
            .GetComponent<TMP_InputField>().text;
        credentials.password = GameObject.Find("InputField Password")
            .GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(credentials);
        StartCoroutine(RegisterPost(postData));
    }

    public void Logout()
    {   
        Token = "";
        Username = "";
        
        authPanel.SetActive(true);
        Token = GameObject.Find("InputField Username")
            .GetComponent<TMP_InputField>().text;
        Username =GameObject.Find("InputField Password")
            .GetComponent<TMP_InputField>().text;
        _text.text = "Bienvenid@ a Sid";
        
        
    }
   IEnumerator RegisterPost(string postData)
 {
     string path = "/api/usuarios";
     UnityWebRequest www = UnityWebRequest.Put(url+path, postData);
     www.method = "POST";
     www.SetRequestHeader("Content-Type", "application/json");
     yield return www.SendWebRequest();

     if (www.result == UnityWebRequest.Result.ConnectionError)
     {
         Debug.Log(www.error);
     }
     else
     {
         if (www.responseCode == 200)
         {
             Debug.Log(www.downloadHandler.text);
             StartCoroutine(LoginPost(postData));
         }
            
         else 
         {
             string mensaje = "status:" + www.responseCode;
             mensaje += "\nError: " + www.downloadHandler.text;
             Debug.Log(mensaje);
         }
     }
 }

 IEnumerator LoginPost(string postData)
 {
     string path = "/api/auth/login";
     UnityWebRequest www = UnityWebRequest.Put(url+path, postData);
     www.method = "POST";
     www.SetRequestHeader("Content-Type", "application/json");
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

             AuthResponse response = JsonUtility.FromJson<AuthResponse>(json);


             GameObject.Find("PanelAuth").SetActive(false);
             _text.text = "Bienvenid@ "+Username+" puntua para ser el 1er lugar";


             PlayerPrefs.SetString("token", response.token);
             PlayerPrefs.SetString("username", response.usuario.username);  
             StartCoroutine("GetUsers");

         }
         else
         {
             string mensaje = "status:" + www.responseCode;
             mensaje += "\nError: " + www.downloadHandler.text;
             Debug.Log(mensaje);
         }
     }
 }

 IEnumerator GetProfile()
 {
     string path = "/api/usuarios"+Username;
     UnityWebRequest www = UnityWebRequest.Get(url + path);
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
             AuthResponse response = JsonUtility.FromJson<AuthResponse>(json);
             GameObject.Find("PanelAuth").SetActive(false);
             StartCoroutine("GetUsers");
         }
         else
         {
             Debug.Log("Token Vencido... redirecionar a Login");
         }
     }
 }
 IEnumerator GetUsers()
 {
     string path = "/api/usuarios";
     UnityWebRequest www = UnityWebRequest.Get(url + path);
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
             UsersList response = JsonUtility.FromJson<UsersList>(json);

             UserModel[] leaderboard = response.usuarios
                 .OrderByDescending(u => u.data.score)
                 .Take(3).ToArray();
             foreach (var user in response.usuarios)
             {
                 Debug.Log(user.username +"|"+user.data.score);
             }
         }
         else
         {
             Debug.Log("Token Vencido... redirecionar a Login");
         }
     }
 }
}

public class Credentials
{
    public string username;
    public string password; 
}
[System.Serializable]
public class AuthResponse
{
    public UserModel usuario;
    public string token;
    
}
[System.Serializable]
public class UserModel
{
    public int _id;
    public string username;
    public bool estado;
    public DataUser data;
}
[System.Serializable]
public class UsersList
{
    public UserModel[] usuarios;
}
[System.Serializable]
public class DataUser
{
    public int score;
}
