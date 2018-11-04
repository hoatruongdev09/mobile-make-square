using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServicesControl : MonoBehaviour {
    public static ServicesControl Instance { get; private set; }
    public GameObject services;
    public AdServices adServices { get; private set; }
    public PurchaseServices purchaseServices { get; private set; }
    public PlayGameServices playGameServices { get; private set; }

    private void OnEnable () {
        Instance = this;
    }
    private void Start () {
        playGameServices = services.GetComponent<PlayGameServices> ();
        adServices = services.GetComponent<AdServices> ();
        purchaseServices = services.GetComponent<PurchaseServices> ();

        StartCoroutine(InitializeServices());
    }

    private IEnumerator InitializeServices(){
        yield return new WaitForSecondsRealtime(1);
        purchaseServices.Init();
        yield return new WaitForSecondsRealtime(1);
        playGameServices.Init();
        yield return new WaitForSecondsRealtime(2);
        adServices.Init();

    }

}