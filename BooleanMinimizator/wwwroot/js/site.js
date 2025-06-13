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

// Функция инициализации обработчиков событий
function initializeEventHandlers() {
    // Ограничение ввода с клавиатуры
    const inputField = document.getElementById("inputField");
    if (inputField) {
        inputField.addEventListener("input", function(e) {
            if (this.value.length > 64) {
                this.value = this.value.slice(0, 64);
                // Показываем визуальную обратную связь
                this.classList.add('input-error');
                setTimeout(() => this.classList.remove('input-error'), 500);
            }
        });
    }
}

// Инициализируем обработчики при загрузке страницы
document.addEventListener('DOMContentLoaded', initializeEventHandlers);

// Подсказка - теперь кнопка поддержки является прямой ссылкой HTML
