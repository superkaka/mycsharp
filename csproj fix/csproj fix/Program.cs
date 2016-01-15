using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace csproj_fix
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var doc = new XmlDocument();
                //doc.Load(@"D:\work\unity\project\xiaomu\gdclient\UnityVS.gdclient.CSharp.csproj");
                doc.Load(@"UnityVS.gdclient.CSharp.csproj");
                var list_ItemGroup = doc.DocumentElement.GetElementsByTagName("ItemGroup");
                /*
                var docX = XDocument.Parse(doc.OuterXml);
                var list = from item in docX.Descendants("Project").Descendants("ItemGroup")
                           select item;

                foreach (var item in list)
                {
                    var reference = item.Descendants("Reference");
                    Console.WriteLine(reference);
                }
                */
                foreach (XmlElement ItemGroup in list_ItemGroup)
                {
                    var node = doc.CreateElement("Reference", null);

                    node.SetAttribute("Include", "System.Data");
                    //ItemGroup.AppendChild(node);
                    ItemGroup.InnerXml += node.OuterXml;
                }

                //doc.Save(@"D:\work\unity\project\xiaomu\gdclient\UnityVS.gdclient.CSharp - my.csproj");
                doc.Save(@"UnityVS.gdclient.CSharp - my.csproj");
                Console.WriteLine("创建成功");
                //Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }

        }
    }
}
