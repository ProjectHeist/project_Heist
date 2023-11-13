using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;


public class ControlUI
{
    public GameObject MovePanel;
    public GameObject AttackPanel;
    public GameObject InteractPanel;
    public GameObject EXPanel;
    float speed = 50.0f;

    public ControlUI(GameObject Move, GameObject Attack, GameObject Interact, GameObject EX)
    {
        MovePanel = Move;
        AttackPanel = Attack;
        InteractPanel = Interact;
        EXPanel = EX;
    }

    public IEnumerator DisplaySelect(GameObject Panel)
    {
        RectTransform rect = Panel.GetComponent<RectTransform>();
        Vector2 target = rect.anchoredPosition + new Vector2(0, 20);
        while (rect.anchoredPosition != target)
        {
            rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, target, speed);
            yield return null;
        }
    }

    public IEnumerator DisplayDeselect(GameObject Panel)
    {
        RectTransform rect = Panel.GetComponent<RectTransform>();
        Vector2 target = rect.anchoredPosition + new Vector2(0, -20);
        while (rect.anchoredPosition != target)
        {
            rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, target, speed);
            yield return null;
        }
    }

    public void changeColor(GameObject Panel, Color color)
    {
        Panel.GetComponent<Image>().color = color;
    }

}
