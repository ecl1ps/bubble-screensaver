using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Bubbles
{
    public class BubblesSettings
    {
        public const string SettingsFile = "BouncingBubbles.xml";
        public float PowerSavings { get; set; }
        public float BackgroundAlpha { get; set; }
        public int RadiusMin { get; set; }
        public int RadiusMax { get; set; }
        public int SpeedMin { get; set; }
        public int SpeedMax { get; set; }
        public int Count { get; set; }
        public byte SphereTpcy { get; set; } 
        public bool WeBeSphere { get; set; }    
        public byte CenterTpcy { get; set; }

 
        /// <summary>
        /// Instantiate the class, loading settings from a specified file.
        /// If the file doesn't exist, use default values.
        /// </summary>
        public BubblesSettings()
        {
            SetDefaults();
        }
 
        /// <summary>
        /// Set all values to their defaults
        /// </summary>
        public void SetDefaults()
        {
            PowerSavings = 0.3f;
            BackgroundAlpha = 0.2f;
            RadiusMin = 60;
            RadiusMax = 80;
            SpeedMin = 100;
            SpeedMax = 200;
            Count = 20;
            SphereTpcy = 170;
            WeBeSphere = false;
            CenterTpcy = 190;
        }
 
        /// <summary>
        /// Save current settings to external file
        /// </summary>
        public void Save(string sSettingsFilename)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(BubblesSettings));

                using (FileStream fs = new FileStream(sSettingsFilename, FileMode.Create))
                    using (TextWriter writer = new StreamWriter(fs, new UTF8Encoding()))
                        serializer.Serialize(writer, this);
            }
            catch { }
        }
 
        /// <summary>
        /// Attempt to load settings from external file.  If the file doesn't
        /// exist, or if there is a problem, no settings are changed.
        /// </summary>
        public static BubblesSettings Load(string sSettingsFilename)
        {
            BubblesSettings settings = null;
 
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof (BubblesSettings));
                using (FileStream fs = new FileStream(sSettingsFilename, FileMode.OpenOrCreate))
                    using (TextReader reader = new StreamReader(fs))
                        settings = (BubblesSettings)serializer.Deserialize(reader);
            }
            catch {
                // If we can't load, just create a new object, which gets default values
                settings = new BubblesSettings();     
            }
 
            return settings;
        }
    }
}
