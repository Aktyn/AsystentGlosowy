import SpeechModule from './speechModule';

const SERVER_URL = "ws://localhost:7000";
let socket;
let connected = false;//TODO: emitting connection change events

export const MESSAGE_TYPE = {
	speech_result: 1
};

/** @param {Object} data */
function handleJSON(data) {
	console.log( 'data', data );
	
	if( data.res === 'executed' ) {
		SpeechModule.ignoreIndex(data.index);
	}
}

function connect() {
	socket = new WebSocket(SERVER_URL);
	
	socket.onopen = function (e) {
		console.log("CONNECTED");
		connected = true;
	};
	
	socket.onclose = function (e) {
		console.log("DISCONNECTED");
		connected = false;
		setTimeout(reconnect, 5000);
	};
	
	socket.onmessage = function (e) {
		console.log("Message from asystent:", e.data);
		
		if(typeof e.data !== 'string')
			return;
		try {
			let json = JSON.parse(e.data);
			handleJSON(json);
		}
		catch(e) {}
	};
	
	/*socket.onerror = function(e) {
		console.log("ERROR:", e.data);
	};*/
}

function reconnect() {
	if (connected)
		return;
	console.log("RECONNECTING");
	connect();
}

connect();

/** @type {string | {type: number}} command */
export function sendCommand(command) {
	if (!connected)
		return;
	
	if(typeof command === 'object')
		command = JSON.stringify(command);
		
	console.log('sending command:', command);
	socket.send(command);
}