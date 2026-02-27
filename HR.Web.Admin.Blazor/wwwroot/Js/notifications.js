// wwwroot/js/notifications.js
window.playNotificationSound = function () {
  console.log("System: Notification Triggered");

  const audioUrl = 'https://assets.mixkit.co/active_storage/sfx/2869/2869-preview.mp3';
  const audio = new Audio(audioUrl);

  // Set properties
  audio.volume = 0.8; // High but not 100% to avoid distortion
  audio.preload = 'auto';

  // Playback logic
  audio.play().then(() => {
    console.log("System: Notification sound played successfully.");
  }).catch(err => {
    console.error("System: Sound failed. Reason: ", err.name);

    // If it failed because of "NotAllowedError", the user hasn't clicked anything yet.
    if (err.name === "NotAllowedError") {
      console.warn("ACTION REQUIRED: You must click anywhere on the page once to enable notification sounds.");
    }
  });
};

window.showBootstrapToast = function (elementId) {
  const toastElement = document.getElementById(elementId);
  if (toastElement) {
    // Bootstrap's internal toast trigger
    const toast = new bootstrap.Toast(toastElement);
    toast.show();
  }
};