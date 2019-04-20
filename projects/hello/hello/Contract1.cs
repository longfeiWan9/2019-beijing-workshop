/* SC example:hello world.
 * 部署：sc deploy <script-hashFa> False False Fasle 10 07
 * 调用：sc invoke <script-hash> 'beijing'
*/
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace hello
{
    public class Contract1 : SmartContract
    {
        public static string Main(object[] args)
        {
            string text = "Welcome to NEO, " + args[0];
            return text;
        }
    }
}