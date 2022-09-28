using StreamDeckLib;
using StreamDeckLib.Messages;
using StreamDeckTradfri.Tradfri;
using System.Linq;
using System.Threading.Tasks;
using Tomidix.NetStandard.Tradfri.Models;

namespace StreamDeckTradfri.Actions
{
    [ActionUuid(Uuid = "no.heinandre.tradfri.action.set-light")]
    public class SetLightAction : BaseStreamDeckActionWithSettingsModel<Models.SetLightActionSettings>
    {
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
                    SetLight(device, SettingsModel.Dimmer);
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
