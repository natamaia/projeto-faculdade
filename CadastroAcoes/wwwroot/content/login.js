document.addEventListener('DOMContentLoaded', function() {
    const loginForm = document.getElementById('loginForm');

    loginForm.addEventListener('submit', function(event) {
        event.preventDefault();

        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;

        if (validateForm(username, password)) {
            authenticateUser(username, password);
        } else {
            if (window.showToast) showToast('Preencha ambos os campos.', 'error'); else alert('Please fill in both fields.');
        }
    });

    function validateForm(username, password) {
        return username.trim() !== '' && password.trim() !== '';
    }

    function authenticateLocally(username, password) {
        // default admin as requested
        if (username === 'admin' && password === 'userAdminPs') {
            localStorage.setItem('token', 'local-admin');
            localStorage.setItem('username', username);
            window.location.href = '/app/home.html';
            return true;
        }

        // check locally registered users
        try {
            const users = JSON.parse(localStorage.getItem('users') || '[]');
            const found = users.find(u => u.username === username && u.password === password);
            if (found) {
                localStorage.setItem('token', 'local-' + username);
                localStorage.setItem('username', username);
                window.location.href = '/app/home.html';
                return true;
            }
        } catch (e) {
            console.warn('Erro lendo usuários locais', e);
        }

        return false;
    }

    async function authenticateUser(username, password) {
        // try local first (simple offline dev flow)
        if (authenticateLocally(username, password)) return;

        // then try API if available; otherwise fallback to failure
        try {
            const res = await fetch('/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username: username, passwordHash: password })
            });

            if (!res.ok) {
                const txt = await res.text();
                if (window.showToast) showToast('Falha no login: ' + (txt || res.statusText), 'error'); else alert('Falha no login: ' + (txt || res.statusText));
                return;
            }

            const data = await res.json();
            if (data && data.token) {
                localStorage.setItem('token', data.token);
                localStorage.setItem('username', username);
                window.location.href = '/app/home.html';
            } else {
                if (window.showToast) showToast('Resposta inesperada do servidor.', 'error'); else alert('Resposta inesperada do servidor.');
            }
        } catch (err) {
            console.error('Erro ao chamar API de login', err);
            if (window.showToast) showToast('Usuário não encontrado localmente e servidor indisponível. Verifique credenciais.', 'error'); else alert('Usuário não encontrado localmente e servidor indisponível. Verifique credenciais.');
        }
    }
});