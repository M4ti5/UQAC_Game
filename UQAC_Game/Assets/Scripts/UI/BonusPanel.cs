using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BonusPanel : MonoBehaviour
{
    public float cooldownMax;
    public float currentCooldown;
    private int bonusNumber;

    // Start is called before the first frame update
    void Start()
    {
        cooldownMax = 1.2f;
        if (gameObject.name == "Bonus 1")
        {
            bonusNumber = 1;
        }
        else if (gameObject.name == "Bonus 2")
        {
            bonusNumber = 2;
        }
        else 
        {
            bonusNumber = 3;
        }


    }

    // Update is called once per frame
    void Update()
    {
        // Active bonus if corresponding is pressed
        if (bonusNumber == 1)
        {
            ActivateBonus(Input.GetKeyDown(KeyCode.Alpha1));
        }
        else if (bonusNumber == 2)
        {
            ActivateBonus(Input.GetKeyDown(KeyCode.Alpha2));
        }
        else
        {
            ActivateBonus(Input.GetKeyDown(KeyCode.Alpha3));
        }

        
    }

    private void ActivateBonus(bool keyCode)
    {
        // if key is pressed and cooldown is ending
        if (keyCode && gameObject.GetComponent<Image>().fillAmount == 1)
        {
            gameObject.GetComponent<Image>().fillAmount = 0;
            gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
            currentCooldown = 0;
        }
        
        // rotate circle while cooldown to use bonus is not ending
        if (gameObject.GetComponent<Image>().fillAmount != 1)
        {
            currentCooldown += Time.deltaTime;
            if (currentCooldown >= cooldownMax)
            {
                gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
                currentCooldown = cooldownMax;
            }
            gameObject.GetComponent<Image>().fillAmount = currentCooldown / cooldownMax;
            gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = currentCooldown / cooldownMax;
        }
    }

}
