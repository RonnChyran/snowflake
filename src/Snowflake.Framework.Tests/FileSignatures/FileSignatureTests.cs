﻿using System;
using System.Collections.Generic;
using System.Text;
using Snowflake.Romfile;
using Snowflake.Stone.FileSignatures.Nintendo;
using Snowflake.Tests;
using Xunit;

namespace Snowflake.FileSignatures.Tests
{
    public class FileSignatureTests
    {
        [Theory]
        [InlineData(typeof(NintendoEntertainmentSystemiNesFileSignature), "test.nes")]
        [InlineData(typeof(NintendoEntertainmentSystemUnifFileSignature), "scanline.unif")]
        [InlineData(typeof(SuperNintendoHeaderlessFileSignature), "bsnesdemo_v1.sfc")]
        [InlineData(typeof(SuperNintendoSmcHeaderFileSignature), "bsnesdemo_v1.smc")]
        [InlineData(typeof(GameboyAdvancedFileSignature), "suite.gba")]
        [InlineData(typeof(NintendoDSFileSignature), "slot1launch.nds", typeof(GameboyAdvancedFileSignature))]
        [InlineData(typeof(GameboyFileSignature), "flappyboy.gb", typeof(GameboyColorFileSignature))]
        [InlineData(typeof(GameboyColorFileSignature), "infinity.gbc", typeof(GameboyFileSignature))]
        [InlineData(typeof(Nintendo64ByteswappedFileSignature), "rx-mm64.v64", typeof(Nintendo64BigEndianFileSignature))]
        [InlineData(typeof(Nintendo64LittleEndianFileSignature), "rx-mm64.n64", typeof(Nintendo64ByteswappedFileSignature))]
        [InlineData(typeof(Nintendo64BigEndianFileSignature), "setscreenntsc.z64", typeof(Nintendo64LittleEndianFileSignature))]

        public void Verify_Test(Type fileSignature, string filename, Type exclusionFs = null)
        {
            using var testStream = TestUtilities.GetResource($"TestRoms.{filename}");
            IFileSignature signature = (IFileSignature)Activator.CreateInstance(fileSignature);
            Assert.True(signature.HeaderSignatureMatches(testStream));

            if (exclusionFs != null)
            {
                IFileSignature exclusion = (IFileSignature)Activator.CreateInstance(exclusionFs);
                Assert.False(exclusion.HeaderSignatureMatches(testStream));
            }
        }

        [Theory]
        [InlineData(typeof(NintendoEntertainmentSystemiNesFileSignature), "test.nes", null)]
        [InlineData(typeof(NintendoEntertainmentSystemUnifFileSignature), "scanline.unif", null)]
        [InlineData(typeof(SuperNintendoHeaderlessFileSignature), "bsnesdemo_v1.sfc", "SNES")]
        [InlineData(typeof(SuperNintendoSmcHeaderFileSignature), "bsnesdemo_v1.smc", "SNES")]
        [InlineData(typeof(GameboyAdvancedFileSignature), "suite.gba", "TEST")]
        [InlineData(typeof(NintendoDSFileSignature), "slot1launch.nds", "TWL1")]
        [InlineData(typeof(GameboyFileSignature), "flappyboy.gb", null)]
        [InlineData(typeof(GameboyColorFileSignature), "infinity.gbc", null)]
        [InlineData(typeof(Nintendo64ByteswappedFileSignature), "rx-mm64.v64", "NMME")]
        [InlineData(typeof(Nintendo64LittleEndianFileSignature), "rx-mm64.n64", "NMME")]
        [InlineData(typeof(Nintendo64BigEndianFileSignature), "setscreenntsc.z64", "")]


        public void Verify_Serial(Type fileSignature, string filename, string expected)
        {
            using var testStream = TestUtilities.GetResource($"TestRoms.{filename}");

            IFileSignature signature = (IFileSignature)Activator.CreateInstance(fileSignature);
            Assert.Equal(expected, signature.GetSerial(testStream));
        }


        [Theory]
        [InlineData(typeof(NintendoEntertainmentSystemiNesFileSignature), "test.nes", null)]
        [InlineData(typeof(NintendoEntertainmentSystemUnifFileSignature), "scanline.unif", null)]
        [InlineData(typeof(SuperNintendoHeaderlessFileSignature), "bsnesdemo_v1.sfc", "bsnes test demo")]
        [InlineData(typeof(SuperNintendoSmcHeaderFileSignature), "bsnesdemo_v1.smc", "bsnes test demo")]
        [InlineData(typeof(GameboyAdvancedFileSignature), "suite.gba", "TESTSUITE")]
        [InlineData(typeof(NintendoDSFileSignature), "slot1launch.nds", "TWLMENUPP-S1")]
        [InlineData(typeof(GameboyFileSignature), "flappyboy.gb", "FLAPPYBOY")]
        [InlineData(typeof(GameboyColorFileSignature), "infinity.gbc", "INFINITY")]
        [InlineData(typeof(Nintendo64ByteswappedFileSignature), "rx-mm64.v64", "Manic Miner 64")]
        [InlineData(typeof(Nintendo64LittleEndianFileSignature), "rx-mm64.n64", "Manic Miner 64")]

        [InlineData(typeof(Nintendo64BigEndianFileSignature), "setscreenntsc.z64", "N64 PROGRAM TITLE")]
        public void Verify_InternalName(Type fileSignature, string filename, string expected)
        {
            using var testStream = TestUtilities.GetResource($"TestRoms.{filename}");
            IFileSignature signature = (IFileSignature)Activator.CreateInstance(fileSignature);
            Assert.Equal(expected, signature.GetInternalName(testStream));
        }
    }
}
