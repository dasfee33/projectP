using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_PlayersListPopup : UI_Popup
{
    enum GameObjects
    {
        CloseArea,
        EquippedHeroesList,
        WaitingHeroesList,
        UnownedHeroesList,
    }

    enum Texts
    {
        EquippedHeroesCountText,
        WaitingHeroesCountText,
        UnownedHeroesCountText,
    }

    enum Buttons
    {
        CloseButton,
    }

    List<UI_PlayersList_PlayerItem> _equippedHeroes = new List<UI_PlayersList_PlayerItem>();
    List<UI_PlayersList_PlayerItem> _waitingHeroes = new List<UI_PlayersList_PlayerItem>();
    List<UI_PlayersList_PlayerItem> _unownedHeroes = new List<UI_PlayersList_PlayerItem>();

    const int MAX_ITEM_COUNT = 100;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        {
            var parent = GetObject((int)GameObjects.EquippedHeroesList).transform;
            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                UI_PlayersList_PlayerItem item = Managers.UI.MakeSubItem<UI_PlayersList_PlayerItem>(parent);
                _equippedHeroes.Add(item);
            }
        }
        {
            var parent = GetObject((int)GameObjects.WaitingHeroesList).transform;
            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                UI_PlayersList_PlayerItem item = Managers.UI.MakeSubItem<UI_PlayersList_PlayerItem>(parent);
                _waitingHeroes.Add(item);
            }
        }
        {
            var parent = GetObject((int)GameObjects.UnownedHeroesList).transform;
            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                UI_PlayersList_PlayerItem item = Managers.UI.MakeSubItem<UI_PlayersList_PlayerItem>(parent);
                _unownedHeroes.Add(item);
            }
        }

        Refresh();

        return true;
    }

    public void SetInfo()
    {
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        GetText((int)Texts.EquippedHeroesCountText).text = $"{Managers.Game.PickedPlayerCount} / ??";
        GetText((int)Texts.WaitingHeroesCountText).text = $"{Managers.Game.OwnedPlayerCount} / ??";
        GetText((int)Texts.UnownedHeroesCountText).text = $"{Managers.Game.UnownedPlayerCount} / ??";

        Refresh_Hero(_equippedHeroes, PlayerOwningState.Picked);
        Refresh_Hero(_waitingHeroes, PlayerOwningState.Owned);
        Refresh_Hero(_unownedHeroes, PlayerOwningState.Unowned);
    }

    void Refresh_Hero(List<UI_PlayersList_PlayerItem> list, PlayerOwningState owningState)
    {
        List<PlayerSaveData> heroes = Managers.Game.AllPlayers.Where(h => h.OwningState == owningState).ToList();

        for (int i = 0; i < list.Count; i++)
        {
            if (i < heroes.Count)
            {
                PlayerSaveData hero = heroes[i];
                list[i].SetInfo(hero.DataId);
                list[i].gameObject.SetActive(true);
            }
            else
            {
                list[i].gameObject.SetActive(false);
            }
        }
    }

    void OnClickCloseArea(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickCloseButton(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI(this);
    }


}
