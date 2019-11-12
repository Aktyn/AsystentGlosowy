import React from 'react';

const micIsOnImage = require('../icons/mic-on-24px.svg');
const micIsOffImage = require('../icons/mic-off-24px.svg');

export default class Microphone extends React.Component {
    render() {
        return (
            <div>
                <img src={this.props.micIsOn ? micIsOnImage : micIsOffImage} alt="microphone on" style={{display: 'block', margin: 'auto'}}/>
            </div>
        )
    }
}