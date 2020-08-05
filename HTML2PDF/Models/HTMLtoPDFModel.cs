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

        /// <summary>
        /// Perform the HTML-PDF conversion/
        /// </summary>
        /// <returns></returns>
        public async Task Convert()
        {
            try
            {
                using (StreamReader sr = new StreamReader(SourceHTMLPath))
                {
                    string all = await sr.ReadToEndAsync();
                    PdfDocument pdf = PdfGenerator.GeneratePdf(all, PageSize.A4, cssData: PdfGenerator.ParseStyleSheet(@"p, li, h1, h2, h3, b {page-break-inside: avoid;}; img {width: 100vw}"));
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