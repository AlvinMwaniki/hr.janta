window.initCropper = (imageElement, dotnetHelper) => {
  const cropper = new Cropper(imageElement, {
    aspectRatio: 1, // Force square for the passport look
    viewMode: 1,
    dragMode: 'move',
    ready: function () {
      // Optional: make the crop box a circle visually
      document.querySelector('.cropper-view-box').style.borderRadius = '50%';
      document.querySelector('.cropper-face').style.borderRadius = '50%';
    }
  });

  window.getCroppedImage = () => {
    // Get the canvas, resize it to 300x300 for performance
    const canvas = cropper.getCroppedCanvas({ width: 300, height: 300 });
    return canvas.toDataURL('image/png'); // Send back as Base64 string
  };
};