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

    componentDidMount() {
        eventEmitter.on('play', () => {
            YouTubeEmbed.playVideo(this.e)
        });

        eventEmitter.on('pause', () => {
            YouTubeEmbed.pauseVideo(this.e)
        });

        eventEmitter.on('mute', () => {
            YouTubeEmbed.muteVideo(this.e)
        });

        eventEmitter.on('unmute', () => {
            YouTubeEmbed.unmuteVideo(this.e)
        });

        eventEmitter.on('setVolume', (volume) => {
            YouTubeEmbed.setVolume(this.e, volume)
        });
    }

    render() {
        return (
            <YouTube
                videoId={this.props.videoId}
                opts={this.options}
                onReady={e => this.e = e}
            />
        );
    }

    // Control the video player using these functions
    static pauseVideo(event) {
        event.target.pauseVideo();
        console.log('Pausing the video')
    }

    static playVideo(event) {
        event.target.playVideo();
        console.log('Resuming the video')
    }

    static muteVideo(event) {
        event.target.mute();
        console.log('Muting the video')
    }

    static unmuteVideo(event) {
        event.target.unMute();
        console.log('Muting the video')
    }

    static setVolume(event, volume) {
        event.target.setVolume(volume)
    }
}
