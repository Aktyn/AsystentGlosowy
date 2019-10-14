import SpeechModule from './speechModule';

const EventEmitter = require('events');
export const eventEmitter = new EventEmitter();

const SERVER_URL = "ws://localhost:7000";
let socket;
let connected = false;//TODO: emitting connection change events

export const MESSAGE_TYPE = {
	speech_result: 1
};

/** @param {Object} data */
export function handleJSON(data) {
	console.log('data', data);
	
	switch (data.res) {
		case 'executed':
			SpeechModule.ignoreIndex(data.index);
			break;
		case 'ignored': break;
		case 'request_song':
			if(data.video_id && data.title) {
				eventEmitter.emit('songRequested', data.video_id, data.title);
			}
			else {
				console.error('Problem with requesting song, no video_id or title provided')
			}
			break;
		case 'play':
			eventEmitter.emit('play');
			break;
		case 'pause':
			eventEmitter.emit('pause');
			break;
		case 'mute':
			eventEmitter.emit('mute');
			break;
		case 'unmute':
			eventEmitter.emit('unmute');
			break;
		case 'setVolume':
			eventEmitter.emit('setVolume', data.volume);
			break;
		default:
			console.warn('Unknown server message');
			break;
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