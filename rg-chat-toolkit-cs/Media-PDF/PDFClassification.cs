using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;


namespace rg_chat_toolkit_cs.Media_PDF;

public class ImageExtractor
{
    public static List<MemoryStream> ExtractImagesFromPdf(string pdfPath)
    {
        List<MemoryStream> imageStreams = new List<MemoryStream>();

        using (PdfReader pdfReader = new PdfReader(pdfPath))
        using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
        {
            for (int pageNum = 1; pageNum <= pdfDocument.GetNumberOfPages(); pageNum++)
            {
                var pdfPage = pdfDocument.GetPage(pageNum);
                var images = new ImageRenderListener(imageStreams);

                PdfCanvasProcessor processor = new PdfCanvasProcessor(images);
                processor.ProcessPageContent(pdfPage);
            }
        }

        return imageStreams;
    }
}

// Listener to handle image extraction into memory streams
public class ImageRenderListener : IEventListener
{
    private readonly List<MemoryStream> _imageStreams;

    public ImageRenderListener(List<MemoryStream> imageStreams)
    {
        _imageStreams = imageStreams;
    }

    public void EventOccurred(IEventData data, EventType eventType)
    {
        if (eventType != EventType.RENDER_IMAGE) return;

        var renderInfo = (ImageRenderInfo)data;
        PdfImageXObject image = renderInfo.GetImage();
        if (image != null)
        {
            try
            {
                // Save the image to a MemoryStream
                byte[] imageBytes = image.GetImageBytes(true);
                MemoryStream ms = new MemoryStream(imageBytes);
                _imageStreams.Add(ms);
                Console.WriteLine($"Image extracted and stored in memory.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting image: {ex.Message}");
            }
        }
    }

    public ICollection<EventType> GetSupportedEvents()
    {
        return new HashSet<EventType> { EventType.RENDER_IMAGE };
    }
}
