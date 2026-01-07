// wwwroot/js/imageManager.js
let dotNetHelper;

// This is called once when the page loads
window.setImageHelper = (reference) => {
  dotNetHelper = reference;
};

window.processProfileImage = async (input) => {
  const file = input.files[0];
  if (!file) return;

  const reader = new FileReader();
  reader.onload = (e) => {
    const img = new Image();
    img.onload = () => {
      const canvas = document.createElement('canvas');
      const ctx = canvas.getContext('2d');

      const size = 400;
      canvas.width = size;
      canvas.height = size;

      let sWidth = img.width, sHeight = img.height, sx = 0, sy = 0;

      if (img.width > img.height) { // Landscape
        sWidth = img.height;
        sx = (img.width - img.height) / 2;
      } else { // Portrait
        sHeight = img.width;
        sy = (img.height - img.width) * 0.15; // Face focus
      }

      ctx.drawImage(img, sx, sy, sWidth, sHeight, 0, 0, size, size);

      // High-speed WebP format
      const result = canvas.toDataURL("image/webp", 0.8);

      if (dotNetHelper) {
        // Call the C# [JSInvokable] method
        dotNetHelper.invokeMethodAsync('OnImageProcessed', result);
      }
    };
    img.src = e.target.result;
  };
  reader.readAsDataURL(file);
};