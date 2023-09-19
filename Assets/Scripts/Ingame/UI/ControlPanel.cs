using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;


public class ControlPanel : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public GameObject Move;
    public GameObject Attack;
    public GameObject Interact;
    public GameObject EX;
    public float speed = 50.0f;

    void Update()
    {
        DisplayName();
    }

    public void DisplayName()
    {
        if (IngameManager.Instance.playerController.currentPlayer == null)
        {
            playerName.text = "Not Selected";
        }
        else
        {
            playerName.text = IngameManager.Instance.playerController.currentPlayer.GetComponent<PlayerState>().playerName;
        }
    }
    public void DisplayMove(bool canMove, bool isSelected)
    {
        if (canMove)
        {
            if (isSelected) //선택 시
            {
                RectTransform rect = Move.GetComponent<RectTransform>();
                StartCoroutine(movefordirection(Move, rect.anchoredPosition + new Vector2(0, 20)));
            }
            else //취소 시
            {
                RectTransform rect = Move.GetComponent<RectTransform>();
                StartCoroutine(movefordirection(Move, rect.anchoredPosition + new Vector2(0, -20)));
            }
        }
        else
        {
            Move.GetComponent<Image>().color = new Color(255, 0, 0, 0.5f);
        }
    }
    public void DisplayAttack(bool canAttack, bool isSelected)
    {
        if (canAttack)
        {
            if (isSelected) //선택 시
            {
                RectTransform rect = Attack.GetComponent<RectTransform>();
                StartCoroutine(movefordirection(Attack, rect.anchoredPosition + new Vector2(0, 20)));

            }
            else //취소 시
            {
                RectTransform rect = Attack.GetComponent<RectTransform>();
                StartCoroutine(movefordirection(Attack, rect.anchoredPosition + new Vector2(0, -20)));
            }
        }
        else
        {
            Attack.GetComponent<Image>().color = new Color(255, 0, 0, 0.5f);
        }

    }
    public void DisplayInteract(bool isSelected)
    {
        if (isSelected) //선택 시
        {
            RectTransform rect = Interact.GetComponent<RectTransform>();
            StartCoroutine(movefordirection(Interact, rect.anchoredPosition + new Vector2(0, 20)));

        }
        else //취소 시
        {
            RectTransform rect = Interact.GetComponent<RectTransform>();
            StartCoroutine(movefordirection(Interact, rect.anchoredPosition + new Vector2(0, -20)));
        }
    }
    public void DisplayEX(bool canEX, bool isSelected)
    {
        if (canEX)
        {
            if (isSelected) //선택 시
            {
                RectTransform rect = EX.GetComponent<RectTransform>();
                StartCoroutine(movefordirection(EX, rect.anchoredPosition + new Vector2(0, 20)));
            }
            else //취소 시
            {
                RectTransform rect = Attack.GetComponent<RectTransform>();
                StartCoroutine(movefordirection(EX, rect.anchoredPosition + new Vector2(0, -20)));
            }
        }
        else
        {
            EX.GetComponent<Image>().color = new Color(255, 0, 0, 0.5f);
        }
    }

    public IEnumerator movefordirection(GameObject panel, Vector2 target)
    {
        RectTransform Rect = panel.GetComponent<RectTransform>();
        while (Rect.anchoredPosition != target)
        {
            Rect.anchoredPosition = Vector2.MoveTowards(Rect.anchoredPosition, target, speed);
            yield return null;
        }
    }
}
