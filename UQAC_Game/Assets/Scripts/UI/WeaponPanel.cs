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
        // synchro rotation of background circle and waepon image
        display.GetComponent<Image>().fillAmount = 1 - (cooldownMax - (Time.time - currentCooldown)) / cooldownMax;
        display.GetChild(0).gameObject.GetComponent<Image>().fillAmount = 1 - (cooldownMax - (Time.time - currentCooldown)) / cooldownMax;
        // show if we have a weapon in equipment gameobject (equipment is set via UpdateWeaponDisplay called in player stat manager)
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
        // save equipment object of our player
        equipment = equipments;
        if (equipments.transform.childCount == 0)
        {
            display.gameObject.SetActive(false);
        }
        else
        {
            currentWeapon = equipments.transform.GetChild(0).name;
            
            // save cooldown of different objects (get previous cooldown by compare weapon name)
            foreach (Sprite elem in allWeapons)
            {
                if (elem.name == currentWeapon)
                {
                    currentCooldown = (float)cooldownByWeapon[currentWeapon];// get cooldown of the current equipped weapon
                    display.GetChild(0).GetComponent<Image>().sprite = elem;
                    display.GetComponent<Image>().fillAmount = (cooldownMax - (Time.time - currentCooldown)) / cooldownMax;
                    display.GetChild(0).gameObject.GetComponent<Image>().fillAmount = (cooldownMax - (Time.time - currentCooldown)) / cooldownMax;
                    display.gameObject.SetActive(true);
                }
            }
        }
    }

    // save weapon cooldown and hide display (when put it in inventary for exemple)
    public void HideDisplay()
    {
        cooldownByWeapon[currentWeapon] = Time.time - cooldownMax;

        display.gameObject.SetActive(false);
    }

}
