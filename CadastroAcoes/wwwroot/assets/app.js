document.addEventListener('DOMContentLoaded', () => {
  const toggleBtn = document.getElementById('toggleBtn');
  const sidebar = document.getElementById('sidebar');
  const navUser = document.getElementById('navUser');
  const pageUser = document.getElementById('pageUser');
  const pageHome = document.getElementById('pageHome');
  const logoutBtn = document.getElementById('logoutBtn');
  const userLabel = document.getElementById('userLabel');

  // show username if present
  const username = localStorage.getItem('username') || '';
  userLabel.textContent = username ? `Olá, ${username}` : '';

  // require token to access
  const token = localStorage.getItem('token');
  if (!token) {
    // not authenticated - redirect to login
    window.location.href = '/layouts/Pages/login.html';
    return;
  }

  toggleBtn.addEventListener('click', () => {
    sidebar.classList.toggle('collapsed');
  });

  navUser.addEventListener('click', (e) => {
    e.preventDefault();
    pageHome.classList.add('hidden');
    pageUser.classList.remove('hidden');
  });

  logoutBtn.addEventListener('click', () => {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    window.location.href = '/layouts/index.html';
  });

  // user form handling (local demo only)
  const userForm = document.getElementById('userForm');
  if (userForm) {
    userForm.addEventListener('submit', (e) => {
      e.preventDefault();
      alert('Salvar configurações não implementado no backend neste demo.');
    });
  }
});
