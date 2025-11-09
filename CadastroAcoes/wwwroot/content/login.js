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

    async function authenticateUser(username, password) {
        try {
            const res = await fetch('/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ identifier: username, password: password })
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
            if (window.showToast) showToast('Erro ao tentar autenticar. Verifique a conexão com o servidor.', 'error'); else alert('Erro ao tentar autenticar. Verifique a conexão com o servidor.');
        }
    }
});