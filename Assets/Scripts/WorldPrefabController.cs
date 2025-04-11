using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Assets.Scripts.DTO_Model;

public class WorldPrefabController : MonoBehaviour
{
    public ApiWorldClient apiManager;
    public TMP_InputField NameInput;
    public TMP_Text worldName;
    public Button loadWorldButton;
    public Button createWorldButton;
    public Button deleteWorldButton;

    public void Initialize(ApiWorldClient manager, PostWorldCreateRequestDto world = null)
    {
        apiManager = manager;

        if (world != null)
        {
            NameInput.text = world.name;
            worldName.text = world.name;

            // Stel de onClick listener in voor de Load World button
            loadWorldButton.onClick.AddListener(() => apiManager.LoadSpecificWorld(world.id));
            deleteWorldButton.onClick.AddListener(() => apiManager.DeleteWorld(world.id));

            createWorldButton.gameObject.SetActive(false);
            NameInput.gameObject.SetActive(false);
        }
        else
        {
            NameInput.text = "";
            worldName.text = "Create New World";

            // Stel de onClick listener in voor de Create World button
            createWorldButton.onClick.AddListener(() => apiManager.CreateWorld(this));

            loadWorldButton.gameObject.SetActive(false);
            createWorldButton.gameObject.SetActive(true);
            deleteWorldButton.gameObject.SetActive(false);
        }
    }
}