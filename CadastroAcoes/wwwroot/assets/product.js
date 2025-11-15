(function(){
  async function init(){
    const params = new URLSearchParams(location.search);
    const id = params.get('id');
    if (id) {
      // load product
      try{
        const res = await fetch('/api/products/' + encodeURIComponent(id));
        if (!res.ok) { document.getElementById('msg').textContent = 'Falha ao carregar produto.'; return; }
        const p = await res.json();
        document.getElementById('productId').value = p.id || p.Id || '';
        document.getElementById('name').value = p.name || p.Name || '';
        document.getElementById('description').value = p.description || p.Description || '';
        document.getElementById('price').value = p.price || p.Price || 0;
        document.getElementById('quantity').value = p.quantity || p.Quantity || 0;
      }catch(err){ console.error(err); document.getElementById('msg').textContent = 'Erro ao carregar produto.'; }
    }

    document.getElementById('productForm').addEventListener('submit', async function(e){
      e.preventDefault();
      const id = document.getElementById('productId').value;
      const payload = {
        name: document.getElementById('name').value.trim(),
        description: document.getElementById('description').value.trim(),
        price: parseFloat(document.getElementById('price').value) || 0,
        quantity: parseInt(document.getElementById('quantity').value) || 0,
        VendorId: (window.getVendorIdFromToken ? window.getVendorIdFromToken() : (localStorage.getItem('vendor-id') || localStorage.getItem('username') || 'dev-vendor'))
      };
      try{
        let res;
        if (id) {
          res = await fetch('/api/products/' + encodeURIComponent(id), { method: 'PUT', headers: {'Content-Type':'application/json'}, body: JSON.stringify(payload) });
        } else {
          res = await fetch('/api/products', { method: 'POST', headers: {'Content-Type':'application/json'}, body: JSON.stringify(payload) });
        }
        if (!res.ok) { const t = await res.text(); document.getElementById('msg').innerText = 'Falha: ' + (t||res.statusText); return; }
        document.getElementById('msg').innerText = 'Salvo com sucesso.';
        setTimeout(()=>location.href='/app/product-list.html',800);
      }catch(err){ console.error(err); document.getElementById('msg').innerText = 'Erro ao salvar.'; }
    });
  }
  init();
})();