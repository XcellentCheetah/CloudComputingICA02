using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject itemHolder;
    [SerializeField] private GameObject individualItemHolder;

    private List<Item> ShopItems = new List<Item>();

    void Start()
    {
        //ShopItems.Add(new Shield("Weak Shield", 10, 1));
        //ShopItems.Add(new Shield("Strong Shield", 10, 3));
        //ShopItems.Add(new Shield("Durable Shield", 30, 1));
        GetShopFromPF();
    }

    public void AddToCart(string itemName)
    {
        foreach (Item item in ShopItems)
        {
            if (item.name == itemName)
            {
                CartManager.Instance.AddItemToCart(item);
                return;
            }
        }
        Debug.LogError("No Item Found");
    }

    public void DisplayShop()
    {
        DeleteAllChildren(itemHolder);
        foreach (Item item in ShopItems)
        {
            GameObject currentItemHolder = Instantiate(individualItemHolder, itemHolder.transform);
            TMP_Text nameText = currentItemHolder.transform.GetChild(0).GetComponent<TMP_Text>();
            Button addButton = currentItemHolder.transform.GetChild(2).GetComponent<Button>();

            nameText.text = item.name;
            addButton.onClick.AddListener(delegate{AddToCart(item.name);});
        }
    }

    public void DeleteAllChildren(GameObject parent)
    {
        if (parent == null)
        {
            Debug.LogWarning("Parent object is null. Cannot delete children.");
            return;
        }

        for (int i = parent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.transform.GetChild(i);
            Destroy(child.gameObject);
        }

        Debug.Log("All children of " + parent.name + " have been deleted.");
    }

    //public void GetShopFromPF()
    //{
    //    var request = new GetStoreItemsRequest()
    //    {
    //        CatalogVersion = "MainCatalog",
    //        StoreId = "MainStore"
    //    };

    //    PlayFabClientAPI.GetStoreItems(request, OnGetStoreItemSuccess, OnError);
    //}

    //private void OnGetStoreItemSuccess(GetStoreItemsResult r)
    //{
    //    ShopItems.Clear();
    //    foreach (var item in r.Store)
    //    {
    //        if (item.ItemId == "Weak Shield")
    //        {
    //            Shield newShield;
    //            newShield = new Shield(item.ItemId, 10, 1, );
    //            ShopItems.Add(newShield);

    //        }
    //        else if (item.ItemId == "Strong Shield")
    //        {
    //            Shield newShield;
    //            newShield = new Shield(item.ItemId, 10, 3);
    //            ShopItems.Add(newShield);
    //        }
    //        else if (item.ItemId == "Durable Shield")
    //        {
    //            Shield newShield;
    //            newShield = new Shield(item.ItemId, 30, 1);
    //            ShopItems.Add(newShield);
    //        }
    //    }

    //    DisplayShop();
    //}

    public void GetShopFromPF()
    {
        var request = new GetStoreItemsRequest()
        {
            CatalogVersion = "MainCatalog",
            StoreId = "MainStore"
        };

        PlayFabClientAPI.GetStoreItems(request, OnGetStoreItemSuccess, OnError);
    }

    private void OnGetStoreItemSuccess(GetStoreItemsResult r)
    {
        ShopItems.Clear();

        foreach (var item in r.Store)
        {
            item.VirtualCurrencyPrices.TryGetValue("BC", out uint price);

            if (item.ItemId == "Weak Shield")
            {
                Shield newShield = new Shield(item.ItemId, 10, 1, (int)price);
                ShopItems.Add(newShield);
            }
            else if (item.ItemId == "Strong Shield")
            {
                Shield newShield = new Shield(item.ItemId, 10, 3, (int)price);
                ShopItems.Add(newShield);
            }
            else if (item.ItemId == "Durable Shield")
            {
                Shield newShield = new Shield(item.ItemId, 30, 1, (int)price);
                ShopItems.Add(newShield);
            }
        }

        DisplayShop();
    }


    private void OnError(PlayFabError e)
    {
        Debug.LogError("Error: " + e.GenerateErrorReport());
    }
}
