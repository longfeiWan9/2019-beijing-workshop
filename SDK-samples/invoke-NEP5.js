const { default: Neon, wallet, api, sc} = require("@cityofzion/neon-js");

//1. 通过neoscan URL创建API客户端
const apiProvider = new api.neoscan.instance("http://192.168.99.100:4000/api/main_net");

//2. 通过私钥或者WIF打开钱包账户
const privateKey = "KwsUexAyjQmstvaeYAQqA5bUx9YSC7CHFqaeZYLpoSG18G5XGYYC";
const account = new wallet.Account(privateKey);

/**
 * 3. 构建调用合约的脚本
 */
const contractScript = "e486e98552a656dacb843796ef8a5c6661e2c4cb";
const fromAddress = sc.ContractParam.byteArray( account.address, "address");
const toAddress = sc.ContractParam.byteArray("AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y", "address");
const amount = Neon.create.contractParam("Integer", 100);
   
  // build contract script
  const props = {
    scriptHash: contractScript,
    operation: "transfer",
    args: [fromAddress, toAddress, amount]
  };
   
  const script = Neon.create.script(props);
   
  const config = {
    api: apiProvider, // The API Provider that we rely on for balance and rpc information
    url: "http://192.168.99.100:30333",
    account: account, // The sending Account
    script: script, // The Smart Contract invocation script
    gas: 0, //This is additional gas paying to system fee.
    fees: 0, //Additional gas paying for network fee(prioritizing, oversize transaction).
  };
   
  // use "doInvoke" function
  Neon.doInvoke(config)
   .then(config => {
      console.log("\n\n--- Response ---");
      console.log(config.response);
   })
   .catch(config => {
      console.log(config);
   });