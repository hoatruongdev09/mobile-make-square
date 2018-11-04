using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
public class PurchaseServices : MonoBehaviour, IStoreListener {
    private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;

    private static string id_product_buybomb = "purchasebomb";
    private static string id_product_buyswap = "purchaseswap";
    private static string id_product_noads = "noad";

   
    public void Init(){
        if (storeController == null) {
            InitializePurchasing ();
        }
    }
    private void InitializePurchasing () {
        if (IsInitialized ())
            return;
        var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());
        builder.AddProduct (id_product_buybomb, ProductType.Consumable);
        builder.AddProduct (id_product_buyswap, ProductType.Consumable);
        builder.AddProduct (id_product_noads, ProductType.NonConsumable);

        UnityPurchasing.Initialize (this, builder);
    }
    public void BuyProductId (string productId) {
        Debug.Log ("buy product: " + productId);
        if (IsInitialized ()) {
            Product product = storeController.products.WithID (productId);
            if (product != null && product.availableToPurchase) {
                Debug.Log (string.Format ("Purchasing product: {0}", product.definition));
                storeController.InitiatePurchase (product);
            } else {
                Debug.Log (string.Format ("Purchase fail; reason: product: {0} && product.availabelToPurchase: {1}", product != null, (product != null) ? product.availableToPurchase.ToString () : "product null"));
            }
        } else {
            Debug.Log ("Purchase fail. Not initialized");
        }
    }
    public void RestorePurchases () {
        if (!IsInitialized ()) {
            Debug.Log ("Restore purchase Failed. Not initialized");
            return;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer) {
            Debug.Log ("Restore purchase started ");
            var apple = storeExtensionProvider.GetExtension<IAppleExtensions> ();
            apple.RestoreTransactions (OnRestore);
        } else {
            Debug.Log ("Resotre purchases Fail. Not supported onthis platform. Currn = " + Application.platform);
        }
    }
    private void OnRestore (bool result) {
        // restore stuff here
    }
    public static bool CheckNoAdsBought () {
        Product product = storeController.products.WithID (id_product_noads);
//        Debug.Log (String.Format ("product: {0} && product.hasReceipt: {1}", product == null, product.hasReceipt));
        return product != null && product.hasReceipt;
    }
    private bool IsInitialized () {
        return storeController != null && storeExtensionProvider != null;
    }

    public void OnInitializeFailed (InitializationFailureReason error) {
        Debug.Log ("Initialize Failed: " + error);
    }

    public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e) {
        if (string.Equals (e.purchasedProduct.definition.id, id_product_buybomb, StringComparison.Ordinal)) {
            ScoreManager.Instance.AddBombAmount (15);
            UIControl.Instance.UpdateBombText(ScoreManager.Instance.GetBombAmount());
            Debug.Log (string.Format ("Purchase success. Product: {0}", e.purchasedProduct.definition.id));
        } else if (string.Equals (e.purchasedProduct.definition.id, id_product_buyswap, StringComparison.Ordinal)) {
            ScoreManager.Instance.AddSwapTurn (15);
            UIControl.Instance.UpdateSwapText(ScoreManager.Instance.GetSwapTurn());
            Debug.Log (string.Format ("Purchase success. Product: {0}", e.purchasedProduct.definition.id));
        } else if (string.Equals (e.purchasedProduct.definition.id, id_product_noads, StringComparison.Ordinal)) {
            AdServices.DisableAd ();
            UIControl.Instance.DisableButtonPurchaseAd ();
            Debug.Log (string.Format ("Purchase success. Product: {0}", e.purchasedProduct.definition.id));
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed (Product i, PurchaseFailureReason p) {
        Debug.Log (string.Format ("Purchase: Fail. Product '{0}', Reasion: {1}", i.definition.storeSpecificId, p));
    }

    public void OnInitialized (IStoreController controller, IExtensionProvider extensions) {
        Debug.Log ("OnInitialized: PASS");
        storeController = controller;
        storeExtensionProvider = extensions;
        if (CheckNoAdsBought ()) {
            AdServices.DisableAd ();
            UIControl.Instance.DisableButtonPurchaseAd ();
            Debug.Log ("No ad bought");
        } else {
            Debug.Log ("No ad didnt buy");
        }
    }

}