import React from 'react';
import Microphone from './components/Microphone';
import YouTubeEmbed from "./components/YouTubeEmbed";
import {MESSAGE_TYPE, sendCommand, eventEmitter, handleJSON} from './serverConnection';
import SpeechModule, {RESULT_TYPE} from './speechModule';

import './App.css';

export default class App extends React.Component {

	state = {
		videoId: null,
		title: null,
		speechText: '',
	};
	
	componentDidMount() {
		SpeechModule.onstart = this._onSpeechStarted.bind(this);
		SpeechModule.onend = this._onSpeechEnded.bind(this);
		SpeechModule.onresult = this._onSpeechResults.bind(this);
	
		SpeechModule.init();
		SpeechModule.start();//auto-start
		// SpeechModule.end();

		// Listening to events
		eventEmitter.on('songRequested', (videoId, title) => {
			this.setState({
				videoId: videoId,
				title: title,
			});
			console.log('VideoId:', videoId, 'Title:', title);
		})
	}

	_onSpeechStarted() {
		console.log('Speech recognition started');
	}
	
	_onSpeechEnded() {
		console.log('Speech recognition ended');
	}
	
	/**
	 * @param {{
	 *     result: string,
	 *     confidence: number,
	 *     type: RESULT_TYPE
	 * }[]} results
	 *  @param {number} result_index
	 */
	_onSpeechResults(results, result_index) {
		console.log( results, result_index );
		this.setState({
			speechText: results[0].result
		})
		
		sendCommand({
			type: MESSAGE_TYPE.speech_result,
			results,
			result_index
		});
	}
	
	render() {
		return <div className="App">
			<header className="App-header">
				<Microphone/>
				<div><input type="text" placeholder="Example command" onKeyDown={e => { // command input
					if (e.key === 'Enter') {
						// noinspection JSUnresolvedVariable
						if(e.target.value === 'test1') {
							sendCommand({
								type: MESSAGE_TYPE.speech_result,//test speech results (some predefined code)
								results: [
									{
										result: 'przykładowa komenda costam',//example command
										confidence: 0.618,
										type: RESULT_TYPE.FINAL
									},
									{
										result: 'przykładowe polecenie',//example command
										confidence: 0.95,
										type: RESULT_TYPE.ALTERNATIVE
									},
								],
								result_index: 2
							});
						}
						else if(e.target.value === 'test2') {
							sendCommand({
								type: MESSAGE_TYPE.speech_result,
								results: [{
									result: 'zagraj piosenkę explosion majlo club',
									confidence: 0.700,
									type: RESULT_TYPE.FINAL
								}],
								result_index: 3
							})
						}
						else if(e.target.value === 'test3') {
							sendCommand({
								type: MESSAGE_TYPE.speech_result,
								results: [{
									result: 'zagraj piosenkę tiesto adagio for strings',
									confidence: 0.700,
									type: RESULT_TYPE.FINAL
								}],
								result_index: 4
							})
						}
						else { // noinspection JSUnresolvedVariable
							sendCommand(e.target.value);
						}
						
						e.target.value = '';
					}
				}}/></div>
				{this.state.videoId && this.state.title ? <YouTubeEmbed videoId={this.state.videoId} /> : null}
				<div>
					{this.state.speechText ? <p>{this.state.speechText}</p> : null}
				</div>
				<button id='playBtn' onClick={ () => handleJSON({res: 'play'})}>Play</button>
				<button id='pauseBtn' onClick={ () => handleJSON({res: 'pause'})}>Pause</button>
				<button id='muteBtn' onClick={ () => handleJSON({res: 'mute'})}>Mute</button>
				<button id='unmuteBtn' onClick={ () => handleJSON({res: 'unmute'})}>Unmute</button>
				<input id='setVolumeInput' type='number' min='0' max='100' placeholder='volume' onChange={() => handleJSON({res: 'setVolume', volume: document.getElementById('setVolumeInput').value})}/>
			</header>
		</div>;
	}
}
