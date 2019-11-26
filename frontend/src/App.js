import React from 'react';
import {MESSAGE_TYPE, sendCommand, eventEmitter} from './serverConnection';
import SpeechModule, {RESULT_TYPE} from './speechModule';

import './styles/App.css';
import Header from './components/Header';
import LeftPanel from './components/LeftPanel';
import Content from './components/Content';
import Footer from './components/Footer';
import RightPanel from './components/RightPanel';
import Notifications from './components/Notifications';

export default class App extends React.Component {
	//listeners
	songRequestListener = this._onSongRequested.bind(this);
	commandExecutedListener = this._onCommandExecuted.bind(this);
	commandIgnoredListener = this._onCommandIgnored.bind(this);

	state = {
		currentVideos: null,
		/** @type {{
		 * index: number, 
		 * confidence: number, 
		 * content: string, 
		 * executed: boolean
		 * }[]} */
		sentences: [],
		debugInterimResults: false,
		testCommandIndex: 0,
		/** @type {string | null} */
		activeProcedureName: null
	};
	
	componentDidMount() {
		SpeechModule.onstart = App._onSpeechStarted.bind(this);
		SpeechModule.onend = App._onSpeechEnded.bind(this);
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

	static _onSpeechStarted() {
		console.log('Speech recognition started');
	}
	
	static _onSpeechEnded() {
		console.log('Speech recognition ended');
	}

	_onSongRequested(videos) {
		this.setState({
			currentVideos: videos
		});
		console.log('VideoId:', videos.selected.id, 'Title:', videos.selected.title);
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

		//descending sorting over confidence value
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
	fakeSpokenMessage(msg) {
		this._onSpeechResults([{
			result: msg,
			confidence: 1.0,
			type: this.state.debugInterimResults ? 
				RESULT_TYPE.INTERIM : RESULT_TYPE.FINAL
		}], this.state.testCommandIndex);
		this.setState({testCommandIndex: this.state.testCommandIndex+1});
	}
	
	render() {
		return <div className={"layout"}>
			<Header onReset={() => this.setState({testCommandIndex: 0})} />
			<LeftPanel sentences={this.state.sentences} />
			<Content
				videos={this.state.currentVideos}
				activeProcedureName={this.state.activeProcedureName}
				testCommandIndex={this.state.testCommandIndex}
				onTestCommandIndexChange={testCommandIndex => {
					this.setState({testCommandIndex});
				}}
				debugInterimResults={this.state.debugInterimResults}
				onDebugInterimResultsChange={(debugInterimResults => {
					this.setState({debugInterimResults})
				})}
				fakeSpokenMessage={this.fakeSpokenMessage.bind(this)}
			/>
			<RightPanel />
			<Footer />
			<Notifications />
		</div>;
	}
}
