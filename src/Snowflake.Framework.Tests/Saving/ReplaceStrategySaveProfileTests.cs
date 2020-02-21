﻿using Snowflake.Filesystem;
using Snowflake.Orchestration.Saving.SaveProfiles;
using Snowflake.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Snowflake.Orchetsration.Saving.Tests
{
    public class ReplaceStrategySaveProfileTests
    {
        [Fact]
        public async Task Create_Test()
        {
            var profileRoot = TestUtilities.GetTemporaryDirectory();
            var saveContents = TestUtilities.GetTemporaryDirectory();

            saveContents.OpenFile("savecontent").WriteAllText("test content");

            var profileGuid = Guid.NewGuid();
            var profile = new ReplaceStrategySaveProfile(profileGuid, "Test", "testsave", profileRoot);
            var save = await profile.CreateSave(saveContents);

            var retrievedSave = profile.GetHeadSave();

            Assert.Equal(save.CreatedTimestamp, retrievedSave.CreatedTimestamp);
        }

        [Fact]
        public async Task History_Test()
        {
            var profileRoot = TestUtilities.GetTemporaryDirectory();
            var saveContents = TestUtilities.GetTemporaryDirectory();

            saveContents.OpenFile("savecontent").WriteAllText("test content");

            var profileGuid = Guid.NewGuid();
            var profile = new ReplaceStrategySaveProfile(profileGuid, "Test", "testsave", profileRoot);
            var save = await profile.CreateSave(saveContents);
            var save2 = await profile.CreateSave(save);

            var retrievedSave = profile.GetHeadSave();

            Assert.Equal(save2.CreatedTimestamp, retrievedSave.CreatedTimestamp);
            Assert.Single(profile.GetHistory());
            Assert.Single(profileRoot.EnumerateDirectories());
        }
    }
}
