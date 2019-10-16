import React from 'react';
import YouTube from "react-youtube";

import {eventEmitter} from "../serverConnection";

export default class YouTubeEmbed extends React.Component {
    options = {
        height: '390',
        width: '640',
        playerVars: { // https://developers.google.com/youtube/player_parameters
            autoplay: 1
        }
    };

    /** @type {any} */
    e = null;

    playEventListener = this._playVideo.bind(this);
    pauseEventListener = this._pauseVideo.bind(this);
    muteEventListener = this._muteVideo.bind(this);
    unmuteEventListener = this._unmuteVideo.bind(this);
    setVolumeEventListener = this._setVolume.bind(this);

    componentDidMount() {
        eventEmitter.on('play', this.playEventListener);
        eventEmitter.on('pause', this.pauseEventListener);
        eventEmitter.on('mute', this.muteEventListener);
        eventEmitter.on('unmute', this.unmuteEventListener);
        eventEmitter.on('setVolume', this.setVolumeEventListener);
    }

    componentWillUnmount() {
        eventEmitter.off('play', this.playEventListener);
        eventEmitter.off('pause', this.pauseEventListener);
        eventEmitter.off('mute', this.muteEventListener);
        eventEmitter.off('unmute', this.unmuteEventListener);
        eventEmitter.off('setVolume', this.setVolumeEventListener);
    }

    // Control the video player using these functions
    _pauseVideo() {
        this.e.target.pauseVideo();
        console.log('Pausing the video')
    }

    _playVideo() {
        this.e.target.playVideo();
        console.log('Resuming the video')
    }

    _muteVideo() {
        this.e.target.mute();
        console.log('Muting the video')
    }

    _unmuteVideo() {
        this.e.target.unMute();
        console.log('Muting the video')
    }

    _setVolume(volume) {
        this.e.target.setVolume(volume)
    }

    render() {
        return <YouTube videoId={this.props.videoId} opts={this.options} 
            onReady={e => this.e = e} />;
    }
}
