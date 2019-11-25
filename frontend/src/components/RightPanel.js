import React, {useEffect, useState} from 'react';
import {eventEmitter} from '../serverConnection';
import '../styles/RightPanel.css';

function RightPanel() {
    // id: "wEvUHknyFng"
    // thumbnail: "https://i.ytimg.com/vi/wEvUHknyFng/default.jpg"
    // title: "Kalwi &amp; Remi - Explosion (MAJLO &amp; Gazell Club Mix)"
    const [currentVideo, setCurrentVideo] = useState(null);
    const [videos, setVideos] = useState([]);

    const onPlaylistUpdate = (state, current) => {
        setCurrentVideo(current);
        setVideos([current, ...state]);
    };

    useEffect(() => {
        eventEmitter.on('playlistUpdate', onPlaylistUpdate);
        return () => {
            eventEmitter.off('playlistUpdate', onPlaylistUpdate);
        }
    }, []);

    return <aside className="right-panel">
        {videos.length ? <div className="current-playlist">
            <h4>Filmy w kolejce</h4>
            {videos.map(videos => {
                if(videos === null)
                    return undefined;
                return <section className={`queue${videos === currentVideo ? ' current' : ''}`}>
                    <div>
                        <label>{videos.selected.title}</label>
                        <img src={videos.selected.thumbnail} alt="thumbnail" />
                    </div>
                </section>;
            })}
        </div> : undefined}
        <div style={{textAlign: 'center'}}>
            <h4>Playlisty</h4>
            <section className="playlists">
                <div>Szalone lata 60 (13)</div>
                <div>Trailery (69)</div>
            </section>
        </div>
    </aside>
}

export default RightPanel;