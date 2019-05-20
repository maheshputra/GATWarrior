﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBox : MonoBehaviour
{
    public PlayerData playerData;
    public InputSetup inputSetup;
    public Inventory inventory;
    public GameMenuManager gameMenuManager;
    public UsableItem usableItem;

    [Header("UI Settings")]
    public GameObject inventoryBoxUI;
    public GameObject inventoryBoxView;
    public GameObject inventoryBoxViewPort;
    public GameObject inventoryBoxContent;
    public Scrollbar inventoryBoxScrollbar;

    public InventoryIndicator[] inventoryBoxIndicator;
    public InventoryIndicator[,] inventoryBoxIndicatorPos;
    public int inventoryBoxColumn;
    public int inventoryBoxRow;
    public int inventoryBoxContentChildCount;
    public int inventoryBoxColumnIndex;
    public int inventoryBoxRowIndex;

    //swap
    public InventoryIndicator invenSwap1;
    public InventoryIndicator invenSwap2;
    public Item itemSwap1;
    public Item itemSwap2;
    public bool isSwapping;
    public Item temporaryItem;

    //put ke inven
    public bool isSettingQuantity;
    public GameObject slider;
    public Slider inventoryBoxQuantitySlider;

    private void Awake()
    {
        playerData = GameObject.FindGameObjectWithTag("PlayerData").GetComponent<PlayerData>();
        inputSetup = GameObject.FindGameObjectWithTag("InputSetup").GetComponent<InputSetup>();
        gameMenuManager = GameObject.FindGameObjectWithTag("GameMenuManager").GetComponent<GameMenuManager>();
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        usableItem = GameObject.FindGameObjectWithTag("UsableItem").GetComponent<UsableItem>();

        inventoryBoxUI = GameObject.FindGameObjectWithTag("InventoryBoxUI");
        inventoryBoxView = inventoryBoxUI.transform.Find("InventoryBoxView").gameObject;
        inventoryBoxViewPort = inventoryBoxView.transform.Find("InventoryBoxViewPort").gameObject;
        inventoryBoxContent = inventoryBoxViewPort.transform.Find("InventoryBoxContent").gameObject;
        inventoryBoxScrollbar = inventoryBoxView.transform.Find("InventoryBoxScrollbar").GetComponent<Scrollbar>();

        slider = inventoryBoxView.transform.Find("Slider").gameObject;
        inventoryBoxQuantitySlider = slider.transform.Find("InventoryBoxQuantitySlider").GetComponent<Slider>();

        inventoryBoxContentChildCount = inventoryBoxContent.transform.childCount;
        inventoryBoxColumn = inventoryBoxContent.GetComponent<GridLayoutGroup>().constraintCount;
        inventoryBoxRow = inventoryBoxContentChildCount / inventoryBoxColumn;
        inventoryBoxIndicatorPos = new InventoryIndicator[inventoryBoxRow, inventoryBoxColumn];
        inventoryBoxColumnIndex = 0;
        inventoryBoxRowIndex = 0;

        int c = 0;
        inventoryBoxIndicator = new InventoryIndicator[inventoryBoxContent.transform.childCount];
        for (int i = 0; i < inventoryBoxRow; i++)
        {
            for (int j = 0; j < inventoryBoxColumn; j++)
            {
                inventoryBoxIndicatorPos[i, j] = inventoryBoxContent.transform.GetChild(c).GetComponent<InventoryIndicator>();
                inventoryBoxIndicator[c] = inventoryBoxIndicatorPos[i, j].GetComponent<InventoryIndicator>();
                c++;
            }
        }
    }

    public void Start()
    {
        inventoryBoxView.SetActive(false);
        slider.SetActive(false);
        RefreshInventoryBox();
    }

    public void InventoryBoxSelection()
    {
        if (gameMenuManager.inputAxis.y == -1) // kebawah
        {
            if (inventoryBoxRowIndex < inventoryBoxRow - 1)
            {
                inventoryBoxRowIndex++;
            }
            else
            {
                inventoryBoxRowIndex = 0;
            }
        }
        else if (gameMenuManager.inputAxis.y == 1) // keatas
        {
            if (inventoryBoxRowIndex > 0)
            {
                inventoryBoxRowIndex--;
            }
            else
            {
                inventoryBoxRowIndex = inventoryBoxRow - 1;
            }
        }

        if (gameMenuManager.inputAxis.x == -1) // kekiri
        {
            if (inventoryBoxColumnIndex > 0)
            {
                inventoryBoxColumnIndex--;
            }
            else
            {
                inventoryBoxColumnIndex = inventoryBoxColumn - 1;
            }
        }
        else if (gameMenuManager.inputAxis.x == 1) //kekanan
        {
            if (inventoryBoxColumnIndex < inventoryBoxColumn - 1)
            {
                inventoryBoxColumnIndex++;
            }
            else
            {
                inventoryBoxColumnIndex = 0;
            }
        }
        MarkInventoryBox();
        ScrollInventoryBox();
    }

    public void MarkInventoryBox()
    {
        for (int i = 0; i < inventoryBoxRow; i++)
        {
            for (int j = 0; j < inventoryBoxColumn; j++)
            {
                inventoryBoxIndicatorPos[i, j].markIndicator.SetActive(false);
            }
        }
        inventoryBoxIndicatorPos[inventoryBoxRowIndex, inventoryBoxColumnIndex].markIndicator.SetActive(true);
    }

    public void ScrollInventoryBox()
    {
        if (inventoryBoxRowIndex == inventoryBoxRow - 1)
        {
            inventoryBoxScrollbar.value = 0;
        }
        else
        {
            float a = (float)inventoryBoxRow;
            float b = (float)inventoryBoxRowIndex;
            float c = a - b;
            float d = c / a;
            inventoryBoxScrollbar.value = c / a;
        }
    }

    public void InventoryBoxSwapping()
    {
        if (!isSwapping)
        {
            Select1stItem();
        }
        else if (isSwapping)
        {
            Select2ndItem();
            Swap();
            RefreshInventoryBox();
            ResetInventoryBoxSwap();
        }
    }

    void Select1stItem() {
        invenSwap1 = inventoryBoxIndicatorPos[inventoryBoxRowIndex, inventoryBoxColumnIndex].GetComponent<InventoryIndicator>();
        if (invenSwap1.item != null)
        {
            itemSwap1 = invenSwap1.item;
        }
        else
            itemSwap1 = null;


        invenSwap1.selectIndicator.SetActive(true);
        isSwapping = true;
        StartCoroutine(gameMenuManager.ButtonInputHold());
    }

    void Select2ndItem() {
        invenSwap2 = inventoryBoxIndicatorPos[inventoryBoxRowIndex, inventoryBoxColumnIndex];
        if (invenSwap2.item != null)
        {
            itemSwap2 = invenSwap2.item;
        }
        else
            itemSwap2 = null;
    }

    void Swap() {
        if (itemSwap1 != null && itemSwap2 != null)
        {
            invenSwap1.item = itemSwap2;
            invenSwap2.item = itemSwap1;
        }
        else if (itemSwap1 == null && itemSwap2 != null)
        {
            invenSwap1.item = itemSwap2;
            invenSwap2.item = null;
        }
        else if (itemSwap1 != null && itemSwap2 == null)
        {
            invenSwap1.item = null;
            invenSwap2.item = itemSwap1;
        }
    }

    public void ResetInventoryBoxSwap()
    {
        invenSwap1.selectIndicator.SetActive(false);
        isSwapping = false;
        StartCoroutine(gameMenuManager.ButtonInputHold());
        MarkInventoryBox();
    }

    public void PutIntoInventory()
    {
        if (!isSettingQuantity && !isSwapping)
        {
            SetInitialQuantityToPut();
        }
        else if (isSettingQuantity)
        {
            inventory.PlaceItem(temporaryItem, (int)inventoryBoxQuantitySlider.value);
            slider.SetActive(false);
            isSettingQuantity = false;
        }
    }

    public void SetInitialQuantityToPut()
    {
        if (inventoryBoxIndicatorPos[inventoryBoxRowIndex, inventoryBoxColumnIndex].item != null)
        {
            bool thereIsSpace = false;
            inventoryBoxQuantitySlider.value = 1;
            temporaryItem = inventoryBoxIndicatorPos[inventoryBoxRowIndex, inventoryBoxColumnIndex].item;

            for (int i = 0; i < playerData.inventoryItem.Count; i++)
            {
                if (playerData.inventoryItem[i].id == temporaryItem.id)
                    if (playerData.inventoryItem[i].quantity == playerData.inventoryItem[i].maxQuantityOnInventory)
                    {
                        Debug.Log("you cannot carry more " + temporaryItem.itemName);
                        return;
                    }
                    else
                    {
                        slider.SetActive(true);
                        inventoryBoxQuantitySlider.minValue = 1;
                        inventoryBoxQuantitySlider.maxValue = temporaryItem.quantity;
                        if(temporaryItem.quantity > (playerData.inventoryItem[i].maxQuantityOnInventory - playerData.inventoryItem[i].quantity))
                            inventoryBoxQuantitySlider.maxValue = (playerData.inventoryItem[i].maxQuantityOnInventory - playerData.inventoryItem[i].quantity);

                        inventoryBoxQuantitySlider.transform.Find("Text").GetComponent<Text>().text = inventoryBoxQuantitySlider.value.ToString();
                        isSettingQuantity = true;
                        return;
                    }
            }

            for (int i = 0; i < inventory.inventoryIndicator.Length; i++)
            {
                if (inventory.inventoryIndicator[i].item == null)
                    thereIsSpace = true;
            }

            if (!thereIsSpace)
                return;

            slider.SetActive(true);
            inventoryBoxQuantitySlider.minValue = 1;
            inventoryBoxQuantitySlider.maxValue = temporaryItem.quantity;
            inventoryBoxQuantitySlider.transform.Find("Text").GetComponent<Text>().text = inventoryBoxQuantitySlider.value.ToString();
            isSettingQuantity = true;
        }
        else
        {
            Debug.Log("no item");
        }
    }

    public void IncreaseQuantityToPut()
    {
        inventoryBoxQuantitySlider.value += 1;
        inventoryBoxQuantitySlider.transform.Find("Text").GetComponent<Text>().text = inventoryBoxQuantitySlider.value.ToString();
    }

    public void DecreaseQuantityToPut()
    {
        inventoryBoxQuantitySlider.value -= 1;
        inventoryBoxQuantitySlider.transform.Find("Text").GetComponent<Text>().text = inventoryBoxQuantitySlider.value.ToString();
    }

    public void PlaceItem(Item newItem, int quantity)
    {
        bool isItemExist = false;
        //check if item exist or not
        for (int i = 0; i < playerData.inventoryBoxItem.Count; i++)
        {
            if (playerData.inventoryBoxItem[i].id == newItem.id)
            {
                isItemExist = true;
                //quantity item ditambah quantity baru
                playerData.inventoryBoxItem[i].quantity += quantity;
                newItem.quantity -= quantity;
                Debug.Log(playerData.inventoryBoxItem[i].quantity);
                Debug.Log(newItem.quantity);
                Debug.Log("exist item in box");
                break;
            }
        }

        if (isItemExist == false)
        {
            Item newItemInBox = new Item(newItem.id, newItem.itemImage, newItem.itemName,
                newItem.description, newItem.maxQuantityOnInventory, newItem.price, newItem.isUsable, 
                newItem.isConsumable, newItem.isASingleTool, newItem.itemType);
            if (newItem.itemType != null)
                if (newItem.itemType.ToLower().Equals("seed".ToLower()))
                    newItemInBox.plantID = newItem.plantID;

            newItemInBox.quantity = quantity;
            playerData.inventoryBoxItem.Add(newItemInBox);
            newItem.quantity -= quantity;
        }

        //check inventory ke quest
        for (int i = 0; i < playerData.inventoryItem.Count; i++)
        {
            if (playerData.inventoryItem[i].id == newItem.id)
            {
                playerData.CheckNewItem(playerData.inventoryItem[i]);
            }
        }
        inventory.RefreshInventory();
        RefreshInventoryBox();
    }

    public void RefreshInventoryBox()
    {
        for (int i = 0; i < inventoryBoxIndicator.Length; i++)
        {
            if (inventoryBoxIndicator[i].itemID == 0)
            {
                try
                {
                    for (int j = 0; j < playerData.inventoryBoxItem.Count; j++)
                    {
                        if (playerData.inventoryBoxItem[j].isOnItemBox == false)
                        {
                            inventoryBoxIndicator[i].item = playerData.inventoryBoxItem[j];
                            inventoryBoxIndicator[i].itemID = playerData.inventoryBoxItem[j].id;
                            playerData.inventoryBoxItem[j].isOnItemBox = true;
                            break;
                        }
                    }
                }
                catch
                {
                    //Debug.Log("There is no item in " + i + " inventory box.");
                }
            }
            inventoryBoxIndicator[i].RefreshInventoryBox();
        }
        usableItem.GetUsableItem();
    }
}
