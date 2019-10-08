import React from 'react';
import './App.css';
import {sendCommand} from './serverConnection';

import Microphone from './Microphone'

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <Microphone />
        <div><input type="text" placeholder="Example command" onKeyDown={e => {
            if(e.key === 'Enter') {
              sendCommand(e.target.value);
              e.target.value = '';
            }       
        }} /></div>
      </header>
    </div>
  );
}

export default App;
