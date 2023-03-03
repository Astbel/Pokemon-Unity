using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, MoveForget, Busy }

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] Text categoryText;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;
    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveSelectUI moveSelectUI;
    Action<itemBase> onItemUsed;
    int selectedItem = 0;
    int selectedCategory = 0;
    /*要學習招式*/
    MoveBase moveToLearn;
    Inventory inventory;
    List<ItemSlotUI> slotUIList;
    RectTransform itemListRect;
    InventoryUIState state;
    /*包包軸限制轉動數*/
    const int itemInViewport = 4;
    /*由於包包UI跟玩家是分開的所以用findobject來查詢玩家的物品*/
    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdataItemList();
        inventory.OnUpdated += UpdataItemList;
    }

    void UpdataItemList()
    {
        //清除所有Itemlist的子類別
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);
        /*初始化list用來存放所有子類別物件*/
        slotUIList = new List<ItemSlotUI>();
        /*把所有物品實例化並加進list*/
        foreach (var itemSlot in inventory.GetSlotByCategory(selectedCategory))
        {
            /*實例化物件給父類傳遞*/
            var slotUIobj = Instantiate(itemSlotUI, itemList.transform);
            slotUIobj.SetData(itemSlot);
            slotUIList.Add(slotUIobj);
        }
        UpdateItemSelection();
    }
    /*道具事件新增兩個
    onBack      關閉菜單
    onItemUsed  使用過後
    */
    public void HandleUpdate(Action onBack, Action<itemBase> onItemUsed = null)
    {
        this.onItemUsed = onItemUsed;

        /*開啟選單*/
        if (state == InventoryUIState.ItemSelection)
        {
            /*紀錄當前選擇*/
            int prevSelection = selectedItem;
            int prevCategory = selectedCategory;
            /*選擇部分*/
            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --selectedItem;
            /*ITEM 類別切換*/
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                ++selectedCategory;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                --selectedCategory;

            /*項目類別旋轉*/
            if (selectedCategory > Inventory.ItemCategories.Count - 1)
                selectedCategory = 0;
            else if (selectedCategory < 0)
                selectedCategory = Inventory.ItemCategories.Count - 1;


            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotByCategory(selectedCategory).Count - 1);
            selectedCategory = Mathf.Clamp(selectedCategory, 0, Inventory.ItemCategories.Count - 1);
            /*如果有跟新選擇才跟新選擇標籤顏色*/
            if (prevCategory != selectedCategory)
            {
                ResetSelection();
                categoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdataItemList();
            }
            else if (prevSelection != selectedItem)
            {
                UpdateItemSelection();
            }

            if (Input.GetKeyDown(KeyCode.Z))
                StartCoroutine(ItemSelected());

            else if (Input.GetKeyDown(KeyCode.X))
                onBack?.Invoke();
        }
        /*開啟pokemon隊伍選單*/
        else if (state == InventoryUIState.PartySelection)
        {
            Action onSelected = () =>
            {
                /*當被選用時使用道具在POKEMON*/
                StartCoroutine(UseItem());
            };

            Action onBackPartyScreen = () =>
           {
               ClosePartyScreen();
           };

            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
        else if (state == InventoryUIState.MoveForget)
        {
            Action<int> onMoveSelected = (int moveIndex) =>
            {
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
            };

            moveSelectUI.HandleForgetMoves(onMoveSelected);
        }
    }

    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;
        var item = inventory.GetItem(selectedItem, selectedCategory);
        /*判斷在何處能使用道具*/
        if (GameController.Instance.State == GameState.Battle)
        {
            //In Battle
            if (!item.CanUseInBattle)
            {
                yield return DialogManger.Instance.ShowDialogText($"This Item can't use in battle ! ");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            //OutSide Battle
            if (!item.CanUseOutsideBattle)
            {
                yield return DialogManger.Instance.ShowDialogText($"This Item can't use right now ! ");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }

        if (selectedCategory == (int)ItemCategory.Pokeballs)
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();
            if (item is TMiItem)
                partyScreen.ShowIfTmIsUsable(item as TMiItem);
        }
    }

    IEnumerator UseItem()
    {
        /*避免玩家按太多次Z造成道具重複使用*/
        state = InventoryUIState.Busy;

        yield return HandleTmItem();
        /*進化石種類確認物品,以及種類*/
        var item = inventory.GetItem(selectedItem, selectedCategory);
        var pokemon = partyScreen.SelectedMember;
        /*進化石事件*/
        if (item is EvolutionItem)
        {
            var evolution = pokemon.CheckForEvolution(item);
            if (evolution != null)
            {
                yield return EvolutionManager.i.Evolve(pokemon, evolution);
            }
            /*如果沒有效果則跳出協成*/
            else
            {
                yield return DialogManger.Instance.ShowDialogText($"It won't have any effect ! ");
                ClosePartyScreen();
                yield break;
            }
        }

        /*var usedItem 返回使用道具*/
        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if (usedItem != null)
        {
            /*只顯示當使用為回覆道具*/
            if (usedItem is RecoveryItem)
                yield return DialogManger.Instance.ShowDialogText($"The player used {usedItem.Name}");
            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            /*確認當前類別是否為回覆類道具*/
            if (selectedCategory == (int)ItemCategory.Items)
                yield return DialogManger.Instance.ShowDialogText($"It won't have any effect ! ");
        }
        ClosePartyScreen();
    }

    IEnumerator HandleTmItem()
    {
        var tmItem = inventory.GetItem(selectedItem, selectedCategory) as TMiItem;
        /*如果不是TM物件終止協成*/
        if (tmItem == null)
            yield break;
        /*學習招式機*/
        var pokemon = partyScreen.SelectedMember;
        /*確認是否已經學習該招式*/
        if (pokemon.HasMove(tmItem.Move))
        {
            yield return DialogManger.Instance.ShowDialogText($" {pokemon.Base.Name} has already learned {tmItem.Move.Name}");
            yield break;
        }
        if (!tmItem.CanBeTaught(pokemon))
        {
            yield return DialogManger.Instance.ShowDialogText($" {pokemon.Base.Name} can't learn {tmItem.Move.Name}");
            yield break;
        }
        /*如果未達到最多技能時*/
        if (pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DialogManger.Instance.ShowDialogText($" {pokemon.Base.Name} learned {tmItem.Move.Name}");
        }
        /*大於四招*/
        else
        {
            yield return DialogManger.Instance.ShowDialogText($"{pokemon.Base.Name} trying to learned {tmItem.Move.Name}");
            yield return DialogManger.Instance.ShowDialogText($"But it can't learn more than {PokemonBase.MaxNumOfMoves} moves ");
            yield return ChooseMoveToForget(pokemon, tmItem.Move);
            yield return new WaitUntil(() => state != InventoryUIState.MoveForget);
        }
    }
    /*呼叫UI介面窗口來選擇忘記招式*/
    IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogManger.Instance.ShowDialogText($"Choose a move you wan't to forget", true, false);
        moveSelectUI.gameObject.SetActive(true);
        /*List move to List move Base method by Linq*/
        /*UI顯示原來的技能以及要忘記的技能*/
        moveSelectUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;
        state = InventoryUIState.MoveForget;
    }

    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotByCategory(selectedCategory);
        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighhlightColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }
        if (slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }

        HandleSrcolling();
    }

    void HandleSrcolling()
    {
        /*如果道具小於這個數量就不需要顯示滾動軸*/
        if (slotUIList.Count <= itemInViewport) return;

        float scrollPos = Mathf.Clamp(selectedItem - itemInViewport, 0, selectedItem) * slotUIList[0].Height;
        /*包包選項時只有y軸移動*/
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
        /*包包箭頭介面*/
        bool showUpArrow = selectedItem > itemInViewport;
        upArrow.gameObject.SetActive(showUpArrow);
        bool shodownpArrow = selectedItem + itemInViewport < slotUIList.Count;
        downArrow.gameObject.SetActive(shodownpArrow);

    }

    void ResetSelection()
    {
        selectedItem = 0;
        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
        // partyScreen.SetPartyData(playerController.GetComponent<PokemonParty>().Pokemons);
    }
    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;
        partyScreen.ClearMemberSlotMessages();
        partyScreen.gameObject.SetActive(false);
    }

    IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        var pokemon = partyScreen.SelectedMember;
        DialogManger.Instance.CloseDialog();
        moveSelectUI.gameObject.SetActive(false);
        if (moveIndex == PokemonBase.MaxNumOfMoves)
        {
            /*不學習招式*/
            yield return DialogManger.Instance.ShowDialogText($"{pokemon.Base.Name} did not learn {moveToLearn.Name}");
        }
        else
        {
            /*忘記招式學新招式*/
            var selectedMove = pokemon.Moves[moveIndex].Base;
            yield return DialogManger.Instance.ShowDialogText($"{pokemon.Base.Name} forget {selectedMove.Name} and learned {moveToLearn.Name}");

            pokemon.Moves[moveIndex] = new Move(moveToLearn);
        }
        /*Resset*/
        moveToLearn = null;
        state = InventoryUIState.ItemSelection;
    }

}
