using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayUnlocker : MonoBehaviour
{
    public Button playButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AccessChecker();
    }
    public void AccessChecker()
    {
        if(varStorage.accessToken == null)
        {
            playButton.interactable = false;
        }
        else 
        {
            playButton.interactable = true;
        }
    }
}
