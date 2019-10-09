import React from 'react';
import Microphone from './components/Microphone';
import {MESSAGE_TYPE, sendCommand} from './serverConnection';
import SpeechModule, {RESULT_TYPE} from './speechModule';

import './App.css';

export default class App extends React.Component {
	
	componentDidMount() {
		SpeechModule.onstart = this._onSpeechStarted.bind(this);
		SpeechModule.onend = this._onSpeechEnded.bind(this);
		SpeechModule.onresult = this._onSpeechResults.bind(this);
	
		SpeechModule.init();
		SpeechModule.start();//auto-start
		// SpeechModule.end();
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
				<div><input type="text" placeholder="Example command" onKeyDown={e => {
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
									result: 'zagraj piosenkę nie bądź taki szybki bil',
									confidence: 0.700,
									type: RESULT_TYPE.FINAL
								}],
								result_index: 3
							})
						}
						else { // noinspection JSUnresolvedVariable
							sendCommand(e.target.value);
						}
						
						e.target.value = '';
					}
				}}/></div>
			</header>
		</div>;
	}
}
