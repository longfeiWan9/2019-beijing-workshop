const { default: Neon, wallet, rpc, api} = require("@cityofzion/neon-js");
/**
* 创建钱包
*/
// We create a wallet with name 'myWallet'. This is optional. The constructor is fine with no arguments.
const myWallet = Neon.create.wallet({ name: "MyWallet" });
 
/**
* 添加账户
* 方式一：系统默认创建账户
*/
// 新生成的钱包
myWallet.addAccount();
 
/**
* 方式二：通过WIF或私钥导入账户
*/
const privateKey = "KwsUexAyjQmstvaeYAQqA5bUx9YSC7CHFqaeZYLpoSG18G5XGYYC";
const account1 = new wallet.Account(privateKey);
myWallet.addAccount(account1);
 
/**
* 查询钱包余额：使用Neoscan API查询余额
*/
const privateNetConfig = {
    name: "PrivateNet",
    nodes: [
      "192.168.99.100:20333",
      "192.168.99.100:20334",
      "192.168.99.100:20335",
      "192.168.99.100:20336"
   ],
    extra: {
      neoscan: "http://192.168.99.100:4000/api/main_net"
   }
  };
  
const privateNet = new rpc.Network(privateNetConfig);
Neon.add.network(privateNet, true);

// You will be able to lookup an instance of PrivateNet neoscan
const apiProvider = new api.neoscan.instance("PrivateNet");

apiProvider
 .getBalance(myWallet.accounts[1].address)
 .then(response =>
    console.log(
      `NEO: ${response.assets["NEO"].balance.toNumber()}, GAS: ${response.assets["GAS"].balance.toNumber()}`
   )
 )
 .catch("Get Balance Error!");
 
/**
* 查询钱包余额：使用节点提供的RPC Query查询余额 
*/
// Before export the account, you have to encrypt it.
const nodeUrl = "http://192.168.99.100:30333"
const rpcClient = new rpc.RPCClient(nodeUrl);
rpcClient.getAccountState(myWallet.accounts[1].address)
  .then(response => {
    console.log(
      `NEO: ${response.balances[0].value}, GAS: ${response.balances[1].value}`
    );
  })
  .catch("Get Balance Error!");