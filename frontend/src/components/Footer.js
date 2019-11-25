import React from 'react';
import {eventEmitter} from "../serverConnection";

export default class Footer extends React.Component {
    //listener
    notifyListener = this._onNotify.bind(this);

    state = {
      message: null,
    };

    componentDidMount() {
        eventEmitter.on('notify', this.notifyListener);
    }

    componentWillUnmount() {
        eventEmitter.off('notify', this.notifyListener);
    }

    _onNotify(message) {
        this.setState({
            message: message,
        });

        setTimeout(() => {
            this.setState({
                message: null,
            });
        }, 3000);
    }

    render() {
        return <div className='footer'>{this.state.message}</div>;
    }
}