document.addEventListener('DOMContentLoaded', function() {
    const loginForm = document.getElementById('loginForm');

    loginForm.addEventListener('submit', function(event) {
        event.preventDefault();

        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;

        if (validateForm(username, password)) {
            authenticateUser(username, password);
        } else {
            alert('Please fill in both fields.');
        }
    });

    function validateForm(username, password) {
        return username.trim() !== '' && password.trim() !== '';
    }

    async function authenticateUser(username, password) {
        // Quick local admin shortcut: if both are 'admin' allow access (dev convenience)
        if (username === 'admin' && password === 'admin') {
            localStorage.setItem('token', 'local-admin');
            localStorage.setItem('role', 'admin');
            window.location.href = '/layouts/index.html';
            return;
        }

        try {
            const res = await fetch('/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username: username, passwordHash: password })
            });

            if (!res.ok) {
                const txt = await res.text();
                alert('Falha no login: ' + (txt || res.statusText));
                return;
            }

            const data = await res.json();
            if (data && data.token) {
                localStorage.setItem('token', data.token);
                // save username for simple client-side checks
                localStorage.setItem('username', username);
                // redirect to protected home app
                window.location.href = '/app/home.html';
            } else {
                alert('Resposta inesperada do servidor.');
            }
        } catch (err) {
            console.error('Erro ao chamar API de login', err);
            alert('Erro ao conectar ao servidor de autenticação.');
        }
    }
});