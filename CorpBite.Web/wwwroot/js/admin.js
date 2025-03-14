// Current timestamp: 2025-03-14 09:25:44
// Current user: sendtoaswinak

document.addEventListener('DOMContentLoaded', function () {
    // Initialize tooltips and popovers
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Sidebar toggle functionality
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });

    // Handle responsive sidebar
    $(window).on('resize', function () {
        if ($(window).width() <= 768) {
            $('#sidebar').addClass('active');
        }
    });

    // Initialize datepickers if present
    if ($.fn.datepicker) {
        $('.datepicker').datepicker({
            format: 'yyyy-mm-dd',
            autoclose: true,
            todayHighlight: true
        });
    }

    // Global ajax setup
    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        },
        beforeSend: function () {
            showLoader();
        },
        complete: function () {
            hideLoader();
        }
    });
});

// Utility functions
const adminUtils = {
    // Format currency
    formatCurrency: function (amount) {
        return new Intl.NumberFormat('en-IN', {
            style: 'currency',
            currency: 'INR'
        }).format(amount);
    },

    // Format date
    formatDate: function (dateString) {
        return new Date(dateString).toLocaleDateString('en-IN', {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    },

    // Format number with commas
    formatNumber: function (number) {
        return new Intl.NumberFormat('en-IN').format(number);
    }
};

// Loading spinner functions
function showLoader() {
    if (!document.querySelector('.loading-spinner')) {
        const spinner = document.createElement('div');
        spinner.className = 'loading-spinner';
        spinner.innerHTML = `
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        `;
        document.body.appendChild(spinner);
    }
    document.querySelector('.loading-spinner').classList.add('show');
}

function hideLoader() {
    const spinner = document.querySelector('.loading-spinner');
    if (spinner) {
        spinner.classList.remove('show');
    }
}

// Alert functions
function showAlert(message, type = 'success') {
    const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;

    const alertContainer = document.createElement('div');
    alertContainer.innerHTML = alertHtml;
    document.querySelector('main').insertBefore(
        alertContainer.firstChild,
        document.querySelector('main').firstChild
    );

    setTimeout(() => {
        const alert = document.querySelector('.alert');
        if (alert) {
            alert.remove();
        }
    }, 5000);
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

// Handle image preview
function previewImage(input, previewId) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById(previewId).src = e.target.result;
        };
        reader.readAsDataURL(input.files[0]);
    }
}

// Export functions
function exportToExcel(tableId, fileName) {
    let table = document.getElementById(tableId);
    let html = table.outerHTML;
    let url = 'data:application/vnd.ms-excel,' + encodeURIComponent(html);
    let downloadLink = document.createElement("a");
    document.body.appendChild(downloadLink);
    downloadLink.href = url;
    downloadLink.download = fileName + '.xls';
    downloadLink.click();
    document.body.removeChild(downloadLink);
}

// Chart utilities
const chartUtils = {
    // Common chart colors
    colors: {
        primary: '#0d6efd',
        success: '#198754',
        warning: '#ffc107',
        danger: '#dc3545',
        info: '#0dcaf0',
        secondary: '#6c757d'
    },

    // Common chart options
    defaultOptions: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                position: 'bottom'
            }
        }
    },

    // Create line chart
    createLineChart: function (ctx, data, options = {}) {
        return new Chart(ctx, {
            type: 'line',
            data: data,
            options: { ...this.defaultOptions, ...options }
        });
    },

    // Create bar chart
    createBarChart: function (ctx, data, options = {}) {
        return new Chart(ctx, {
            type: 'bar',
            data: data,
            options: { ...this.defaultOptions, ...options }
        });
    },

    // Create pie chart
    createPieChart: function (ctx, data, options = {}) {
        return new Chart(ctx, {
            type: 'pie',
            data: data,
            options: { ...this.defaultOptions, ...options }
        });
    }
};

// Date range picker initialization
function initDateRangePicker(elementId, callback) {
    const element = document.getElementById(elementId);
    if (!element) return;

    $(element).daterangepicker({
        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        },
        alwaysShowCalendars: true,
        startDate: moment().subtract(29, 'days'),
        endDate: moment(),
        locale: {
            format: 'YYYY-MM-DD'
        }
    }, callback);
}

// Table sorting
function sortTable(table, column, type = 'string') {
    const tbody = table.querySelector('tbody');
    const rows = Array.from(tbody.querySelectorAll('tr'));
    const direction = table.dataset.sortDirection === 'asc' ? -1 : 1;

    rows.sort((a, b) => {
        let aVal = a.cells[column].textContent.trim();
        let bVal = b.cells[column].textContent.trim();

        if (type === 'number') {
            aVal = parseFloat(aVal.replace(/[₹,]/g, ''));
            bVal = parseFloat(bVal.replace(/[₹,]/g, ''));
        } else if (type === 'date') {
            aVal = new Date(aVal);
            bVal = new Date(bVal);
        }

        if (aVal < bVal) return -1 * direction;
        if (aVal > bVal) return 1 * direction;
        return 0;
    });

    rows.forEach(row => tbody.appendChild(row));
    table.dataset.sortDirection = direction === 1 ? 'asc' : 'desc';
}

// Initialize admin dashboard
function initDashboard() {
    // Update real-time statistics
    setInterval(() => {
        $.get('/Admin/GetDashboardStats')
            .done(data => {
                Object.keys(data).forEach(key => {
                    const element = document.getElementById(key);
                    if (element) {
                        element.textContent = data[key];
                    }
                });
            });
    }, 60000); // Update every minute
}