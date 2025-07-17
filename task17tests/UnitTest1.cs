using System;
using System.Threading;
using Xunit;
using task17;

namespace task17tests
{
    public class ServerThreadTests
    {
        [Fact]
        public void SoftStop_Stops_Thread_After_Commands_Completed()
        {
            var thread = new ServerThread();
            var completed = new ManualResetEvent(false);

            thread.EnqueueCommand(new TestCommand((token) =>
            {
                Thread.Sleep(100);
                completed.Set();
            }));

            thread.EnqueueCommand(new SoftStopCommand());

            Assert.True(completed.WaitOne(1000));
            thread.Join(1000);
            Assert.False(thread.IsAlive);
        }

        [Fact]
        public void HardStop_Stops_Thread_Immediately()
        {
            var thread = new ServerThread();
            var stopReceived = new ManualResetEvent(false);

            thread.EnqueueCommand(new TestCommand((token) =>
            {
                stopReceived.Set();
                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(50);
                }
            }));

            thread.EnqueueCommand(new HardStopCommand());

            Assert.True(stopReceived.WaitOne(1000));
            thread.ForceStop();
            thread.Join(1000);
            Assert.False(thread.IsAlive);
        }

        [Fact]
        public void HardStop_From_Another_Thread_Throws_Exception()
        {
            var thread = new ServerThread();
            var exceptionThrown = false;
            var hardStop = new HardStopCommand();

            try
            {
                hardStop.Execute(CancellationToken.None);
            }
            catch (InvalidOperationException ex)
            {
                Assert.Contains("target thread", ex.Message);
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public void Command_Exception_Is_Handled()
        {
            var thread = new ServerThread();
            thread.EnqueueCommand(new FailingCommand((token) => throw new InvalidOperationException("Test failure")));
            thread.Stop();
            thread.Join(500);
        }

        private class TestCommand : ICommand
        {
            private readonly Action<CancellationToken> _action;
            public TestCommand(Action<CancellationToken> action) => _action = action;
            public void Execute(CancellationToken token) => _action(token);
        }

        private class FailingCommand : ICommand
        {
            private readonly Action<CancellationToken> _action;
            public FailingCommand(Action<CancellationToken> action) => _action = action;
            public void Execute(CancellationToken token)
            {
                _action(token);
            }
        }
    }
}
