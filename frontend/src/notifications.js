import {eventEmitter} from "./serverConnection";

/** @param {string} message */
export default function notify(message) {
    eventEmitter.emit('notify', message);
}