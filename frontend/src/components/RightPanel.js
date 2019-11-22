import React from 'react';
import '../styles/RightPanel.css';

function RightPanel() {

    return <aside className="right-panel">
        <div className="current-playlist">
            <h4>Filmy w kolejce</h4>
            <section className="queue">
                <div>
                    <label>Przykład</label>
                    <img src="https://i.ytimg.com/vi/BDr742MGZJ8/hqdefault.jpg" alt="thumbnail" />
                </div>
            </section>
        </div>
        <div>
            <h4>Playlisty</h4>
            <section className="playlists">
                <div>Szalone lata 60 (13)</div>
                <div>Trailery (69)</div>
            </section>
        </div>
    </aside>
}

export default RightPanel;