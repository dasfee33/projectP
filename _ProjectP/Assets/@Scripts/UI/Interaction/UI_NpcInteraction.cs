using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_NpcInteraction : UI_Base
{
    private Npc _owner;

    enum Buttons
    {
        InteractionButton
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));

        GetComponent<Canvas>().worldCamera = Camera.main;

        return true;
    }

    public void SetInfo(int dataId, Npc owner)
    {
        _owner = owner;
        GetButton((int)Buttons.InteractionButton).gameObject.BindEvent(OnClickInteractionButton);
    }

    private void OnClickInteractionButton(PointerEventData evt)
    {
         switch (_owner.Data.NpcType)
        {
            case Define.NpcTypes.Camp:
                break;
            case Define.NpcTypes.Portal:
                break;
            case Define.NpcTypes.Waypoint:
                Managers.UI.ShowPopupUI<UI_WaypointPopup>();
                break;
            case Define.NpcTypes.BlackSmith:
                break;
            case Define.NpcTypes.Guild:
                break;
            case Define.NpcTypes.TreasureBox:
                break;
            case Define.NpcTypes.Dungeon:
                break;
            default:
				break;
		}

        Debug.Log("OnClickInteractionButton");
    }
}
