using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ShopState { Menu, Buying, Selling, Busy }

public class ShopController : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSeletorUI countSeletorUI;
    [SerializeField] ShopUI shopUI;
    public event Action OnStartShopping;
    public event Action OnFinishShopping;
    Inventory inventory;
    /*商店狀態機*/
    ShopState state;
    Merchant merchant;

    public static ShopController i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    private void Start()
    {
        inventory = Inventory.GetInventory();
    }
    int selectedChoice = 0;
    public IEnumerator StartTrading(Merchant merchant)
    {
        this.merchant = merchant;
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
            state = ShopState.Buying;
            walletUI.Show();
            shopUI.Show(merchant.AvailableItems, (item) => StartCoroutine(BuyItem(item))
                    , OnBackFromBuying);
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
        else if (state == ShopState.Buying)
        {
            shopUI.HandleUpdate();
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

        int countToSell = 1;
        /*包包回傳道具數量*/
        int itemCount = inventory.GetItemCount(item);
        /*NPC詢問要販售多少數量*/
        if (itemCount > 1)
        {
            yield return DialogManger.Instance.ShowDialogText($"How many would you like to sell?"
            , waitForInput: false, autoClose: false);

            yield return countSeletorUI.ShowSelector(itemCount, SellingPrice,
            (selectedCount) => countToSell = selectedCount);

            DialogManger.Instance.CloseDialog();
        }

        SellingPrice = SellingPrice * countToSell;

        /*NPC 店員顯示販賣對話選項*/
        yield return DialogManger.Instance.ShowDialogText($"The price for the item will be {SellingPrice} ,Would u like to sell it?",
        waitForInput: false,
        choices: new List<string>() { "YES", "No" },
        onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            //Sell Item
            inventory.RemoveItem(item, countToSell);
            //需要增加變數給玩家當作金錢
            Wallet.i.AddMoney(SellingPrice);
            //販售對話顯示
            yield return DialogManger.Instance.ShowDialogText($"Turned over {item.Name} and received {SellingPrice} !");
        }

        walletUI.Close();

        state = ShopState.Selling;
    }

    IEnumerator BuyItem(itemBase item)
    {
        state = ShopState.Busy;

        yield return DialogManger.Instance.ShowDialogText("How many would you like to buy?",
            waitForInput: false, autoClose: false);

        int countToBuy = 1;
        yield return countSeletorUI.ShowSelector(100, item.Price,
            (selectedCount) => countToBuy = selectedCount);

        DialogManger.Instance.CloseDialog();

        float totalPrice = item.Price * countToBuy;

        if (Wallet.i.HasMoney(totalPrice))
        {
            int selectedChoice = 0;
            /*NPC 店員顯示購買對話選項*/
            yield return DialogManger.Instance.ShowDialogText($"That will be {totalPrice}",
            waitForInput: false,
            choices: new List<string>() { "YES", "No" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
            if (selectedChoice == 0)
            {
                //Select YES
                inventory.AddItem(item, countToBuy);
                Wallet.i.TakeMoney(totalPrice);
                yield return DialogManger.Instance.ShowDialogText("Thank you for shoopping with us!");

            }
        }
        else
        {
            yield return DialogManger.Instance.ShowDialogText("Not enough money for that!");
        }
        state = ShopState.Buying;
    }

    void OnBackFromBuying()
    {
        shopUI.Close();
        walletUI.Close();
        StartCoroutine(StartMenuState());
    }

}
