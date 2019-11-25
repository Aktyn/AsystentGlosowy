import React from 'react';
import YouTubeEmbed from "../components/YouTubeEmbed";

const logoImg = require('../icons/logo.png');

/** @param {{
 * video: {id: string, title: string} | null,
 * activeProcedureName: string | null,
 * testCommandIndex: number,
 * onTestCommandIndexChange: Function,
 * debugInterimResults: boolean,
 * onDebugInterimResultsChange: Function,
 * fakeSpokenMessage: Function
 * }} props */
export default function Content(props) {

    /** @param {React.KeyboardEvent<HTMLInputElement>} e */
	function onCommandKeyDown(e) {
		if (e.key === 'Enter') {
			switch(e.target.value) {
				default:
					props.fakeSpokenMessage(e.target.value);
					break;
				case 'test1':
					props.fakeSpokenMessage('przykładowa komenda i przykładowe dane');
					break;
				case 'test2':
					props.fakeSpokenMessage('zagraj piosenkę explosion majlo club');
					break;
			}

			//TODO: history system of commands
			
			e.target.value = '';
		}
	}

    return <main className="content">
        {props.videos ? 
			<YouTubeEmbed videos={props.videos} /> :
			<div>
                <img src={logoImg} height={256} alt="Logo"/>
            </div>
        }
        <p style={{display: 'inline-flex', alignItems: 'center'}}>
            <input type="text" placeholder="Example command" 
                onKeyDown={onCommandKeyDown}/>
            <input type="checkbox" onChange={e => {
                props.onDebugInterimResultsChange(e.target.checked);
            }} checked={props.debugInterimResults} />
            <input type="number" onChange={e => {
                props.onTestCommandIndexChange(parseInt(e.target.value));
            }} value={props.testCommandIndex} style={{
                width: '50px'
            }} />
        </p>
        <div>{props.activeProcedureName ?
            <span>Active procedure: {props.activeProcedureName}</span> :
            <span>&nbsp;</span>
        }</div>
    </main>
}