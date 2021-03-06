using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public RectTransform staminaBar;
    private float staminaBarMaxSize;
    public TextMeshProUGUI staminaText;
    public float staminaMax = 100;
    public float currentStamina = 100;
    private float stepDecrease = 0.3f;
    private float stepIncrease = 0.1f;
    public Movement movementScript;
    
    private float timeStoppedSprint;
    public float minimalRestTime = 2;

    private bool wasRunning;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Stamina script");
        wasRunning = false;
        
        // Get automaticaly size of the bar
        staminaBarMaxSize = staminaBar.rect.width;

        FindMovementScript();
        
        // First display
        ModifyDisplay();
    }

    void FindMovementScript()
    {
        // Find mine player in all children of players parent
        foreach (Transform player in GameObject.Find("Players").transform)
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                movementScript = player.GetComponent<Movement>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (movementScript == null)
        {
            FindMovementScript();
        }
        
        CanIHaveEnoughStaminaToRun();
        
#if UNITY_EDITOR // allow cheat code just when we start game with unity
        //Test of inputs' fonctions
        if (Input.GetKeyDown(KeyCode.G))
        {
            IncreaseMaxStamina(10f);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            DecreaseMaxStamina(10f);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            UpgradeRestTime(1f);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            DowngradeRestTime(1f);
        }
#endif
        CheckToRecoverStamina();
    }

    void CanIHaveEnoughStaminaToRun()
    {
        if (movementScript != null)
        {
            movementScript.canRun = currentStamina > 0;
            if (currentStamina <= 0)
            {
                movementScript.inRun = false;// Disable run if not enough stamina
            }
        }
    }

    void CheckToRecoverStamina()
    {
        if (movementScript != null)
        {
            // if run : loose stamina
            if (movementScript.inRun && currentStamina > 0)
            {
                wasRunning = true;
                DecreaseStamina(stepDecrease);
            }
            // if not run && have not enough stamina : increase stamina after a short time
            else if (movementScript.inRun == false && currentStamina < staminaMax)
            {
                // if was running and stop : start timer
                if (wasRunning)
                {
                    wasRunning = false;
                    timeStoppedSprint = Time.time;
                }

                // increase stamina when delta time is ended
                if (Time.time - timeStoppedSprint > minimalRestTime)
                {
                    IncreaseStamina(stepIncrease);
                }
            }
        }

    }
    // --- current stamina ---
    void IncreaseStamina(float bonusStamina)
    {
        currentStamina += bonusStamina;
        if (currentStamina > staminaMax)
        {
            currentStamina = staminaMax;
        }
        ModifyDisplay();
    }

    void DecreaseStamina(float malusStamina)
    {
        currentStamina -= malusStamina;
        if (currentStamina < 0)
        {
            currentStamina = 0;
        }
        ModifyDisplay();
    }
    
    // --- max stamina ---
    void IncreaseMaxStamina(float bonusStamina)
    {
        staminaMax += bonusStamina;
        currentStamina -= bonusStamina;
        ModifyDisplay();
    }

    void DecreaseMaxStamina(float malusStamina)
    {
        staminaMax -= malusStamina; 
        if (staminaMax < currentStamina)
        {
            currentStamina = staminaMax;
        }
        ModifyDisplay();
    }

    // --- rest time stamina ---
    void UpgradeRestTime(float bonusRestTime)
    {
        minimalRestTime -= bonusRestTime;
        ModifyDisplay();
    }

    void DowngradeRestTime(float malusRestTime)
    {
        minimalRestTime += malusRestTime;
        ModifyDisplay();
    }


    // --- display ---
    private void ModifyDisplay()
    {
        //Modify color of stamina bar
        if (currentStamina >= staminaMax / 2.0f)
        {
            staminaBar.GetComponent<Image>().color = Color.green;
        }
        else if (currentStamina >= staminaMax / 5.0f)
        {
            staminaBar.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            staminaBar.GetComponent<Image>().color = Color.red;
        }

        //Modify progress and text of stamina bar
        staminaBar.transform.localPosition = new Vector3(currentStamina * staminaBarMaxSize / staminaMax - staminaBarMaxSize, 0, 0);
        staminaText.text = "Stamina : " + (int) ((float) currentStamina / (float) staminaMax * 100) + " %";
    }
}
