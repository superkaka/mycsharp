using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KLib.tools;

namespace AssetBundleBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var dic = CommandParse.parse(args);
            if (dic.ContainsKey("input") && dic.ContainsKey("output"))
            {
                try
                {
                    var thread = 3;
                    if (dic.ContainsKey("thread"))
                        thread = Convert.ToInt32(dic["thread"]);
                    UnityAssetBundleBuilder.build(dic["input"], dic["output"], thread);
#if DEBUG
                    Console.ReadLine();
#endif
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("-input 源assetbundle文件夹，必须是包含svn信息的根目录");
                Console.WriteLine("-output 导出目录");
                Console.ReadLine();
            }
        }
    }
}
