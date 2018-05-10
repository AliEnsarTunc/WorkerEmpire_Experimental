﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotType
{
    All,
    MainHand,
    OffHand,
    Head,
    Body,
    Leg,
    Feet,
    Ring,
    Amulet
}

public class Slot : MonoBehaviour, IPointerClickHandler 
{    
    public GameObject HeldItem;
    public string InventoryName;
    public SlotType SlotType;
    public int SlotNumber;

    static GameObject OldSlot;
    static GameObject ItemInMotion;

    private void Update()
    {

    }


    
    void OnTransformChildrenChanged()
    {
        if (SlotNumber == -99) // Don't do anything if it's the trash can
            return;
        
        if (transform.childCount > 0)
        {
            HeldItem = FirstChildWithItemObject();
            if (HeldItem != null)
            {
                HeldItem.GetComponent<RectTransform>().localPosition = Vector2.zero;
                HeldItem.GetComponent<RectTransform>().localScale = Vector3.one;

                var iod = HeldItem.GetComponent<ItemObject>().ItemObjectData;
                var inventory = InventoryManager.Instance.GetInventoryByName(InventoryName);

                var oldSlotNumber = iod.SlotNumber;
                var oldInventoryName = iod.InventoryName;

                iod.SlotNumber = SlotNumber;
                iod.InventoryName = InventoryName;

                if (!(oldSlotNumber == SlotNumber && oldInventoryName == iod.InventoryName))
                {
                    if (oldSlotNumber == -77) // -77 means we spawned an item?
                    {
                        inventory.AddItemToSlot(SlotNumber, iod);
                    }
                    else if (inventory.Items.ContainsKey(SlotNumber)) //Swap
                    {
                        inventory.AddItemToSlot(SlotNumber, iod);
                    }
                    else if (!inventory.Items.ContainsKey(SlotNumber)) //move
                    {
                        inventory.AddItemToSlot(SlotNumber, iod);
                        InventoryManager.Instance.GetInventoryByName(oldInventoryName).Items.Remove(oldSlotNumber);
                    }
                }
            }
        }
        else
        {
            HeldItem = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // If currently there's an item in motion, motion means we are trying to move an item by clicking on it
        if (ItemInMotion != null)
        {
            // If the target slot is empty
            if (HeldItem == null) 
            {
                if (CanMove(gameObject, ItemInMotion))
                {
                    HeldItem = ItemInMotion;
                    HeldItem.transform.SetParent(transform);
                    ItemInMotion = null;
                    //Clear the color of previous slot
                    OldSlot.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                }
            }
            // If the target slot is not empty
            // We do the item swap here
            else
            {
                if (OldSlot != gameObject)
                {
                    if (CanSwap(gameObject, OldSlot, ItemInMotion))
                    {
                        ItemInMotion.transform.parent = null;
                        HeldItem.transform.SetParent(OldSlot.transform);
                        ItemInMotion.transform.SetParent(gameObject.transform);
                    }
                }

                OldSlot.GetComponent<Image>().color = new Color32(255,255,255,255);
                ItemInMotion = null;
            }
        }
        // If there's an item in slot
        else if (HeldItem != null) 
        {
            // If there's no item in motion when we click on a slot with an item in it
            if (ItemInMotion == null) 
            {
                GetComponent<Image>().color = new Color32(50, 0, 0, 120);
                ItemInMotion = HeldItem;
                OldSlot = gameObject;
            }
        }
    }

    //Check if the target and old slot is capable of swapping item in slot with item in motion.
    bool CanSwap(GameObject slot, GameObject oldSlot, GameObject item)
    {
        var oldSlotType = oldSlot.GetComponent<Slot>().SlotType;
        var itemSlotType = item.GetComponent<ItemObject>().ItemObjectData.item.slotType();
        
        if (SlotType == SlotType.All && oldSlotType == SlotType.All)
            return true;
        if (SlotType == itemSlotType && oldSlotType == itemSlotType)
            return true;
        if (slot.GetComponent<Slot>().HeldItem.GetComponent<ItemObject>().ItemObjectData.item.slotType() == itemSlotType)
            return true;

        return false;
    }
    //Check if the target slot accepts the item in motion
    bool CanMove(GameObject slot, GameObject item)
    {
        if (SlotType == SlotType.All)
            return true;
        if (SlotType == item.GetComponent<ItemObject>().ItemObjectData.item.slotType())
            return true;

        return false;
    }

    GameObject FirstChildWithItemObject()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).tag == "ItemObject")
            {
                return transform.GetChild(i).gameObject;
            }
        }
        return null;
    }

}
