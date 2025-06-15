let tooltipsActive = true;
let activeTooltip = null;

function initializeTooltips() {
    const elements = document.querySelectorAll('[data-help]');
    
    elements.forEach(element => {
        element.removeEventListener('mouseenter', handleMouseEnter);
        element.removeEventListener('mouseleave', handleMouseLeave);

        element.addEventListener('mouseenter', handleMouseEnter);
        element.addEventListener('mouseleave', handleMouseLeave);
    });
}

function handleMouseEnter(event) {
    const inputField = document.getElementById('inputField');
    if (inputField && inputField.value.length > 0) {
        return;
    }

    const element = event.currentTarget;
    const helpText = element.getAttribute('data-help');
    
    if (helpText) {
        if (activeTooltip && activeTooltip.parentNode) {
            activeTooltip.parentNode.removeChild(activeTooltip);
        }

        const tooltip = document.createElement('div');
        tooltip.className = 'tooltip';
        tooltip.textContent = helpText;
        document.body.appendChild(tooltip);
        activeTooltip = tooltip;

        const rect = element.getBoundingClientRect();
        tooltip.style.top = rect.bottom + window.scrollY + 5 + 'px';
        tooltip.style.left = rect.left + window.scrollX + 'px';
    }
}

function handleMouseLeave() {
    if (activeTooltip && activeTooltip.parentNode) {
        activeTooltip.parentNode.removeChild(activeTooltip);
        activeTooltip = null;
    }
}

function deactivateAllTooltips() {
    tooltipsActive = false;
    if (activeTooltip && activeTooltip.parentNode) {
        activeTooltip.parentNode.removeChild(activeTooltip);
        activeTooltip = null;
    }
}

document.addEventListener("DOMContentLoaded", () => {
    let lastHoveredElement = null;
    let lastFocusedElement = null;

    // Сохраняем элемент, на который навели мышку
    document.body.addEventListener("mouseover", (e) => {
        const inputField = document.getElementById('inputField');
        if (inputField && inputField.value.length > 0) {
            lastHoveredElement = null;
            return;
        }
        if (e.target.dataset.help) {
            lastHoveredElement = e.target;
        }
    });

    // Убираем, когда мышка уходит
    document.body.addEventListener("mouseout", (e) => {
        if (lastHoveredElement === e.target) {
            lastHoveredElement = null;
        }
    });

    // Сохраняем элемент, на котором фокус (например, input)
    document.body.addEventListener("focusin", (e) => {
        const inputField = document.getElementById('inputField');
        if (inputField && inputField.value.length > 0) {
            lastFocusedElement = null;
            return;
        }
        if (e.target.dataset.help) {
            lastFocusedElement = e.target;
        }
    });

    // Убираем фокусЙФ
    document.body.addEventListener("focusout", (e) => {
        if (lastFocusedElement === e.target) {
            lastFocusedElement = null;
        }
    });

    // Обработка нажатия F1
    document.addEventListener("keydown", (e) => {
        if (e.key === "F1") {
            e.preventDefault();

            let helpText = "";

            // Сначала проверим фокус, потом ховер
            if (lastFocusedElement?.dataset.help) {
                helpText = lastFocusedElement.dataset.help;
            } else if (lastHoveredElement?.dataset.help) {
                helpText = lastHoveredElement.dataset.help;
            }

            // Показать справку
            if (helpText) {
                alert("Справка: " + helpText);
            }
        }
    });

    initializeTooltips();
});

document.addEventListener("DOMContentLoaded", () => {
    const helpButton = document.getElementById("helpButton");

    if (helpButton) {
        helpButton.addEventListener("click", () => {
            // Открываем справочный документ в новой вкладке
            window.open("/UserHelp/vvedenie.htm", "_blank");
        });
    }
});

let currentStep = 0;
const steps = document.querySelectorAll('.karnaugh-step');

function showStep(step) {
    steps.forEach((s, i) => s.style.display = i === step ? 'block' : 'none');
    document.getElementById('currentStep').textContent = step + 1;
}

function nextStep() {
    if (currentStep < steps.length - 1) {
        currentStep++;
        showStep(currentStep);
    }
}

function prevStep() {
    if (currentStep > 0) {
        currentStep--;
        showStep(currentStep);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    if (steps.length > 0) {
        showStep(0);
    }
});

const toggleKeyboardBtn = document.getElementById('toggleKeyboard');
const keyboard = document.getElementById('keyboard');

if (toggleKeyboardBtn && keyboard) {
    toggleKeyboardBtn.addEventListener('click', () => {
        const isVisible = keyboard.style.display !== 'none';
        keyboard.style.display = isVisible ? 'none' : 'grid';
        toggleKeyboardBtn.textContent = isVisible ? 'Показать клавиатуру' : 'Скрыть клавиатуру';
    });
}

const inputField = document.getElementById('inputField');
const keys = document.querySelectorAll('.key');

keys.forEach(key => {
    key.addEventListener('click', () => {
        if (inputField) {
            const symbol = key.textContent;
            const start = inputField.selectionStart;
            const end = inputField.selectionEnd;
            const value = inputField.value;
            
            inputField.value = value.substring(0, start) + symbol + value.substring(end);
            inputField.focus();
            inputField.setSelectionRange(start + symbol.length, start + symbol.length);
        }
    });
});

const supportText = document.getElementById('supportText');

