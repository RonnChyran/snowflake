﻿using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Snowflake.Execution.Extensibility;
using Snowflake.Model.Records.Game;
using Xunit;

namespace Snowflake.Emulator.Execution.Tests
{
    public class EmulatorTaskTests
    {
        [Fact]
        public void EmulatorTaskPragma_Test()
        {
            var gameRecord = new Mock<IGameRecord>();
            var emulatorTask = new EmulatorTask(gameRecord.Object);
            emulatorTask.AddPragma("testpragma", "test");
            Assert.Equal("test", (emulatorTask as IEmulatorTask)?.Pragmas["testpragma"]);
        }
    }
}
