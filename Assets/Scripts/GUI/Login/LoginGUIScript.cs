using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoginGUIScript : GenericGUI
{
    private string username;
    private string userpw;

    [SerializeField]
    private InputField IfWorkerName;

    [SerializeField]
    private InputField IfWorkerPw;

    [SerializeField]
    private Text msgText;
    
    #region Login
    /// <summary>
    /// Read credentials and tries to login if both username and password are not empty.
    /// </summary>
    public void ReadCredentials()
    {
        username = IfWorkerName.text.Trim().ToLower();
        userpw = IfWorkerPw.text;

        if (string.IsNullOrEmpty(username)) return;

        ControlInputFields(false);

        msgText.text = "Trying to login...";
        GameMng.RequestLogin(username, userpw);
        return;
    }

    /// <summary>
    /// Activate or desactivate the input fields of username and password
    /// </summary>
    /// <param name="active"></param>
    public void ControlInputFields(bool active)
    {
        IfWorkerName.gameObject.SetActive(active);
        IfWorkerPw.text = "";
        IfWorkerPw.gameObject.SetActive(active);
    }
    #endregion

    /// <summary>
    /// Prints on the message space of the screen a message to the user.
    /// </summary>
    /// <param name="msg">To message to inform the user.</param>
    public void InformUser(string msg)
    {
        msgText.text = msg;
    }
    
    #region Getters
    public string GetUsername()
    {
        return this.username;
    }
    #endregion
}