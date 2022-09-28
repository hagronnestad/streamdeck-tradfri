using StreamDeckLib;
using StreamDeckTradfri.Settings;
using StreamDeckTradfri.Tradfri;
using System.Threading.Tasks;

namespace StreamDeckTradfri
{
    class Program
    {
        public static SetupState SetupState = SetupState.Unknown;

        public static PluginSettings Settings { get; set; }

        static async Task Main(string[] args)
        {
            await CheckSettingsAndInitTradfriLib();

            using (var config = StreamDeckLib.Config.ConfigurationBuilder.BuildDefaultConfiguration(args))
            {
                await ConnectionManager
                    .Initialize(args, config.LoggerFactory)
                    .RegisterAllActions(typeof(Program).Assembly)
                    .StartAsync();
            }
        }

        public static bool CheckSettings()
        {
            Settings = PluginSettings.Load();

            // Check settings
            if (Settings.GatewaySecret == null || string.IsNullOrWhiteSpace(Settings.GatewaySecret))
            {
                SetupState = SetupState.SettingsIncomplete;
                return false;
            }

            if (Settings.AppName == null || string.IsNullOrWhiteSpace(Settings.AppName))
            {
                SetupState = SetupState.SettingsIncomplete;
                return false;
            }

            return true;
        }

        public static async Task CheckSettingsAndInitTradfriLib()
        {
            if (!CheckSettings()) return;

            // Settings complete
            SetupState = SetupState.SettingsComplete;
            await TradfriHelper.InitTradfriLib();
        }
    }

    public enum SetupState
    {
        Unknown,
        SettingsIncomplete,
        SettingsComplete,
        TradfriGetGatewayAddress,
        TradfriCreatingAppKey,
        TradfriAppNameInUse,
        TradfriConnected
    }
}
