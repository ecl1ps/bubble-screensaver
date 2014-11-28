using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace Bubbles
{
    public class Worker
    {
        private readonly ConcurrentQueue<Action> synchronizationQueue = new ConcurrentQueue<Action>(); 
        private readonly List<IMovable> elements = new List<IMovable>();
        private bool isRunning = true;
        private Size bounds;

        public Worker(Size bounds)
        {
            this.bounds = bounds;
        }

        public void AddElement(IMovable el)
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
            while (isRunning)
            {
                while (!synchronizationQueue.IsEmpty)
                {
                    Action a;
                    synchronizationQueue.TryDequeue(out a);
                    a();
                }
                    
                foreach (IMovable e in elements)
                    e.Update(bounds);

                Thread.Sleep(10);
            }
        }

        public void SetNewBounds(Size size)
        {
            synchronizationQueue.Enqueue(() => bounds = size);
        }
    }
}
