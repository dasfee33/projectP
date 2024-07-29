using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


public class Item
{
    public ItemSaveData SaveData { get; set; }

    public int InstanceId
    {
        get { return SaveData.InstanceId; }
        set { SaveData.InstanceId = value; }
    }

    public long DbId
    {
        get { return SaveData.DbId; }
    }

    public int TemplateId
    {
        get { return SaveData.TemplateId; }
        set { SaveData.TemplateId = value; }
    }

    public int Count
    {
        get { return SaveData.Count; }
        set { SaveData.Count = value; }
    }

    public int EquipSlot
    {
        get { return SaveData.EquipSlot; }
        set { SaveData.EquipSlot = value; }
    }

    public ItemData TemplateData
    {
        get
        {
            return Managers.Data.ItemDic[TemplateId];
        }
    }

    public ItemTypes ItemType { get; private set; }
    public ItemSubTypes SubType { get; private set; }

    public Item(int templateId)
    {
        TemplateId = templateId;
        ItemType = TemplateData.Type;
        SubType = TemplateData.SubType;
    }

    public virtual bool Init()
    {
        return true;
    }

    public static Item MakeItem(ItemSaveData itemInfo)
    {
        if (Managers.Data.ItemDic.TryGetValue(itemInfo.TemplateId, out ItemData itemData) == false)
            return null;

        Item item = null;

        switch (itemData.Type)
        {
            case ItemTypes.Weapon:
                item = new Equipment(itemInfo.TemplateId);
                break;
            case ItemTypes.Armor:
                item = new Equipment(itemInfo.TemplateId);
                break;
            case ItemTypes.Potion:
                item = new Consumable(itemInfo.TemplateId);
                break;
            case ItemTypes.Scroll:
                item = new Consumable(itemInfo.TemplateId);
                break;
        }

        if (item != null)
        {
            item.SaveData = itemInfo;
            item.InstanceId = itemInfo.InstanceId;
            item.Count = itemInfo.Count;
        }

        return item;
    }

    #region Helpers
    public bool IsEquippable()
    {
        return GetEquipItemEquipSlot() != EquipSlotTypes.None;
    }

    public EquipSlotTypes GetEquipItemEquipSlot()
    {
        if (ItemType == ItemTypes.Weapon)
            return EquipSlotTypes.Weapon;

        if (ItemType == ItemTypes.Armor)
        {
            switch (SubType)
            {
                case ItemSubTypes.Helmet:
                    return EquipSlotTypes.Helmet;
                case ItemSubTypes.Armor:
                    return EquipSlotTypes.Armor;
                case ItemSubTypes.Shield:
                    return EquipSlotTypes.Shield;
                case ItemSubTypes.Gloves:
                    return EquipSlotTypes.Gloves;
                case ItemSubTypes.Shoes:
                    return EquipSlotTypes.Shoes;
            }
        }

        return EquipSlotTypes.None;
    }

    public bool IsEquippedItem()
    {
        return SaveData.EquipSlot > (int)EquipSlotTypes.None && SaveData.EquipSlot < (int)EquipSlotTypes.EquipMax;
    }

    public bool IsInInventory()
    {
        return SaveData.EquipSlot == (int)EquipSlotTypes.Inventory;
    }

    public bool IsInWarehouse()
    {
        return SaveData.EquipSlot == (int)EquipSlotTypes.WareHouse;
    }
    #endregion
}

public class Equipment : Item
{
    public int Damage { get; private set; }
    public int Defence { get; private set; }
    public double Speed { get; private set; }

    protected EquipmentData EquipmentData { get { return (EquipmentData)TemplateData; } }

    public Equipment(int templateId) : base(templateId)
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if (TemplateData == null)
            return false;

        if (TemplateData.Type != ItemTypes.Armor || TemplateData.Type != ItemTypes.Weapon)
            return false;

        EquipmentData data = (EquipmentData)TemplateData;
        {
            Damage = data.Damage;
            Defence = data.Defence;
            Speed = data.Speed;
        }

        return true;
    }
}

public class Consumable : Item
{
    public double Value { get; private set; }

    public Consumable(int templateId) : base(templateId)
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if (TemplateData == null)
            return false;

        if (TemplateData.Type != ItemTypes.Potion || TemplateData.Type != ItemTypes.Scroll)
            return false;

        ConsumableData data = (ConsumableData)TemplateData;
        {
            Value = data.Value;
        }

        return true;
    }
}
