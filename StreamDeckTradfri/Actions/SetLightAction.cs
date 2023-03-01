using StreamDeckLib;
using StreamDeckLib.Messages;
using StreamDeckTradfri.Tradfri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tomidix.NetStandard.Tradfri.Models;

namespace StreamDeckTradfri.Actions
{
    [ActionUuid(Uuid = "no.heinandre.tradfri.action.set-light")]
    public class SetLightAction : BaseStreamDeckActionWithSettingsModel<Models.SetLightActionSettings>
    {
        public Dictionary<string, string> TradfriColors = new()
        {
            { "None", "" },
            { "Blue", "4a418a" },
            { "LightBlue", "6c83ba" },
            { "SaturatedPurple", "8f2686" },
            { "Lime", "a9d62b" },
            { "LightPurple", "c984bb" },
            { "Yellow", "d6e44b" },
            { "SaturatedPink", "d9337c" },
            { "DarkPeach", "da5d41" },
            { "SaturatedRed", "dc4b31" },
            { "ColdSky", "dcf0f8" },
            { "Pink", "e491af" },
            { "Peach", "e57345" },
            { "WarmAmber", "e78834" },
            { "LightPink", "e8bedd" },
            { "CoolDaylight", "eaf6fb" },
            { "CandleLight", "ebb63e" },
            { "WarmGlow", "efd275" },
            { "WarmWhite", "f1e0b5" },
            { "Sunrise", "f2eccf" },
            { "CoolWhite", "f5faf6 "}
        };

        public override Task OnApplicationDidLaunchAsync(StreamDeckEventPayload args)
        {
            return base.OnApplicationDidLaunchAsync(args);
        }

        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            if (string.IsNullOrWhiteSpace(SettingsModel.Light))
            {
                await Manager.ShowAlertAsync(args.context);
                return;
            }

            var names = SettingsModel.Light.Split(";").Select(x => x.Trim());
            var devices = TradfriHelper.Devices.Where(x => names.Contains(x.Name));

            if (devices != null && devices.Any())
            {
                foreach (var device in devices)
                {
                    SetLight(device, SettingsModel.Dimmer, TradfriColors[SettingsModel.Color]);
                }
            }
        }

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
        }

        private void SetLight(TradfriDevice device, int dimmer = -1, string color = "")
        {
            TradfriHelper.DeviceController.SetLight(device, dimmer > 0);

            if (dimmer > -1)
            {
                var value = 254 / 100.0f * dimmer;
                TradfriHelper.DeviceController.SetDimmer(device, (int)value);
            }

            if (!string.IsNullOrWhiteSpace(color))
            {
                TradfriHelper.DeviceController.SetColor(device, color);
            }
        }
    }
}
