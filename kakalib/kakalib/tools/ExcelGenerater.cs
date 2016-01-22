using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.OleDb;
using KLib.interfaces;
using KLib.tools;
using KLib.enums;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using KLib.utils;
using System.Text.RegularExpressions;


namespace KLib.tools
{
    public class ExcelGenerater
    {

        static public string templatePath;
        static public Endian endian;
        static public string fileExt = ".kk";

        static public bool invalid
        {
            get
            {
                var date = DateTime.Now;
                return date > ExpiresTime;
            }
        }

        static public DateTime ExpiresTime
        {
            get { return new DateTime(2016, 3, 22); }
        }

        static public void export(String inputPath, String outputPath, CompressOption op, String prefix_primaryKey, String prefix_IgnoreSheet, String prefix_IgnoreLine, String prefix_IgnoreColumn, Boolean ignoreBlank)
        {

            if (Directory.Exists(inputPath))
            {
                DirectoryInfo di = new DirectoryInfo(inputPath);

                FileInfo[] fileInfos = di.GetFiles("*.xls", SearchOption.AllDirectories);

                for (int k = 0; k < fileInfos.Length; k++)
                {
                    //如果不是隐藏文件  则解析
                    if ((fileInfos[k].Attributes & FileAttributes.Hidden) == 0)
                    {
                        export(fileInfos[k].FullName, outputPath, op, prefix_primaryKey, prefix_IgnoreSheet, prefix_IgnoreLine, prefix_IgnoreColumn, ignoreBlank);
                    }

                }
            }
            else
            {
                ExcelTable[] sheets = doExport(inputPath, prefix_primaryKey, prefix_IgnoreSheet, prefix_IgnoreLine, prefix_IgnoreColumn, ignoreBlank);

                if (null == outputPath || "" == outputPath)
                {
                    FileInfo fi = new FileInfo(inputPath);
                    outputPath = fi.DirectoryName;
                }

                if (!Directory.Exists(outputPath))
                    throw new Exception("导出路径\"" + outputPath + "\"不存在");

                foreach (ExcelTable sheet in sheets)
                {

                    String path = outputPath + sheet.name + fileExt;

                    Byte[] bytes = sheet.ToBytes(endian);

                    MemoryStream inStream = new MemoryStream(bytes);
                    MemoryStream outStream = new MemoryStream();

                    ICompresser compresser;

                    switch (op)
                    {

                        case CompressOption.lzma:
                            compresser = new LZMACompresser();
                            compresser.compress(inStream, outStream);
                            break;

                        case CompressOption.zlib:
                            compresser = new ZlibCompresser();
                            compresser.compress(inStream, outStream);
                            break;

                        case CompressOption.gzip:
                            compresser = new GZipCompresser();
                            compresser.compress(inStream, outStream);
                            break;

                        case CompressOption.none:
                            outStream = inStream;
                            break;

                        default:
                            throw new Exception();

                    }

                    FileStream fs = File.Create(path);
                    outStream.WriteTo(fs);
                    fs.Close();

                    outStream.Dispose();

                    Console.WriteLine("已生成数据文件");
                    Console.WriteLine(path);
                    Console.WriteLine();

                }

            }

        }

        static public void export(String[] inputPathList, String outputPath, CompressOption op, String prefix_primaryKey, String prefix_IgnoreSheet, String prefix_IgnoreLine, String prefix_IgnoreColumn, Boolean ignoreBlank)
        {

            int i = 0;
            int len = inputPathList.Length;

            while (i < len)
            {

                export(inputPathList[i], outputPath, op, prefix_primaryKey, prefix_IgnoreSheet, prefix_IgnoreLine, prefix_IgnoreColumn, ignoreBlank);

                i++;

            }

            Console.WriteLine("已生成代码至");
            Console.WriteLine(codeFolderPath);

        }

        static ExcelTable[] doExport(String source, String prefix_primaryKey, String prefix_IgnoreSheet, String prefix_IgnoreLine, String prefix_IgnoreColumn, Boolean ignoreBlank)
        {

            //HDR参数：YES忽视第一行；NO包括第一行
            String ConnStr = "Provider=Microsoft.Ace.OleDB.12.0;Data Source=" + source + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1;'";

            OleDbConnection conn = new OleDbConnection(ConnStr);
            conn.Open();

            ArrayList sheets = new ArrayList();

            String[] sheetNames = getSheetName(conn);

            //ExcelTable[] list = new ExcelTable[sheetNames.Length];

            foreach (String sheetName in sheetNames)
            {

                //忽略表
                if (isPrefix(sheetName, prefix_IgnoreSheet))
                    continue;

                try
                {
                    ExcelTable excelSheet = processSheet(conn, sheetName, prefix_primaryKey, prefix_IgnoreLine, prefix_IgnoreColumn, ignoreBlank);
                    if (excelSheet != null)
                    {
                        sheets.Add(excelSheet);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("处理表{0}失败\r\n", sheetName) + e.Message, e);
                }

            }

            ExcelTable[] tables = (ExcelTable[])sheets.ToArray(typeof(ExcelTable));

            conn.Close();
            conn.Dispose();


            if (!Directory.Exists(codeFolderPath))
                Directory.CreateDirectory(codeFolderPath);

            var codeTemplate = new ExcelCodeTemplate();
            //codeTemplate.load(@"D:\work\VS\mycsharp\excelExport\excelExport\template_csharp.xml");
            codeTemplate.load(templatePath);

            var reg_enter = new Regex(@"[\r\n]");

            foreach (var sheet in tables)
            {
                string str_definition = "";
                string str_decode = "";
                var fileClassName = codeTemplate.getFinalClassName(sheet.name);
                for (int i = 0; i < sheet.types.Length; i++)
                {
                    var memberName = /*sheet.header[i] =*/ codeTemplate.getFinalMemberName(sheet.header[i]);
                    var relationName = sheet.relations[i];
                    var type = sheet.types[i];
                    var isArray = sheet.isArray[i];
                    var comment = reg_enter.Replace(sheet.comments[i], " ");
                    var className = codeTemplate.getTypeClassName(type);


                    if (!string.IsNullOrEmpty(isArray))
                    {
                        str_decode += codeTemplate.getDecode(type, memberName, true);
                        str_definition += codeTemplate.getArrayDefinition(className, memberName, comment);
                    }
                    else
                    {
                        str_decode += codeTemplate.getDecode(type, memberName, false);
                        str_definition += codeTemplate.getDefinition(className, memberName, comment);
                        if (!string.IsNullOrEmpty(relationName))
                        {
                            str_definition += codeTemplate.getRelationMember(codeTemplate.getFinalClassName(relationName), memberName);
                        }
                    }
                }

                var classText = codeTemplate.getClassText(sheet.name, fileClassName, str_definition, str_decode, sheet.types[sheet.primaryKeyIndex], codeTemplate.getFinalMemberName(sheet.header[sheet.primaryKeyIndex]));
                FileUtil.writeFile(codeFolderPath + fileClassName + codeTemplate.ClassExtension, Encoding.UTF8.GetBytes(classText));

            }


            return tables;

        }

        //static public string codeFolderPath = @"J:\code\";
        static public string codeFolderPath;


        static private ExcelTable processSheet(OleDbConnection conn, String sheetName, String prefix_primaryKey, String prefix_IgnoreLine, String prefix_IgnoreColumn, Boolean ignoreBlank)
        {

            string query = "SELECT   *   FROM  [" + sheetName + "]";


            OleDbCommand oleCommand = new OleDbCommand(query, conn);
            OleDbDataAdapter oleAdapter = new OleDbDataAdapter(oleCommand);
            DataSet myDataSet = new DataSet();

            //   将   Excel   的[Sheet1]表内容填充到   DataSet   对象 
            oleAdapter.Fill(myDataSet, sheetName);


            DataTable dt = myDataSet.Tables[0];

            //字段名+类型  至少2行
            if (dt.Rows.Count < 3 && dt.Columns.Count == 1)
            {
                return null;
            }


            //while (true)
            //{
            //    //忽略行
            //    if (isPrefix(dt.Rows[0][0].ToString(), prefix_IgnoreLine))
            //    {

            //        dt.Rows.RemoveAt(0);
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}

            Object[] header = new Object[dt.Rows[0].ItemArray.Length];
            dt.Rows[0].ItemArray.CopyTo(header, 0);

            if (ignoreBlank)
            {
                ///消除空白行
                int maxColumn = header.Length;
                int row = dt.Rows.Count - 1;
                while (row >= 0)
                {
                    Boolean hasData = false;
                    DataRow dataRow = dt.Rows[row];
                    int column = 0;
                    while (column < maxColumn)
                    {
                        if (dataRow[column].ToString() != "")
                        {
                            hasData = true;
                            break;
                        }
                        column++;
                    }

                    if (!hasData)
                    {
                        dt.Rows.RemoveAt(row);
                    }

                    row--;
                }
            }

            int i = dt.Rows.Count - 1;
            while (i >= 0)
            {
                //忽略行
                if (isPrefix(dt.Rows[i][0].ToString(), prefix_IgnoreLine))
                {
                    dt.Rows.RemoveAt(i);
                }
                i--;
            }

            header = new Object[dt.Rows[0].ItemArray.Length];
            dt.Rows[0].ItemArray.CopyTo(header, 0);


            i = header.Length - 1;
            while (i >= 0)
            {
                //忽略列
                if (isPrefix(header[i].ToString(), prefix_IgnoreColumn))
                {
                    dt.Columns.RemoveAt(i);
                }
                i--;
            }


            header = new Object[dt.Rows[1].ItemArray.Length];
            dt.Rows[1].ItemArray.CopyTo(header, 0);

            if (ignoreBlank)
            {
                ///消除空白列
                int column = header.Length - 1;
                while (column >= 0)
                {
                    if (header[column].ToString() == "")
                    {
                        dt.Columns.RemoveAt(column);
                    }
                    column--;
                }

                header = new String[dt.Rows[1].ItemArray.Length];
                dt.Rows[1].ItemArray.CopyTo(header, 0);

            }

            //关联字段解析
            var reg_relation = new Regex(@"\[(\w+)\]$");
            var relations = new string[header.Length];

            for (int i_header = 0; i_header < header.Length; i_header++)
            {
                var str_header = header[i_header].ToString().Trim();
                //字段名先不换成大写开头
                //str_header = firstCharToUp(str_header);

                Match m_relation = reg_relation.Match(str_header);
                if (m_relation.Groups.Count > 1)
                {
                    str_header = reg_relation.Replace(str_header, "");
                    relations[i_header] = firstCharToUp(m_relation.Groups[1].Value);
                }
                header[i_header] = str_header;
            }

            var comments = new String[dt.Rows[0].ItemArray.Length];
            for (int i_comments = 0; i_comments < comments.Length; i_comments++)
            {
                comments[i_comments] = dt.Rows[0].ItemArray[i_comments].ToString();
            }

            //数组相关解析
            var reg_array = new Regex(@"\[(.*)\]$");
            var types = new String[dt.Rows[2].ItemArray.Length];
            var isArray = new String[types.Length];
            dt.Rows[2].ItemArray.CopyTo(types, 0);
            for (int i_types = 0; i_types < types.Length; i_types++)
            {
                var str_type = types[i_types].ToLower();

                Match m_isArray = reg_array.Match(str_type);
                if (m_isArray.Groups.Count > 1)
                {
                    str_type = reg_array.Replace(str_type, "");
                    isArray[i_types] = m_isArray.Groups[1].Value;
                    if (isArray[i_types] == "")
                        isArray[i_types] = ",";
                }
                types[i_types] = str_type;
            }

            ///去掉表头
            dt.Rows.RemoveAt(0);
            dt.Rows.RemoveAt(0);
            dt.Rows.RemoveAt(0);




            ExcelTable sheet = new ExcelTable();

            ///查找主键并赋值
            i = 0;
            while (i < header.Length)
            {
                String head = header[i].ToString();

                dt.Columns[i].ColumnName = head;

                if (isPrefix(head, prefix_primaryKey))
                {
                    head = head.Substring(prefix_primaryKey.Length);
                    sheet.primaryKeyIndex = i;
                    header[i] = head;
                    break;
                }
                i++;
            }

            sheet.name = sheetName.Substring(0, sheetName.Length - 1);
            sheet.header = (String[])header;
            sheet.relations = relations;
            sheet.comments = comments;
            sheet.types = types;
            sheet.isArray = isArray;
            sheet.dataTable = dt;

            return sheet;

        }

        static private string firstCharToUp(string str)
        {
            var firstChar = str.Substring(0, 1);
            str = firstChar.ToUpper() + str.Substring(1);
            return str;
        }

        /// <summary>
        /// 获取所有工作表名
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        static private String[] getSheetName(OleDbConnection conn)
        {

            DataTable tb = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            String[] excelSheets = new String[tb.Rows.Count];
            int i = 0;

            // Add the sheet name to the string array.
            foreach (DataRow row in tb.Rows)
            {
                var sheetName = row["TABLE_NAME"].ToString().Trim('\'');
                //有些带筛选区域的表会导致出现多余的表 类似   "表名$_xlnm#_FilterDatabase"
                if (sheetName.IndexOf("FilterDatabase") >= 0)
                    continue;

                excelSheets[i] = sheetName;
                i++;
            }

            return excelSheets;

        }

        /// <summary>
        /// 是否包含前缀
        /// </summary>
        /// <param name="str"></param>
        /// <param name="refix"></param>
        /// <returns></returns>
        static private Boolean isPrefix(String str, String prefix)
        {
            if (null == prefix) return false;
            prefix = prefix.Trim();
            if (prefix == "") return false;
            return str.IndexOf(prefix) == 0;
        }

    }

    public class ExcelCodeTemplate
    {

        private XElement config;
        private const string mark_member = "$(member)";
        private const string mark_className = "$(className)";
        private const string mark_comment = "$(comment)";

        private string classExtension;

        public string ClassExtension
        {
            get { return classExtension; }
        }

        private string template_class;
        private string template_definitionMember;
        private string template_memberDecode;
        private string template_relationMember;
        private string template_definitionArray;
        private string template_arrayDecode;

        public XElement element_SingleProtocolFile;
        public XElement element_ProtocolEnumClass;
        public XElement element_MessageRegisterClass;

        private XElement xml_template;
        private Dictionary<string, ParamVO> dic_param = new Dictionary<string, ParamVO>();

        public void load(XElement xml_template)
        {
            this.xml_template = xml_template;

            config = xml_template.Element("config");
            classExtension = getConfig("classExtension");
            if (classExtension.IndexOf(".") < 0)
                classExtension = "." + classExtension;

            template_class = xml_template.Element("Class").Value;
            template_class = Properties.Resources.logo_excel + template_class;
            template_definitionMember = xml_template.Element("definitionMember").Value;
            template_memberDecode = xml_template.Element("decodeMember").Value;
            template_relationMember = xml_template.Element("relationMember").Value;
            template_definitionArray = xml_template.Element("definitionArray").Value;
            template_arrayDecode = xml_template.Element("decodeArray").Value;

            var list_type = xml_template.Element("params").Elements("param");
            foreach (var item in list_type)
            {
                var paramVO = new ParamVO()
                {
                    paramType = item.Attribute("type").Value.Trim().ToLower(),
                    className = item.Attribute("class").Value.Trim(),
                    template_decode = item.Element("decode").Value,
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
            var item = config.Element(name);
            if (item != null)
                return item.Value;
            return "";
        }

        public string getTypeClassName(string paramType)
        {
            var paramVO = getParamVO(paramType);
            return paramVO.className;
        }

        private string getDecode(string paramType)
        {
            var paramVO = getParamVO(paramType);
            var result = paramVO.template_decode;
            return result.Trim();
        }

        public string getDecode(string paramType, string member, bool isArray)
        {
            var paramVO = getParamVO(paramType);
            var structClassName = paramVO.className;
            var decode = getDecode(paramType);
            string result;
            if (isArray)
                result = template_arrayDecode;
            else
                result = template_memberDecode;

            result = result.Replace("$(decode)", decode);
            result = result.Replace(mark_member, member);
            result = result.Replace(mark_className, structClassName);
            return result;
        }

        public string getDefinition(string typeClassName, string member, string comment)
        {
            var result = template_definitionMember.Replace(mark_className, typeClassName);
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

        public string getRelationMember(string className, string member)
        {
            var result = template_relationMember.Replace(mark_className, className);
            result = result.Replace(mark_member, member);
            return result;
        }

        public string getClassText(string tableName, string className, string definition, string decode, string primaryKeyType, string primaryKeyName, string comment = "")
        {
            var text = template_class;
            text = text.Replace("$(tableName)", tableName);
            text = text.Replace(mark_className, className);
            text = text.Replace("$(definition)", definition);
            text = text.Replace("$(decode)", decode);
            text = text.Replace("$(primaryKeyType)", primaryKeyType);
            text = text.Replace("$(primaryKeyName)", primaryKeyName);

            text = text.Replace(mark_comment, comment);
            text = text.Replace("\n", "\r\n");
            text = text.Replace("\r\r\n", "\r\n");
            return text;
        }

        public string getFinalMemberName(string member)
        {
            if (getConfig("memberStartUpperCase") == "true")
                member = firstCharToUp(member);
            return member;
        }

        public string getFinalClassName(string className)
        {
            if (getConfig("classStartUpperCase") == "true")
                className = firstCharToUp(className);
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
            public string template_decode;
        }

    }

    public class ExcelTable
    {
        /// <summary>
        /// 工作薄显示名
        /// </summary>
        public String name;

        /// <summary>
        /// 字段名列表
        /// </summary>
        public String[] header;
        public String[] relations;
        public String[] comments;
        public String[] types;
        public String[] isArray;

        public int primaryKeyIndex;


        /// <summary>
        /// 数据表对象
        /// </summary>
        public DataTable dataTable;

        public byte[] ToBytes(Endian endian)
        {

            var ms = new MemoryStream();
            var binWriter = new EndianBinaryWriter(endian, ms);
            binWriter.Write(endian == Endian.LittleEndian);


            var jumpPos = binWriter.BaseStream.Position;
            binWriter.Write((int)-1);

            binWriter.Write(header.Length);

            for (var i = 0; i < header.Length; i++)
            {
                binWriter.WriteUTF(header[i]);
                binWriter.WriteUTF(types[i]);
                binWriter.Write(isArray[i] != null);
            }


            var nowPos = binWriter.BaseStream.Position;

            binWriter.BaseStream.Position = jumpPos;
            binWriter.Write((int)nowPos);
            binWriter.BaseStream.Position = nowPos;

            int rowCount = (int)dataTable.Rows.Count;

            if (ExcelGenerater.invalid && rowCount > 1)
            {
                rowCount /= 2;
            }

            binWriter.Write(rowCount);

            for (int i = 0; i < rowCount; i++)
            {
                var row = dataTable.Rows[i];
                for (int j = 0; j < header.Length; j++)
                {

                    if (isArray[j] != null)
                    {
                        var list_arrayValue = row[j].ToString().Split(new string[] { isArray[j] }, StringSplitOptions.RemoveEmptyEntries);

                        binWriter.Write(list_arrayValue.Length);
                        for (int k = 0; k < list_arrayValue.Length; k++)
                        {
                            binWriter.WriteUTF(list_arrayValue[k]);
                        }
                    }
                    else
                        binWriter.WriteUTF(row[j].ToString());

                }
            }

            return ms.ToArray();
        }

    }

}
