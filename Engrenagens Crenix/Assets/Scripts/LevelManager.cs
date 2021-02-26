using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public enum GameState
    {
        GAMEPLAY, 
        GAMEWIN
    }
    public GameState currentState;

    private void Awake()
    {
        if (LevelManager.Instance == null)
            LevelManager.Instance = this;
    }

    public GearDragManager[] gears;
    public GearPlaceManager[] places;
    public RectTransform[] gearSlots;

    //[HideInInspector]
    public int[] ocuppiedSlots;

    [Space(4)]
    public Text nuggetText;

    // Start is called before the first frame update
    void Start()
    {
        gears = FindObjectsOfType<GearDragManager>();
        places = GetComponentsInChildren<GearPlaceManager>();

        ResetGears();
    }

    public Vector3 GetSlotPos(Vector3 gearPos)
    {
        Vector3 closestPos = new Vector3(999, 999, 999);

        int closestIndex = 0;

        for (int i = 0; i < gearSlots.Length; i++)
        {
            if (ocuppiedSlots[i] == 1) continue;

            if(Vector3.Distance(gearSlots[i].position, gearPos) < Vector3.Distance(closestPos, gearPos))
            {
                closestPos = gearSlots[i].position;
                closestIndex = i;
            }
        }

        ocuppiedSlots[closestIndex] = 1;

        return closestPos;
    }

    public int GetSlotIndex(Vector3 gearPos)
    {
        Vector3 closestPos = new Vector3(999, 999, 999);

        int closestIndex = 0;

        for (int i = 0; i < gearSlots.Length; i++)
        {
            if (Vector3.Distance(gearSlots[i].position, gearPos) < Vector3.Distance(closestPos, gearPos))
            {
                closestPos = gearSlots[i].position;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    public bool CheckGameWin()
    {
        bool gameWin = false;
        int gearsPlacedCount = 0;

        for (int i = 0; i < places.Length; i++)
        {
            if (places[i].locked)
                gearsPlacedCount ++;
        }

        if (gearsPlacedCount >= 5) /// Game win
        {
            gameWin = true;

            StartGameWin();
        }

        return gameWin;
    }

    private void StartGameWin()
    {
        if (currentState == GameState.GAMEWIN) return;

        currentState = GameState.GAMEWIN;

        nuggetText.text = "YEY, PARABÉNS. TASK CONCLUÍDA!!  AGORA É MINHA VEZ!";

        for (int i = 0; i < places.Length; i++)
        {
            places[i].StartSpin();
        }
    }

    public void StopGameWin()
    {
        if (currentState == GameState.GAMEPLAY) return;

        currentState = GameState.GAMEPLAY;

        nuggetText.text = "ENCAIXE AS ENGRENAGENS EM QUALQUER ORDEM!";

        for (int i = 0; i < places.Length; i++)
        {
            places[i].StopSpin();
        }
    }

    public void ResetGears()
    {
        for (int i = 0; i < ocuppiedSlots.Length; i++)
        {
            ocuppiedSlots[i] = 0;
        }

        for (int i = 0; i < gears.Length; i++)
        {
            //places[i].DeactivateGear(places[i].filledGear);

            if (gears[i].tag != "Nugget")
            {
                gears[i].placed = false;
                gears[i].beingDraged = false;
                gears[i].myRenderer.enabled = true;
                gears[i].targetPos = GetSlotPos(gears[i].transform.position);
            }

            if(i <= places.Length - 1)
                places[i].UnlockGear();
        }

        StopGameWin();
    }

}
