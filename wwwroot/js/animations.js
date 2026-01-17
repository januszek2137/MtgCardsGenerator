if (typeof AOS !== 'undefined') {
  AOS.init({
    duration: 400,
    easing: 'ease-out',
    once: true,
    offset: 20,
    delay: 0,
    disable: false
  });
}

function refreshAnimations() {
  if (typeof AOS !== 'undefined') {
    AOS.refresh();
  }
}

window.refreshAnimations = refreshAnimations;
