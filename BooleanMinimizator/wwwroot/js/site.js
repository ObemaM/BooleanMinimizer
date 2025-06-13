let toggleButton = document.getElementById("toggleKeyboard");
let keyboard = document.getElementById("keyboard");

// Проверяем состояние из localStorage при загрузке
if (localStorage.getItem("keyboardVisible") === "false") {
    keyboard.style.display = "none";
    toggleButton.innerText = "Показать клавиатуру";
} else {
    keyboard.style.display = "grid";
    toggleButton.innerText = "Скрыть клавиатуру";
}

toggleButton.addEventListener("click", function () {
    if (keyboard.style.display === "none") {
        keyboard.style.display = "grid";
        toggleButton.innerText = "Скрыть клавиатуру";
        localStorage.setItem("keyboardVisible", "true");
    } else {
        keyboard.style.display = "none";
        toggleButton.innerText = "Показать клавиатуру";
        localStorage.setItem("keyboardVisible", "false");
    }
});

// Логика ввода символов с клавиатуры
document.querySelectorAll(".key").forEach(button => {
    button.addEventListener("click", function () {
        const input = document.getElementById("inputField");
        const start = input.selectionStart;
        const end = input.selectionEnd;
        const text = this.innerText;
        const currentLength = input.value.length;
        const selectionLength = end - start;
        const newLength = currentLength - selectionLength + text.length;

        // Проверяем, не превысит ли ввод максимальную длину
        if (newLength > 64) {
            // Показываем визуальную обратную связь
            input.classList.add('input-error');
            setTimeout(() => input.classList.remove('input-error'), 500);
            return;
        }

        // Вставка текста в позицию курсора
        input.value = input.value.slice(0, start) + text + input.value.slice(end);

        // Перемещение курсора после вставленного текста
        const newPos = start + text.length;
        input.setSelectionRange(newPos, newPos);
        input.focus(); // Сохраняем фокус
    });
});

// Ограничение ввода с клавиатуры
document.getElementById("inputField").addEventListener("input", function(e) {
    if (this.value.length > 64) {
        this.value = this.value.slice(0, 64);
        // Показываем визуальную обратную связь
        this.classList.add('input-error');
        setTimeout(() => this.classList.remove('input-error'), 500);
    }
});

// Подсказка - теперь кнопка поддержки является прямой ссылкой HTML
