using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearPlaceManager : MonoBehaviour
{
    public enum SpinDirection
    {
        CLOCKWISE,
        COUNTERCLOCK
    }

    [HideInInspector]
    public SpriteRenderer placedSprite;
    public bool filled = false;
    public bool locked = false;

    public GearDragManager filledGear;

    [Header("Spinning Variables")]
    public bool spinning = false;
    public float spinSpeed = 1;
    public SpinDirection spin = SpinDirection.CLOCKWISE;
    [Range(0, 1)]
    public float spinDamp;

    private Color initialColor;

    private void Start()
    {
        placedSprite = GetComponentsInChildren<SpriteRenderer>()[1];
        placedSprite.enabled = false;

        initialColor = placedSprite.color;
    }

    private void FixedUpdate()
    {
        if(spinning)
        {
           SpinGear();
        }
    }

    private void SpinGear() /// Applies rotation to sprite icon gear
    {
        float direction = -1;

        if (spin == SpinDirection.COUNTERCLOCK)
            direction = 1;

        float currentAngle = filledGear.transform.rotation.eulerAngles.z;
        currentAngle += spinSpeed * direction;
        Quaternion newRot = Quaternion.Euler(0, 0, currentAngle);
        filledGear.transform.rotation = Quaternion.Lerp(filledGear.transform.rotation, newRot, 1 - spinDamp);

        currentAngle = filledGear.transform.rotation.eulerAngles.z;
        currentAngle += spinSpeed * direction;
        newRot = Quaternion.Euler(0, 0, currentAngle + 22.31f);

        placedSprite.transform.rotation = Quaternion.Lerp(placedSprite.transform.rotation,
            newRot, 1 - spinDamp);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!filled)
        {
            filled = true;

            //other.GetComponent<GearDragManager>().SetupSpin(spin);
            filledGear = collision.gameObject.GetComponent<GearDragManager>();

            filledGear.PlaceGear(transform);

            float currentAngle = filledGear.transform.rotation.eulerAngles.z;
            Quaternion newRot = Quaternion.Euler(0, 0, currentAngle + 22.31f);

            placedSprite.transform.rotation = newRot;
            placedSprite.enabled = true;

            Color newColor = filledGear.myRenderer.color;
            newColor.a = .5f;
            placedSprite.color = newColor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GearDragManager gear = collision.gameObject.GetComponent<GearDragManager>();

        DeactivateGear(gear);
    }

    public void DeactivateGear(GearDragManager gearManager)
    {
        if (gearManager != filledGear) return;

        if (filled)
        {
            filled = false;

            placedSprite.enabled = false;

            placedSprite.color = initialColor;

            if (gearManager.myPlace == this)
                gearManager.placed = false;

            gearManager.myPlace = null;
        }
    }

    public void LockGear()
    {
        if (locked) return;

        locked = true;

        LevelManager.Instance.CheckGameWin();
    }

    public void UnlockGear()
    {
        if (!locked) return;

        locked = false;

        filled = true;

        placedSprite.enabled = true;

        Color newColor = filledGear.myRenderer.color;
        newColor.a = .5f;
        placedSprite.color = newColor;
    }

    public void StartSpin()
    {
        spinning = true;
    }

    public void StopSpin()
    {
        spinning = false;
    }
}
