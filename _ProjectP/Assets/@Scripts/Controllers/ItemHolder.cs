using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using UnityEngine;

public class ItemHolder : BaseObject
{
    //Owner
    //HolderSprite?
    //DespawnTime
    private ItemData _data;
    private SpriteRenderer _currentSprite;
    private ParabolaMotion _parabolaMotion;

    public override bool Init()
    {
        if (base.Init() == false) return false;

        ObjectType = Define.ObjectTypes.ItemHolder;
        _currentSprite = gameObject.GetOrAddComponent<SpriteRenderer>();
        _parabolaMotion = gameObject.GetOrAddComponent<ParabolaMotion>();

        return true;
    }

    public void SetInfo(int itemHolderId, int itemDataId, Vector2 pos)
    {
        _data = Managers.Data.ItemDic[itemDataId];
        _currentSprite.sprite = Managers.Resource.Load<Sprite>("Object_Meat.sprite"); //TODO
        _parabolaMotion.SetInfo(0, transform.position, pos, endCallback: Arrived);
    }

    private void Arrived()
    {
        _currentSprite.DOFade(0, 1f).OnComplete(() =>
        {
            if (_data != null)
            {
                //Acquire item
            }

            Managers.Object.Despawn(this);
        });
    }
}
