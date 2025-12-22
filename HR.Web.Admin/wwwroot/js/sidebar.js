window.sidebar = {
  toggle: function (menu) {
    const container = document.getElementById("submenu-" + menu);
    const chevron = document.getElementById("chevron-" + menu);

    if (!container) return;

    if (container.classList.contains("expanded")) {
      container.classList.remove("expanded");
      if (chevron) chevron.classList.remove("open");
    } else {
      container.classList.add("expanded");
      if (chevron) chevron.classList.add("open");
    }
  }
};
