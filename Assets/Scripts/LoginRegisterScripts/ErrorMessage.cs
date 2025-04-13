using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ErrorMessage : MonoBehaviour
{
        public TMP_InputField passwordInput;
        public TMP_Text errorText;

        private void Start()
        {
            passwordInput.onValueChanged.AddListener(ValidatePassword);
            errorText.text = "";
        }

        private void ValidatePassword(string password)
        {
            string errorMessage = "";

            if (password.Length < 8)
                errorMessage += "Minimaal 8 tekens vereist.\n";

            if (!Regex.IsMatch(password, @"[A-Z]"))
                errorMessage += "Minimaal 1 hoofdletter vereist.\n";

            if (!Regex.IsMatch(password, @"[a-z]"))
                errorMessage += "Minimaal 1 kleine letter vereist.\n";

            if (!Regex.IsMatch(password, @"[0-9]"))
                errorMessage += "Minimaal 1 cijfer vereist.\n";

            if (!Regex.IsMatch(password, @"[\W_]"))
                errorMessage += "Minimaal 1 speciaal teken vereist.\n";

            errorText.text = errorMessage.Trim();
        }
}