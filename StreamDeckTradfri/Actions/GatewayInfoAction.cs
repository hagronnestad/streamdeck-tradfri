using StreamDeckLib;
using StreamDeckLib.Messages;
using StreamDeckTradfri.Settings;
using StreamDeckTradfri.Tradfri;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace StreamDeckTradfri.Actions
{
    [ActionUuid(Uuid = "no.heinandre.tradfri.action.gateway-info")]
    public class GatewayInfoAction : BaseStreamDeckAction
    {
        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            switch (Program.SetupState)
            {
                case SetupState.Unknown:
                    break;
                case SetupState.SettingsIncomplete:
                    if (Program.CheckSettings())
                    {
                        await Manager.SetTitleAsync(args.context, $"Checking\n...");
                        await Program.CheckSettingsAndInitTradfriLib();
                    }
                    else
                    {
                        await Manager.SetTitleAsync(args.context, $"Fix\nSettings");
                        OpenSettingsDirectory();
                    }
                    break;
                case SetupState.SettingsComplete:
                    break;
                case SetupState.TradfriGetGatewayAddress:
                    break;
                case SetupState.TradfriAppNameInUse:
                    if (Program.CheckSettings())
                    {
                        await Manager.SetTitleAsync(args.context, $"Checking\n...");
                        await Program.CheckSettingsAndInitTradfriLib();
                    }
                    else
                    {
                        await Manager.SetTitleAsync(args.context, $"Fix\nSettings");
                        OpenSettingsDirectory();
                    }
                    break;
                case SetupState.TradfriCreatingAppKey:
                    break;
                case SetupState.TradfriConnected:
                    await Manager.SetTitleAsync(args.context, $"Reset\n...");
                    Program.CheckSettings();
                    await TradfriHelper.InitTradfriLib();
                    break;
                default:
                    break;
            }
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);

            await UpdateTitleAsync(args);
        }

        private async Task UpdateTitleAsync(StreamDeckEventPayload args)
        {
            switch (Program.SetupState)
            {
                case SetupState.Unknown:
                    await Manager.SetTitleAsync(args.context, $"State:\n{Program.SetupState}");
                    break;
                case SetupState.SettingsIncomplete:
                    await Manager.SetTitleAsync(args.context, $"Open\nSettings");
                    break;
                case SetupState.SettingsComplete:
                    await Manager.SetTitleAsync(args.context, $"Settings\nOK");
                    break;
                case SetupState.TradfriGetGatewayAddress:
                    await Manager.SetTitleAsync(args.context, $"Get\nIP\n...");
                    break;
                case SetupState.TradfriAppNameInUse:
                    await Manager.SetTitleAsync(args.context, $"App\nName\nUsed");
                    break;
                case SetupState.TradfriCreatingAppKey:
                    await Manager.SetTitleAsync(args.context, $"Create\nKey\n...");
                    break;
                case SetupState.TradfriConnected:
                    await Manager.SetTitleAsync(args.context, $"{TradfriHelper.GatewayAddressTitle}\n{TradfriHelper.GatewayVersion}");
                    break;
                default:
                    await Manager.SetTitleAsync(args.context, $"State:\n{Program.SetupState}");
                    break;
            }
        }

        private void OpenSettingsDirectory()
        {
            var p = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pi = new ProcessStartInfo()
            {
                FileName = Path.Combine(p, PluginSettings.DIRECTORY),
                UseShellExecute = true,
            };
            Process.Start(pi);
        }
    }
}
