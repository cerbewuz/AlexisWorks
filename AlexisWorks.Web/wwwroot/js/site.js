// Auto-dismiss alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function () {
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            if (bsAlert) bsAlert.close();
        }, 5000);
    });
});

// Global Modal System
let globalModal;
document.addEventListener('DOMContentLoaded', function() {
    const modalEl = document.getElementById('globalModal');
    if (modalEl) globalModal = new bootstrap.Modal(modalEl);
});

function showGlobalModal(url, title, isLarge = false) {
    const modalDialog = document.querySelector('#globalModal .modal-dialog');
    if (isLarge) {
        modalDialog.classList.add('modal-lg');
    } else {
        modalDialog.classList.remove('modal-lg');
    }
    
    document.getElementById('globalModalLabel').innerText = title || 'Form';
    document.getElementById('globalModalBody').innerHTML = '<div class="text-center py-4"><div class="spinner-border text-primary" role="status"></div></div>';
    
    globalModal.show();
    
    fetch(url, {
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
        .then(response => response.text())
        .then(html => {
            document.getElementById('globalModalBody').innerHTML = html;
        })
        .catch(err => {
            console.error('Error loading global modal:', err);
            document.getElementById('globalModalBody').innerHTML = '<div class="alert alert-danger">Error loading content.</div>';
        });
}

// Global Modal AJAX Form Submission Interceptor
document.addEventListener('submit', function (e) {
    const modalBody = document.getElementById('globalModalBody');
    // Check if the submit event originated from a form inside the global modal
    if (modalBody && modalBody.contains(e.target) && e.target.tagName === 'FORM') {
        e.preventDefault(); // Stop native full-page navigation

        const form = e.target;
        const submitBtn = form.querySelector('[type="submit"]');
        let originalBtnText = '';

        // Add loading state to button
        if (submitBtn) {
            originalBtnText = submitBtn.innerHTML;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Saving...';
            submitBtn.disabled = true;
        }

        fetch(form.action, {
            method: form.method || 'POST',
            body: new FormData(form),
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
        .then(response => {
            // Check if server returned a 302 Redirect indicating success
            if (response.redirected) {
                window.location.href = response.url;
                return null;
            }
            return response.text();
        })
        .then(html => {
            if (html) {
                // If it wasn't a redirect, place the returned HTML (with validation errors) back into the modal
                modalBody.innerHTML = html;
                
                // Re-initialize jQuery Unobtrusive Validation on the newly injected inputs
                if (typeof $ !== 'undefined' && typeof $.validator !== 'undefined' && typeof $.validator.unobtrusive !== 'undefined') {
                    $.validator.unobtrusive.parse(modalBody);
                }

                // Re-trigger any custom init scripts (like adding line items for bookings)
                if (typeof initLineItems === 'function') {
                    initLineItems();
                }
            }
        })
        .catch(err => {
            console.error('AJAX modal form submission error:', err);
            modalBody.innerHTML = '<div class="alert alert-danger">An unexpected error occurred during submission.</div>';
        });
    }
});
