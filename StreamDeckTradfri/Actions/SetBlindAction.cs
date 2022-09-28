using StreamDeckLib;
using StreamDeckLib.Messages;
using StreamDeckTradfri.Tradfri;
using System.Linq;
using System.Threading.Tasks;

namespace StreamDeckTradfri.Actions
{
    [ActionUuid(Uuid = "no.heinandre.tradfri.action.set-blind")]
    public class SetBlindAction : BaseStreamDeckActionWithSettingsModel<Models.SetBlindActionSettings>
    {
        public override Task OnApplicationDidLaunchAsync(StreamDeckEventPayload args)
        {
            return base.OnApplicationDidLaunchAsync(args);
        }

        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            if (string.IsNullOrWhiteSpace(SettingsModel.Blind))
            {
                await Manager.ShowAlertAsync(args.context);
                return;
            }

            var names = SettingsModel.Blind.Split(";");
            var devices = TradfriHelper.Devices.Where(x => names.Contains(x.Name));

            if (devices != null && devices.Any())
            {
                foreach (var device in devices)
                {
                    await TradfriHelper.DeviceController.SetBlind(device, SettingsModel.Position);
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
    }
}
