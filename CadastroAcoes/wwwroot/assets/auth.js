// small helper to decode JWT payload (no validation) and extract vendor id from claims
function parseJwt(token) {
  if (!token) return null;
  const parts = token.split('.');
  if (parts.length < 2) return null;
  try {
    const payload = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    const json = decodeURIComponent(atob(payload).split('').map(function(c) { return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2); }).join(''));
    return JSON.parse(json);
  } catch (e) { return null; }
}

window.getVendorIdFromToken = function(){
  try {
    const token = localStorage.getItem('token');
    if (!token) return localStorage.getItem('vendor-id') || localStorage.getItem('username') || '';
    const p = parseJwt(token);
    if (!p) return localStorage.getItem('vendor-id') || localStorage.getItem('username') || '';
    // common claim names: nameid (http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier) or sub
    return p.nameid || p.name || p.sub || localStorage.getItem('vendor-id') || localStorage.getItem('username') || '';
  } catch (e) { return localStorage.getItem('vendor-id') || localStorage.getItem('username') || ''; }
}
