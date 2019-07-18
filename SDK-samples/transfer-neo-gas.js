const { default: Neon, wallet, api} = require("@cityofzion/neon-js");

// 通过neoscan URL创建API客户端
const apiProvider = new api.neoscan.instance("http://192.168.99.100:4000/api/main_net");

//1. 通过私钥或者WIF打开钱包账户
const privateKey = "KwsUexAyjQmstvaeYAQqA5bUx9YSC7CHFqaeZYLpoSG18G5XGYYC";
const account = new wallet.Account(privateKey);

//2. 转账接收地址
const receivingAddress = "AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y";
const intent = api.makeIntent({ NEO: 10, GAS: 1 }, receivingAddress);
 
const config = {
  api: apiProvider, // The network to perform the action, MainNet or TestNet.
  url: "http://192.168.99.100:30333",
  account: account, // This is the address which the assets come from.
  intents: intent, // This is where you want to send assets to.
  gas: 0, //This is additional gas paying to system fee.
  fees: 0 //Additional gas paying for network fee(prioritizing, oversize transaction).
};
 
Neon.sendAsset(config)
 .then(config => {
    console.log("\n\n--- Response ---");
    console.log(config.response.txid);
 })
 .catch(config => {
    console.log(config);
 });