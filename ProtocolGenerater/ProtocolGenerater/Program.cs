using KLib.net.protocol;
using KLib.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolGenerater
{
    class Program
    {
        static void Main(string[] args)
        {

            var dic_params = CommandParse.parse(args);

            if (dic_params.ContainsKey("protocol") && dic_params.ContainsKey("template") && dic_params.ContainsKey("output"))
            {
                try
                {
                    var codeGenerater = new CodeGenerater();

                    codeGenerater.generate(
        dic_params["protocol"],
        dic_params["template"],
        dic_params["output"]
        );
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine(Properties.Resources.usage);
                Console.ReadLine();
            }


        }
    }
}
