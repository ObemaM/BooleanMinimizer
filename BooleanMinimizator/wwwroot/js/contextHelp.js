document.addEventListener("DOMContentLoaded", () => {
    let lastHoveredElement = null;
    let lastFocusedElement = null;

    // Сохраняем элемент, на который навели мышку
    document.body.addEventListener("mouseover", (e) => {
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

// Karnaugh map navigation
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

// Initialize first step
document.addEventListener('DOMContentLoaded', () => {
    if (steps.length > 0) {
        showStep(0);
    }
});

// Toggle keyboard visibility
const toggleKeyboardBtn = document.getElementById('toggleKeyboard');
const keyboard = document.getElementById('keyboard');

if (toggleKeyboardBtn && keyboard) {
    toggleKeyboardBtn.addEventListener('click', () => {
        const isVisible = keyboard.style.display !== 'none';
        keyboard.style.display = isVisible ? 'none' : 'grid';
        toggleKeyboardBtn.textContent = isVisible ? 'Показать клавиатуру' : 'Скрыть клавиатуру';
    });
}

// Virtual keyboard functionality
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

// Support button is now a direct link - no JavaScript needed
const supportText = document.getElementById('supportText');

// Context help tooltips
document.addEventListener('DOMContentLoaded', () => {
    const elements = document.querySelectorAll('[data-help]');
    
    elements.forEach(element => {
        const tooltip = document.createElement('div');
        tooltip.className = 'tooltip';
        tooltip.textContent = element.getAttribute('data-help');
        
        element.addEventListener('mouseenter', () => {
            document.body.appendChild(tooltip);
            const rect = element.getBoundingClientRect();
            tooltip.style.top = rect.bottom + window.scrollY + 5 + 'px';
            tooltip.style.left = rect.left + window.scrollX + 'px';
        });
        
        element.addEventListener('mouseleave', () => {
            if (tooltip.parentNode) {
                tooltip.parentNode.removeChild(tooltip);
            }
        });
    });
});

