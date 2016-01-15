using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.IO;
using KLib.tools;

namespace KLib.net.protocol
{
    public class CodeGenerater
    {

        private List<XElement> list_message;
        private CodeTemplate codeTemplate;
        private Dictionary<string, bool> dic_name = new Dictionary<string, bool>();
        private Dictionary<string, bool> dic_id = new Dictionary<string, bool>();
        private Dictionary<string, XElement> dic_struct = new Dictionary<string, XElement>();

        public string readItem(XElement item)
        {
            var name = item.Attribute("name").Value;
            var idStr = item.Attribute("id").Value;
            int id;
            try
            {
                id = Convert.ToInt32(idStr);
                idStr = id.ToString();
            }
            catch
            {
                throw new Exception(String.Format("转换消息id:{0} 失败", idStr));
            }

            if (dic_name.ContainsKey(name))
            {
                throw new Exception(String.Format("重复的协议名:{0}", name));
            }
            if (dic_id.ContainsKey(idStr))
            {
                throw new Exception(String.Format("重复的协议id:{0}", idStr));
            }

            dic_name.Add(name, true);
            dic_id.Add(idStr, true);

            return readParams(item);
        }

        public string readParams(XElement item, string parent = "")
        {

            string text = "";
            string singleVOText = "";

            var isMessage = item.Name.LocalName.ToLower() == "message";

            var itemName = item.Attribute("name").Value;
            var pathName = parent + itemName;

            string str_definition = "";
            string str_encode = "";
            string str_decode = "";

            var list_param = item.Elements().ToList<XElement>();
            for (int i = 0; i < list_param.Count; i++)
            {
                var param = list_param[i];
                var paramPrefix = param.Name.LocalName.ToLower();
                var memberName = param.Attribute("name").Value;
                var type = param.Attribute("type").Value.ToLower();
                var className = codeTemplate.getClassName(type);

                if (type == "struct")
                {
                    XElement structXML;

                    var attr_structName = param.Attribute("structName");
                    if (attr_structName != null)
                    {
                        if (dic_struct.ContainsKey(attr_structName.Value))
                        {
                            structXML = dic_struct[attr_structName.Value];
                            className = attr_structName.Value;
                            //readParams(structXML);
                        }
                        else
                        {
                            throw new Exception("不存在的结构体:" + attr_structName.Value);
                        }
                    }
                    else
                    {
                        structXML = param;
                        className = pathName + "_" + memberName;
                        text += readParams(structXML, pathName + "_");
                    }

                    className = codeTemplate.getFinalClassName(className);

                }

                if (paramPrefix == "param")
                {
                    str_decode += codeTemplate.getDecode(type, memberName, className);
                    str_encode += codeTemplate.getEncode(type, memberName, className);
                    str_definition += codeTemplate.getDefinition(className, memberName, readComment(param));
                }
                else if (paramPrefix == "listparam")
                {
                    str_decode += codeTemplate.getArrayDecode(type, memberName, className);
                    str_encode += codeTemplate.getArrayEncode(type, memberName, className);
                    str_definition += codeTemplate.getArrayDefinition(className, memberName, readComment(param));
                }
                else
                {
                    throw new Exception(String.Format("参数节点前缀必须为param或listParam。 name : {0}", memberName));
                }

            }

            var fileClassName = codeTemplate.getFinalClassName(pathName);

            if (isMessage)
                singleVOText = codeTemplate.getProtocolVOText(fileClassName, str_definition, item.Attribute("name").Value, str_decode, str_encode, readComment(item));
            else
                singleVOText = codeTemplate.getStructText(fileClassName, str_definition, str_decode, str_encode, readComment(item));

            if (singleMode)
                text += singleVOText;
            else
                FileUtil.writeFile(voFolderPath + fileClassName + codeTemplate.ClassExtension, Encoding.UTF8.GetBytes(singleVOText));

            return text;

        }

        public string readComment(XElement item)
        {
            var attribute_Comment = item.Attribute("comment");
            if (attribute_Comment != null)
                return attribute_Comment.Value;
            return "";
        }

        private string codeFolderPath;
        private string voFolderPath;
        private bool singleMode;
        private string voFolder;
        public void generate(string protocolXmlPath, string templateXmlPath, string codeFolderPath)
        {

            var template = XElement.Load(templateXmlPath);
            codeTemplate = new CodeTemplate();
            codeTemplate.load(template);

            singleMode = codeTemplate.getConfig("singleMode") == "true";

            voFolder = codeTemplate.getConfig("voFolder");

            codeFolderPath += "/";
            this.codeFolderPath = codeFolderPath;

            if (!Directory.Exists(codeFolderPath))
            {
                Directory.CreateDirectory(codeFolderPath);
            }

            if (!singleMode)
            {
                if (String.IsNullOrEmpty(voFolder))
                    this.voFolderPath = this.codeFolderPath;
                else
                {
                    this.voFolderPath = codeFolderPath + voFolder + "/";
                    if (Directory.Exists(voFolderPath))
                        Directory.Delete(voFolderPath, true);

                    Directory.CreateDirectory(voFolderPath);
                }
            }


            var text = "";

            var rootE = XElement.Load(protocolXmlPath);


            var list_struct = (from item in rootE.Elements("struct")
                               select item).ToList<XElement>();
            foreach (var item in list_struct)
            {
                var structName = item.Attribute("name").Value;
                if (dic_struct.ContainsKey(structName))
                    throw new Exception("重复的struct节点:" + structName);
                else
                {
                    text += readParams(item);
                    dic_struct.Add(structName, item);
                }
            }


            list_message =
                      (from item in rootE.Elements("message")
                       orderby Convert.ToInt32(item.Attribute("id").Value)
                       select item).ToList<XElement>();
            foreach (var item in list_message)
            {
                text += readItem(item);
            }


            if (singleMode)
                FileUtil.writeFile(codeFolderPath + codeTemplate.element_SingleProtocolFile.Attribute("fileName").Value, Encoding.UTF8.GetBytes(codeTemplate.getSingleFileText(text)));


            var str_enum = "";
            var str_registerMessage = "";
            var str_createMessage = "";
            for (int i = 0; i < list_message.Count; i++)
            {
                str_enum += codeTemplate.getEnumDefinition(list_message[i].Attribute("name").Value, list_message[i].Attribute("id").Value, readComment(list_message[i]));

                str_registerMessage += codeTemplate.getMessageRegister(list_message[i].Attribute("name").Value, list_message[i].Attribute("name").Value + "VO");

                str_createMessage += codeTemplate.getMessageCreater(list_message[i].Attribute("name").Value + "VO");
            }
            str_enum = codeTemplate.getProtocolEnumClass(str_enum);
            str_registerMessage = codeTemplate.getMessageRegisterClass(str_registerMessage, str_createMessage);

            FileUtil.writeFile(codeFolderPath + codeTemplate.element_ProtocolEnumClass.Attribute("fileName").Value, Encoding.UTF8.GetBytes(str_enum));
            FileUtil.writeFile(codeFolderPath + codeTemplate.element_MessageRegisterClass.Attribute("fileName").Value, Encoding.UTF8.GetBytes(str_registerMessage));


            var copyFiles = template.Element("CopyFiles");
            if (copyFiles != null)
            {
                var list_copy = copyFiles.Elements("CopyFile");
                foreach (var item in list_copy)
                {
                    var fileName = item.Attribute("fileName").Value.Trim();
                    var content = item.Value;
                    FileUtil.writeFile(codeFolderPath + fileName, Encoding.UTF8.GetBytes(content));
                }
            }

            var templateFileInfo = new FileInfo(templateXmlPath);
            var copyDirInfo = new DirectoryInfo(templateFileInfo.Directory + "/CopyFiles");
            if (copyDirInfo.Exists)
                FileUtil.copyFolder(copyDirInfo.FullName, codeFolderPath);


            Console.WriteLine("已生成代码至{0}", codeFolderPath);

        }

    }
}
