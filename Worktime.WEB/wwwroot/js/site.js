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
    return function() {
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
    let date = this.value;
    location.href = '?' + new URLSearchParams({ ViewDate: date });
}

async function SendName() {
    let nameElement = this;
    let inputName = nameElement.value;
    let found = false;
    let description = nameElement.closest('form').querySelector('#Description');
    let listOptions = nameElement.nextElementSibling.options;
    for (let item of listOptions) {
        if (inputName === item.innerText) {
            description.value = item.dataset.description;
            found = true;
            break;
        }
    }
    if (inputName) {
        let response = await fetch('/Home/ShowTasks?' + new URLSearchParams({ name: inputName }));
        let data = await response.json();
        var dataArray = JSON.parse(data);
        nameElement.nextElementSibling.innerHTML = '';

        if (JSON.stringify(dataArray) == JSON.stringify({}))
            return;

        if (found && dataArray.length === 1)
            return;

        for (let key of dataArray) {
            var option = document.createElement('option');
            option.textContent = key.Name;
            option.setAttribute("data-description", key.Description);
            nameElement.nextElementSibling.appendChild(option);
        }
    }
}

function AddEventListenerByName(name, event, func) {
    let elements = document.getElementsByName(name);
    for (let elem of elements) {
        elem.addEventListener(event, func);
    }
}