using UnityEngine;

public class CriminalLeak : MonoBehaviour {
    public GameObject players;
    private GameObject criminal;

    public Camera criminalLeakCam;
    public RenderTexture criminalFace;

    public void ShowCriminalLeak () { 
        criminal = FindCriminal();
        RemoveProgressPanel();
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

    private void RemoveProgressPanel () {
        GameObject progressPannel = GameObject.FindGameObjectWithTag("Score");
        progressPannel.SetActive(false);
    }

    public void CaptureScreen () {
        new WaitForEndOfFrame();

        //Positioning of Camera
        criminalLeakCam.transform.position = criminal.transform.position;
        criminalLeakCam.transform.Translate(Vector3.forward);
        criminalLeakCam.transform.rotation = criminal.transform.rotation;
        criminalLeakCam.transform.Rotate(new Vector3(0 , -180 , 0));

        float height = Screen.height;
        float width = Screen.width;

        // creates off-screen render texture that can rendered into
        RenderTexture renderTexture = new RenderTexture((int)width , (int)height , 24);

        // manually render scene into rt
        criminalLeakCam.targetTexture = criminalFace;
        criminalLeakCam.Render();

        criminalLeakCam.targetTexture = null;
        RenderTexture.active = null;

        Destroy(criminalLeakCam.gameObject);

    }

    private void ShowLeakPanel () {
        CaptureScreen();
        this.gameObject.SetActive(true);
    }
}
