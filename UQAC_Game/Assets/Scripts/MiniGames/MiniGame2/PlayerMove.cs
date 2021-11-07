using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

// from http://gyanendushekhar.com/2019/11/11/move-canvas-ui-mouse-drag-unity-3d-drag-drop-ui/

public class PlayerMove : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    //création des variables
    private Vector2 lastMousePosition;

    bool victory = false;
    bool dragActive = false;
    Vector2 diff;
    private float update;
    private Vector3 previousPlayerPos;

    public GameObject victoryText;
    RectTransform rect;

    GameObject translationFromPlayer;
    RectTransform rectTranslationFromPlayer;
    BoxCollider colliderTranslationFromPlayer;


    // Start is called before the first frame update
    private void Start()
    {
        //Etre sûr que l'affichage du text de victoire est bien désactivé
        victoryText.SetActive(false);

        //Récupération du RectTransform du player
        rect = GetComponent<RectTransform>();
    }

    /// <summary>
    /// This method will be called on the start of the mouse drag
    /// </summary>
    /// <param name="eventData">mouse pointer event data</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        lastMousePosition = eventData.position;
        dragActive = true;

        //Création d'un objet permettant de récupérer le trajet effectué par le joueur
        //Si cet objet est en collision avec un mur, le joueur ne se déplacera pas
        translationFromPlayer = new GameObject("TranslationFromPlayer");
        translationFromPlayer.transform.parent = this.gameObject.transform;
        translationFromPlayer.transform.position = transform.position;
        translationFromPlayer.AddComponent<BoxCollider>();
        translationFromPlayer.AddComponent<RectTransform>();
        
        previousPlayerPos = rect.position;
        rectTranslationFromPlayer  = translationFromPlayer.GetComponent<RectTransform>();
        colliderTranslationFromPlayer = translationFromPlayer.GetComponent<BoxCollider>();

    }

    /// <summary>
    /// This method will be called during the mouse drag
    /// </summary>
    /// <param name="eventData">mouse pointer event data</param>
    public void OnDrag(PointerEventData eventData)
    {
        update += Time.deltaTime;
        if (update > 1.0f / 40f)
        {
            //Le déplacement est possible jusqu'à ce que le joueur atteigne l'arrivée
            if (!victory)
            {
                //Récupération des déplacements effectués par la souri
                Vector2 currentMousePosition = eventData.position;
                diff = currentMousePosition - lastMousePosition;

                //On applique ces déplacement à l'objet permettant de suivre la translation du joueur
                translationFromPlayer.transform.position = transform.position + new Vector3(diff.x / 2, diff.y / 2);
                rectTranslationFromPlayer.sizeDelta = new Vector2(Mathf.Abs(diff.x), Mathf.Abs(diff.y)) + rect.sizeDelta;
                colliderTranslationFromPlayer.size = new Vector2(Mathf.Abs(diff.x), Mathf.Abs(diff.y)) + rect.sizeDelta;
                colliderTranslationFromPlayer.transform.Translate(-diff);

                //Récupère la position du joueur avant son déplacement
                previousPlayerPos = rect.transform.position;
                
                //On déplace le joueur (reviendra en arrière si il rencontre un objet dans la méthode OnTriggerStay)
                rect.transform.Translate(new Vector3(diff.x, diff.y, transform.position.z));

                lastMousePosition = currentMousePosition;
            }
            else if (victory)
            {
                StartCoroutine(EndMiniGame());
            }
        }
    }

    IEnumerator EndMiniGame()
    {
        Debug.Log("victory");
        victoryText.SetActive(true);
        yield return new WaitForSeconds(2);
        Debug.Log("Fin de partie");
    }

    /// <summary>
    /// This method will be called at the end of mouse drag
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        dragActive = false;

        //Détruit l'objet permettant de récupérer le trajet effectué par le joueur
        Destroy(translationFromPlayer);
    }

    //Appelé lorsque le joueur rentre en contact avec un autre collider
    private void OnTriggerStay(Collider other)
    {
        //Si le joueur rentre en contact avec l'arrivée, le mini-jeu est terminé
        //Si le joueur retourne à la position qu'il avait avant sa dernière translation
        if (other.name == "Arrive" && rect.name == "Player")
        {
            victory = true;
        }
        else
        {
            rect.transform.position = previousPlayerPos;
        }
    }

    private void Update()
    {
        update += Time.deltaTime;
        if (update > 1.0f/40f)
        {
            update = 0.0f;
            if (dragActive == false && victory == false)
            {
                //Récupère  la position du joueur avant son déplacement
                previousPlayerPos = rect.position;
                int moveSpeed = 4;

                //On regarde les touches sélectionnées par le joueur et on déplace l'objet player en conséquence
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    rect.transform.Translate(new Vector3(0, moveSpeed, transform.position.z));
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    rect.transform.Translate(new Vector3(0, -moveSpeed, transform.position.z));
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    rect.transform.Translate(new Vector3(moveSpeed, 0, transform.position.z));
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    rect.transform.Translate(new Vector3(-moveSpeed, 0, transform.position.z));
                }
            }
            if (victory == true && !victoryText.activeSelf)
            {
                StartCoroutine(EndMiniGame());
            }
        }
    } 
}
