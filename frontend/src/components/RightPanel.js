import React, {useEffect, useState} from 'react';
import {eventEmitter} from '../serverConnection';
import '../styles/RightPanel.css';

function RightPanel() {
    // id: "wEvUHknyFng"
    // thumbnail: "https://i.ytimg.com/vi/wEvUHknyFng/default.jpg"
    // title: "Kalwi &amp; Remi - Explosion (MAJLO &amp; Gazell Club Mix)"
    const [currentVideo, setCurrentVideo] = useState(null);
    const [playlists, setPlaylists] = useState([]);
    const [videos, setVideos] = useState([]);

    const onPlaylistUpdate = (state, current) => {
        setCurrentVideo(current);
        setVideos([current, ...state]);
    };

    const onPlaylistsListUpdate = (playlists) => {
        setPlaylists(Array.from(new Set(playlists)));
    };

    useEffect(() => {
        eventEmitter.on('playlistUpdate', onPlaylistUpdate);
        eventEmitter.on('playlistsListUpdate', onPlaylistsListUpdate);
        return () => {
            eventEmitter.off('playlistUpdate', onPlaylistUpdate);
            eventEmitter.off('playlistsListUpdate', onPlaylistsListUpdate);
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
                {playlists.map(playlist => (
                    <div key={playlist}>{playlist}</div>
                ))}
            </section>
        </div>
    </aside>
}

export default RightPanel;