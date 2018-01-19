using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class Health : NetworkBehaviour
{

    public const int maxHealth = 100;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;

    public RectTransform healthBar;

    public void TakeDamage(int amount)
    {
        if (!isServer)
            return;
        Debug.Log("Take Damage" + amount);
        Debug.Log("current health" + currentHealth);
        
        currentHealth -= amount;
        Debug.Log("new health " + currentHealth);
        Debug.Log("current / max" + (float)currentHealth / 100.0f);

        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;

            // called on the Server, but invoked on the Clients
            RpcRespawn();
        }
    }

    void OnChangeHealth(int currentHealth)
    {
        healthBar.GetComponent<Image>().fillAmount = ((float)currentHealth / (float)maxHealth) ;
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            // move back to zero location
            transform.position = Vector3.zero;
        }
    }
}