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
        private readonly BubblesSettings settings;
        private static Size minimumSize = new Size(400, 400);

        public MainWindow(BubblesSettings settings)
        {
            InitializeComponent();
            this.settings = settings;
            MainGrid.Background = new SolidColorBrush(Color.FromArgb((byte)(settings.BackgroundAlpha * byte.MaxValue), 0, 0, 0));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Size bounds;
            if (double.IsNaN(Width))
            {
                bounds = minimumSize;
                MainGrid.Width = minimumSize.Width;
                MainGrid.Height = minimumSize.Height;
            }
            else
            {
                bounds = new Size(Width, Height);
            }

            worker = new Worker(MainVisualArea, bounds, settings);
            CreateElements(bounds);
            worker.Start();
        }

        private void CreateElements(Size bounds)
        {
            Random rand = new Random(Environment.TickCount);

            for (var i = 0; i < settings.Count; i++)
                CreateRandomSphere(bounds, rand);
        }

        private void CreateRandomSphere(Size bounds, Random rand)
        {
            var color = Color.FromRgb((byte)rand.Next(0, byte.MaxValue + 1), (byte)rand.Next(0, byte.MaxValue + 1), (byte)rand.Next(0, byte.MaxValue + 1));
            var position = new Vector(rand.Next(0, (int) (bounds.Width - settings.RadiusMax)),
                rand.Next(0, (int) (bounds.Height - settings.RadiusMax)));
            var radius = rand.Next(settings.RadiusMin, settings.RadiusMax);

            worker.AddElement(new UpdatableSphere(bounds,
                CreateElement(color, radius, position), 
                position,
                new Vector(rand.NextDouble() - 0.5, rand.NextDouble() - 0.5), 
                rand.Next(settings.SpeedMin * 100, settings.SpeedMax * 100) / 100.0,
                radius)
                );
        }

        private DrawingGroup CreateElement(Color color, int radius, Vector position)
        {
            var transparent = color;
            transparent.A = 1;

            EllipseGeometry geom = new EllipseGeometry(new Point(radius, radius), radius, radius);
            geom.Freeze();
            var d = new DrawingGroup();
            d.Children.Add(new GeometryDrawing(new RadialGradientBrush(color, transparent), new Pen(new SolidColorBrush(color), 1), geom));

            TransformGroup tg = new TransformGroup();
            tg.Children.Add(new TranslateTransform(position.X, position.Y));
            d.Transform = tg;

            MainVisualArea.Add(d);
            return d;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            worker.Stop();

            base.OnClosing(e);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
