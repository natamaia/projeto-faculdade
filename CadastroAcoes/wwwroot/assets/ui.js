/* Small toast utility using Tailwind classes. Exposes showToast(message, type, duration) globally */
(function () {
  function ensureContainer() {
    let c = document.getElementById('toast-container');
    if (!c) {
      c = document.createElement('div');
      c.id = 'toast-container';
      c.className = 'fixed top-4 right-4 space-y-2 z-50';
      document.body.appendChild(c);
    }
    return c;
  }

  function createToast(message, type, duration) {
    const colors = {
      info: 'bg-blue-600',
      success: 'bg-green-600',
      error: 'bg-red-600',
      warn: 'bg-yellow-500'
    };

    const el = document.createElement('div');
    el.className = `max-w-sm text-white px-4 py-2 rounded shadow ${colors[type] || colors.info} opacity-0 transform translate-y-2 transition-all`; 
    el.innerHTML = `<div class="flex items-center"><div class="flex-1">${message}</div><button aria-label="close" class="ml-3 font-bold">&times;</button></div>`;

    const container = ensureContainer();
    container.appendChild(el);

    // animate in
    requestAnimationFrame(() => {
      el.classList.remove('opacity-0');
      el.classList.remove('translate-y-2');
      el.classList.add('opacity-100');
      el.classList.add('translate-y-0');
    });

    const close = () => {
      el.style.transition = 'opacity .18s, transform .18s';
      el.classList.add('opacity-0');
      el.classList.add('translate-y-2');
      setTimeout(() => el.remove(), 200);
    };

    el.querySelector('button').addEventListener('click', close);
    if (duration > 0) setTimeout(close, duration || 4000);
  }

  window.showToast = function (message, type = 'info', duration = 4000) {
    try {
      createToast(message, type, duration);
    } catch (e) {
      // fallback to alert if DOM fails
      try { alert(message); } catch (e) { /* ignore */ }
    }
  };
})();
