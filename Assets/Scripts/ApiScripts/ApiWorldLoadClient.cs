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
using System;

public class ApiWorldLoadClient : MonoBehaviour
{
    public List<GameObject> prefabs; // Lijst van beschikbare prefabs
    public List<Object2DData> instantiatedObject2DData = new List<Object2DData>();
    public static ApiWorldLoadClient instance { get; private set; }
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
    }

    public async void SaveWorld()
    {
        DeleteDatabaseObjects();
        instantiatedObject2DData.Clear();
        GameObject[] instantiatedObjects = GameObject.FindGameObjectsWithTag("Instantiated");
        List<Object2DData> currentData = new List<Object2DData>();

        // Haal de huidige data op van de API.
        string url = $"https://avansict2207628.azurewebsites.net/Object2D/{varStorage.wereldId}";
        string response = await PerformApiCall(url, "GET", null, varStorage.accessToken);

        if (response != null)
        {
            currentData = JsonConvert.DeserializeObject<List<Object2DData>>(response);
        }

        foreach (GameObject obj in instantiatedObjects)
        {
            string prefabName = obj.name.Replace("(Clone)", "");
            if (prefabName != null) // Controleer of de prefab is gevonden
            {
                Object2DData newData = new Object2DData
                {
                    id = "<string>",
                    environmentId = varStorage.wereldId,
                    prefabId = prefabName,
                    positionX = obj.transform.position.x,
                    positionY = obj.transform.position.y,
                    scaleX = obj.transform.localScale.x,
                    scaleY = obj.transform.localScale.y,
                    rotationZ = obj.transform.rotation.eulerAngles.z,
                    sortingLayer = obj.GetComponent<SpriteRenderer>().sortingLayerID
                };
                
                    // Voeg nieuwe data toe.
                    string postJsonData = JsonConvert.SerializeObject(newData, Formatting.Indented);
                    string postUrl = "https://avansict2207628.azurewebsites.net/Object2D";
                    string postResponse = await PerformApiCall(postUrl, "POST", postJsonData, varStorage.accessToken);

                    if (postResponse != null)
                    {
                        Debug.Log("Object data saved to API: " + postResponse);
                    }
                    else
                    {
                        Debug.LogError("Failed to save Object data to API.");
                    }
            }
        }
    }

    public async void LoadWorld()
    {
        string url = $"https://avansict2207628.azurewebsites.net/Object2D/{varStorage.wereldId}";
        string response = await PerformApiCall(url, "GET", null, varStorage.accessToken);

        GameObject[] instantiatedObjects = GameObject.FindGameObjectsWithTag("Instantiated");
        foreach (GameObject prefabs in instantiatedObjects)
        {
            Destroy(prefabs);
        }

        if (response != null)
        {
            List<Object2DData> loadedData = JsonConvert.DeserializeObject<List<Object2DData>>(response);
            if (loadedData != null)
            {
                foreach (Object2DData data in loadedData)
                {
                    // Probeer de prefabnaam (string) om te zetten naar de enum PrefabNames
                    if (Enum.TryParse(data.prefabId, out PrefabIdToName prefabName))
                    {
                        // Zoek de index van de prefab in de lijst 'prefabs'
                        int prefabIndex = (int)prefabName;

                        if (prefabIndex >= 0 && prefabIndex < prefabs.Count)
                        {
                            GameObject prefab = prefabs[prefabIndex]; // Haal de prefab op uit de lijst
                            if (prefab != null)
                            {
                                GameObject instantiatedObject = Instantiate(prefab, new Vector3(data.positionX, data.positionY, 0), Quaternion.Euler(0, 0, data.rotationZ));
                                instantiatedObject.transform.localScale = new Vector3(data.scaleX, data.scaleY, 1);
                                instantiatedObject.GetComponent<SpriteRenderer>().sortingLayerID = data.sortingLayer;
                                instantiatedObject.tag = "Instantiated";
                                MenuPanel.items.Add(instantiatedObject);

                                // Stel de MenuPanel referentie in
                                DragDrop dragDrop = instantiatedObject.GetComponent<DragDrop>();
                                if (dragDrop != null)
                                {
                                    dragDrop.menuPanel = GameObject.Find("Panel").GetComponent<MenuPanel>(); // Zoek het MenuPanel component op LeftPanel
                                }
                            }

                        }

                    }

                }
            }
        }

    }

    public async void DeleteDatabaseObjects()
    {
        GameObject[] instantiatedObjects = GameObject.FindGameObjectsWithTag("Instantiated");

        string url = $"https://avansict2207628.azurewebsites.net/Object2D/environment/{varStorage.wereldId}";
        string response = await PerformApiCall(url, "DELETE", null, varStorage.accessToken);

        Debug.Log(response);
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
}