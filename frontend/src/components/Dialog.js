import React, { useEffect, useState } from 'react';
import { eventEmitter } from '../serverConnection';

import '../styles/Dialog.css';
import notify from '../notifications';

export default function Dialog() {
    const [open, setOpen] = useState(false);
    const [content, setContent] = useState('');

    function onShowDialog(_content) {
        setContent(_content);
        setOpen(true);
        try {
            document.activeElement.blur();
        }
        catch(e) {}
    }

    function confirm() {
        notify("Potwierdzono");
        setOpen(false);
    }
    
    function reject() {
        notify("Odrzucono");
        setOpen(false);
    }

    useEffect(() => {
        eventEmitter.on('showConfirmationDialog', onShowDialog);
        eventEmitter.on('confirmProcedure', confirm);
        eventEmitter.on('rejectProcedure', reject);
        return () => {
            eventEmitter.off('showConfirmationDialog', onShowDialog);
            eventEmitter.off('confirmProcedure', confirm);
            eventEmitter.off('rejectProcedure', reject);
        }
    }, []);

    return <div style={{display: open ? 'flex' : 'none'}} className="dialog-container">
        <div className="dialog">
            <label>{content}</label>
            <hr />
            <div>
                <span>Tak / Nie</span>
            </div>
        </div>
    </div>;
}