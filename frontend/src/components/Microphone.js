import React from 'react';

const micIsOnImage = require('../icons/mic-on-24px.svg');
const micIsOffImage = require('../icons/mic-off-24px.svg');

export default class Microphone extends React.Component {
    state = {
        micIsOn: true,
        currentImage: micIsOnImage,
    };

    clickHandler = () => {
        if(this.state.micIsOn) {
            this.setState(prevState => ({
                micIsOn: !prevState.micIsOn,
                currentImage: micIsOffImage,
            }));
        } else {
            this.setState(prevState => ({
                micIsOn: !prevState.micIsOn,
                currentImage: micIsOnImage,
            }));
        }

    };

    render() {
        return (
            <img src={this.state.currentImage} alt="microphone on" onClick={this.clickHandler}/>
        )
    }
}