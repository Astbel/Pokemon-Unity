using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ShopState { Menu, Buying, Selling, Busy }

public class ShopController : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] WalletUI walletUI;
    public event Action OnStartShopping;
    public event Action OnFinishShopping;
    Inventory inventory;
    /*商店狀態機*/
    ShopState state;
    public static ShopController i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    private void Start()
    {
        inventory=Inventory.GetInventory();
    }
    int selectedChoice = 0;
    public IEnumerator StartTrading(Merchant merchant)
    {
        OnStartShopping?.Invoke();
        yield return StartMenuState();

    }

    /*商店買賣選單控制method*/
    IEnumerator StartMenuState()
    {
        state = ShopState.Menu;
        /*NPC 店員顯示對話選項*/
        yield return DialogManger.Instance.ShowDialogText("How may I serve you",
        waitForInput: false,
        choices: new List<string>() { "Buy", "Sell", "Quit" },
        onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            //Buy
        }
        else if (selectedChoice == 1)
        {
            //Sell
            state = ShopState.Selling;
            inventoryUI.gameObject.SetActive(true);
        }
        else if (selectedChoice == 2)
        {
            //Quit
            OnFinishShopping?.Invoke();
            yield break;
        }
    }

    /*呼叫其他的狀態控制器*/
    public void HandleUpdate()
    {
        if (state == ShopState.Selling)
        {
            inventoryUI.HandleUpdate(onBackFromSelling, (selectedItem) => StartCoroutine(SellItem(selectedItem)));
        }
    }
    /*退出販售狀態*/
    void onBackFromSelling()
    {
        inventoryUI.gameObject.SetActive(false);
        StartCoroutine(StartMenuState());
    }
    /*販售物品*/
    IEnumerator SellItem(itemBase item)
    {
        state = ShopState.Busy;

        if (!item.IsSellable)
        {
            yield return DialogManger.Instance.ShowDialogText("You can't sell that!");
            state = ShopState.Selling;
            yield break;
        }
        walletUI.Show();

        float SellingPrice = Mathf.Round(item.Price / 2);

        /*NPC 店員顯示販賣對話選項*/
        yield return DialogManger.Instance.ShowDialogText($"The price for the item will be {SellingPrice} ,Would u like to sell it?",
        waitForInput: false,
        choices: new List<string>() { "YES", "No" },
        onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice==0)
        {
            //Sell Item
            inventory.RemoveItem(item);
            //需要增加變數給玩家當作金錢
            Wallet.i.AddMoney(SellingPrice);
            //販售對話顯示
            yield return DialogManger.Instance.ShowDialogText($"Turned over {item.Name} and received {SellingPrice} !");
        }

        walletUI.Close();

        state=ShopState.Selling;
    }
}
