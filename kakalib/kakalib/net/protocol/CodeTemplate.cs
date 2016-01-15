using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace KLib.net.protocol
{
    public class CodeTemplate
    {

        private const string mark_member = "$(member)";
        private const string mark_className = "$(className)";
        private const string mark_comment = "$(comment)";

        private string classExtension;

        public string ClassExtension
        {
            get { return classExtension; }
        }

        private string template_singleFile;
        private string template_class;
        private string template_structClass;
        private string template_arrayDecode;
        private string template_arrayEncode;
        private string template_arrayAccess;
        private string template_definitionMember;
        private string template_definitionArray;

        private string template_enumClass;
        private string template_enumDefinition;

        private string template_messageRegisterClass;
        private string template_messageRegister;
        private string template_messageCreater;

        public XElement element_SingleProtocolFile;
        public XElement element_ProtocolEnumClass;
        public XElement element_MessageRegisterClass;

        private XElement xml_template;
        private Dictionary<string, ParamVO> dic_param = new Dictionary<string, ParamVO>();
        public CodeTemplate()
        {

        }

        public void load(XElement xml_template)
        {
            this.xml_template = xml_template;

            classExtension = getConfig("classExtension");
            if (classExtension.IndexOf(".") < 0)
                classExtension = "." + classExtension;

            element_SingleProtocolFile = xml_template.Element("SingleProtocolFile");
            if (element_SingleProtocolFile != null)
                template_singleFile = element_SingleProtocolFile.Value;

            template_class = xml_template.Element("ProtocolVOClass").Value;
            template_class = Properties.Resources.logo_protocol + template_class;
            template_structClass = xml_template.Element("StructClass").Value;
            template_structClass = Properties.Resources.logo_protocol + template_structClass;
            template_definitionMember = xml_template.Element("definitionMember").Value;
            template_definitionArray = xml_template.Element("definitionArray").Value;
            template_arrayDecode = xml_template.Element("decodeArray").Value;
            template_arrayEncode = xml_template.Element("encodeArray").Value;
            template_arrayAccess = xml_template.Element("accessArray").Value.Trim();

            element_ProtocolEnumClass = xml_template.Element("ProtocolEnumClass");
            template_enumClass = element_ProtocolEnumClass.Value;
            template_enumClass = Properties.Resources.logo_protocol + template_enumClass;
            template_enumDefinition = xml_template.Element("definitionEnum").Value;

            element_MessageRegisterClass = xml_template.Element("MessageRegisterClass");
            template_messageRegisterClass = element_MessageRegisterClass.Value;
            template_messageRegisterClass = Properties.Resources.logo_protocol + template_messageRegisterClass;
            template_messageRegister = xml_template.Element("MessageRegister").Value;
            template_messageCreater = xml_template.Element("MessageCreater").Value;

            var list_type = xml_template.Element("params").Elements("param");
            foreach (var item in list_type)
            {
                var paramVO = new ParamVO()
                {
                    paramType = item.Attribute("type").Value.Trim().ToLower(),
                    className = item.Attribute("class").Value.Trim(),
                    template_decode = item.Element("decode").Value,
                    template_encode = item.Element("encode").Value,
                };
                dic_param[paramVO.paramType] = paramVO;
            }

        }

        public void load(string templatePath)
        {
            load(XElement.Load(templatePath));
        }

        public string getConfig(string name)
        {
            var config = xml_template.Element("config");
            var item = config.Element(name);
            if (item != null)
                return item.Value;
            return null;
        }

        public string getClassName(string paramType)
        {
            var paramVO = getParamVO(paramType);
            return paramVO.className;
        }

        public string getDecode(string paramType, string member, string structClassName = "structClassName")
        {
            var paramVO = getParamVO(paramType);
            var result = paramVO.template_decode.Replace(mark_member, member);
            result = result.Replace(mark_className, structClassName);
            return result;
        }

        public string getArrayDecode(string paramType, string member, string structClassName = "structClassName")
        {
            var paramVO = getParamVO(paramType);
            var arrayMember = template_arrayAccess.Replace(mark_member, member);
            var decode = getDecode(paramType, arrayMember, structClassName);
            var result = template_arrayDecode.Replace("$(decode)", decode);
            result = result.Replace(mark_member, member);
            result = result.Replace(mark_className, structClassName);
            return result;
        }

        public string getEncode(string paramType, string member, string structClassName = "structClassName")
        {
            var paramVO = getParamVO(paramType);
            var result = paramVO.template_encode.Replace(mark_member, member);
            result = result.Replace(mark_className, structClassName);
            return result;
        }

        public string getArrayEncode(string paramType, string member, string structClassName = "structClassName")
        {
            var paramVO = getParamVO(paramType);
            var arrayMember = template_arrayAccess.Replace(mark_member, member);
            var encode = getEncode(paramType, arrayMember, structClassName);
            var result = template_arrayEncode.Replace("$(encode)", encode);
            result = result.Replace(mark_member, member);
            result = result.Replace(mark_className, structClassName);
            return result;
        }

        public string getDefinition(string className, string member, string comment)
        {
            var result = template_definitionMember.Replace(mark_className, className);
            result = result.Replace(mark_member, member);
            result = result.Replace(mark_comment, comment);
            return result;
        }

        public string getArrayDefinition(string className, string member, string comment)
        {
            var result = template_definitionArray.Replace(mark_className, className);
            result = result.Replace(mark_member, member);
            result = result.Replace(mark_comment, comment);
            return result;
        }

        public string getProtocolVOText(string className, string definition, string messageName, string decode, string encode, string comment)
        {
            var text = template_class;
            text = text.Replace(mark_className, className);
            text = text.Replace("$(definition)", definition);
            text = text.Replace("$(messageName)", messageName);
            text = text.Replace("$(decode)", decode);
            text = text.Replace("$(encode)", encode);
            text = text.Replace(mark_comment, comment);
            text = text.Replace("\n", "\r\n");
            text = text.Replace("\r\r\n", "\r\n");
            return text;
        }

        public string getStructText(string className, string definition, string decode, string encode, string comment)
        {
            var text = template_structClass;
            text = text.Replace(mark_className, className);
            text = text.Replace("$(definition)", definition);
            text = text.Replace("$(decode)", decode);
            text = text.Replace("$(encode)", encode);
            text = text.Replace(mark_comment, comment);
            text = text.Replace("\n", "\r\n");
            text = text.Replace("\r\r\n", "\r\n");
            return text;
        }

        public string getSingleFileText(string content)
        {
            var text = template_singleFile;
            text = text.Replace("$(content)", content);
            return text;
        }

        public string getFinalClassName(string className)
        {
            return className + getConfig("classNameTail");
        }

        private string firstCharToUp(string str)
        {
            var firstChar = str.Substring(0, 1);
            str = firstChar.ToUpper() + str.Substring(1);
            return str;
        }

        private ParamVO getParamVO(string paramType)
        {
            paramType = paramType.ToLower();
            if (dic_param.ContainsKey(paramType))
                return dic_param[paramType];
            throw new Exception("不支持的paramType:" + paramType);
        }

        private class ParamVO
        {
            public string paramType;
            public string className;
            public string template_encode;
            public string template_decode;
        }


        public string getEnumDefinition(string member, string value, string comment)
        {
            var result = template_enumDefinition.Replace(mark_member, member);
            result = result.Replace("$(value)", value);
            result = result.Replace(mark_comment, comment);
            return result;
        }

        public string getProtocolEnumClass(string content)
        {
            var text = template_enumClass;
            text = text.Replace("$(content)", content);
            text = text.Replace("\n", "\r\n");
            text = text.Replace("\r\r\n", "\r\n");
            return text;
        }

        public string getMessageRegister(string member, string className)
        {
            var result = template_messageRegister.Replace(mark_member, member);
            result = result.Replace(mark_className, className);
            return result;
        }

        public string getMessageCreater(string className)
        {
            var result = template_messageCreater.Replace(mark_className, className);
            return result;
        }

        public string getMessageRegisterClass(string content, string creater)
        {
            var text = template_messageRegisterClass;
            text = text.Replace("$(content)", content);
            text = text.Replace("$(creater)", creater);
            text = text.Replace("\n", "\r\n");
            text = text.Replace("\r\r\n", "\r\n");
            return text;
        }

    }
}
