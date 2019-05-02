﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Snowflake.Installation
{
    public class TaskCachingTests
    {
        [Fact]
        public async Task TaskCaching_Test()
        {
            var task = new TrivialCounterTask();
            await foreach(var i in  EmitCounter(task));
            Assert.Equal(1, task.Counter);
        }

        [Fact]
        public async Task TaskEnumerableCaching_Test()
        {
            var task = new TrivialCounterEnumerableTask();
            await foreach (var i in EmitCounter(task)) ;
            Assert.Equal(2, task.Counter);
        }

        public async IAsyncEnumerable<ITaskResult> EmitCounter(TrivialCounterTask t)
        {            
            yield return await t;
            yield return await t;
            yield return await new TrivialCounterSubTask(await t);
        }

        public async IAsyncEnumerable<ITaskResult> EmitCounter(TrivialCounterEnumerableTask t)
        {
            await foreach (var i in t)
            {
                yield return i;
                yield return i;
                yield return i;
                yield return await new TrivialCounterSubTask(i);

            }
        }

    }

    public sealed class TrivialCounterTask : InstallTaskAwaitable<int>
    {
        public TrivialCounterTask()
        {
            this.Counter = 0;
        }

        public int Counter { get; set; }

        protected override string TaskName => "Test";

        protected override Task<int> ExecuteOnce()
        {
            return Task.Run(() => this.Counter++);
        }
    }

    public sealed class TrivialCounterEnumerableTask : InstallTaskAwaitableEnumerable<int>
    {
        public TrivialCounterEnumerableTask()
        {
            this.Counter = 0;
        }

        public int Counter { get; set; }

        protected override string TaskName => "Test";

        protected override async IAsyncEnumerable<int> ExecuteOnce()
        {
            yield return await Task.Run(() => this.Counter++);
            yield return await Task.Run(() => this.Counter++);

        }
    }

    public sealed class TrivialCounterSubTask : InstallTaskAwaitable<int>
    {
        public TrivialCounterSubTask(TaskResult<int> t)
        {
            this.Counter = 0;
            T = t;
        }

        public int Counter { get; set; }
        public TaskResult<int> T { get; }

        protected override string TaskName => "Test";

        protected async override Task<int> ExecuteOnce()
        {
            this.Counter = await T;
            return this.Counter;
        }
    }
}
