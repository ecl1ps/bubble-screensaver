using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Bubbles.Elements;
using System.Runtime.InteropServices;

namespace Bubbles
{
    public partial class MainWindow : Window
    {
        private Worker worker;
        private readonly BubblesSettings settings;
        private static Size minimumSize = new Size(400, 400);
        public const uint ES_CONTINUOUS = 0x80000000;
        public const uint ES_SYSTEM_REQUIRED = 0x00000001;
        public const uint ES_DISPLAY_REQUIRED = 0x00000002;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SetThreadExecutionState([In] uint esFlags);


        public MainWindow(BubblesSettings settings)
        {
            InitializeComponent();
            this.settings = settings;
            MainGrid.Background = new SolidColorBrush(Color.FromArgb((byte)(settings.BackgroundAlpha * byte.MaxValue), 0, 0, 0));
            SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED);
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
            var color = Color.FromArgb(settings.CenterTpcy, (byte)rand.Next(0, byte.MaxValue + 1), (byte)rand.Next(0, byte.MaxValue + 1), (byte)rand.Next(0, byte.MaxValue + 1));
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
            Color endColor = Color.FromArgb(settings.SphereTpcy, Convert.ToByte(color.R * settings.EdgeRatio), 
                Convert.ToByte(color.G * settings.EdgeRatio), Convert.ToByte(color.B * settings.EdgeRatio));
            Color borderColor = color;
            if (settings.WeBeSphere) borderColor = endColor;
 
            d.Children.Add(new GeometryDrawing(new RadialGradientBrush(color, endColor), 
                new Pen(new SolidColorBrush(borderColor), 1), geom));

            TransformGroup tg = new TransformGroup();
            tg.Children.Add(new TranslateTransform(position.X, position.Y));
            d.Transform = tg;

            MainVisualArea.Add(d);
            return d;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            worker.Stop();
            SetThreadExecutionState(ES_CONTINUOUS);

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
