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

    if (!username || !email || !password) { alert('Preencha os campos obrigatórios'); return; }

    try {
      if (type === 'client') {
        const cfp = parseInt(document.getElementById('cpf').value || '0');
        const address = document.getElementById('address').value;
        const cep = parseInt(document.getElementById('cep_vendor').value || '0');
        const age = parseInt(document.getElementById('age').value || '0');

        const body = { username, cfp, email, address, phoneNumber: phone, cep, passwordHash: password };
        const res = await fetch('/api/clients/register', { method: 'POST', headers: {'Content-Type':'application/json'}, body: JSON.stringify(body) });
        if (!res.ok) { const txt = await res.text(); alert('Erro: ' + (txt || res.statusText)); return; }
      } else {
        const cnpj = parseInt(document.getElementById('cnpj').value || '0');
        const empresa = document.getElementById('empresa').value;
        const res = await fetch('/api/vendors/register', { method: 'POST', headers: {'Content-Type':'application/json'}, body: JSON.stringify({ username, cnpj, email, phoneNumber: phone, empresa, passwordHash: password }) });
        if (!res.ok) { const txt = await res.text(); alert('Erro: ' + (txt || res.statusText)); return; }
      }

      alert('Cadastro realizado com sucesso. Você pode fazer login agora.');
      window.location.href = '/layouts/Pages/login.html';
    } catch (err) {
      console.error(err);
      alert('Erro ao cadastrar. Veja o console.');
    }
  });
});
