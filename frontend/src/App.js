import React from 'react';
import Microphone from './components/Microphone';
import YouTubeEmbed from "./components/YouTubeEmbed";
import {MESSAGE_TYPE, sendCommand, eventEmitter, handleJSON} from './serverConnection';
import SpeechModule, {RESULT_TYPE} from './speechModule';

import './App.css';

export default class App extends React.Component {
	/** @type {HTMLDivElement | null} */
	sentencesListDiv = null;
	sentencesCount = 0;

	//listeners
	songRequestListener = this._onSongRequested.bind(this);
	commandExecutedListener = this._onCommandExecuted.bind(this);

	state = {
		videoId: null,
		title: null,
		/** @type {{index: number, confidence: number, content: string, executed: boolean}[]} */
		sentences: [],
	};
	
	componentDidMount() {
		SpeechModule.onstart = this._onSpeechStarted.bind(this);
		SpeechModule.onend = this._onSpeechEnded.bind(this);
		SpeechModule.onresult = this._onSpeechResults.bind(this);
	
		SpeechModule.init();
		SpeechModule.start();//auto-start
		// SpeechModule.end();

		// Listening to events
		eventEmitter.on('songRequested', this.songRequestListener);
		eventEmitter.on('executed', this.commandExecutedListener);
	}

	componentWillUnmount() {
		eventEmitter.off('songRequested', this.songRequestListener);
		eventEmitter.off('executed', this.commandExecutedListener);
	}

	componentDidUpdate(_prevProps, prevState) {
		if(this.sentencesCount < this.state.sentences.length && this.sentencesListDiv) {
			this.sentencesCount = this.state.sentences.length;
			this.sentencesListDiv.scrollTop = this.sentencesListDiv.scrollHeight;
		}
	}

	_onSpeechStarted() {
		console.log('Speech recognition started');
	}
	
	_onSpeechEnded() {
		console.log('Speech recognition ended');
	}

	_onSongRequested(videoId, title) {
		this.setState({
			videoId: videoId,
			title: title,
		});
		console.log('VideoId:', videoId, 'Title:', title);
	}

	_onCommandExecuted(index) {
		let sentences = this.state.sentences;
		for(let i=sentences.length-1; i>=0; i--) {
			if(sentences[i].index === index)
				sentences[i].executed = true;
		}
		this.setState({sentences});
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

		//descending sorting over confidence valu
		let most_confident_result = results.sort((a,b) => b.confidence - a.confidence)[0];

		let sentences = this.state.sentences;
		let last_sentence = sentences.length > 0 ? sentences[sentences.length-1] : null;
		if(!last_sentence || last_sentence.index < result_index) {
			sentences.push({
				index: result_index,
				confidence: most_confident_result.confidence,
				content: most_confident_result.result
			});
		}
		else if(last_sentence.confidence < most_confident_result.confidence ||
			last_sentence.content.split(' ').length < most_confident_result.result.split(' ').length ||
			most_confident_result.type !== RESULT_TYPE.INTERIM) 
		{
			last_sentence.confidence = most_confident_result.confidence;
			last_sentence.content = most_confident_result.result;
		}
		this.setState({sentences});
		
		sendCommand({
			type: MESSAGE_TYPE.speech_result,
			results,
			result_index
		});
	}

	/** @param {React.KeyboardEvent<HTMLInputElement>} e */
	onCommandKeyDown(e) {
		if (e.key === 'Enter') {
			switch(e.target.value) {
				default:
					sendCommand(e.target.value);
					break;
				case 'test1':
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
					break;
				case 'test2':
					sendCommand({
						type: MESSAGE_TYPE.speech_result,
						results: [{
							result: 'zagraj piosenkę explosion majlo club',
							confidence: 0.700,
							type: RESULT_TYPE.FINAL
						}],
						result_index: 3
					});
					break;
				case 'test3':
					sendCommand({
						type: MESSAGE_TYPE.speech_result,
						results: [{
							result: 'zagraj piosenkę tiesto adagio for strings',
							confidence: 0.700,
							type: RESULT_TYPE.FINAL
						}],
						result_index: 4
					});
					break;
			}
			
			e.target.value = '';
		}
	}

	renderRecognizeSentences() {
		return this.state.sentences.map(sentence => (
			<div key={sentence.index} className={sentence.executed ? 'executed' : ''}>
				{sentence.content}
			</div>
		));
	}
	
	render() {
		return <div className="App">
			<section>
				<Microphone/>
				<p>
					<input type="text" placeholder="Example command" 
						onKeyDown={this.onCommandKeyDown.bind(this)}/>
				</p>
				{this.state.videoId && this.state.title && 
					<YouTubeEmbed videoId={this.state.videoId} />}
				<div className='sentences' ref={el => this.sentencesListDiv = el}>{
					this.renderRecognizeSentences()
				}</div>
				<p style={{
					display: 'inline-grid', 
					gridColumnGap: '10px', 
					gridAutoFlow: 'column'
				}}>
					<button id='playBtn' onClick={() => handleJSON({res: 'play'})}>Play</button>
					<button id='pauseBtn' onClick={() => handleJSON({res: 'pause'})}>Pause</button>
					<button id='muteBtn' onClick={() => handleJSON({res: 'mute'})}>Mute</button>
					<button id='unmuteBtn' onClick={() => handleJSON({res: 'unmute'})}>Unmute</button>
					<input id='setVolumeInput' type='number' min='0' max='100' placeholder='volume' onChange={() => 
					{
						handleJSON({
							res: 'setVolume', 
							volume: document.getElementById('setVolumeInput').value
						});
					}}/>
				</p>
			</section>
		</div>;
	}
}
