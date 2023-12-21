using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ingame;

public enum PanelType
{
    Move,
    Attack,
    Interact,
    EX
}

public class IngameUIManager : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public GameObject Move;
    public GameObject Attack;
    public GameObject Interact;
    public GameObject EX;
    public List<GameObject> panelList;
    public float speed = 50.0f;
    public ControlUI control;
    public DisplayRange range;
    private bool[] bools = { false, false, false, false };

    public void Init()
    {
        range = new DisplayRange();
        control = new ControlUI(Move, Attack, Interact, EX);
        panelList.Add(Move);
        panelList.Add(Attack);
        panelList.Add(Interact);
        panelList.Add(EX);
    }

    public void DisplayName(GameObject player)
    {
        if (player != null)
        {
            playerName.text = player.GetComponent<PlayerState>().playerName;
        }
        else
        {
            playerName.text = "Not Selected";
        }
    }

    public void SelectPanel(PanelType panelType) // 패널을 선택할 때
    {
        switch (panelType)
        {
            case PanelType.Move:
                selecthelper(0);
                break;
            case PanelType.Attack:
                selecthelper(1);
                break;
            case PanelType.Interact:
                selecthelper(2);
                break;
            case PanelType.EX:
                selecthelper(3);
                break;
        }
    }

    public void DeselectPanel(PanelType panelType) // 패널 선택을 취소할 때
    {
        switch (panelType)
        {
            case PanelType.Move:
                deselecthelper(0);
                break;
            case PanelType.Attack:
                deselecthelper(1);
                break;
            case PanelType.Interact:
                deselecthelper(2);
                break;
            case PanelType.EX:
                deselecthelper(3);
                break;
        }
    }

    public void IsSelected(PanelType panelType, bool selected)
    {
        switch (panelType)
        {
            case PanelType.Move:
                colorhelper(0, selected);
                break;
            case PanelType.Attack:
                colorhelper(1, selected);
                break;
            case PanelType.Interact:
                colorhelper(2, selected);
                break;
            case PanelType.EX:
                colorhelper(3, selected);
                break;
        }
    }

    private void colorhelper(int i, bool a)
    {
        if (a)
        {
            control.changeColor(panelList[i], new Color(255, 255, 255, 0.5f));
        }
        else
        {
            control.changeColor(panelList[i], new Color(255, 0, 0, 0.5f));
        }
    }

    private void selecthelper(int i)
    {
        if (!bools[i])
        {
            bools[i] = true;
            StartCoroutine(control.DisplaySelect(panelList[i]));
            for (int j = 0; j < 4; j++)
            {
                if (bools[j] && !bools[i]) //만약 다른 패널이 활성화 된거라면
                {
                    StartCoroutine(control.DisplayDeselect(panelList[j]));
                }
            }
        }
    }

    private void deselecthelper(int i)
    {
        if (bools[i])
        {
            bools[i] = false;
            StartCoroutine(control.DisplayDeselect(panelList[i]));
        }
    }
}
