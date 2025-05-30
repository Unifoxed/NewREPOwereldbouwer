using System;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;



public class ApiClient : MonoBehaviour
{
    public TMP_InputField email;
    public TMP_InputField password;
    private string errorMessage;
    public TMP_Text errorText;
    private async Task<string> PerformApiCall(string url, string method, string jsonData = null, string token = null)
    {
        errorMessage = "";
        using (UnityWebRequest request = new UnityWebRequest(url, method))
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }

            await request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError("Fout bij API-aanroep: " + request.error);
                errorMessage = request.error;
                return null;
            }
        }
    }
    public async void Register()
    {
        errorText.text = "";
        var registerDto = new PostRegisterRequestDto()
        {
            email = email.text,
            password = password.text
        };
        string json = JsonUtility.ToJson(registerDto);
        var response = await PerformApiCall("https://avansict2207628.azurewebsites.net/account/register", "POST", json);
        if(errorMessage.Contains("Bad Request"))
        {
            errorText.text = "Email is al in gebruik";
        }
        Debug.Log(response);
    }
    public async void Login()
    {
        errorText.text = "";
        var loginDto = new PostLoginRequestDto()
        {
            email = email.text,
            password = password.text
        };
        string json = JsonUtility.ToJson(loginDto);
        var response = await PerformApiCall("https://avansict2207628.azurewebsites.net/account/login", "POST", json); 
        if (errorMessage.Contains("Unauthorized"))
        {
            errorText.text = "Wachtwoord is verkeerd";
        }
        var responseDto = JsonUtility.FromJson<PostLoginResponseDto>(response);
        var userIdResponse = await PerformApiCall("https://avansict2207628.azurewebsites.net/wereldbouwer/getuserid", "GET", null, responseDto.accessToken);
        Debug.Log(response);
        varStorage.accessToken = responseDto.accessToken;
        varStorage.userId = userIdResponse;
    }
}

