using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemons;
    PokemonParty party;
    int selection = 0;
    public Pokemon SelectedMember => pokemons[selection];
    public BattleState? CalledFrom { get; set; }  //回合制狀態機

    /*Override 讓格子可以使用*/
    /*用靜態方法在遊戲開始時就獲取玩家隊伍資料,但是只會初始化一次*/
    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);

        party = PokemonParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated +=SetPartyData;
    }

    public void SetPartyData()
    {
        pokemons=party.Pokemons;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(pokemons[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        UpdateMemberSelection(selection);
        messageText.text = "Choose a Pokemon for Battle!";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < pokemons.Count-1; i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    /*隊伍選單*/
    /*陣列如下所以在上下Key則是增加為2
      0  1
      2  3
      4  5
    */
    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var prevSelection = selection;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selection;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selection;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            selection += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selection -= 2;
        /*限制選單賦值由於限制跟腳色技能樹有關所以參照玩家pokemon擁有技能上線做定義*/
        selection = Math.Clamp(selection, 0, pokemons.Count - 1);

        if (selection != prevSelection)
            UpdateMemberSelection(selection);

        /*選擇腳色*/
        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
        }
        /*返回鍵*/
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }




    /*字串顯示*/
    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
