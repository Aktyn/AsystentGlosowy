import React, {useState} from 'react';

const micIsOnImage = require('../icons/mic-on-24px.svg');
const micIsOffImage = require('../icons/mic-off-24px.svg');

export default function Microphone() {
    const [micIsOn, setMicIsOn] = useState(false);

    navigator.mediaDevices.getUserMedia({ audio: true }).then(microphone => {
        if(microphone) {
            setMicIsOn(true);
            console.log(microphone);
        }
    }).catch(() => {
        setMicIsOn(false);
    });

    return <img src={micIsOn ? micIsOnImage : micIsOffImage} 
        alt="microphone on" style={{display: 'block', margin: 'auto'}}/>;
}