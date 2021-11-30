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
        foreach (GameObject player in players.GetComponentsInChildren<GameObject>()) {
            if (player.GetComponent<PlayerStatManager>().criminal) {
                return criminal = player;
            }
        }
        return null;
    }

    private void RemoveProgressPanel () {
        GameObject progressPannel = GameObject.FindGameObjectWithTag("score");
        Destroy(progressPannel);
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
        criminalLeakCam.targetTexture = renderTexture;
        criminalLeakCam.Render();

        Destroy(criminalLeakCam);
        criminalFace = renderTexture;
    }

    private void ShowLeakPanel () {
        this.gameObject.SetActive(true);
    }
}
