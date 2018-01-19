using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInfo : NetworkBehaviour
{
    public UnityEngine.UI.Text Text;
    [SyncVar(hook = "OnNameChange")]
    public string pName = "";

    [Command]
    public void CmdChangeName(string value)
    {
        Debug.Log("CmdChangeName localPlayer");
        pName = value;
    }

    void OnNameChange(string value)
    {     
            Text.text = value;       
    }
}
