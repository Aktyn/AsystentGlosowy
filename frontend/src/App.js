import React from 'react';
import Microphone from './components/Microphone';
import YouTubeEmbed from "./components/YouTubeEmbed";
import {MESSAGE_TYPE, sendCommand, eventEmitter, handleJSON} from './serverConnection';
import SpeechModule, {RESULT_TYPE} from './speechModule';

import './App.css';

const logoImg = require('./icons/logo.png');

export default class App extends React.Component {
	/** @type {HTMLDivElement | null} */
	sentencesListDiv = null;
	sentencesCount = 0;

	//listeners
	songRequestListener = this._onSongRequested.bind(this);
	commandExecutedListener = this._onCommandExecuted.bind(this);
	commandIgnoredListener = this._onCommandIgnored.bind(this);

	state = {
		/** @type {string | null} */
		videoId: null,
		title: null,
		/** @type {{index: number, confidence: number, content: string, executed: boolean}[]} */
		sentences: [],
		debugInterimResults: false,
		testCommandIndex: 0,
		/** @type {string | null} */
		activeProcedureName: null
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
		eventEmitter.on('ignored', this.commandIgnoredListener);
	}

	componentWillUnmount() {
		eventEmitter.off('songRequested', this.songRequestListener);
		eventEmitter.off('executed', this.commandExecutedListener);
		eventEmitter.off('ignored', this.commandIgnoredListener);
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
		this.setState({sentences, activeProcedureName: null});
	}

	_onCommandIgnored(procedureName) {
		this.setState({activeProcedureName: procedureName});
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
		if(!last_sentence || last_sentence.index !== result_index) {
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

	/** @param {string} msg */
	_fakeSpokenMessage(msg) {
		this._onSpeechResults([{
			result: msg,
			confidence: 1.0,
			type: this.state.debugInterimResults ? 
				RESULT_TYPE.INTERIM : RESULT_TYPE.FINAL
		}], this.state.testCommandIndex);
		this.setState({testCommandIndex: this.state.testCommandIndex+1});
	}

	/** @param {React.KeyboardEvent<HTMLInputElement>} e */
	onCommandKeyDown(e) {
		if (e.key === 'Enter') {
			switch(e.target.value) {
				default:
					this._fakeSpokenMessage(e.target.value);
					break;
				case 'test1':
					this._fakeSpokenMessage('przykładowa komenda i przykładowe dane');
					break;
				case 'test2':
					this._fakeSpokenMessage('zagraj piosenkę explosion majlo club');
					break;
			}

			//TODO: history system of commands
			
			e.target.value = '';
		}
	}

	renderRecognizeSentences() {
		return this.state.sentences.map((sentence, index) => (
			<div key={index} className={sentence.executed ? 'executed' : ''}>
				{sentence.content}
			</div>
		));
	}
	
	render() {
		return <div className={"layout"}>
			<nav className="header-panel">
				<div>TODO - server connection status</div>
				<Microphone/>
			</nav>
			<div className="App">
				{this.state.videoId && this.state.title ? 
					<YouTubeEmbed videoId={this.state.videoId} /> :
					<div>
						<img src={logoImg} height={256} />
					</div>}
				<p style={{display: 'inline-flex', alignItems: 'center'}}>
					<input type="text" placeholder="Example command" 
						onKeyDown={this.onCommandKeyDown.bind(this)}/>
					<input type="checkbox" onChange={e => {
						this.setState({debugInterimResults: e.target.checked});
					}} checked={this.state.debugInterimResults} />
					<input type="number" onChange={e => {
						this.setState({
							testCommandIndex: parseInt(e.target.value)
						});
					}} value={this.state.testCommandIndex} style={{
						width: '50px'
					}} />
				</p>
				<div className='sentences' ref={el => this.sentencesListDiv = el}>{
					this.renderRecognizeSentences()
				}</div>
				<div>{this.state.activeProcedureName && 
					<span>Active procedure: {this.state.activeProcedureName}</span>
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
					<button onClick={() => {
						this.setState({testCommandIndex: 0});
						SpeechModule.init();
						SpeechModule.start();
					}}>Reset speech module</button>
				</p>
			</div>
		</div>;
	}
}
