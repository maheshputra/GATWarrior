﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Collection Quest", menuName = "Database/Collection Quest")]
public class CollectionQuest : ScriptableObject
{
    public int sourceID;
    public int id;
    public List<int> chainQuestID = new List<int>();
    public int colAmount;
    public int curAmount;
    public Item itemToCollect;
    public string title;
    public string verb;
    public string resourcePath;
    public string description;
    public bool isComplete;
    public bool isOptional;

    public Dialogue startDialogue;
    public Dialogue endDialogue;
    public bool QuestEvent;

    public void Duplicate(CollectionQuest cq) {
        this.sourceID = cq.sourceID;
        this.id = cq.id;
        this.chainQuestID = cq.chainQuestID;
        this.colAmount = cq.colAmount;
        this.itemToCollect = cq.itemToCollect;
        this.title = cq.title;
        this.verb = cq.verb;
        this.description = cq.description;
        this.isOptional = cq.isOptional;
        this.startDialogue = cq.startDialogue;
        this.endDialogue = cq.endDialogue;
        this.QuestEvent = cq.QuestEvent;
    }

    public string GetGameObjectName() {
        return itemToCollect.name;
    }

    public void CheckProgress()
    {
        bool itemExist=false;
        for (int i = 0; i < PlayerData.instance.inventoryItem.Count; i++)
        {
            if (PlayerData.instance.inventoryItem[i].id == itemToCollect.id)
            {
                curAmount = PlayerData.instance.inventoryItem[i].quantity;
                itemExist = true;
                break;
            }
        }
        if (itemExist == false)
        {
            curAmount = 0;
        }

        if (curAmount >= colAmount)
        {
            isComplete = true;
            Debug.Log(title + " quest is complete");
        }
        else
        {
            isComplete = false;
        }
        CurrentQuestUI.instance.Refresh();
    }

    public void QuestComplete()
    {
        for (int i = 0; i < PlayerData.instance.inventoryItem.Count; i++)
        {
            if (PlayerData.instance.inventoryItem[i].id == itemToCollect.id)
            {
                PlayerData.instance.inventoryItem[i].quantity -= colAmount;
                Debug.Log("innn");
                break;
            }
        }

        try
        {
            for (int i = 0; i < chainQuestID.Count; i++)
                for (int j = 0; j < GameDataBase.instance.colQuestList.Length; j++)
                    if (chainQuestID[i] == GameDataBase.instance.colQuestList[j].id)
                    {
                        Quest.instance.collectionQuestActive.Add(GameDataBase.instance.colQuestList[j]);
                        break;
                    }
        }
        catch {
            Debug.Log("This quest doesnt have chain quest");
        }
        CurrentQuestUI.instance.Refresh();
    }

    public override string ToString()
    {
        return curAmount + "/" + colAmount + " " + itemToCollect.itemName + " " +verb + "ed.";
    }

    public void QuestCompleteEvent() {
        if (id == 3)
        {
            SpawnSlimeKing.instance.StartEvent();
        }
    }
}
