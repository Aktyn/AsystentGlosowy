import React, {useState} from 'react';
import '../styles/RightPanel.css';

function RightPanel() {

    const [showCurrentPlaylist, setShowCurrentPlaylist] = useState(false);

    return <aside className="right-panel">
        <div>
            TODO: lista zapisanych playlist, przykład:<br/>
            <div>Szalone lata 60 (13 utworów)</div>
            <div>Trailery (69 utworów)</div>
        </div>
        <div className={`current-playlist${showCurrentPlaylist ? ' open' : ''}`}>
            TODO: aktualnie odtwarzana playlista (lista miniaturek filmów z tytułami)
        </div>
        <div className='panel-header'>Jakieś wspólne info</div>
        <button className='view-switcher' onClick={() => 
            setShowCurrentPlaylist(x=>!x)}></button>
    </aside>
}

export default RightPanel;