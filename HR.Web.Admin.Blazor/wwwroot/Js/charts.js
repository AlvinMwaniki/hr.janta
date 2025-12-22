// wwwroot/js/charts.js

// Global variable to store the chart instance for proper destruction/re-rendering
window.advanceChartInstance = null;

window.renderAdvanceChart = function (approved, pendingOrRejected) {

  // 1. Get the Canvas Element using the ID defined in AdvanceChartCard.razor
  const ctx = document.getElementById('AdvanceChartData');

  // Safety check
  if (!ctx) {
    console.error("Advance Chart: Canvas element with ID 'AdvanceChartData' not found.");
    return;
  }

  // 2. Destroy any existing chart instance to prevent errors (crucial in Blazor)
  if (window.advanceChartInstance) {
    window.advanceChartInstance.destroy();
  }

  // 3. Create the new chart instance
  window.advanceChartInstance = new Chart(ctx, {
    type: 'doughnut',
    data: {
      labels: ['Approved', 'Pending / Rejected'],
      datasets: [{
        data: [approved, pendingOrRejected],
        backgroundColor: [
          '#4BC0C0', // Teal/Green for Approved
          '#FF6384'  // Red for Pending/Rejected
        ],
        hoverOffset: 8
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom',
        },
        title: {
          display: true,
          text: 'Advance Approval Status'
        }
      }
    }
  });
};
//===============LEAVE CHAAART=================================
// Global variable for the leave chart
window.leaveChartInstance = null;

window.renderLeaveStatusChart = function (approved, pending, rejected) {

  const ctx = document.getElementById('LeaveStatusChartData');

  if (!ctx) {
    console.error("Leave Status Chart: Canvas element not found.");
    return;
  }

  if (window.leaveChartInstance) {
    window.leaveChartInstance.destroy();
  }

  window.leaveChartInstance = new Chart(ctx, {
    type: 'doughnut',
    data: {
      labels: ['Approved', 'Pending', 'Rejected'],
      datasets: [{
        data: [approved, pending, rejected],
        backgroundColor: [
          '#4BC0C0', // Approved (Teal)
          '#FFCE56', // Pending (Yellow)
          '#FF6384'  // Rejected (Red)
        ],
        hoverOffset: 4
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { position: 'bottom' },
        title: { display: true, text: 'Leave Request Status' }
      }
    }
  });
};


