// Current timestamp: 2025-03-14 09:01:16
// Current user: sendtoaswinak

// Initialize tooltips and popovers
document.addEventListener('DOMContentLoaded', function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
});

// Cart functions
const cart = {
    addItem: function (menuItemId, quantity, instructions) {
        return $.post('/Cart/Add', {
            menuItemId: menuItemId,
            quantity: quantity,
            specialInstructions: instructions
        });
    },

    updateItem: function (menuItemId, quantity) {
        return $.post('/Cart/Update', {
            menuItemId: menuItemId,
            quantity: quantity
        });
    },

    removeItem: function (menuItemId) {
        return $.post('/Cart/Remove', {
            menuItemId: menuItemId
        });
    },

    refreshCart: function () {
        return $.get('/Cart/GetSummary');
    }
};

// Rating functions
function initializeRating(containerId) {
    const container = document.getElementById(containerId);
    const stars = container.getElementsByClassName('rating-star');
    const input = container.querySelector('input[type="hidden"]');

    Array.from(stars).forEach((star, index) => {
        star.addEventListener('click', () => {
            input.value = index + 1;
            updateStars(stars, index + 1);
        });

        star.addEventListener('mouseover', () => {
            updateStars(stars, index + 1);
        });

        star.addEventListener('mouseout', () => {
            updateStars(stars, input.value);
        });
    });
}

function updateStars(stars, rating) {
    Array.from(stars).forEach((star, index) => {
        star.classList.toggle('active', index < rating);
    });
}

// Form validation
function validateForm(formId) {
    const form = document.getElementById(formId);
    if (!form) return true;

    let isValid = true;
    const requiredInputs = form.querySelectorAll('[required]');

    requiredInputs.forEach(input => {
        if (!input.value.trim()) {
            isValid = false;
            input.classList.add('is-invalid');
        } else {
            input.classList.remove('is-invalid');
        }
    });

    return isValid;
}

// Alert messages
function showAlert(message, type = 'success') {
    const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;

    const alertContainer = document.createElement('div');
    alertContainer.innerHTML = alertHtml;
    document.querySelector('main').insertBefore(alertContainer, document.querySelector('main').firstChild);

    setTimeout(() => {
        alertContainer.remove();
    }, 5000);
}

// Handle favorite items
function toggleFavorite(menuItemId) {
    return $.post('/Menu/ToggleFavorite', { menuItemId })
        .done(response => {
            if (response.success) {
                const icon = document.querySelector(`.favorite-icon[data-id="${menuItemId}"]`);
                icon.classList.toggle('bi-heart');
                icon.classList.toggle('bi-heart-fill');
                icon.classList.toggle('text-danger');
            }
        });
}

// Format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('en-IN', {
        style: 'currency',
        currency: 'INR'
    }).format(amount);
}

// Format date
function formatDate(dateString) {
    return new Date(dateString).toLocaleDateString('en-IN', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}