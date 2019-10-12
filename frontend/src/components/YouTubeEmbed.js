import React from 'react';
import YouTube from "react-youtube";

export default class YouTubeEmbed extends React.Component {
    options = {
        height: '390',
        width: '640',
        playerVars: { // https://developers.google.com/youtube/player_parameters
            autoplay: 1
        }
    };

    render() {
        return (
            <YouTube
                videoId={this.props.videoId}
                opts={this.options}
                onReady={YouTubeEmbed.pauseVideo}
            />
        );
    }

    // Control the video player using these functions
    static pauseVideo(event) {
        event.target.pauseVideo();
        console.log('Pausing the video')
    }

    static resumeVideo(event) {
        event.target.resumeVideo();
        console.log('Resuming the video')
    }

    static muteVideo(event) {
        event.target.mute();
        console.log('Muting the video')
    }

    static setVolume(event) {
        event.target.setVolume(70) //todo
    }
}
