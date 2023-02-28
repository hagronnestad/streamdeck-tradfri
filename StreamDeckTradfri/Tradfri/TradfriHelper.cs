using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tomidix.NetStandard.Tradfri;
using Tomidix.NetStandard.Tradfri.Controllers;
using Tomidix.NetStandard.Tradfri.Models;
using Zeroconf;

namespace StreamDeckTradfri.Tradfri
{
    public static class TradfriHelper
    {
        public const string APP_NAME = "SD";

        public static TradfriController Controller;
        public static GatewayController GatewayController;
        public static DeviceController DeviceController;
        public static List<TradfriDevice> Devices;

        public static string GatewayAddress { get; set; } = "";
        public static string GatewayVersion { get; set; } = "";

        public static string GatewayAddressTitle
        {
            get
            {
                var parts = GatewayAddress.Split(".");
                if (parts.Length < 4) return "";

                return $"{parts[0]}.{parts[1]}\n{parts[2]}.{parts[3]}";
            }
        }

        public static string Status { get; set; } = null;
        public static bool IsConnected { get; set; } = false;

        public static async Task InitTradfriLib()
        {
            Program.SetupState = SetupState.TradfriGetGatewayAddress;
            Status = "Finding\nGateway\n...";

            GatewayAddress = await GetGatewayAddressAsync();

            Controller = new TradfriController("", GatewayAddress);

            // Create app key if not present
            if (Program.Settings.AppKey == null || string.IsNullOrWhiteSpace(Program.Settings.AppKey))
            {
                try
                {
                    Status = "Creating\nApp\nKey";
                    Program.SetupState = SetupState.TradfriCreatingAppKey;

                    var appSecret = Controller.GenerateAppSecret(Program.Settings.GatewaySecret, Program.Settings.AppName);
                    Program.Settings.AppKey = appSecret.PSK;
                    Program.Settings.Save();

                    Status = GatewayAddressTitle;
                }
                catch (Exception e)
                {
                    Status = "App\nName\nTaken";
                    Program.SetupState = SetupState.TradfriAppNameInUse;
                    return;
                }
            }

            Controller.ConnectAppKey(Program.Settings.AppKey, Program.Settings.AppName);

            var gwInfo = await Controller.GatewayController.GetGatewayInfo();
            GatewayVersion = gwInfo.Firmware;

            GatewayController = Controller.GatewayController;
            DeviceController = Controller.DeviceController;
            Devices = await GatewayController.GetDeviceObjects();

            Program.SetupState = SetupState.TradfriConnected;
        }

        public static async Task<string> GetGatewayAddressAsync()
        {
            var results = await ZeroconfResolver.ResolveAsync("_coap._udp.local.",
                TimeSpan.FromMilliseconds(5000));
            
            if (!results.Any()) return null;

            var res = results.FirstOrDefault(x => x.DisplayName.StartsWith("gw-"));
            if (res == null || string.IsNullOrWhiteSpace(res.IPAddress)) return null;

            return res.IPAddress;
        }
    }

	public static class TradfriColors
	{
		public const string Blue = "4a418a";
		public const string LightBlue = "6c83ba";
		public const string SaturatedPurple = "8f2686";
		public const string Lime = "a9d62b";
		public const string LightPurple = "c984bb";
		public const string Yellow = "d6e44b";
		public const string SaturatedPink = "d9337c";
		public const string DarkPeach = "da5d41";
		public const string SaturatedRed = "dc4b31";
		public const string ColdSky = "dcf0f8";
		public const string Pink = "e491af";
		public const string Peach = "e57345";
		public const string WarmAmber = "e78834";
		public const string LightPink = "e8bedd";
		public const string CoolDaylight = "eaf6fb";
		public const string CandleLight = "ebb63e";
		public const string WarmGlow = "efd275";
		public const string WarmWhite = "f1e0b5";
		public const string Sunrise = "f2eccf";
		public const string CoolWhite = "f5faf6";
	}
}
