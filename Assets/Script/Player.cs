using UnityEngine;
using Mirror;
using System.Collections;

public class Player : NetworkBehaviour
{
    public bool isDead
    {
        get{return _isDead;}
        protected set {_isDead = value;}
    }

    [SerializeField]
    private float maxHealth = 100;

    [SyncVar]
    private float currentHealth;

    [SyncVar]
    private bool _isDead = false;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnableOnStart;

    public void Setup() 
    {
        wasEnableOnStart = new bool[disableOnDeath.Length];
        for (var i = 0; i < disableOnDeath.Length; i++)
        {
            wasEnableOnStart[i] = disableOnDeath[i].enabled;
        }

        SetDefaults();
    }

    public void SetDefaults() 
    {
        isDead =false;
        currentHealth = maxHealth;

        for (var i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnableOnStart[i];
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(float amount) 
    {
        if(isDead)
        {
            return;
        }

        currentHealth -= amount;
        Debug.Log(transform.name + "a maintenant :" + currentHealth + "PV");

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTimer);
        SetDefaults();
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

    private void Update() 
    {
        if(!isLocalPlayer)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(999);
        }
    }

    private void Die()
    {
        isDead = true;

        for (var i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        Debug.Log(transform.name + "a été éliminé.");

        StartCoroutine(Respawn());
    }
}
