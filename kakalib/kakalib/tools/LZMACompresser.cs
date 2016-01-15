﻿using System;
using System.Collections.Generic;
using System.Text;
using KLib.interfaces;
using System.IO;
using SevenZip;
using lzma = SevenZip.Compression.LZMA;

namespace KLib.tools
{
    public class LZMACompresser : ICompresser
    {

        public byte[] compress(byte[] bytes)
        {
            var outStream = new MemoryStream();
            compress(new MemoryStream(bytes), outStream);
            outStream.Position = 0;
            var outBytes = new byte[outStream.Length];
            outStream.Read(outBytes, 0, outBytes.Length);
            return outBytes;
        }

        public byte[] uncompress(byte[] bytes)
        {
            var outStream = new MemoryStream();
            uncompress(new MemoryStream(bytes), outStream);
            outStream.Position = 0;
            var outBytes = new byte[outStream.Length];
            outStream.Read(outBytes, 0, outBytes.Length);
            return outBytes;
        }

        public void compress(Stream inStream, Stream outStream)
        {

            var coder = new lzma.Encoder();

            // Write the encoder properties
            coder.WriteCoderProperties(outStream);

            // Write the decompressed file size.
            outStream.Write(BitConverter.GetBytes(inStream.Length), 0, 8);

            // Encode the file.
            coder.Code(inStream, outStream, inStream.Length, -1, null);

        }

        public void uncompress(Stream inStream, Stream outStream)
        {

            var coder = new lzma.Decoder();

            // Read the decoder properties
            byte[] properties = new byte[5];
            inStream.Read(properties, 0, 5);

            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            inStream.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            // Decompress the file.
            coder.SetDecoderProperties(properties);
            coder.Code(inStream, outStream, inStream.Length, fileLength, null);

        }



    }
}
