using PdfSharp;
using PdfSharp.Pdf;
using System;
using System.IO;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace HTML2PDF.Models
{
    internal class HTMLtoPDFModel
    {
        public string SourceHTMLPath;
        public string DestinationPDFPath;

        public HTMLtoPDFModel(string SourceHTMLPath, string DestinationPDFPath)
        {
            this.SourceHTMLPath = SourceHTMLPath;
            this.DestinationPDFPath = DestinationPDFPath;
        }

        private PdfDocument GetDocument(string html)
        {
            return PdfGenerator.GeneratePdf(html, PageSize.A4, cssData: PdfGenerator.ParseStyleSheet(@"p, li, h1, h2, h3, b {page-break-inside: avoid;}; img {width: 100vw}"));
        }

        /// <summary>
        /// Perform the HTML-PDF conversion asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task ConvertAsync(IProgress<string> progress)
        {
            try
            {
                progress.Report("Opening file for writing...");
                using (StreamReader sr = new StreamReader(SourceHTMLPath))
                {
                    progress.Report("Reading source file...");
                    string all = await sr.ReadToEndAsync();
                    progress.Report("Generating output PDF...");
                    PdfDocument pdf = GetDocument(all);
                    progress.Report("Saving to destination...");
                    pdf.Save(DestinationPDFPath);
                    progress.Report("Done!");
                }
            }
            catch (NotSupportedException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Perform the HTML-PDF conversion/
        /// </summary>
        /// <returns></returns>
        public void Convert()
        {
            try
            {
                using (StreamReader sr = new StreamReader(SourceHTMLPath))
                {
                    string all = sr.ReadToEnd();
                    PdfDocument pdf = GetDocument(all);
                    pdf.Save(DestinationPDFPath);
                }
            }
            catch (NotSupportedException ex)
            {
                throw ex;
            }
        }
    }
}