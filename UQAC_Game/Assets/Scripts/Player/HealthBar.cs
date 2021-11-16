using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBar;
    public GameObject mask;
    public TextMeshProUGUI hpText;
    private int hpMax;
    private int currentHP;

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
        //test des fonctions
        if (Input.GetKeyDown(KeyCode.R))
        {
            TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            RecoverHP(10);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            IncreaseHpMax(10);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            DecreaseHpMax(10);
        }

        if (currentHP <= 0)
        {
            //Debug.Log("Game Over");
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
        //Modifie la couleur de la barre de vie
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

        //modifie l'avancement de la barre de vie ainsi que le texte correspondant
        healthBar.transform.position = mask.transform.position + new Vector3(currentHP * 200 / hpMax - 200, 0, 0);
        hpText.text = "HP : " + currentHP + " / " + hpMax;
    }
}
