using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPanel : MonoBehaviour
{
    public List<Sprite> allWeapons = new List<Sprite>();
    public Hashtable cooldownByWeapon = new Hashtable();
    private string currentWeapon = "";
    public float currentCooldown = 10f;
    public float cooldownMax = 10f;

    // Start is called before the first frame update
    void Start()
    {
        currentCooldown = Time.time - cooldownMax;
        gameObject.SetActive(false);
        foreach(var elem in allWeapons)
        {
            cooldownByWeapon.Add(elem.name, currentCooldown);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //currentCooldown += Time.deltaTime;
        gameObject.GetComponent<Image>().fillAmount = 1 - (cooldownMax - (Time.time - currentCooldown)) / cooldownMax;
        gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = 1 - (cooldownMax - (Time.time - currentCooldown)) / cooldownMax;
    }

    public void UpdateWeaponDisplay(GameObject equipments)
    {
        if (equipments.transform.childCount == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            currentWeapon = equipments.transform.GetChild(0).name;
            
            foreach (Sprite elem in allWeapons)
            {
                if (elem.name == currentWeapon)
                {
                    currentCooldown = (float)cooldownByWeapon[currentWeapon];
                    transform.GetChild(0).GetComponent<Image>().sprite = elem;
                    gameObject.GetComponent<Image>().fillAmount = (cooldownMax - (Time.time - currentCooldown)) / cooldownMax;
                    gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = (cooldownMax - (Time.time - currentCooldown)) / cooldownMax;
                    gameObject.SetActive(true);
                }
            }
        }
    }

}
