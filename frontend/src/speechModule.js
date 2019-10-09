export const RESULT_TYPE = {
	INTERIM: 1,
	FINAL: 2,
	ALTERNATIVE: 3
};
const noop = function () {};

// noinspection JSUnresolvedVariable
let SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
if (!SpeechRecognition) {
	console.warn('SpeechRecognition not supported');
	SpeechRecognition = function () {
		// noinspection JSUnusedGlobalSymbols
		this.start = noop;
	};
}

/** @type {SpeechRecognition | null} */
let recognition = null;
let recognition_active = false;

let ignore_index = -1;

function init() {
	if (recognition)
		recognition.stop();
	
	recognition = new SpeechRecognition();
	// recognition.lang = 'en-US';
	recognition.lang = 'pl-PL';
	recognition.continuous = true;
	recognition.interimResults = true;
	recognition.maxAlternatives = 5;
	
	console.log(recognition);
	
	let recognition_start_timestamp = 0;
	
	recognition.onstart = () => {
		ignore_index = -1;
		recognition_active = true;
		recognition_start_timestamp = Date.now();
		
		if (typeof SpeechModule.onstart === 'function')
			SpeechModule.onstart();
	};
	recognition.onend = () => {
		if (recognition_active) {
			recognition_active = false;
			if (Date.now() - recognition_start_timestamp > 1000) {//at least 1 second difference
				console.log('recognition restarted');
				recognition_start_timestamp = Date.now();
				recognition.start();//restart recognition
			} else
				console.log('todo');
		} else {
			recognition_active = false;
			console.log('recognition ended');
		}
		if (typeof SpeechModule.onend === 'function')
			SpeechModule.onend();
	};
	
	//recognition.onerror = e => console.error(e);
	
	/** @param {SpeechRecognitionEvent} event */
	recognition.onresult = async (event) => {
		let result = event.results[event.results.length - 1];
		
		if (ignore_index === event.resultIndex) {//recognition already succeeded
			console.log('further results ignored');
			return;
		}
		
		if (!result.isFinal) {
			if (SpeechModule.onresult) {
				SpeechModule.onresult([{
					result: result[0].transcript,
					confidence: result[0].confidence,
					type: RESULT_TYPE.INTERIM
				}], event.resultIndex);
			}
			return;
		}
		
		if (SpeechModule.onresult) {
			let out_res = [];
			
			for (let j = 0; j < result.length; j++) {
				out_res.push({
					result: result[j].transcript,
					confidence: result[j].confidence,
					type: j > 0 ? RESULT_TYPE.ALTERNATIVE : RESULT_TYPE.FINAL
				});
			}
			
			SpeechModule.onresult(out_res, event.resultIndex);
		}
	};
}

/** This function returns interim or final results of speech recognizing
 @name OnResult
 @function
 * @param {{
 *     result: string,
 *     confidence: number,
 *     type: RESULT_TYPE
 * }[]} results
 * @param {number} index
 */

const SpeechModule = {
	RESULT_TYPE,
	
	init,
	
	start() {
		if (!recognition) {
			console.info('Recognition must be initialized before starting it');
			return;
		}
		recognition.start();
	},
	/** @type {Function | null} */
	onstart: null,
	
	end() {
		if (!recognition)//nothing to end
			return;
		recognition_active = false;
		recognition.stop();
	},
	/** @type {Function | null} */
	onend: null,
	
	/** @type {OnResult | null | Function} */
	onresult: null,
	
	isActive() {
		return recognition_active;
	},
	
	/** @param {number} index */
	ignoreIndex(index) {
		ignore_index = index;
	}
};

export default SpeechModule;