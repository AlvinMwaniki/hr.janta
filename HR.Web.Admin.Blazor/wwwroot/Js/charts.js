// wwwroot/js/charts.js
// Helper to get colors based on theme
const getChartTheme = () => {
  const isDark = document.body.classList.contains('dark');
  return {
    primary: isDark ? '#030391' : '#222240', // High contrast blue for Dark Mode
    text: isDark ? '#FCFCFC' : '#1e293b',
    subText: isDark ? 'rgba(255,255,255,0.6)' : '#64748b'
  };
};
// Keep the helper function
const getThemeColor = (variableName) => {
  const root = document.documentElement;
  const body = document.body;

  // Check if dark mode is active on body or html
  const isDark = body.classList.contains('dark') || root.getAttribute('data-bs-theme') === 'dark';

  // Get the actual computed color
  let color = getComputedStyle(body).getPropertyValue(variableName).trim();

  // Hard fallback: If color is empty or we are in dark mode, force white/light gray
  if (!color || color === "") {
    return isDark ? '#FCFCFC' : '#212529';
  }
  return color;
};

// Define as a standard object so your existing code doesn't break

// Global variable to store the chart instance for proper destruction/re-rendering
window.advanceChartInstance = null;
window.leaveChartInstance = null;
// Function for a modern, rounded doughnut look
const centerTextPlugin = {
  id: 'centerText',
  afterDraw: (chart) => {
    const { ctx, chartArea: { left, top, right, bottom } } = chart;
    const data = chart.data.datasets[0].data;
    const pendingValue = data[1] || 0;
    const total = data.reduce((a, b) => a + b, 0);

    // DYNAMIC COLOR FETCH
    const mainColor = getThemeColor('--text-main');
    const subColor = document.body.classList.contains('dark') ? 'rgba(255,255,255,0.6)' : '#64748b';

    ctx.save();
    const centerX = (left + right) / 2;
    const centerY = (top + bottom) / 2;
    ctx.textAlign = 'center';

    // 1. Main Number - Now flips to white in dark mode
    ctx.font = 'bold 3.2rem sans-serif';
    ctx.fillStyle = mainColor;
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(pendingValue, centerX, centerY );

    // 2. Sub-Label - Now readable in dark mode
    ctx.font = '500 0.65rem sans-serif';
    ctx.fillStyle = subColor;
    ctx.fillText(`Total: ${total}`, centerX, centerY + 50);
    ctx.restore();

    // 3. Status Label - Now readable in dark mode
    ctx.font = '500 0.75rem sans-serif';
    ctx.fillStyle = subColor;
    ctx.fillText("Pending", centerX, centerY + 22); // The word "Pending" below

    ctx.restore();
  }
};
const getModernOptions = () => ({
  responsive: true,
  maintainAspectRatio: false,
  cutout: '70%', // Even thinner for that 2026 look
  plugins: {
    legend: {
      position: 'bottom',
      labels: { color: getThemeColor('--text-main'), usePointStyle: true, padding: 25, font: { size: 12, weight: '600' } }
    }
  }
});

window.renderAdvanceChart = function (approved, pendingOrRejected) {
  const ctx = document.getElementById('AdvanceChartData');
  if (!ctx) return;
  // HIGH CONTRAST COLOR LOGIC

  const theme = getChartTheme(); // Fetch colors right before rendering

  if (window.advanceChartInstance) window.advanceChartInstance.destroy();

  window.advanceChartInstance = new Chart(ctx, {
    type: 'doughnut',
    plugins: [centerTextPlugin], // Apply the plugin here
    data: {
      labels: ['Approved', 'Pending / Rejected'],
      datasets: [{
        data: [approved, pendingOrRejected],
        backgroundColor: ['#222240', '#0EA5E9'],
        hoverOffset: 20,
        borderWidth: 0,
        borderRadius: 1
      }]
    },
    options: getModernOptions()
  });
};

window.renderLeaveStatusChart = function (approved, pending, rejected) {
  const ctx = document.getElementById('LeaveStatusChartData');
  if (!ctx) return;
  const theme = getChartTheme(); // Fetch colors right before rendering
  if (window.leaveChartInstance) window.leaveChartInstance.destroy();

  window.leaveChartInstance = new Chart(ctx, {
    type: 'doughnut',
    plugins: [centerTextPlugin], // Apply the plugin here
    data: {
      labels: ['Approved', 'Pending', 'Rejected'],
      datasets: [{
        data: [approved, pending, rejected],
        backgroundColor: ['#0EA5E9', '#222240', '#7863FF'],
        hoverOffset: 20,
        borderWidth: 0,
        borderRadius: 1
      }]
    },
    options: getModernOptions()
  });
};