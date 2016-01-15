// These Codes are generated by kakaTools ExcelExport v2.0
// ------------------------------------------------------------------
//
// Copyright (c) 2015 linchen.
// All rights reserved.
//
// Email: superkaka.org@gmail.com
//
// ------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using KLib.utils;

public class SchoolVO
{
    
    private int _Id;
    /// <summary>
    /// 
    /// </summary>
    public int Id
    {
        get { return _Id; }
    }
    
    private string _Name;
    /// <summary>
    /// 名字
    /// </summary>
    public string Name
    {
        get { return _Name; }
    }
    
    public StudentVO NameToStudentVO
    {
        get { return StudentVO.GetVO(Name); }
    }
    
    private int _Age;
    /// <summary>
    /// 年龄
    /// </summary>
    public int Age
    {
        get { return _Age; }
    }
    
    private bool _Sex;
    /// <summary>
    /// 性别
    /// </summary>
    public bool Sex
    {
        get { return _Sex; }
    }
    
    private float _Jump;
    /// <summary>
    /// 
    /// </summary>
    public float Jump
    {
        get { return _Jump; }
    }
    
    private DateTime _Time;
    /// <summary>
    /// 
    /// </summary>
    public DateTime Time
    {
        get { return _Time; }
    }
    
    private string[] _StringList;
    /// <summary>
    /// 注释1 222 注释3 
    /// </summary>
    public ReadOnlyCollection<string> StringList
    {
        get { return new ReadOnlyCollection<string>(_StringList); }
    }
    

    public void decode(EndianBinaryReader binReader)
    {
        
        _Id = int.Parse(binReader.ReadUTF());
        
        _Name = binReader.ReadUTF();
        
        _Age = int.Parse(binReader.ReadUTF());
        
        _Sex = binReader.ReadUTF().ToLower() == "true";
        
        _Jump = float.Parse(binReader.ReadUTF());
        
        _Time = DateTime.Parse(binReader.ReadUTF());
        
        var len_StringList = binReader.ReadInt32();
        _StringList = new string[len_StringList];
        for (int i = 0; i < len_StringList; i++)
        {
            _StringList[i] = binReader.ReadUTF();
        }
        
    }

    static public void Fill(byte[] bytes)
    {

        var binReader = new EndianBinaryReader(Endian.LittleEndian, new MemoryStream(bytes));
        binReader.Endian = binReader.ReadBoolean() ? Endian.LittleEndian : Endian.BigEndian;
        
        
        var jumpPos = binReader.ReadInt32();
        
        //跳过表头信息
        binReader.BaseStream.Position = jumpPos;
        
        /*
        var headerCount = binReader.ReadInt32();
        var headers = new string[headerCount];
        var types = new string[headerCount];
        for (var i = 0; i < headerCount; i++)
        {
            headers[i] = binReader.ReadUTF();
            types[i] = binReader.ReadUTF();
        }
        */
        
        var count = binReader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var vo = new SchoolVO();
            vo.decode(binReader);
            list_vo.Add(vo);
            dic_vo.Add(vo.Age, vo);
        }

    }

    static private Dictionary<int, SchoolVO> dic_vo = new Dictionary<int, SchoolVO>();
    static private List<SchoolVO> list_vo = new List<SchoolVO>();

    static public IList<SchoolVO> GetVOList()
    {
        return list_vo.AsReadOnly();
    }

    static public SchoolVO GetVO(int Age)
    {
        return dic_vo[Age];
    }

    static public IList<SchoolVO> Where(Func<SchoolVO, bool> filter)
    {
        return list_vo.Where(filter).ToList();
    }

}