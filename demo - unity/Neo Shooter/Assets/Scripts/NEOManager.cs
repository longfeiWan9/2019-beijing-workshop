using System.Collections;
using UnityEngine;
using System;
using UniRx;
using Neo.Lux.Core;
using Neo.Lux.Cryptography;
using UnityEngine.UI;

public class NEOManager : MonoBehaviour
{
    public NeoAPI API;
    public NEP5 GPT;
    [SerializeField] private string RpcIP;
    public const string NEOSymbol = "NEO";
    public const string GASSymbol = "GAS";
    public string ContractHash;
    public string wif;
    [HideInInspector] public KeyPairReactiveProperty PlayerKeyPair = new KeyPairReactiveProperty();
    [SerializeField] private Text addressText;
    [SerializeField] public Text neoBalanceText;
    [SerializeField] public Text gasBalanceText;
    [SerializeField] public Text gptBalanceText;
    public Decimal NEOBalance;
    public Decimal GASBalance;
    public Decimal GPTBalance;

    private void OnEnable()
    {
        PlayerKeyPair.Value = KeyPair.FromWIF(wif);
        this.API = new CustomRPC(30333, 4000, "http://" + RpcIP);
        this.GPT = new NEP5(this.API, ContractHash);
        //this.API = NeoRPC.ForTestNet();

        this.PlayerKeyPair.DistinctUntilChanged().Where(kp => kp != null).Subscribe(keyPair =>
            {
                this.addressText.text = keyPair.address;
                this.neoBalanceText.text = "Balance: Please  wait, syncing balance...";
                this.gasBalanceText.text = "Balance: Please  wait, syncing balance...";
                this.gptBalanceText.text = "Balance: Please  wait, syncing balance...";
                StartCoroutine(SyncBalance());
            }).AddTo(this);
    }

    private void Update() {
        StartCoroutine(SyncBalance());
    }

    private IEnumerator SyncBalance()
    {
        yield return null;
        var balances = this.API.GetAssetBalancesOf(this.PlayerKeyPair.Value);
        this.NEOBalance = balances.ContainsKey(NEOSymbol) ? balances[NEOSymbol] : 0;
        this.GASBalance = balances.ContainsKey(GASSymbol) ? balances[GASSymbol] : 0;
        this.GPTBalance = this.GPT.BalanceOf(PlayerKeyPair.Value);
        this.neoBalanceText.text = "NEO : " NEOBalance.ToString();
        this.gasBalanceText.text = "GAS : " + GASBalance.ToString();
        this.gptBalanceText.text = "GPT : " + GPTBalance.ToString();

    }
    
}
[Serializable]
public class KeyPairReactiveProperty : ReactiveProperty<KeyPair>
{
    public KeyPairReactiveProperty() { }
    public KeyPairReactiveProperty(KeyPair initialValue) : base(initialValue) { }
}