using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPanel : MonoBehaviour
{
    public List<Sprite> allWeapons = new List<Sprite>();
    public Hashtable cooldownByWeapon = new Hashtable();
    [SerializeField] private string currentWeapon = "";
    public float currentCooldown = 10f;
    public float cooldownMax = 10f;

    public GameObject equipment;

    public Transform display;

    // Start is called before the first frame update
    void Start()
    {
        currentCooldown = Time.time - cooldownMax;
        display.gameObject.SetActive(false);
        foreach(var elem in allWeapons)
        {
            cooldownByWeapon.Add(elem.name, currentCooldown);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //currentCooldown += Time.deltaTime;
        display.GetComponent<Image>().fillAmount = 1 - (cooldownMax - (Time.time - currentCooldown)) / cooldownMax;
        display.GetChild(0).gameObject.GetComponent<Image>().fillAmount = 1 - (cooldownMax - (Time.time - currentCooldown)) / cooldownMax;
        if (equipment != null)
        {
            if (equipment.transform.childCount == 0)
            {
                display.gameObject.SetActive(false);
            }
            else
            {
                display.gameObject.SetActive(true);
            }
        }
    }

    public void UpdateWeaponDisplay(GameObject equipments)
    {
        equipment = equipments;
        if (equipments.transform.childCount == 0)
        {
            display.gameObject.SetActive(false);
        }
        else
        {
            currentWeapon = equipments.transform.GetChild(0).name;
            
            foreach (Sprite elem in allWeapons)
            {
                if (elem.name == currentWeapon)
                {
                    currentCooldown = (float)cooldownByWeapon[currentWeapon];
                    display.GetChild(0).GetComponent<Image>().sprite = elem;
                    display.GetComponent<Image>().fillAmount = (cooldownMax - (Time.time - currentCooldown)) / cooldownMax;
                    display.GetChild(0).gameObject.GetComponent<Image>().fillAmount = (cooldownMax - (Time.time - currentCooldown)) / cooldownMax;
                    display.gameObject.SetActive(true);
                }
            }
        }
    }

    public void HideDisplay()
    {
        print("HideDisplay");
        if (equipment.transform.childCount == 0)
        {
            //currentWeapon = equipment.transform.GetChild(0).name;
        }
        cooldownByWeapon[currentWeapon] = Time.time - cooldownMax;

        display.gameObject.SetActive(false);
    }

}
