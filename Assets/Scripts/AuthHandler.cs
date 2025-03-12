using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class AuthHandler : MonoBehaviour
{
    private string url = "https://sid-restapi.onrender.com";
    private string Token;
    private string Username;

    [SerializeField]
    TMP_Text[] score = new TMP_Text[3];
    
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
        Username = "";
        Token = "";
        authPanel.SetActive(true);
        _text.text = "Bienvenid@ a Sid";
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.Save(); // Guarda los cambios en disco
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
             

             PlayerPrefs.SetString("token", response.token);
             PlayerPrefs.SetString("username", response.usuario.username);
             StartCoroutine("GetUsers");
             _text.text = "Bienvenid@ "+response.usuario.username+", puntua para ser el 1er lugar";

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
         }
         else
         {
             Debug.Log("Token Vencido... redirecionar a Login");
         }
     }
 }
 public void GetScoreboard()
 {
     StartCoroutine("GetUsers");
 }
 IEnumerator GetUsers()
 {
     Token = PlayerPrefs.GetString("token");
     Username = PlayerPrefs.GetString("username");
     
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
             int index = 0;
             foreach (var user in leaderboard)
             {
               
                 Debug.Log(user.username +" | "+user.data.score);
                 score[index].text = index+1 + ". " + user.username + "     " + user.data.score;
                 index++;    
             }
         }
         else
         {
             Debug.Log("Token Vencido... redirecionar a Login");
         }
     }
 }
}

[System.Serializable]
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
    public DataUser data = new DataUser();
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
