using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace CompleteProject
{
    public class PlayerInventory : MonoBehaviour
    {

        int inviSize = 3;
        Weapon[] inventoryArr;
        int currentSlot = 0;

        //UI elements
        public Image[] inviSlotUIArr = new Image[3];
        public Color inviHighlightColor = new Color(1f, 1f, 0f, 1f);
        public Color inviBaseColor = new Color(0f, 0f, 0f, 1f);
        public float unselectedAlpha = .3f;

        void Start()
        {
            inventoryArr = new Weapon[inviSize];
            inviSlotUIArr[currentSlot].color = inviHighlightColor;
        }

        private void Update()
        {
            if (Input.GetButtonDown("Inv1")) { currentSlot = SwapSlot(currentSlot, 0); }
            else if (Input.GetButtonDown("Inv2")) { currentSlot = SwapSlot(currentSlot, 1); }
            else if (Input.GetButtonDown("Inv3")) { currentSlot = SwapSlot(currentSlot, 2); }
            else if (Input.GetButtonDown("InvNext"))
            {
                if (currentSlot < inviSize - 1) { currentSlot = SwapSlot(currentSlot, currentSlot + 1); }
                else { currentSlot = SwapSlot(currentSlot, 0); }
            }
            else if (Input.GetButtonDown("InvLast"))
            {
                if (currentSlot > 0) { currentSlot = SwapSlot(currentSlot, currentSlot - 1); }
                else { currentSlot = SwapSlot(currentSlot, inviSize - 1); }
            }

            if (Input.GetButtonDown("Fire1")) { UseCurrentItem(); }
            if (Input.GetButtonDown("InvDrop")) { DropCurrentItem(); }
        }

        ////////////////////////////////// helpers //////////////////////////////////////////////////////////////

        //helper for processing the selecting of invi slots
        private int SwapSlot(int currentSlot, int nextSlot)
        {
            //set the UI
            Image currentSlotImage = inviSlotUIArr[currentSlot].GetComponentsInChildren<Image>()[1];
            Image nextSlotImage = inviSlotUIArr[nextSlot].GetComponentsInChildren<Image>()[1];

            inviSlotUIArr[currentSlot].color = inviBaseColor;
            inviSlotUIArr[nextSlot].color = inviHighlightColor;

            if (currentSlotImage.enabled) { SetImageAlpha(currentSlotImage, unselectedAlpha); }
            if (nextSlotImage.enabled) { SetImageAlpha(nextSlotImage, 1f); }

            //enable the weapon mesh, if it exists
            if (inventoryArr[currentSlot]) { inventoryArr[currentSlot].GetComponent<SkinnedMeshRenderer>().enabled = false; }
            if (inventoryArr[nextSlot]) { inventoryArr[nextSlot].GetComponent<SkinnedMeshRenderer>().enabled = true; }

            return nextSlot;
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

            Weapon thisWeapon = pickedItem.pickupWeapon.GetComponent<Weapon>();

            for (int i = 0; i < inviSize; i++)
            {
                Weapon inviWeapon = inventoryArr[i];

                //if a weapon exists in the current slot..
                if (inviWeapon)
                {
                    //and its of the same type with ammo space...
                    if (inviWeapon.wepName == thisWeapon.wepName && inviWeapon.AmmoNotFull())
                    {
                        inviWeapon.AddAmmo(pickedItem.pickupAmount);
                        inviSlotUIArr[i].GetComponentInChildren<Text>().text = inviWeapon.ReturnCurrentAmmo().ToString();
                        return true;
                    }
                    //and its of the same type with no ammo space...
                    else if (inviWeapon.wepName == thisWeapon.wepName && !inviWeapon.AmmoNotFull())
                    {
                        return false;
                    }
                }
                //if the item is not in the inventory and an empty slot hasn't been found yet...
                else if (!foundEmptySlot)
                {
                    print("found empty slot at " + firstEmptySlot.ToString());
                    foundEmptySlot = true;
                    firstEmptySlot = i;
                }
            }

            //if we found an empty slot and should add the weapon, get the instance and setup the HUD
            if (foundEmptySlot)
            {
                //find the reference to the weapon on the character
                Weapon[] childWeaponsArr = GetComponentsInChildren<Weapon>();
                Weapon weaponToAdd = null;

                //and add it to the inventory array, when found
                for (int k = 0; k < childWeaponsArr.Length; k++)
                {
                    if (childWeaponsArr[k].wepName == thisWeapon.wepName) { weaponToAdd = childWeaponsArr[k]; }
                }

                if (weaponToAdd) { inventoryArr[firstEmptySlot] = weaponToAdd; }
                else { Debug.Log("<color=red>Fatal error:</color> Pickup name didn't match any current weapons"); }
                weaponToAdd.AddAmmo(pickedItem.pickupAmount);

                //NOTE: because getcomponentsinchildren returns ALL components, including on self, 
                //      default to the second one found, which should always be the item icon.
                //      Should be a better way that doesn't depend on strict file structure.

                //initialize the UI icon
                Image SlotUIImage = inviSlotUIArr[firstEmptySlot].GetComponentsInChildren<Image>()[1];
                SlotUIImage.enabled = true;
                SlotUIImage.sprite = pickedItem.inviUIIcon;

                //if the current slot is the slot being added to, enable the weapon mesh
                if (currentSlot == firstEmptySlot)
                {
                    if (weaponToAdd.GetComponent<SkinnedMeshRenderer>()) { weaponToAdd.GetComponent<SkinnedMeshRenderer>().enabled = true; }
                    else { Debug.Log("<color=red>Fatal error:</color> Target asset misconfigured, missing skinned mesh renderer"); }
                }
                //otherwise, dim the icon
                else { SetImageAlpha(SlotUIImage, unselectedAlpha); }

                Text SlotUIText = inviSlotUIArr[firstEmptySlot].GetComponentInChildren<Text>();
                SlotUIText.enabled = true;
                SlotUIText.text = weaponToAdd.ReturnCurrentAmmo().ToString();

                return true;
            }

            return false;
        }

        void DropCurrentItem()
        {
            if (inventoryArr[currentSlot])
            {
                inventoryArr[currentSlot].ResetAmmo();
                inventoryArr[currentSlot].gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                inviSlotUIArr[currentSlot].GetComponentsInChildren<Image>()[1].enabled = false;
                inviSlotUIArr[currentSlot].GetComponentInChildren<Text>().enabled = false;
                inventoryArr[currentSlot] = null;
            }
            else { print("slot " + currentSlot + " already empty"); }
        }

        //use the weapon and update HUD ammo count
        void UseCurrentItem()
        {
            if (inventoryArr[currentSlot])
            {
                inventoryArr[currentSlot].UseWeapon();
                Text SlotUIText = inviSlotUIArr[currentSlot].GetComponentInChildren<Text>();

                //update ammo HUD if there's still ammo, otherwise remove the item from the inventory
                if ( inventoryArr[currentSlot].ReturnCurrentAmmo() > 0) { SlotUIText.text = inventoryArr[currentSlot].ReturnCurrentAmmo().ToString(); }
                else { DropCurrentItem(); }                
            }
            else { print("slot " + currentSlot + " is empty"); }
        }
    }
}
