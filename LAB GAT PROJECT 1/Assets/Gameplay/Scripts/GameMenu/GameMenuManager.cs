﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    public Inventory inventory;
    public Quest quest;
    public InventoryBox inventoryBox;
    public PlayerData playerData;
    public InputSetup inputSetup;
    public bool isOpen;
    public bool cantOpenMenu;

    [Header("Input Settings")]
    public Vector3 inputAxis;
    public bool pointerInputHold;
    public bool buttonInputHold;

    [Header("Menu Pointer")]
    public int menuNumber;//0 inventory, 1 quest    
    public Color32 normalColor;
    public Color32 markColor;
    public Color32 selectedColor;
    public GameObject[] menuPointer;
    public GameObject[] itemBoxPointer;

    public void Start()
    {
        cantOpenMenu = true;
        inputSetup = GameObject.FindGameObjectWithTag("InputSetup").GetComponent<InputSetup>();
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        quest = GameObject.FindGameObjectWithTag("Quest").GetComponent<Quest>();
        inventoryBox = GameObject.FindGameObjectWithTag("InventoryBox").GetComponent<InventoryBox>();
        playerData = GameObject.FindGameObjectWithTag("PlayerData").GetComponent<PlayerData>();
        pointerInputHold = false;
        buttonInputHold = false;

        if (playerData.DEVELOPERMODE == true)
        {
            cantOpenMenu = false;
        }

        ResetMenu();
    }

    private void Update()
    {
        //press start
        OpenCloseMenu();

        if (GameStatus.IsPaused == true && isOpen == true)
        {
            GetInputAxis();
            SelectMenu();
            PointerInput();
            ButtonInput();
        }
    }

    void OpenCloseMenu() {
        if (GameStatus.isTalking == false && cantOpenMenu == false)
        {
            if (Input.GetKeyDown(inputSetup.openGameMenu))
            {
                if (inventory.inventoryView.activeSelf == false && quest.questView.activeSelf == false)
                {
                    OpenMenu();
                }
                //gabisa pake else, soalnya kalo buka item box, inventoryview.activeselfnya nyala
                else if (inventory.inventoryView.activeSelf == true && quest.questView.activeSelf == true && inventory.isSwapping == false)
                {
                    CloseMenu();
                }
            }
            if (Input.GetKeyDown(inputSetup.back))
            {                
                if (inventory.inventoryView.activeSelf == true && quest.questView.activeSelf == true && inventory.isSwapping == false)
                {
                    CloseMenu();
                }
                else if (inventory.inventoryView.activeSelf == true && quest.questView.activeSelf == true && inventory.isSwapping == true)
                {
                    inventory.ResetInventorySwap();
                }
            }
        }
    }

    void PointerInput() {
        if (pointerInputHold == false)
        {
            if ((inputAxis.y == 1 || inputAxis.y == -1 || inputAxis.x == 1 || inputAxis.x == -1) && inventory.isSettingQuantity == false && inventoryBox.isSettingQuantity == false)
            {
                StartCoroutine(PointerInputHold());
                ApplyNavigation();
            }
            //set quantity ke item box (put value)
            else if ((inputAxis.x == 1 || inputAxis.x == -1) && inventory.isSettingQuantity == true)
            {
                StartCoroutine(PointerInputHold());
                if (inputAxis.x == 1)
                {
                    inventory.IncreaseQuantityToPut();
                }
                if (inputAxis.x == -1)
                {
                    inventory.DecreaseQuantityToPut();
                }
            }
            else if ((inputAxis.x == 1 || inputAxis.x == -1) && inventoryBox.isSettingQuantity == true)
            {
                StartCoroutine(PointerInputHold());
                if (inputAxis.x == 1)
                {
                    inventoryBox.IncreaseQuantityToPut();
                }
                if (inputAxis.x == -1)
                {
                    inventoryBox.DecreaseQuantityToPut();
                }
            }
        }
    }

    void ButtonInput() {
        if (buttonInputHold == false)
        {
            if (inventoryBox.isItemBoxOpened == false)
            {
                ButtonOnStartMenu();
            }
            else if(inventoryBox.isItemBoxOpened == true)
            {
                ButtonOnInventoryBox();
            }
        }
    }

    void ButtonOnStartMenu() {
        if (menuNumber == 0) //inventory
        {
            inventory.InventorySwapping();
        }
        if (menuNumber == 1)
        {

        }
    }

    void ButtonOnInventoryBox() {
        if (menuNumber == 0) //inventory
        {
            if (inventory.isSettingQuantity == false)
            {
                inventory.InventorySwapping();
            }
            inventory.PutInventory();
        }
        if (menuNumber == 1)//itemBox
        {
            if (inventoryBox.isSettingQuantity == false)
            {
                inventoryBox.InventoryBoxSwapping();
            }
            inventoryBox.PutInventory();
        }

        if (Input.GetKeyDown(inputSetup.back))
        {
            if (inventoryBox.isItemBoxOpened == true)
            {
                if (inventory.isSwapping == true)
                {
                    inventory.ResetInventorySwap();
                }
                else if (inventoryBox.isSwapping == true)
                {
                    inventoryBox.ResetInventoryBoxSwap();
                }
                else if (inventory.isSettingQuantity == true)
                {
                    inventory.slider.SetActive(false);
                    inventory.isSettingQuantity = false;
                }
                else if (inventoryBox.isSettingQuantity == true)
                {
                    inventoryBox.slider.SetActive(false);
                    inventoryBox.isSettingQuantity = false;
                }
                else
                {
                    CloseInventoryBoxMenu();
                }
            }
        }
    }

    void OpenMenu() {
        inventory.inventoryView.SetActive(true);
        quest.questView.SetActive(true);
        quest.RefreshQuest();
        isOpen = true;
        ResetMenu();
        GameStatus.PauseGame();
    }

    void CloseMenu() {
        inventory.inventoryView.SetActive(false);
        quest.questView.SetActive(false);
        isOpen = false;
        InputHolder.isInputHolded = true;
        GameStatus.ResumeGame();
    }

    void CloseInventoryBoxMenu() {
        inventory.inventoryView.SetActive(false);
        inventoryBox.inventoryBoxView.SetActive(false);
        inventoryBox.isItemBoxOpened = false;
        isOpen = false;
        ResetMenu();
        InputHolder.isInputHolded = true;
        GameStatus.ResumeGame();
        GameStatus.ResumeMove();
    }

    void SelectMenu()
    {
        if (inventoryBox.isItemBoxOpened == false && inventory.isSwapping==false )
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button5))
            {
                if (menuNumber < menuPointer.Length - 1)
                {
                    menuNumber++;
                }
                ResetPointer();
            }
            if (Input.GetKeyDown(KeyCode.Joystick1Button4))
            {
                if (menuNumber > 0)
                {
                    menuNumber--;
                }
                ResetPointer();
            }
        }
        else if (inventoryBox.isItemBoxOpened == true && inventory.isSwapping == false && inventoryBox.isSwapping == false &&
            inventory.isSettingQuantity== false && inventoryBox.isSettingQuantity == false)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button5))
            {
                if (menuNumber < itemBoxPointer.Length - 1)
                {
                    menuNumber++;
                }
                ResetPointer();
            }
            if (Input.GetKeyDown(KeyCode.Joystick1Button4))
            {
                if (menuNumber > 0)
                {
                    menuNumber--;
                }
                ResetPointer();
            }
        }
    }

    void ApplyNavigation()
    {
        if (inventoryBox.isItemBoxOpened == false)
        {
            if (menuNumber == 0) //inven
            {
                inventory.InventorySelection();
            }
            else if (menuNumber == 1) //quest
            {
                quest.QuestSelection();
            }
        }
        else
        {
            if (menuNumber == 0) //inven
            {
                inventory.InventorySelection();
            }
            else if (menuNumber == 1) //itembox
            {
                //itembox selection
                inventoryBox.InventoryBoxSelection();
            }
        }
    }

    #region UTIL
    void GetInputAxis()
    {
        inputAxis.y = Input.GetAxisRaw("D-Pad Up");
        inputAxis.x = Input.GetAxisRaw("D-Pad Right");
    }
    public IEnumerator PointerInputHold()
    {
        pointerInputHold = true;
        yield return new WaitForSeconds(0.15f);
        pointerInputHold = false;
    }
    public IEnumerator ButtonInputHold()
    {
        buttonInputHold = true;
        yield return new WaitForSeconds(0.15f);
        buttonInputHold = false;
    }
    public void ResetPointer()
    {
        if (inventoryBox.isItemBoxOpened == false)
        {
            for (int i = 0; i < menuPointer.Length; i++)
            {
                menuPointer[i].SetActive(false);
            }
            menuPointer[menuNumber].SetActive(true);
        }
        else
        {
            for (int i = 0; i < itemBoxPointer.Length; i++)
            {
                itemBoxPointer[i].SetActive(false);
            }
            itemBoxPointer[menuNumber].SetActive(true);
        }
    }
    public void ResetMenu()
    {
        menuNumber = 0;
        ResetPointer();

        //reset inventory pointer
        inventory.inventoryColumnIndex = 0;
        inventory.inventoryRowIndex = 0;
        inventory.MarkInventory();
        inventoryBox.MarkInventoryBox();

        //reset quest pointer
        quest.questIndex = 0;
        quest.ScrollQuest();
        quest.MarkQuest();
        quest.RefreshQuest();
    }
    #endregion

}