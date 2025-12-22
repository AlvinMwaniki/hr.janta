window.renderLeaveChart = (onLeave, notOnLeave) => {
  const ctx = document.getElementById('leavePieChart').getContext('2d');
  if (window.leaveChart) window.leaveChart.destroy();

  window.leaveChart = new Chart(ctx, {
    type: 'pie',
    data: {
      labels: ['On Leave', 'Not On Leave'],
      datasets: [{
        label: 'Employees',
        data: [onLeave, notOnLeave],
        backgroundColor: ['#ffc107', '#0d6efd']
      }]
    },
    options: {
      responsive: true,
      plugins: {
        legend: { position: 'bottom' }
      }
    }
  });
};

window.updateLeaveChart = (onLeave, notOnLeave) => {
  if (window.leaveChart) {
    window.leaveChart.data.datasets[0].data = [onLeave, notOnLeave];
    window.leaveChart.update();
  }
};
