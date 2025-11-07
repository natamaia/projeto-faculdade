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
    
    // Core user data from the form
    const username = document.getElementById('username').value.trim();
    const apelido = document.getElementById('apelido').value.trim(); // This is the "Apelido"
    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value;

    // --- Client-side validations ---
    if (!apelido || !email || !password) {
      showToast('Apelido, email e senha são obrigatórios.', 'error');
      return;
    }
    if (!/^\S+@\S+\.\S+$/.test(email)) {
      showToast('Email inválido.', 'error');
      return;
    }
    if (password.length < 6) {
      showToast('A senha deve ter pelo menos 6 caracteres.', 'error');
      return;
    }

    try {
      // --- Step 1: Register the core user ---
      const authResponse = await fetch('/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, apelido, email, password })
      });

      if (!authResponse.ok) {
        const errorText = await authResponse.text();
        showToast(`Falha no cadastro: ${errorText}`, 'error');
        return;
      }

      const createdUser = await authResponse.json();

      // --- Step 2: Register the profile (Client or Vendor) ---
      let profileResponse;
      const profileData = {
        userId: createdUser.id, // Assuming the ID of the created user is returned
        username: username,
        email: email,
        phoneNumber: parseInt(document.getElementById('phone').value || '0')
      };

      if (type === 'client') {
        Object.assign(profileData, {
          cpf: parseInt(document.getElementById('cpf').value || '0'),
          address: document.getElementById('address').value,
          age: parseInt(document.getElementById('age').value || '0'),
          // fullname can be added here if needed by the backend
        });
        profileResponse = await fetch('/api/clients', { method: 'POST', headers: {'Content-Type':'application/json'}, body: JSON.stringify(profileData) });
      
      } else { // vendor
        Object.assign(profileData, {
          cnpj: parseInt(document.getElementById('cnpj').value || '0'),
          empresa: document.getElementById('empresa').value,
          // cep_vendor can be added here if needed
        });
        profileResponse = await fetch('/api/vendors', { method: 'POST', headers: {'Content-Type':'application/json'}, body: JSON.stringify(profileData) });
      }

      if (!profileResponse.ok) {
        const errorText = await profileResponse.text();
        // NOTE: At this point, the user exists but profile creation failed.
        // A real app would need a way to handle this inconsistency.
        showToast(`Usuário criado, mas falha ao criar perfil: ${errorText}`, 'error');
        return;
      }

      showToast('Cadastro realizado com sucesso! Você será redirecionado.', 'success');
      
      setTimeout(() => {
        window.location.href = '/layouts/Pages/login.html';
      }, 2000);

    } catch (err) {
      console.error(err);
      showToast('Erro inesperado ao cadastrar. Verifique o console.', 'error');
    }
  });
});
