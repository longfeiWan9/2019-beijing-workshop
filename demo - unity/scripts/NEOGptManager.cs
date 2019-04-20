using Neo.Lux.Core;
using Neo.Lux.Cryptography;
using System;
using System.Collections;
using UniRx;
using UnityEngine;

public class NEORewardManager : MonoBehaviour
{
    [SerializeField] private NEOManager neoManager;
    [SerializeField] private CompleteProject.PlayerHealth playerHealth;

    [SerializeField] private readonly int rewardThreshold = 0;

    private bool isGameOver;

    private void Update()
    {
        Debug.Log("updating result .....");
        Debug.Log("Current Health: " + playerHealth.currentHealth);
        if (playerHealth.currentHealth <= 0)
        {
            Debug.Log("Player is dead .....");
            StartCoroutine(OnGameOver());
            isGameOver = true;
        }
        else if (playerHealth.currentHealth > 0)
        {
            Debug.Log("Player is still alive .....");
            isGameOver = false;
        }
    }

    private IEnumerator OnGameOver()
    {
        Debug.Log("Game Over .....");
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
            //var tx = neoManager.API.SendAsset(neoManager.MasterKeyPair, neoManager.PlayerKeyPair.Value.address, NEOManager.AssetSymbol, (decimal)amount);
            UInt160 scScriptHash = new UInt160(NeoAPI.GetScriptHashFromString(neoManager.ContractHash));
            var tx = neoManager.API.CallContract(neoManager.PlayerKeyPair.Value, scScriptHash, "claimRewards", new object[] { neoManager.PlayerKeyPair.Value.address, (decimal)amount });

            if (tx == null)
            {
                Debug.LogError("Null Transaction returned");
                Time.timeScale = 1;
            }
            else
            {
                Debug.Log("TX received, checking sync...");
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
            //var balances = neoManager.API.GetAssetBalancesOf(neoManager.PlayerKeyPair.Value);
            //neoManager.GASBalance.Value = balances.ContainsKey(NEOManager.AssetSymbol) ? balances[NEOManager.AssetSymbol] : 0;
            //if (Mathf.Approximately((float)NEOManager.GASBalance.Value, GameDataSystem.Coins.Value))
            //{
            //    Debug.Log("GAS transferred successfully");
            //}
            //else
            //{
            //    Debug.LogWarning("Something's not right." +
            //                     //"\nCoins: " + GameDataSystem.Coins.Value +
            //                     "\nGAS: " + NEOManager.GASBalance.Value);
            //}
            var balances = neoManager.API.GetAssetBalancesOf(neoManager.PlayerKeyPair.Value);
            neoManager.NEOBalance = balances.ContainsKey(NEOManager.NEOSymbol) ? balances[NEOManager.NEOSymbol] : 0;
            neoManager.GASBalance = balances.ContainsKey(NEOManager.GASSymbol) ? balances[NEOManager.GASSymbol] : 0;
            neoManager.GPTBalance = neoManager.GPT.BalanceOf(neoManager.PlayerKeyPair.Value);
            neoManager.neoBalanceText.text = "NEO : " + neoManager.NEOBalance.ToString();
            neoManager.gasBalanceText.text = "GAS : " + neoManager.GASBalance.ToString();
            neoManager.gptBalanceText.text = "GPT : " + neoManager.GPTBalance.ToString();
            Debug.Log("Balance synced!");
            Time.timeScale = 1;
        }
        catch (NullReferenceException exception)
        {
            Debug.LogWarning("Something's not right." +
                 //"\nCoins: " + GameDataSystem.Coins.Value +
                 "\nGAS: " + neoManager.GASBalance);
            Time.timeScale = 1;
        }
    }
}