import React, {useState, useEffect} from 'react';
import Microphone from './Microphone';
import Visualizer from "./Visualizer";
import {eventEmitter, handleJSON} from '../serverConnection';
import SpeechModule from '../speechModule';

import '../styles/Header.css';
import notify from '../notifications';

/** @param {{onReset: Function}} props */
export default function Header(props) {
    const [serverConnected, setServerConnected] = useState(false);

    const onServerConnected = () => setServerConnected(true);
    const onServerDisconnected = () => setServerConnected(false);

    useEffect(() => {
        eventEmitter.on('serverConnected', onServerConnected);
        eventEmitter.on('serverDisconnected', onServerDisconnected);
        return () => {
            eventEmitter.off('serverConnected', onServerConnected);
		    eventEmitter.off('serverDisconnected', onServerDisconnected);
        }
    }, []);

    return <nav className="header">
        <div className='buttons-panel'>
            <div>
                <button id='playBtn' onClick={() => handleJSON({res: 'play'})}>Play</button>
                <button id='pauseBtn' onClick={() => handleJSON({res: 'pause'})}>Pause</button>
            </div>
            <div>
                <button id='muteBtn' onClick={() => handleJSON({res: 'mute'})}>Mute</button>
                <button id='unmuteBtn' onClick={() => handleJSON({res: 'unmute'})}>Unmute</button>
            </div>
            <input id='setVolumeInput' type='number' min='0' max='100' placeholder='volume' onChange={() => 
            {
                handleJSON({
                    res: 'setVolume', 
                    volume: document.getElementById('setVolumeInput').value
                });
            }}/>
            <button onClick={() => {
                props.onReset();
                SpeechModule.init();
                SpeechModule.start();
            }}>Reset speech module</button>
            <button onClick={() => notify('Example notification')}>Example notification</button>
        </div>
        <Visualizer />
        <div style={{
            display: 'inline-flex', 
            flexDirection: 'row', 
            alignItems: 'center', 
            justifySelf: 'flex-end'
        }}>
            <span>server {serverConnected ? 
                <span style={{color: '#00df81'}}>CONNECTED</span> : 
                <span style={{color: 'red'}}>DISCONNECTED</span>}
            </span>
            <Microphone />
        </div>
    </nav>;
}