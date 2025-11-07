document.addEventListener('DOMContentLoaded', () => {
  const typeSelect = document.getElementById('typeSelect');
  const registerForm = document.getElementById('registerForm');

  function updateFields() {
    const type = typeSelect.value;
    document.querySelectorAll('.client-field').forEach(el => el.classList.toggle('hidden', type !== 'client'));
    document.querySelectorAll('.vendor-field').forEach(el => el.classList.toggle('hidden', type !== 'vendor'));
  }

  typeSelect.addEventListener('change', updateFields);
  updateFields();

  registerForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const type = typeSelect.value;
    const username = document.getElementById('username').value.trim();
    const email = document.getElementById('email').value.trim();
    const phone = parseInt(document.getElementById('phone').value || '0');
    const password = document.getElementById('password').value;

    // client-side validations
    if (!username || !email || !password) {
      if (window.showToast) showToast('Preencha os campos obrigatórios', 'error'); else alert('Preencha os campos obrigatórios');
      return;
    }
    if (!/^\S+@\S+\.\S+$/.test(email)) {
      if (window.showToast) showToast('Email inválido', 'error'); else alert('Email inválido');
      return;
    }
    if (password.length < 6) {
      if (window.showToast) showToast('Senha deve ter ao menos 6 caracteres', 'error'); else alert('Senha curta');
      return;
    }
    // check duplicate username locally
    const existingUsers = JSON.parse(localStorage.getItem('users') || '[]');
    if (existingUsers.some(u => u.username === username)) {
      if (window.showToast) showToast('Nome de usuário já existe. Escolha outro.', 'error'); else alert('Usuário já existe');
      return;
    }

    try {
      // try backend if available
      let usedBackend = false;
      if (type === 'client') {
        const cfp = parseInt(document.getElementById('cpf').value || '0');
        const address = document.getElementById('address').value;
        const age = parseInt(document.getElementById('age').value || '0');

        const body = { username, cfp, email, address, phoneNumber: phone, age, passwordHash: password };
        try {
          const res = await fetch('/api/clients/register', { method: 'POST', headers: {'Content-Type':'application/json'}, body: JSON.stringify(body) });
          if (res.ok) usedBackend = true;
          else { const txt = await res.text(); console.warn('Backend returned', txt || res.statusText); }
        } catch (e) { /* ignore backend errors, fallback to local */ }
      } else {
        const cnpj = parseInt(document.getElementById('cnpj').value || '0');
        const empresa = document.getElementById('empresa').value;
        try {
          const res = await fetch('/api/vendors/register', { method: 'POST', headers: {'Content-Type':'application/json'}, body: JSON.stringify({ username, cnpj, email, phoneNumber: phone, empresa, passwordHash: password }) });
          if (res.ok) usedBackend = true;
          else { const txt = await res.text(); console.warn('Backend returned', txt || res.statusText); }
        } catch (e) { /* ignore backend errors, fallback to local */ }
      }

      if (!usedBackend) {
        // fallback: save user locally (simple prototype)
        const users = JSON.parse(localStorage.getItem('users') || '[]');
        users.push({ username, password });
        localStorage.setItem('users', JSON.stringify(users));
        if (window.showToast) showToast('Cadastro gravado localmente. Você pode fazer login agora.', 'success'); else alert('Cadastro gravado localmente. Você pode fazer login agora.');
      } else {
        if (window.showToast) showToast('Cadastro realizado com sucesso (backend). Você pode fazer login agora.', 'success'); else alert('Cadastro realizado com sucesso (backend). Você pode fazer login agora.');
      }

      window.location.href = '/layouts/Pages/login.html';
    } catch (err) {
      console.error(err);
      alert('Erro ao cadastrar. Veja o console.');
    }
  });
});
