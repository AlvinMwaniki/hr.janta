window.applyTheme = (mode) => {
  const activeMode = mode || 'light';
  const root = document.documentElement;

  if (mode === 'system') {
    // Check browser preference
    const isDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    root.setAttribute('data-bs-theme', isDark ? 'dark' : 'light');
    isDark ? document.body.classList.add('dark') : document.body.classList.remove('dark');
  } else {
    // Force the chosen mode
    root.setAttribute('data-bs-theme', mode);
    mode === 'dark' ? document.body.classList.add('dark') : document.body.classList.remove('dark');
  }
  
};
// This is the .NET 8/9 way to catch "Enhanced Navigations"
Blazor.addEventListener('enhancedload', () => {
  const saved = localStorage.getItem('theme') || 'light';
  window.applyTheme(saved);
});