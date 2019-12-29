import React from 'react';

import '../styles/Visualizer.css';

export default class YouTubeEmbed extends React.Component {

    componentDidMount() {
        this.visualize();
    }

    visualize() {
        let audioContext;

        const paths = document.getElementsByTagName('path');
        const mask = document.getElementById('mask');

        navigator.mediaDevices.getUserMedia({audio:true})
            .then(stream => {
                audioContext = new AudioContext();
                let audioStream = audioContext.createMediaStreamSource(stream);
                let analyser = audioContext.createAnalyser();
                analyser.fftSize = 128;

                audioStream.connect(analyser);

                let bufferLength = analyser.frequencyBinCount;
                let frequencyArray = new Uint8Array(bufferLength);

                for(let i=0; i<64; i++) {
                    let path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
                    mask.appendChild(path);
                }

                const draw = () => {
                    requestAnimationFrame(draw);
                    analyser.getByteFrequencyData(frequencyArray);
                    let adjustedLength;
                    for (let i=0; i<64; i++) {
                        adjustedLength = Math.floor(frequencyArray[i]) - (Math.floor(frequencyArray[i]) % 5);
                        paths[i].setAttribute('d', 'M '+ (i) +',255 l 0,-' + adjustedLength);
                    }
                };
                draw();
            })
    }

    render() {
        return (
            <div className="visualizer">
                <svg id="visualizer" preserveAspectRatio="none" viewBox="0 0 255 255" version="1.1" xmlns="http://www.w3.org/2000/svg">
                    <defs>
                        <mask id="mask">
                            <g id="maskGroup"/>
                        </mask>
                    </defs>
                    <rect x="0" y="0" width="100%" height="100%" mask="url(#mask)" fill="#00cbff"/>
                </svg>
            </div>

        )
    }
}