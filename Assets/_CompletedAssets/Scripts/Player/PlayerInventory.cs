using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

    int inviSize = 3;
    int[] inventoryArr;
    string[] tempInviNameArr; 
    int currentSlot  = 0;

    //UI elements
    public Image[] inviSlotUIArr = new Image[3];
    public Color inviHighlightColor = new Color(1f, 1f, 0f, 1f);
    public Color inviBaseColor = new Color(0f, 0f, 0f, 1f);
    public float unselectedAlpha = .3f;
    
    //so  that inventory items can affect these? maybe not necessary, should be done on actual items
    PlayerHealth playerHealth;
    PlayerShooting playerShooting;
    PlayerMovement playerMovement;

    private void Awake()
    {
        playerHealth = GetComponent < PlayerHealth > ();
        playerShooting = GetComponent < PlayerShooting > ();
        playerMovement = GetComponent <PlayerMovement> ();
    }

    void Start ()
    {
        inventoryArr = new int[inviSize];
        /////////////////temporary
        tempInviNameArr = new string[inviSize];
        ////////////////
        inviSlotUIArr[currentSlot].color = inviHighlightColor;

        //temporary
        for (int i = 0; i < inviSize; i++)
        {
            inventoryArr[i] = 0;
        }
	}
    
    private void Update()
    {
        if (Input.GetButtonDown("Inv1"))          { currentSlot = HighlightSlot( currentSlot, 0 ); }
        else if (Input.GetButtonDown("Inv2"))     { currentSlot = HighlightSlot( currentSlot, 1 ); }
        else if (Input.GetButtonDown("Inv3"))     { currentSlot = HighlightSlot( currentSlot, 2 ); }
        else if (Input.GetButtonDown("InvNext"))
        {
            if (currentSlot < inviSize - 1) { currentSlot = HighlightSlot(currentSlot, currentSlot + 1 ); }
            else                            { currentSlot = HighlightSlot(currentSlot, 0 ); }
        }
        else if (Input.GetButtonDown("InvLast"))
        {
            if (currentSlot > 0)            { currentSlot = HighlightSlot(currentSlot, currentSlot - 1 ); }
            else                            { currentSlot = HighlightSlot(currentSlot, inviSize - 1 ); }
        }

        if (Input.GetButtonDown("InvDrop")) { DropCurrentItem(); }
        //if (Input.GetButtonDown("Fire2")) { UseCurrentItem(); }
    }

    ////////////////////////////////// helpers //////////////////////////////////////////////////////////////

    private int HighlightSlot( int currentHlt, int nextHlt)
    {
        Image currentSlotImage = inviSlotUIArr[currentHlt].GetComponentsInChildren<Image>()[1];
        Image nextSlotImage = inviSlotUIArr[nextHlt].GetComponentsInChildren<Image>()[1];

        inviSlotUIArr[currentHlt].color = inviBaseColor;
        inviSlotUIArr[nextHlt].color = inviHighlightColor;

        if (currentSlotImage.enabled) { SetImageAlpha(currentSlotImage, unselectedAlpha); }
        if (nextSlotImage.enabled) { SetImageAlpha(nextSlotImage, 1f); }          

        return nextHlt;
    }

    private void SetImageAlpha(Image SlotUIImage, float newAlpha)
    {
        Color newAlphaColor = SlotUIImage.color;
        newAlphaColor.a = newAlpha;
        SlotUIImage.color = newAlphaColor;
    }

    ////////////////////////////////// inventory management /////////////////////////////////////////////////

    //update inventory and inventory UI; return a bool to show the success or failure of the addition
    public bool PickupItem(Pickup pickedItem)
    {
        int firstEmptySlot = 0;
        bool foundEmptySlot = false;
        
        for (int i = 0; i < inviSize; i++)
        {   
            //if the item is already in the inventory...
            if (tempInviNameArr[i] == pickedItem.itemName && pickedItem.CanAdd(inventoryArr[i]))
            {
                inventoryArr[i] += pickedItem.pickupAmount;
                //snip any overflow cause by this pickup
                if (inventoryArr[i] > pickedItem.pickupMax) { inventoryArr[i] = pickedItem.pickupMax; }
                inviSlotUIArr[i].GetComponentInChildren<Text>().text = inventoryArr[i].ToString();
                return true;
            }
            //if the item is in the inventory and inventory is full...
            else if (tempInviNameArr[i] == pickedItem.itemName && !pickedItem.CanAdd(inventoryArr[i]))
            {
                return false;
            }
            //if the item is not in the inventory and an empty slot hasn't been found yet...
            else if (tempInviNameArr[i] == null && !foundEmptySlot)
            {
                print("found empty slot at " + firstEmptySlot.ToString());
                foundEmptySlot = true;
                firstEmptySlot = i;
            }
        }
        //if we found an empty slot...
        if (foundEmptySlot)
        {
            print("Adding to slot " + firstEmptySlot.ToString());
            tempInviNameArr[firstEmptySlot] = pickedItem.itemName;

            //initialize the inventory HUD slot
            inventoryArr[firstEmptySlot] = inventoryArr[firstEmptySlot] + pickedItem.pickupAmount;

            //NOTE: because getcomponentsinchildren returns ALL components, including on self, default to the second one found, which should always be the item icon
            //should be a better way that doesn't depend on strict file structure
            Image SlotUIImage = inviSlotUIArr[firstEmptySlot].GetComponentsInChildren<Image>()[1];
            SlotUIImage.enabled = true;
            SlotUIImage.sprite = pickedItem.inviUIIcon;

            if (currentSlot != firstEmptySlot) { SetImageAlpha(SlotUIImage, unselectedAlpha); }

            Text SlotUIText = inviSlotUIArr[firstEmptySlot].GetComponentInChildren<Text>();
            SlotUIText.enabled = true;
            SlotUIText.text = inventoryArr[firstEmptySlot].ToString();

            return true;
        }


        return false;
    }

    void DropCurrentItem()
    {
        if (inventoryArr[currentSlot] != 0)
        {
            inventoryArr[currentSlot] = 0;
            tempInviNameArr[currentSlot] = null;
            inviSlotUIArr[currentSlot].GetComponentsInChildren<Image>()[1].enabled = false;
            inviSlotUIArr[currentSlot].GetComponentInChildren<Text>().enabled = false;
        }
        else { print("slot " + currentSlot + " already empty"); }
    }
}
