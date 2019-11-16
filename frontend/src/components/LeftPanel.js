import React, {useEffect, useRef} from 'react';
import '../styles/LeftPanel.css';

/** @param {{sentences: {
 * index: number, 
 * confidence: number, 
 * content: string, 
 * executed: boolean
 * }[]}} props */
function LeftPanel(props) {

    /** @type {React.Ref<HTMLElement>} */
    const sentencesList = useRef();

    useEffect(() => {
        if(sentencesList.current)
		    sentencesList.current.scrollTop = sentencesList.current.scrollHeight;
    }, [props.sentences.length, sentencesList]);

    return <aside className="left-panel" ref={sentencesList}>
        <div className='sentences'>{props.sentences.map((sentence, index) => (
             <div key={index} className={sentence.executed ? 'executed' : ''}>
                {sentence.content}
             </div>
        ))}</div>
    </aside>
}

export default LeftPanel;