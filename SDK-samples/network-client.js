const { default: Neon, rpc, api} = require("@cityofzion/neon-js");
/*
 方式一：直接连NEO-cli私链节点，测试网节点，或者主网节点.
         但是只可以调用neo-cli提供的RPC接口
*/
const nodeUrl = "http://192.168.99.100:30333"
const rpcClient = new rpc.RPCClient(nodeUrl);

//查询快高
rpcClient.getBlockCount()
   .then(blockCount => {
      console.log("--- Current Block Hight 1 ---");
      console.log(blockCount);
    })
    .catch(err => console.log(err)); 

/*
 方式二：通过neoScan提供的接口，可以使用任何neoScan提供的接口和方法
*/
// Connect to neo-local network
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

apiProvider.getHeight()
    .then(blockCount => {
        console.log("--- Current Block Hight 2---");
        console.log(blockCount);
    })
    .catch(err => console.log(err));