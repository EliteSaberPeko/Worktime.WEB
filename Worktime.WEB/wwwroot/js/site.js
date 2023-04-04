"use strict";

async function SendRow(elem) {
    let form = elem.closest('form');
    let response = await fetch(form.action, {
        method: 'post',
        body: new FormData(form)
    });
    let text = await response.text();
    let errorElem = document.getElementById('ErrorSpan');
    if (response.status === 206) {
        errorElem.textContent = '';
        form.innerHTML = text;
    }
    else if (response.status === 500) {
        document.getElementById('ErrorSpan').textContent = text;
    }
    else {
        location.reload();
    }
}

function debounce(func, ms, now) {
    let onLast;
    return () => {
        const context = this;
        const args = arguments;
        const onFirst = now && !onLast;
        clearTimeout(onLast);
        onLast = setTimeout(() => {
            onLast = null;
            if (!now) func.apply(context, args);
        }, ms);
        if (onFirst) func.apply(context, args);
    };
}

function SendDate() {
    let elem = document.getElementById('ViewDate');
    let date = elem.value;
    location.href = '?ViewDate=' + date;
}