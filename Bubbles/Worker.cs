using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace Bubbles
{
    public class Worker
    {
        private readonly ConcurrentQueue<Action> synchronizationQueue = new ConcurrentQueue<Action>(); 
        private readonly List<IUpdatable> elements = new List<IUpdatable>();
        private bool isRunning = true;
        private Size bounds;
        private long minimumUpdateTime = 17;

        public Worker(Size bounds, BubblesSettings settings)
        {
            this.bounds = bounds;

            // 10 - 60 fps -> update every 100 - 17 ms
            minimumUpdateTime = (long)FastMath.LinearInterpolate(17, 100, settings.PowerSavings);
        }

        public void AddElement(IUpdatable el)
        {
            elements.Add(el);
        }

        public void Start()
        {
            var t = new Thread(Run);
            t.Start();
        }

        public void Stop()
        {
            isRunning = false;
        }

        private void Run()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            float tpf;
            long elapsedMs;

            while (isRunning)
            {
                tpf = sw.ElapsedMilliseconds / 1000.0f;

                sw.Restart();

                while (!synchronizationQueue.IsEmpty)
                {
                    Action a;
                    synchronizationQueue.TryDequeue(out a);
                    a();
                }

                if (Application.Current == null)
                    continue;

                Application.Current.Dispatcher.InvokeAsync(() => elements.ForEach(e => e.Update(bounds, tpf)));

                elapsedMs = sw.ElapsedMilliseconds;
                if (elapsedMs < minimumUpdateTime)
                    Thread.Sleep((int)(minimumUpdateTime - elapsedMs));
            }

            sw.Stop();
        }

        public void SetNewBounds(Size size)
        {
            synchronizationQueue.Enqueue(() => bounds = size);
        }
    }
}
