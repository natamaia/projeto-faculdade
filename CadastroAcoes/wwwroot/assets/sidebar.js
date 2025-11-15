// Fetch and inject the sidebar partial, then wire dropdown and toggle behaviors
(async function(){
  async function insertSidebar() {
    try {
      const resp = await fetch('/layouts/Properties/sidebar.html');
      if (!resp.ok) return;
      const html = await resp.text();
      // find placeholder container(s)
      document.querySelectorAll('[data-inject-sidebar]').forEach(el => el.innerHTML = html);

      // wire dropdown
      document.querySelectorAll('#menuProducts').forEach(btn=>{
        const dropdown = btn.parentElement.querySelector('#productsDropdown');
        btn.addEventListener('click', e=>{ e.preventDefault(); dropdown.classList.toggle('hidden'); });
      });

      // wire mobile toggle if exists
      const toggleBtn = document.getElementById('toggleBtn');
      const sidebar = document.querySelector('[data-inject-sidebar] #sidebar');
      if (toggleBtn && sidebar) {
        toggleBtn.addEventListener('click', ()=> sidebar.classList.toggle('hidden'));
      }
    } catch (e) {
      console.warn('Failed to inject sidebar', e);
    }
  }

  if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', insertSidebar); else insertSidebar();
})();
