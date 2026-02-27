window.downloadReportAsPdf = () => {
  const reportElement = document.querySelector('.report-wrapper');
  if (!reportElement) return;

  const reportHtml = reportElement.innerHTML;
  const styles = Array.from(document.querySelectorAll('style, link[rel="stylesheet"]'))
    .map(el => el.outerHTML)
    .join('\n');

  const printWindow = window.open('', '_blank', 'width=1100,height=900');

  printWindow.document.write(`
        <!DOCTYPE html>
        <html>
        <head>
            <title>Executive Export</title>
            ${styles}
            <style>
                /* 1. Kill browser default headers/footers */
                @page { 
                    size: A4 portrait; 
                    margin: 0 !important; 
                }

                /* 2. THE KILLER: Strict overflow hidden prevents the printer from seeing "phantom" content */
                body { 
                    margin: 0 !important; 
                    padding: 0 !important; 
                    background: white !important;
                    overflow: hidden !important; 
                    -webkit-print-color-adjust: exact !important;
                    print-color-adjust: exact !important;
                    overflow: hidden !important; 
                }

                /* 3. Ensure the wrapper matches A4 exactly */
                .report-wrapper { 
                    width: 210mm !important; 
                    margin: 0 auto !important;
                    padding: 0 !important;
                    height: auto;
    overflow: visible;
                }

                /* 4. Force exact heights to prevent the "Page 4" ghost page */
                .paper-sheet {
                    width: 210mm !important;
                    height: 297mm !important; /* Changed from auto to exact height */
                    padding: 20mm !important; 
                    margin: 0 !important;
                    page-break-after: always !important; 
                    box-shadow: none !important;
                    border: none !important;
                    display: block !important;
                    box-sizing: border-box;
                }

                /* 5. Prevent chart slicing */
                .premium-card, .chart-box, .row {
                    page-break-inside: avoid !important;
                    break-inside: avoid !important;
                }
                .paper-sheet:last-of-type {
    page-break-after: avoid !important;
    margin-bottom: 0 !important;
}
            </style>
        </head>
        <body>
            <div class="report-wrapper">
                ${reportHtml}
            </div>
            <script>
                window.onload = () => {
                    setTimeout(() => {
                        window.print();
                        window.close();
                    }, 1000);
                };
            <\/script>
        </body>
        </html>
    `);
  printWindow.document.close();
};