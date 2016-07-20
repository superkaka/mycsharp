using KLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ProtocolGenerater
{
    class Program
    {
        static void Main(string[] args)
        {

            var dic_params = CommandParse.parse(args);

#if DEBUG
            dic_params["protocol"] = @"D:\work\VS\mycsharp\ProtocolGenerater\ProtocolGenerater\protocol";
            dic_params["template"] = @"D:\work\VS\mycsharp\ProtocolGenerater\ProtocolGenerater\templates\csharp\template_csharp.xml";
            dic_params["output"] = @"J:\codes";
#endif

            if (dic_params.ContainsKey("protocol") && dic_params.ContainsKey("template") && dic_params.ContainsKey("output"))
            {
                //            var codeGenerater = new CodeGenerater();

                //            codeGenerater.generate(
                //dic_params["protocol"],
                //dic_params["template"],
                //dic_params["output"]
                //);
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

            Console.WriteLine("按任意键退出");
            Console.ReadLine();
        }
    }
}
