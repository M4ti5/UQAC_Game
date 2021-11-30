using Photon.Pun;
using UnityEngine;

public class CriminalLeak : MonoBehaviour {
    public GameObject players;
    private GameObject criminal;

    public Camera criminalLeakCam;
    public RenderTexture criminalFace;

    public void ShowCriminalLeak () { 
        criminal = FindCriminal();
        HideProgressPanel();
        ShowLeakPanel();
    }

    private GameObject FindCriminal () {
        foreach (PlayerStatManager player in players.GetComponentsInChildren<PlayerStatManager>()) {
            if (player.criminal) {
                return criminal = player.gameObject;
            }
        }
        return null;
    }

    private void HideProgressPanel () {
        GameObject progressPannel = GameObject.FindGameObjectWithTag("Score");
        progressPannel.SetActive(false);
    }

    public void CaptureScreen () {

        if (criminal != null) // pas de screen si pas de criminal
        {
            new WaitForEndOfFrame();
            
            //Positioning of Camera
            criminalLeakCam.transform.position = criminal.transform.position;
            criminalLeakCam.transform.rotation = criminal.transform.rotation;
            criminalLeakCam.transform.Translate(new Vector3(0,1.75f,2));// ajuster la position de la cam ici
            criminalLeakCam.transform.Rotate(new Vector3(0 , -180 , 0));

            float height = Screen.height;
            float width = Screen.width;


            // manually render scene into rt
            criminalLeakCam.targetTexture = criminalFace;
            criminalLeakCam.Render();

            criminalLeakCam.targetTexture = null;
            RenderTexture.active = null;

        }
        
        criminalLeakCam.gameObject.SetActive(false);
        //Destroy(criminalLeakCam.gameObject);
    }

    private void ShowLeakPanel () {
        this.gameObject.SetActive(true);
        CaptureScreen();
    }
}
