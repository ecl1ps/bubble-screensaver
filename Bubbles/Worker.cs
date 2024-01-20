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
        private readonly List<IUpdatable> objects = new List<IUpdatable>();
        private bool isRunning = true;
        private VisualArea area;
        private Size bounds;
        private readonly long minimumUpdateTime = 17;

        public Worker(VisualArea area, Size bounds, BubblesSettings settings)
        {
            this.area = area;
            this.bounds = bounds;

            // 10 - 60 fps -> update every 100 - 17 ms
            minimumUpdateTime = (long)FastMath.LinearInterpolate(17, 100, settings.PowerSavings);
        }

        public void AddElement(IUpdatable el)
        {
            objects.Add(el);
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

                if (Application.Current == null)
                    continue;

                UpdateSceneObjects(tpf);
                UpdateGeomtricState();

                area.RunRender();

                elapsedMs = sw.ElapsedMilliseconds;
                Console.WriteLine(elapsedMs + " : " + tpf);
                if (elapsedMs < minimumUpdateTime)
                    Thread.Sleep((int)(minimumUpdateTime - elapsedMs));
            }

            sw.Stop();
        }

        private void UpdateGeomtricState()
        {
            area.Dispatcher.Invoke(() => objects.ForEach(obj => obj.UpdateGeometric()));
        }

        public void UpdateSceneObjects(float tpf)
        {
            objects.ForEach(obj => obj.Update(bounds, tpf));
        }

        /*public void SetNewBounds(Size size)
        {
            synchronizationQueue.Enqueue(() => bounds = size);
        }*/
    }
}
