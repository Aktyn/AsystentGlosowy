const SERVER_URL = "ws://localhost:6969"
let socket;
let connected = false;

function connect() {
	socket = new WebSocket(SERVER_URL);
	
	socket.onopen = function(e) {
		console.log("CONNECTED");
		connected = true;
	};
	
	socket.onclose = function(e) {
		console.log("DISCONNECTED");
		connected = false;
		setTimeout(reconnect, 5000);
	};
	
	socket.onmessage = function(e) {
		console.log("Message:", e.data);
	};
	
	socket.onerror = function(e) {
		console.log("ERROR:", e.data);
	};
}

function reconnect() {
	if(connected)
		return;
	console.log("RECONNECTING");
	connect();
}

connect();

/** @type {string} text */
export function sendCommand(text) {
	console.log('sending command:', text);
	
	if(connected)
		socket.send(text);
}