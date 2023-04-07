"use strict";

async function SendRow(elem) {
    let form = elem.form;
    let errorElements = elem.closest('tr').querySelectorAll('.error_span');
    let span500error = document.querySelector('.error_500');
    for (let el of errorElements) {
        el.textContent = '';
    }
    span500error.textContent = '';
    let response = await fetch(form.action, {
        method: 'post',
        body: new FormData(form)
    });
    if (response.status === 206) {
        let text = await response.json();
        var dataArray = JSON.parse(text);
        if (JSON.stringify(dataArray) == JSON.stringify({}))
            return;

        for (let key of dataArray) {
            let span = elem.closest('tr').querySelector(`.error_${key.Name}`);
            span.textContent = key.Message;
        }
    }
    else if (response.status === 500) {
        let text = await response.text();
        span500error.textContent = text;
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
    let description = nameElement.closest('tr').querySelector('#Description');
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

async function DeleteRow(elem) {
    let isConfirm = confirm('Вы действительно хотите удалить строку?');
    if (!isConfirm)
        return;

    let form = elem.form;
    let response = await fetch('/home/delete', {
        method: 'post',
        body: new FormData(form)
    });
    if (response.status === 500) {
        let text = await response.text();
        document.querySelector('.error_500').textContent = text;
    }
    else {
        location.reload();
    }
}