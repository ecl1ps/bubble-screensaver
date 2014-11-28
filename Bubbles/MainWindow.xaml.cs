using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Bubbles.Elements;

namespace Bubbles
{
    public partial class MainWindow : Window
    {
        private Worker worker;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Size bounds = new Size(Width, Height);
            worker = new Worker(bounds);
            CreateElements(bounds);
            worker.Start();
        }

        private void CreateElements(Size bounds)
        {
            Random rand = new Random(Environment.TickCount);

            for (var i = 0; i < 50; i++)
                CreateRandomSphere(bounds, rand);
        }

        private void CreateRandomSphere(Size bounds, Random rand)
        {
            var color = Color.FromRgb((byte)rand.Next(0, byte.MaxValue + 1), (byte)rand.Next(0, byte.MaxValue + 1), (byte)rand.Next(0, byte.MaxValue + 1));

            worker.AddElement(new MovableSphere(bounds, 
                CreateElement(color, rand.Next(40, 60)), 
                new Vector(rand.Next(0, (int)(bounds.Width - 50)), rand.Next(0, (int)(bounds.Height - 50))),
                new Vector(rand.NextDouble() - 0.5, rand.NextDouble() - 0.5), rand.Next(100, 250) / 100.0));
        }

        private Ellipse CreateElement(Color color, int radius)
        {
            var transparent = color;
            transparent.A = 1;

            var el = new Ellipse
            {
                Stroke = new SolidColorBrush(color), 
                Fill = new RadialGradientBrush(color, transparent), 
                StrokeThickness = 1, 
                Height = radius, 
                Width =  radius
            };

            MainCanvas.Children.Add(el);
            return el;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            worker.Stop();

            base.OnClosing(e);
        }

        private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //worker.SetNewBounds(e.NewSize);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
