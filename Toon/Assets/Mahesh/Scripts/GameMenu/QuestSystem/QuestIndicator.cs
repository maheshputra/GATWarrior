﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuestIndicator : MonoBehaviour, ISelectHandler, ICancelHandler, IDeselectHandler
{
    public Text questText;
    public int questID;
    public float itemNum;
    public float itemTotal;
    public GameObject markIndicator;

    
    void Start()
    {
        itemTotal = transform.parent.childCount;
        itemNum = transform.GetSiblingIndex();
        markIndicator.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        //set the scrollbar position by selected item
        markIndicator.SetActive(true);
        if (itemNum == 1)
            itemNum = 0;

        Quest.instance.questViewScrollbar.value = 1.0f - (itemNum / itemTotal);

        if (itemNum == transform.parent.childCount - 1)
            Quest.instance.questViewScrollbar.value = 0f;
        else if (itemNum == 0)
            Quest.instance.questViewScrollbar.value = 1.0f;

        Quest.instance.RefreshQuestDetail(questID);
    }

    public void OnCancel(BaseEventData eventData)
    {
        if (Inventory.instance.isSwapping)
        {
            Inventory.instance.CancelSwap();
        }
        else
        {
            UIManager.instance.StartCoroutine(UIManager.instance.ChangeState(UIState.Gameplay));
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        markIndicator.SetActive(false);
    }
}
