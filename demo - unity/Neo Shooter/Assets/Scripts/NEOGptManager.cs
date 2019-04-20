using Neo.Lux.Core;
using Neo.Lux.Cryptography;
using Neo.Lux.Utils;
using System;
using System.Collections;
using UniRx;
using UnityEngine;

public class NEOGptManager : MonoBehaviour
{
    [SerializeField] private NEOManager neoManager;
    [SerializeField] private CompleteProject.PlayerHealth playerHealth;

    [SerializeField] private readonly int rewardThreshold = 50;

    private bool isGameOver;

    private void Update()
    {
        if (playerHealth.currentHealth <= 0 && !isGameOver)
        {
            StartCoroutine(OnGameOver());
            isGameOver = true;
        }
        else if (playerHealth.currentHealth > 0 && isGameOver)
        {
            isGameOver = false;
        }
    }

    private IEnumerator OnGameOver()
    {
        yield return new WaitForSeconds(1);
        Time.timeScale = 0;

        if (CompleteProject.ScoreManager.score >= rewardThreshold)
        {
            Debug.Log("You should receive " + CompleteProject.ScoreManager.score + " GPT.");
            StartCoroutine(TryClaimRewards(CompleteProject.ScoreManager.score));
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private IEnumerator TryClaimRewards(int amount)
    {
        yield return null;

        try
        {
            Debug.Log("Start claim GPT reward");
            UInt160 scScriptHash = new UInt160(NeoAPI.GetScriptHashFromString(neoManager.ContractHash));
            byte[] addressScriptHash = neoManager.PlayerKeyPair.Value.address.GetScriptHashFromAddress();
            var tx = neoManager.API.CallContract(neoManager.PlayerKeyPair.Value, scScriptHash, "claimRewards", new object[] { addressScriptHash, amount });

            if (tx == null)
            {
                Debug.LogError("Null Transaction returned");
                Time.timeScale = 1;
            }
            else
            {
                Debug.Log("TX received: "+ tx);
                Observable.FromCoroutine(SyncBalance).Subscribe().AddTo(this);
            }
        }
        catch (NullReferenceException ee)
        {
            Debug.LogError("There was a problem..." + ee);
            Time.timeScale = 1;
        }
        catch (Exception ee)
        {
            Debug.LogError("There was a problem..." + ee);
            Time.timeScale = 1;
        }
    }

    private IEnumerator SyncBalance()
    {
        yield return null;

        try
        {
            var balances = neoManager.API.GetAssetBalancesOf(neoManager.PlayerKeyPair.Value);
            neoManager.NEOBalance = balances.ContainsKey(NEOManager.NEOSymbol) ? balances[NEOManager.NEOSymbol] : 0;
            neoManager.GASBalance = balances.ContainsKey(NEOManager.GASSymbol) ? balances[NEOManager.GASSymbol] : 0;
            neoManager.GPTBalance = neoManager.GPT.BalanceOf(neoManager.PlayerKeyPair.Value);
            neoManager.neoBalanceText.text = "NEO : " + neoManager.NEOBalance.ToString();
            neoManager.gasBalanceText.text = "GAS : " + neoManager.GASBalance.ToString();
            neoManager.gptBalanceText.text = "GPT : " + neoManager.GPTBalance.ToString();
            Time.timeScale = 1;
        }
        catch (NullReferenceException exception)
        {
            Debug.LogWarning("Something's not right." + exception);
            Time.timeScale = 1;
        }
    }
}