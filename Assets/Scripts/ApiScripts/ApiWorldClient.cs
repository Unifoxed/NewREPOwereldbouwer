using System.Text;
using System.Threading.Tasks;
using Assets.Scripts;
using Assets.Scripts.DTO_Model;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ApiWorldClient : MonoBehaviour
{
    public TMP_InputField NameInput;
    public TMP_Text nameText;
    public GameObject worldPrefab; // Reference to the prefab
    public RectTransform worldContainer; // Reference to the container\
    public Button createButton; // Reference to the create button\
    public TMP_Text errorText;

    public static ApiWorldClient instance { get; private set; }
    void Start()
    {
        LoadWorld();
        // hier controleren we of er al een instantie is van deze singleton
        // als dit zo is dan hoeven we geen nieuwe aan te maken en verwijderen we deze
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }
    private async Task<string> PerformApiCall(string url, string method, string jsonData = null, string token = null)
    {
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
                return null;
            }
        }
    }
    public async void CreateWorld(WorldPrefabController worldPrefabController)
    {
        if (WorldNameRequirements(worldPrefabController))
        { 
        var WorldCreateRequestDto = new PostWorldCreateRequestDto()
        {
            maxHeight = 200,
            maxLength = 200,
            name = worldPrefabController.NameInput.text,
            ownerUserId = varStorage.userId,
            id = "3b6e0048-1f52-40a7-a049-5a467eba0f33" // placeholder id
        };
        string json = JsonUtility.ToJson(WorldCreateRequestDto);
        var response = await PerformApiCall("https://avansict2207628.azurewebsites.net/wereldbouwer", "POST", json, varStorage.accessToken);
        Debug.Log(response);
        LoadWorld();
    }
    }

    public async void LoadWorld()
    {
        var response = await PerformApiCall($"https://avansict2207628.azurewebsites.net/wereldbouwer/getwereld/{varStorage.userId}", "GET", null, varStorage.accessToken);
        if (varStorage.accessToken != null)
        {
            foreach (Transform child in worldContainer)
            {
                Destroy(child.gameObject);
            }
            List<PostWorldCreateRequestDto> worlds = JsonConvert.DeserializeObject<List<PostWorldCreateRequestDto>>(response);

            if (worlds != null)
            {
                int worldCount = worlds.Count;
                for (int i = 0; i < worldCount && i < 5; i++)
                {
                    var world = worlds[i];
                    CreateWorldPrefab(world);
                }

                for (int i = worldCount; i < 5; i++)
                {
                    CreateEmptyWorldPrefab();
                }
            }
            else
            {
                Debug.LogError("Failed to load worlds or no worlds found.");
            }
        }
    }
    public async Task DeleteDatabaseObjects(string worldId)
    {
        GameObject[] instantiatedObjects = GameObject.FindGameObjectsWithTag("Instantiated");

        string url = $"https://avansict2207628.azurewebsites.net/Object2D/environment/{worldId}";
        string response = await PerformApiCall(url, "DELETE", null, varStorage.accessToken);

        Debug.Log(response);
    }
    public async void DeleteWorld(string worldId)
    {
        await DeleteDatabaseObjects(worldId);
        var response = await PerformApiCall($"https://avansict2207628.azurewebsites.net/wereldbouwer/{worldId}", "DELETE", null, varStorage.accessToken);
        Debug.Log(response);
        LoadWorld();
    }

    public bool WorldNameRequirements(WorldPrefabController worldPrefabController)
    {
        errorText.text = "";
        string worldName = worldPrefabController.NameInput.text;
        if (string.IsNullOrEmpty(worldName))
        {
            Debug.Log("De wereldnaam mag niet leeg zijn.");
            errorText.text += "De wereldnaam mag niet leeg zijn.\n";
        }
        if (worldName.Length < 1 || worldName.Length > 25)
        {
            Debug.Log("De wereldnaam moet tussen de 1 en 25 tekens zijn.");
            errorText.text += "De wereldnaam moet tussen de 1 en 25 tekens zijn.\n";
        }
        if (!System.Text.RegularExpressions.Regex.IsMatch(worldName, @"^[a-zA-Z0-9_]+$"))
        {
            Debug.Log("De wereldnaam mag alleen letters, cijfers en underscores bevatten.");
            errorText.text += "De wereldnaam mag alleen letters, cijfers en underscores bevatten.\n";
        }

        if (errorText.text != "")
        {
            return false;
        }
        return true;
    }

    public async void LoadSpecificWorld(string worldId)
    {
        varStorage.wereldId = worldId;
        SceneManager.LoadScene("World");
    }

    private void CreateWorldPrefab(PostWorldCreateRequestDto world)
    {
        GameObject worldObject = Instantiate(worldPrefab, worldContainer);
        WorldPrefabController controller = worldObject.GetComponent<WorldPrefabController>();
        controller.Initialize(this, world);
    }

    private void CreateEmptyWorldPrefab()
    {
        GameObject worldObject = Instantiate(worldPrefab, worldContainer);
        WorldPrefabController controller = worldObject.GetComponent<WorldPrefabController>();
        controller.Initialize(this);
    }
}
