import React, { useEffect, useState } from 'react';

import '../styles/Visualizer.css';

export default function Visualizer() {
    const [lengths, setLengths] = useState([]);
    const [bufferLength, setBufferLength] = useState(0);

    useEffect(() => {
        let running = true;

        navigator.mediaDevices.getUserMedia({audio:true}).then(stream => {
            const audioContext = new AudioContext();
            const audioStream = audioContext.createMediaStreamSource(stream);
            const analyser = audioContext.createAnalyser();
            analyser.fftSize = 128;

            audioStream.connect(analyser);

            const frequencyArray = new Uint8Array(analyser.frequencyBinCount);
            setBufferLength(frequencyArray.length);

            const draw = () => {
                if(running)
                    requestAnimationFrame(draw);
                analyser.getByteFrequencyData(frequencyArray);
                
                const newLengths = [];
                //NOTE: do not use bufferLength in this loop
                for (let i=0; i<frequencyArray.length; i++) {
                    const adjustedLength = Math.floor(frequencyArray[i]) - (Math.floor(frequencyArray[i]) % 5);
                    newLengths.push(adjustedLength);
                }
                setLengths(newLengths);
            };
            draw();
        });

        return () => running = false;
    }, []);
    
    return (
        <div className="visualizer">
            <svg id="visualizer" style={{width: '128px'}} preserveAspectRatio="none" viewBox="0 0 128 255" version="1.1" xmlns="http://www.w3.org/2000/svg">
                <defs>
                    <mask id="mask"><g>{
                        lengths.map((len, i) => (
                            <React.Fragment key={i}>
                                <path d={`M ${bufferLength+i}, ${128+len/2} l 0,-${len}`}></path>
                                <path d={`M ${bufferLength-1-i}, ${128+len/2} l 0,-${len}`}></path>
                            </React.Fragment>
                        ))
                    }</g>
                    </mask>
                </defs>
                <rect x="0" y="0" width="100%" height="100%" mask="url(#mask)" fill="#b0e2ff"/>
            </svg>
        </div>

    );
}