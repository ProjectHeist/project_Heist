using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSelect : MonoBehaviour
{
    // Start is called before the first frame update
    public int index;
    public void SelectPlayer()
    {
        if (PrepareManager.Instance.playerIndexes[index] == -1) // 비어있을 경우 클릭시
        {
            PrepareManager.Instance.addPlayerTo = index;
            PrepareManager.Instance.addWeaponTo = -1;
        }
        else
        {
            PrepareManager.Instance.addPlayerTo = -1;
            PrepareManager.Instance.addWeaponTo = -1;
            PrepareManager.Instance.playerSlots[index].GetComponentInChildren<TextMeshProUGUI>().text = "Character " + (index + 1); //표기 초기화
            PrepareManager.Instance.playerIndexes[index] = -1;
            for (int i = 0; i < PrepareManager.Instance.playerMap.Count; i++) // 딕셔너리에서 해당 밸류 제거
            {
                if (PrepareManager.Instance.playerMap.ContainsKey(i))
                {
                    if (PrepareManager.Instance.playerMap[i] == index)
                        PrepareManager.Instance.playerMap.Remove(i);
                }
            }
        }

    }
}
