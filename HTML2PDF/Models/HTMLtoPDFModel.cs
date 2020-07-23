﻿using PdfSharp;
using PdfSharp.Pdf;
using System.IO;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace HTML2PDF.Models
{
    class HTMLtoPDFModel
    {
        public string SourceHTMLPath;
        public string DestinationPDFPath;
        public HTMLtoPDFModel(string SourceHTMLPath, string DestinationPDFPath)
        {
            this.SourceHTMLPath = SourceHTMLPath;
            this.DestinationPDFPath = DestinationPDFPath;
        }
        public async void Convert()
        {
            using (StreamReader sr = new StreamReader(SourceHTMLPath))
            {
                string all = await sr.ReadToEndAsync();

                PdfDocument pdf = PdfGenerator.GeneratePdf(all, PageSize.A4, 0);
                pdf.Save(DestinationPDFPath);
            }
        }
    }
}
