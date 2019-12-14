import SpeechModule from './speechModule';
import notify from './notifications';

const EventEmitter = require('events');
export const eventEmitter = new EventEmitter();

const SERVER_URL = "ws://localhost:7000";
let socket;
let connected = false;//TODO: emitting connection change events

const url = new URL(window.location.href);
const closeWithServer = url.searchParams.get('closeWithServer') === 'true';

export const MESSAGE_TYPE = {
	speech_result: 1,
	video_finished: 2
};

/** @param {Object} data */
export function handleJSON(data) {
	if(data.res !== 'ignored')
		console.log('data', data);
	
	switch (data.res) {
		case 'executed':
			SpeechModule.ignoreIndex(data.index);
			eventEmitter.emit('executed', data.index);
			break;
		case 'ignored':
			eventEmitter.emit('ignored', data.procedure);
			break;
		case 'request_song':
			if(data.videos) {
				eventEmitter.emit('songRequested', data.videos);
			}
			else {
				console.error('Problem with requesting song, no video_id or title provided');
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
		case 'volume_down':
			eventEmitter.emit('changeVolume', -parseInt(data.volume||5));
			break;
		case 'volume_up':
			eventEmitter.emit('changeVolume', parseInt(data.volume||5));
			break;
		case 'end_playlist':
			notify("Brak filmów w kolejce. Odtwarzanie zakończone.");
			break;
		case 'playlist_update':
			eventEmitter.emit('playlistUpdate', data.state, data.current);
			break;
		case 'playlists_list_update':
			eventEmitter.emit('playlistsListUpdate', data.playlists);
			break;
		case 'request_confirmation':
			eventEmitter.emit('showConfirmationDialog', data.dialog_content);
			break;
		case 'confirmed':
			eventEmitter.emit('confirmProcedure');
			break;
		case 'rejected':
			eventEmitter.emit('rejectProcedure');
			break;
		case 'confirmation_timed_out':
			eventEmitter.emit('rejectProcedure');
			break;
		default:
			console.warn('Unknown server message');
			break;
	}
}

function connect() {
	socket = new WebSocket(SERVER_URL);
	
	socket.onopen = function () {
		console.log("CONNECTED");
		connected = true;
		eventEmitter.emit('serverConnected');
	};
	
	socket.onclose = function () {
		console.log("DISCONNECTED");
		eventEmitter.emit('serverDisconnected');
		if(connected && closeWithServer) {
			console.log('Closing expired session');
			window.close();
			return;
		}
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
		catch(err) {
			console.error('Incorrect message from server: ' + e.data);
		}
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