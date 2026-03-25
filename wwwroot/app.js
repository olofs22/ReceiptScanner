const API = 'https://localhost:7133/api';

// ─── Auth helpers ───────────────────────────────────────────
function getToken() { return localStorage.getItem('token'); }
function setToken(t) { localStorage.setItem('token', t); }
function clearToken() { localStorage.removeItem('token'); }

async function apiFetch(path, options = {}) {
    const token = getToken();
    const res = await fetch(`${API}${path}`, {
        ...options,
        headers: {
            'Content-Type': 'application/json',
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
            ...options.headers
        }
    });
    if (!res.ok) throw new Error(await res.text());
    if (res.status === 204) return null;
    return res.json();
}

// ─── Navigation ─────────────────────────────────────────────
function showRegister() {
    document.getElementById('login-form').style.display = 'none';
    document.getElementById('register-form').style.display = 'block';
}

function showLogin() {
    document.getElementById('register-form').style.display = 'none';
    document.getElementById('login-form').style.display = 'block';
}

function showReceipts() {
    document.getElementById('auth-section').style.display = 'none';
    document.getElementById('receipts-section').style.display = 'block';
    loadReceipts();
}

function toggleForm() {
    const form = document.getElementById('receipt-form');
    form.style.display = form.style.display === 'none' ? 'block' : 'none';
}

// ─── Auth ────────────────────────────────────────────────────
async function login() {
    const emailAdress = document.getElementById('login-email').value;
    const password = document.getElementById('login-password').value;
    try {
        const data = await apiFetch('/auth/login', {
            method: 'POST',
            body: JSON.stringify({ emailAdress, password })
        });
        setToken(data.token);
        showReceipts();
    } catch {
        document.getElementById('login-error').textContent = 'Invalid email or password.';
    }
}

async function register() {
    const emailAdress = document.getElementById('register-email').value;
    const password = document.getElementById('register-password').value;
    try {
        await apiFetch('/auth/register', {
            method: 'POST',
            body: JSON.stringify({ emailAdress, password })
        });
        document.getElementById('register-message').textContent = 'Registered! You can now log in.';
        document.getElementById('register-message').style.color = 'green';
        setTimeout(showLogin, 1500);
    } catch {
        document.getElementById('register-message').textContent = 'Email already in use.';
        document.getElementById('register-message').style.color = 'red';
    }
}

function logout() {
    clearToken();
    document.getElementById('receipts-section').style.display = 'none';
    document.getElementById('auth-section').style.display = 'block';
}

// ─── Receipts ────────────────────────────────────────────────
async function loadReceipts() {
    const list = document.getElementById('receipts-list');
    list.innerHTML = 'Loading...';
    try {
        const receipts = await apiFetch('/receipt');
        list.innerHTML = '';
        if (receipts.length === 0) {
            list.innerHTML = '<p>No receipts yet.</p>';
            return;
        }
        receipts.forEach(r => {
            list.innerHTML += `
                <div class="receipt-card">
                    <h3>${r.storeName || 'Unknown store'} — ${new Date(r.date).toISOString().split('T')[0]}</h3>
                    <ul>
                        ${r.items.map(i => `<li>${i.title} x${i.quantity} — ${i.price} SEK</li>`).join('')}
                    </ul>
                    <button class="delete-btn" onclick="deleteReceipt(${r.id})">Delete</button>
                </div>`;
        });
    } catch {
        list.innerHTML = '<p class="error">Failed to load receipts.</p>';
    }
}

function addItemRow() {
    const container = document.getElementById('items-container');
    const row = document.createElement('div');
    row.className = 'item-row';
    row.innerHTML = `
        <input type="text" placeholder="Item title" class="item-title" />
        <input type="number" placeholder="Price" class="item-price" />
        <input type="number" placeholder="Qty" class="item-qty" value="1" />
    `;
    container.appendChild(row);
}

async function createReceipt() {
    const storeName = document.getElementById('store-name').value;
    const date = document.getElementById('receipt-date').value;
    const rows = document.querySelectorAll('.item-row');

    const items = Array.from(rows).map(row => ({
        title: row.querySelector('.item-title').value,
        price: parseFloat(row.querySelector('.item-price').value),
        quantity: parseInt(row.querySelector('.item-qty').value)
    }));

    if (!date || items.length === 0) {
        document.getElementById('form-error').textContent = 'Date and at least one item are required.';
        return;
    }

    try {
        await apiFetch('/receipt', {
            method: 'POST',
            body: JSON.stringify({ storeName, date, items })
        });
        document.getElementById('receipt-form').style.display = 'none';
        document.getElementById('store-name').value = '';
        document.getElementById('receipt-date').value = '';
        document.getElementById('items-container').innerHTML = `
            <div class="item-row">
                <input type="text" placeholder="Item title" class="item-title" />
                <input type="number" placeholder="Price" class="item-price" />
                <input type="number" placeholder="Qty" class="item-qty" value="1" />
            </div>`;
        loadReceipts();
    } catch {
        document.getElementById('form-error').textContent = 'Failed to create receipt.';
    }
}

async function deleteReceipt(id) {
    if (!confirm('Delete this receipt?')) return;
    try {
        await apiFetch(`/receipt/${id}`, { method: 'DELETE' });
        loadReceipts();
    } catch {
        alert('Failed to delete receipt.');
    }
}

// ─── Init ────────────────────────────────────────────────────
if (getToken()) showReceipts();