using UnityEngine;

public class ExitButtonScript : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Exit Game"); 
        Application.Quit();    
    }
}
