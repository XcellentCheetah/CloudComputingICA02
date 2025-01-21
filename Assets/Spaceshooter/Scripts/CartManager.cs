using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[System.Serializable]
public class CartItem
{
    public Item item;
    public int quantity;

    public CartItem(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

[System.Serializable]
public class SerializableCartItem
{
    public string itemType; // Full type name (e.g., "Shield")
    public string itemData; // Serialized data for the item
    public int quantity;    // Quantity of the item
}

public class CartManager : MonoBehaviour
{
    public static CartManager Instance;

    private List<CartItem> cartItems = new List<CartItem>();

    [SerializeField] private GameObject itemHolder;
    [SerializeField] private GameObject individualItemHolder;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadCart();
    }

    public void AddItemToCart(Item item)
    {
        foreach (CartItem cartItem in cartItems)
        {
            if (item.name == cartItem.item.name)
            {
                cartItem.quantity++;
                DisplayCart();
                return;
            }
        }

        cartItems.Add(new CartItem(item, 1));
        DisplayCart();
    }

    public void DisplayCart()
    {
        DeleteAllChildren(itemHolder);
        foreach (CartItem cartItem in cartItems)
        {
            GameObject currentItemHolder = Instantiate(individualItemHolder, itemHolder.transform);
            TMP_Text nameText = currentItemHolder.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text quantityText = currentItemHolder.transform.GetChild(2).GetComponent<TMP_Text>();
            Button removeButton = currentItemHolder.transform.GetChild(3).GetComponent<Button>();

            nameText.text = cartItem.item.name;
            quantityText.text = cartItem.quantity.ToString();
            removeButton.onClick.AddListener(delegate { RemoveFromCart(cartItem); });
        }
    }

    public void RemoveFromCart(CartItem item)
    {
        if (item.quantity > 1) item.quantity--;
        else cartItems.Remove(item);

        DisplayCart();
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

    public void SendJSON()
    {
        List<SerializableCartItem> serializableCartItems = new List<SerializableCartItem>();

        foreach (CartItem cartItem in cartItems)
        {
            SerializableCartItem serializableCartItem = new SerializableCartItem
            {
                itemType = cartItem.item.GetType().FullName, // Save the full type name
                itemData = JsonUtility.ToJson(cartItem.item), // Serialize the item
                quantity = cartItem.quantity                 // Save the quantity
            };

            serializableCartItems.Add(serializableCartItem);
        }

        string cartItemsAsJSON = JsonUtility.ToJson(new JSListWrapper<SerializableCartItem>(serializableCartItems));

        var req = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "CartItems", cartItemsAsJSON }
            }
        };

        PlayFabClientAPI.UpdateUserData(req, result =>
        {
            Debug.Log("Data sent successfully!");
        }, OnError);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void LoadCart()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), LoadCartSuccess, OnError);
    }

    private void LoadCartSuccess(GetUserDataResult r)
    {
        if (r.Data != null && r.Data.ContainsKey("CartItems"))
        {
            string json = r.Data["CartItems"].Value;

            if (!string.IsNullOrEmpty(json))
            {
                JSListWrapper<SerializableCartItem> serializedCartItems =
                    JsonUtility.FromJson<JSListWrapper<SerializableCartItem>>(json);

                cartItems.Clear();

                foreach (var serializedItem in serializedCartItems.list)
                {
                    Type itemType = Type.GetType(serializedItem.itemType); // Get type info
                    if (itemType == null)
                    {
                        Debug.LogError("Could not resolve type: " + serializedItem.itemType);
                        continue;
                    }

                    Item item = (Item)JsonUtility.FromJson(serializedItem.itemData, itemType);
                    cartItems.Add(new CartItem(item, serializedItem.quantity));
                    DisplayCart();
                }
            }
        }
    }

    public void Buy()
    {
        foreach (var cartItem in cartItems)
        {
            for (int i = 0; i < cartItem.quantity; i++)
            {
                //Debug.Log("Price: " + cartItem.item.price);
                var buyReq = new PurchaseItemRequest
                {
                    CatalogVersion = "MainCatalog",
                    ItemId = cartItem.item.name,
                    VirtualCurrency = "BC",
                    Price = cartItem.item.price
                };

                PlayFabClientAPI.PurchaseItem(buyReq, result =>
                {
                    Debug.Log($"Successfully purchased item: {cartItem.item.name}");
                },
                OnError);
            }
        }

        cartItems.Clear();
        DisplayCart();
        SendJSON();
        Debug.Log("Purchase complete and cart cleared");
    }

    void OnError(PlayFabError e)
    {
        Debug.Log("Error" + e.GenerateErrorReport());
    }
}
