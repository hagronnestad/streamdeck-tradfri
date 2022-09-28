// global websocket, used to communicate from/to Stream Deck software
// as well as some info about our plugin, as sent by Stream Deck software 
var websocket = null,
    uuid = null,
    inInfo = null,
    actionInfo = {},
    settingsModel = {
        Blind: "",
        Position: 0
    };

function connectElgatoStreamDeckSocket(inPort, inUUID, inRegisterEvent, inInfo, inActionInfo) {
    uuid = inUUID;
    actionInfo = JSON.parse(inActionInfo);
    inInfo = JSON.parse(inInfo);
    websocket = new WebSocket('ws://localhost:' + inPort);

    //initialize values
    if (actionInfo.payload.settings.settingsModel) {
        settingsModel.Blind = actionInfo.payload.settings.settingsModel.Blind;
        settingsModel.Position = actionInfo.payload.settings.settingsModel.Position;
    }

    document.getElementById('txtBlind').value = settingsModel.Blind;
    document.getElementById('txtPosition').value = settingsModel.Position;

    websocket.onopen = function () {
        var json = { event: inRegisterEvent, uuid: inUUID };

        // register property inspector to Stream Deck
        websocket.send(JSON.stringify(json));
    };

    websocket.onmessage = function (evt) {
        // Received message from Stream Deck
        var jsonObj = JSON.parse(evt.data);
        var sdEvent = jsonObj['event'];
        switch (sdEvent) {
            case "didReceiveSettings":
                if (jsonObj.payload.settings.settingsModel.Counter) {

                    settingsModel.Blind = jsonObj.payload.settings.settingsModel.Blind;
                    document.getElementById('txtBlind').value = settingsModel.Blind;

                    settingsModel.Position = jsonObj.payload.settings.settingsModel.Position;
                    document.getElementById('txtPosition').value = settingsModel.Position;
                }
                break;
            default:
                break;
        }
    };
}

const setSettings = (value, param) => {
    if (websocket) {
        settingsModel[param] = value;
        var json = {
            "event": "setSettings",
            "context": uuid,
            "payload": {
                "settingsModel": settingsModel
            }
        };
        websocket.send(JSON.stringify(json));
    }
};

