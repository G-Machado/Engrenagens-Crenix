using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearDragManager : MonoBehaviour
{
    [Header("Movement Variables")]
    [Range(0, 1)]
    public float movementDamp = .1f;
    public Vector3 targetPos = Vector3.zero;

    [Header("Scaling Variables")]
    [Range(0, 1)]
    public float scaleDamp = .1f;
    public float dragScale;
    public float placeScale;

    [Header("Control Variables")]
    public bool beingDraged = false;
    public bool placed = false;

    private Vector3 initialPos;
    private Vector3 gearPlacePos;
    private Vector3 offsetMousePos;
    private Vector3 initialScale;

    public GearPlaceManager myPlace;

    [HideInInspector]
    public SpriteRenderer myRenderer;

    [HideInInspector]
    public int slotIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        initialScale = transform.localScale;

        myRenderer = GetComponent<SpriteRenderer>();

        slotIndex = LevelManager.Instance.GetSlotIndex(transform.position);
        LevelManager.Instance.ocuppiedSlots[slotIndex] = 1;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(targetPos != Vector3.zero) /// Se a 'targetPos' for diferente de zero, move a ferramenta
        {
            if (beingDraged)
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offsetMousePos;

            MoveGear();
        }

        if (!Input.GetMouseButton(0) && beingDraged)
        {
            beingDraged = false;

            myRenderer.sortingOrder = 0;

            if (!placed)
            {
                targetPos = LevelManager.Instance.GetSlotPos(transform.position);

                //targetPos = initialPos;
            }
            else
            {
                if(myPlace)
                    myPlace.LockGear();

                //myRenderer.enabled = false;

                targetPos = gearPlacePos;
            }
        }

        Vector3 targetScale = initialScale;

        if (beingDraged)
            targetScale = initialScale * dragScale;
        else if (placed)
        {
            targetScale = initialScale * placeScale;

            if (Vector3.Distance(transform.localScale, initialScale * placeScale) <= .02f && tag != "Nugget")
            {
                myRenderer.enabled = false;
                myPlace.placedSprite.color = myRenderer.color;
            }
        }

        Vector3 finalScale = Vector3.Lerp(transform.localScale, targetScale, 1 - scaleDamp);

        transform.localScale = finalScale;
    }

    private void MoveGear() /// Move a ferramenta para a 'targetPos' atual de acordo com o 'movementDamp' configurado
    {
        Vector3 finalPos = Vector3.Lerp(transform.position, targetPos, 1 - movementDamp);
        finalPos.z = transform.position.z;

        transform.position = finalPos;
    }

    private void OnMouseOver() 
    {
        Debug.Log("mouse over");

        if (Input.GetMouseButtonDown(0) && !beingDraged) /// Inicia o movimento de arrasto e capta o offset do mouse
        {
            beingDraged = true;

            myRenderer.sortingOrder = 1;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = transform.position.z;

            offsetMousePos = transform.position - mousePos;

            targetPos = mousePos;

            if(placed)
            {
                //placed = false;
                
                if(myPlace)
                    myPlace.UnlockGear();

                //myPlace = null;
                if(!myRenderer.enabled)
                    LevelManager.Instance.StopGameWin();

                myRenderer.enabled = true;

            }
            else
            {
                slotIndex = LevelManager.Instance.GetSlotIndex(transform.position);
                LevelManager.Instance.ocuppiedSlots[slotIndex] = 0;
            }
        }
    }

    public void PlaceGear(Transform place)
    {
        if(myPlace != null)
        {
            myPlace.DeactivateGear(this);
        }

        placed = true;

        gearPlacePos = place.position;

        myPlace = place.GetComponent<GearPlaceManager>();

    }
}
