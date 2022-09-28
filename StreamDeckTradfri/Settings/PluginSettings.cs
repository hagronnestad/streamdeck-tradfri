using StreamDeckTradfri.Tradfri;
using System.IO;
using System.Text.Json;

namespace StreamDeckTradfri.Settings
{
    public class PluginSettings
    {
        public const string DIRECTORY = "Settings";
        public const string FILE = DIRECTORY + "/settings.json";

        /// <summary>
        /// This is the key that is printed on the sticker on the under side of your Trådfri gateway.
        /// </summary>
        public string GatewaySecret { get; set; } = "";

        /// <summary>
        /// This is a unique name associated with the app key.
        /// Choose any name, but it's probably good to keep it short.
        /// Once an app key has been created for an app name,
        /// the same app name can not be reused.
        /// </summary>
        public string AppName { get; set; } = TradfriHelper.APP_NAME;

        /// <summary>
        /// This is the key that is returned from your Trådfri gateway
        /// and is associated with the chosen app name.
        /// </summary>
        public string AppKey { get; set; } = "";


        public static void CreateSettings()
        {
            if (File.Exists(FILE)) return;
            var s = new PluginSettings();
            s.Save();
        }

        public static PluginSettings Load()
        {
            if (!File.Exists(FILE)) CreateSettings();

            var t = File.ReadAllText(FILE);
            return JsonSerializer.Deserialize<PluginSettings>(t);
        }

        public void Save()
        {
            var t = JsonSerializer.Serialize(this);
            Directory.CreateDirectory(DIRECTORY);
            File.WriteAllText(FILE, t);
        }
    }
}
