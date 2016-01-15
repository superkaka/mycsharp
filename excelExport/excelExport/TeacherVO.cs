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

public class TeacherVO
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
    
    private int _Age;
    /// <summary>
    /// 年龄
    /// </summary>
    public int Age
    {
        get { return _Age; }
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
    

    public void decode(EndianBinaryReader binReader)
    {
        
        _Id = int.Parse(binReader.ReadUTF());
        
        _Name = binReader.ReadUTF();
        
        _Age = int.Parse(binReader.ReadUTF());
        
        _Jump = float.Parse(binReader.ReadUTF());
        
        _Time = DateTime.Parse(binReader.ReadUTF());
        
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
            var vo = new TeacherVO();
            vo.decode(binReader);
            list_vo.Add(vo);
            dic_vo.Add(vo.Id, vo);
        }

    }

    static private Dictionary<int, TeacherVO> dic_vo = new Dictionary<int, TeacherVO>();
    static private List<TeacherVO> list_vo = new List<TeacherVO>();

    static public IList<TeacherVO> GetVOList()
    {
        return list_vo.AsReadOnly();
    }

    static public TeacherVO GetVO(int Id)
    {
        return dic_vo[Id];
    }

    static public IList<TeacherVO> Where(Func<TeacherVO, bool> filter)
    {
        return list_vo.Where(filter).ToList();
    }

}
