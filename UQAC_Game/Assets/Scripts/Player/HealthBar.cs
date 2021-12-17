using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviourPun
{
    public Image healthBar;
    public GameObject mask;
    public TextMeshProUGUI hpText;
    private int hpMax;
    public int currentHP;
    public GameObject allPlayers;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start HealthBar script");
        hpMax = 100;
        currentHP = 100;
    }

    // Update is called once per frame
    void Update()
    {
        int allPlayersCount = allPlayers.transform.childCount;
        for (int i = 0; i < allPlayersCount; i++)
        {
            if (allPlayers.transform.GetChild(i).GetComponent<PhotonView>().IsMine)
            {
                currentHP = allPlayers.transform.GetChild(i).GetComponent<PlayerStatManager>().currentHP;
                ModifyDisplay();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log("Game Over");
        }
        ModifyDisplay();
    }

    public void IncreaseHpMax(int bonusHealth)
    {
        hpMax += bonusHealth;
        currentHP += bonusHealth;
        ModifyDisplay();
    }

    public void DecreaseHpMax(int malusHealth)
    {
        hpMax -= malusHealth;
        if (hpMax <= 100)
        {
            hpMax = 100;
        }
        if (hpMax < currentHP)
        {
            currentHP = hpMax;
        }
        ModifyDisplay();
    }

    public void RecoverHP(int heal)
    {
        currentHP += heal;
        if (currentHP >= hpMax)
        {
            currentHP = hpMax;
            Debug.Log("Full Life");
        }
        ModifyDisplay();
    }


    private void ModifyDisplay()
    {
        //Modify the color of healthbar
        if (currentHP >= hpMax / 2)
        {
            healthBar.color = Color.green;
        }
        else if (currentHP >= hpMax / 5)
        {
            healthBar.color = Color.yellow;
        }
        else
        {
            healthBar.color = Color.red;
        }
        //Modify the progression of Health Bar and text
        healthBar.transform.localPosition = new Vector3(currentHP * 200 / hpMax - 200, 0, 0);
        hpText.text = "HP : " + (int) ((float) currentHP / (float) hpMax * 100) + " %";
    }
}
