/**
 * 显示提示消息
 * param {string} message - 要显示的消息内容
 * param {boolean} isError - 是否为错误消息（默认为false）
 * param {number} delay - 显示持续时间（毫秒，默认3000）
 */
function showToast(message, isError = false, delay = 3000) {
    const toastContainer = document.querySelector('.toast-container') || document.body;
    const toastId = 'toast-' + Date.now();

    const toastEl = document.createElement('div');
    toastEl.id = toastId;
    toastEl.className = 'toast';
    toastEl.role = 'alert';
    toastEl.setAttribute('aria-live', 'assertive');
    toastEl.setAttribute('aria-atomic', 'true');
    toastEl.style.position = 'fixed';
    toastEl.style.top = '20px';
    toastEl.style.right = '20px';
    toastEl.style.zIndex = '9999';

    const icon = isError ? 'mdi mdi-alert-circle text-danger' : 'mdi mdi-check-circle text-success';
    const iconHtml = `<span class="${icon} me-2" style="font-size: 1.2rem;"></span>`;

    const headerHtml = `
        <div class="toast-header">
            <strong class="me-auto">系统提示</strong>
            <small class="text-muted">${new Date().toLocaleTimeString()}</small>
            <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
    `;

    const bodyHtml = `
        <div class="toast-body d-flex align-items-center">
            ${iconHtml}
            <span>${message}</span>
        </div>
    `;

    toastEl.innerHTML = headerHtml + bodyHtml;
    toastContainer.appendChild(toastEl);

    const toast = new bootstrap.Toast(toastEl, {
        delay: delay
    });
    toast.show();

    setTimeout(() => {
        $(toastEl).remove();
    }, delay + 1000);
}
