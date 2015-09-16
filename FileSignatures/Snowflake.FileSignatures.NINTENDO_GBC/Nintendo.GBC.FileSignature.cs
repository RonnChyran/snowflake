﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Snowflake.Romfile;
using Snowflake.Service;

namespace Snowflake.FileSignatures.NINTENDO_GBC
{
    public sealed class NintendoGBCFileSignature : FileSignature
    {
        [ImportingConstructor]
        public NintendoGBCFileSignature([Import("coreInstance")] ICoreService coreInstance)
            : base(Assembly.GetExecutingAssembly(), coreInstance)
        {
            this.HeaderSignature = new byte[8] { 0xCE, 0xED, 0x66, 0x66, 0xCC, 0x0D, 0x00, 0x0B }; //first 8 bytes of nintendo logo

        }
        
        public override byte[] HeaderSignature { get; }

        public override bool HeaderSignatureMatches(string fileName)
        {
            try
            {
                using (FileStream romStream = File.Open(fileName, FileMode.Open))
                {
                    byte[] buffer = new byte[8]; // read the 8 bytes

                    romStream.Seek(0x104, SeekOrigin.Begin); //seek to nntendo logo
                    romStream.Read(buffer, 0, buffer.Length);
                    romStream.Seek(0x143, SeekOrigin.Begin);
                    int cgb_magic = romStream.ReadByte();
                    return buffer.SequenceEqual(this.HeaderSignature) && (cgb_magic == 0xC0 || cgb_magic == 0x80);
                }
            }
            catch
            {
                return false;
            }
        }
        
        public override string GetInternalName(string fileName)
        {
            using (FileStream romStream = File.Open(fileName, FileMode.Open))
            {
                byte[] buffer = new byte[11]; // cgb internal names are only 11 bytes long
                romStream.Seek(0x134, SeekOrigin.Begin);
                romStream.Read(buffer, 0, buffer.Length);
                string code = Encoding.UTF8.GetString(buffer).Trim('\0'); 
                return code;
            }
        }
    }
}
