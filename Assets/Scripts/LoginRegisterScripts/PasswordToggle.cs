using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordToggleButton : MonoBehaviour
{
    public TMP_InputField passwordInput;
    public Button toggleButton;
    public TextMeshProUGUI buttonText; // Optional: Change button text (Show/Hide)

    private bool isPasswordVisible = false;

    void Start()
    {
        toggleButton.onClick.AddListener(TogglePasswordVisibility);
    }

    void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;

        if (isPasswordVisible)
        {
            passwordInput.contentType = TMP_InputField.ContentType.Standard;
            if (buttonText != null) buttonText.text = "0_0";
        }
        else
        {
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            if (buttonText != null) buttonText.text = "-_-";
        }

        passwordInput.ForceLabelUpdate(); // Refresh display
    }
}