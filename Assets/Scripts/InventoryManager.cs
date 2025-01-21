using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using Unity.VisualScripting;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Msg;

    void UpdateMsg(string msg) //to display in console and messagebox
    {
        Debug.Log(msg);
        Msg.text += msg + '\n';
    }

    void OnError(PlayFabError e)
    {
        UpdateMsg(e.GenerateErrorReport());
    }

    public void LoadScene(string scn)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scn);
    }

    public void GetVirtualCurrencies()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            r =>
            {
                int coins = r.VirtualCurrency["BC"];
                UpdateMsg("Ben Coins: " + coins);
            }, OnError);
    }

    public void GetCatalog()
    {
        var catreq = new GetCatalogItemsRequest
        {
            CatalogVersion = "terranweapons"
        };
        PlayFabClientAPI.GetCatalogItems(catreq,
            result =>
            {
                List<CatalogItem> items = result.Catalog;
                UpdateMsg("Catalog Items");
                foreach (CatalogItem i in items)
                {
                    UpdateMsg(i.DisplayName + ", " + i.VirtualCurrencyPrices["BC"]);
                }
            }, OnError);
    }


    public void BuyItem()
    {
        var buyreq = new PurchaseItemRequest
        {
            // TODO: Current sample is hardcoded, should make it more dynamic
            CatalogVersion = "terranweapons",
            ItemId = "Weapon01LC",
            VirtualCurrency = "BC",
            Price = 2
        };
        PlayFabClientAPI.PurchaseItem(buyreq,
            result=>{UpdateMsg("Bought!");},
            OnError);
    }

    public void GetPlayerInventory()
    {
        var UserInv = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(UserInv,
            result =>
            {
                List<ItemInstance> ii = result.Inventory;
                UpdateMsg("Player Inventory");
                foreach (ItemInstance i in ii)
                {
                    UpdateMsg(i.DisplayName + ", " + i.ItemId + ", " + i.ItemInstanceId);
                }
            }, OnError);
    }
}
