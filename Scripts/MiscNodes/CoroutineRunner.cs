using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace ShiftingSepulcher
{
    public class CoroutineRunner : Node
    {
        public delegate Task Coroutine(CancellationToken cancel);

        private CancellationTokenSource _cancelCoroutine = new CancellationTokenSource();

        /// <summary>
        /// Cancels the current coroutine, if one is running, and then starts
        /// a new one
        /// </summary>
        /// <param name="func"></param>
        public void StartCoroutine(Coroutine func)
        {
            StopCoroutine();
            RunCoroutine(func, _cancelCoroutine.Token);
        }

        /// <summary>
        /// Stops the current coroutine
        /// </summary>
        public void StopCoroutine()
        {
            _cancelCoroutine.Cancel();
            _cancelCoroutine = new CancellationTokenSource();
        }

        public async Task ForSignal(Godot.Object source, string signal, CancellationToken cancel)
        {
            await ToSignal(source, signal);
            cancel.ThrowIfCancellationRequested();
        }

        public async Task ForPhysicsSeconds(float duration, CancellationToken cancel)
        {
            await EnsureInTree();
            var timer = GetTree().CreatePhysicsTimer(duration);
            await ForSignal(timer, nameof(PhysicsTimer.TimerExpired), cancel);
        }

        private async void RunCoroutine(Coroutine func, CancellationToken cancel)
        {
            // HACK: Defer starting the coroutine until the next frame.
            // We need to do it this way because CallDeferred() doesn't work
            // with Func<> or CancellationToken types
            await EnsureInTree();
            await ToSignal(GetTree().CreateTimer(0), "timeout");

            if (cancel.IsCancellationRequested)
            {
                GD.Print("Coroutine cancelled before it began.  Stopping.");
                return;
            }

            GD.Print("Starting coroutine");

            try
            {
                await func(cancel);
            }
            catch (OperationCanceledException)
            {
                GD.Print("Coroutine cancelled during an await.  Stopping.");
                return;
            }
        }

        private async Task EnsureInTree()
        {
            if (!IsInsideTree())
                await ToSignal(this, "tree_entered");
        }
    }
}
