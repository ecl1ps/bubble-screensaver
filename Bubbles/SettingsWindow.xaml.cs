using System.Windows;

namespace Bubbles
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private BubblesSettings settings;

        public SettingsWindow()
        {
            InitializeComponent();

            settings = BubblesSettings.Load(BubblesSettings.SettingsFile);

            ObjectCountSlider.Value = settings.Count;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            settings.Count = (int)ObjectCountSlider.Value;

            settings.Save(BubblesSettings.SettingsFile);
            Application.Current.Shutdown();
        }
    }
}
