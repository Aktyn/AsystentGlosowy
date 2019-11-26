import React, {useEffect, useState} from 'react';
import {eventEmitter} from '../serverConnection';

import '../styles/Notifications.css';

let messagesCounter = 0;

const sleep = milliseconds => new Promise(resolve => setTimeout(resolve, milliseconds));

export default function Notifications() {
    const [messages, setMessages] = useState([]);

    /** @param {string} message */
    const onNotification = async message => {
        const id = messagesCounter++;
        setMessages(messages => messages.concat({
            id,
            message,
            fading: false
        }));

        await sleep(5000);
        setMessages(messages => messages.map(msg => msg.id === id ? {...msg, fading: true} : msg));
        await sleep(500);
        setMessages(messages => messages.filter(msg => msg.id !== id));
    };

    useEffect(() => {
        eventEmitter.on('notify', onNotification);
        return () => eventEmitter.off('notify', onNotification);
    }, []);

    if(messages.length > 0) {
        return messages.map(msg => (
            <div key={msg.id} className={`notification${msg.fading ? ' fading' : ''}`}>
                {msg.message}
            </div>
        ))
    }
    return null;
}