// vendor helper utilities used by vendor pages
function vendor_getId(){
  // prefer an explicit vendor id stored in localStorage, otherwise use username
  return localStorage.getItem('vendor-id') || localStorage.getItem('username') || 'dev-vendor';
}

// small utility to show toast messages if available
function vendor_toast(msg, type){
  if (window.showToast) window.showToast(msg, type);
  else console.log('toast', type, msg);
}
