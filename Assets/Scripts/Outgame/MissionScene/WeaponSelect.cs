using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponSelect : MonoBehaviour
{
    public int index;
    public void SelectWeapon()
    {
        if (PrepareManager.Instance.weaponIndexes[index] == -1) // 비어있을 경우 클릭시
        {
            PrepareManager.Instance.addPlayerTo = -1;
            PrepareManager.Instance.addWeaponTo = index;
        }
        else
        {
            PrepareManager.Instance.addPlayerTo = -1;
            PrepareManager.Instance.addWeaponTo = -1;
            PrepareManager.Instance.weaponSlots[index].GetComponentInChildren<TextMeshProUGUI>().text = "Weapon " + (index + 1); //표기 초기화
            PrepareManager.Instance.weaponIndexes[index] = -1; // 출격 목록에서 제외 
            for (int i = 0; i < PrepareManager.Instance.weaponMap.Count; i++) // 딕셔너리에서 해당 밸류 제거
            {
                if (PrepareManager.Instance.weaponMap.ContainsKey(i))
                {
                    if (PrepareManager.Instance.weaponMap[i] == index)
                        PrepareManager.Instance.playerMap.Remove(i);
                }
            }
        }
    }

}
