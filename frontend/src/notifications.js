import {eventEmitter} from "./serverConnection";

const synth = ('speechSynthesis' in window) ? window.speechSynthesis : {};
const voices = synth.getVoices();

/** @param {string} message */
function speak(message) {
    if(!('SpeechSynthesisUtterance' in window))
        return;
    const utterThis = new SpeechSynthesisUtterance(message);
    utterThis.voice = voices[0];
    synth.speak(utterThis);
}

/** @param {string} message */
export default function notify(message) {
    eventEmitter.emit('notify', message);
    speak(message);
}