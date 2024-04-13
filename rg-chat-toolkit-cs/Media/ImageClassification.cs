using System;
using System.Drawing;
using System.IO;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using rg_chat_toolkit_cs.Configuration;
using static System.Net.Mime.MediaTypeNames;

public class ImageAnalysisService
{
    private readonly AmazonRekognitionClient rekognitionClient;

    public ImageAnalysisService()
    {
        // Initialize the Amazon Rekognition client
        rekognitionClient = new AmazonRekognitionClient(ConfigurationHelper.AWSAccessKeyId, ConfigurationHelper.AWSSecretAccessKey);
    }

    public async Task<(List<Amazon.Rekognition.Model.Label> labels, List<TextDetection> textDetections)> AnalyzeImage(string imageDataBase64)
    {
        // Get bytes
        byte[] imageData = Convert.FromBase64String(imageDataBase64);
        return await AnalyzeImage(imageData);
    }

    public async Task<(List<Amazon.Rekognition.Model.Label> labels, List<TextDetection> textDetections)> AnalyzeImage(byte[] imageData)
    {
        // Resize the image to 300x300
        byte[] resizedImage = ResizeImage(imageData, 300, 300);

        // Convert byte array to MemoryStream for AWS SDK consumption
        using var memoryStream = new MemoryStream(resizedImage);

        // Create tasks for label detection and text detection
        var detectLabelsTask = DetectLabelsAsync(memoryStream);
        var detectTextTask = DetectTextAsync(memoryStream);

        // Wait for both tasks to complete
        await Task.WhenAll(detectLabelsTask, detectTextTask);

        // Return results
        return (detectLabelsTask.Result, detectTextTask.Result);
    }

    private byte[] ResizeImage(byte[] imageData, int width, int height)
    {
        using var ms = new MemoryStream(imageData);
        using var image = System.Drawing.Image.FromStream(ms);
        var resized = new Bitmap(image, new System.Drawing.Size(width, height));

        using var resultStream = new MemoryStream();
        resized.Save(resultStream, System.Drawing.Imaging.ImageFormat.Jpeg);
        return resultStream.ToArray();
    }

    private async Task<List<Amazon.Rekognition.Model.Label>> DetectLabelsAsync(MemoryStream imageStream)
    {
        imageStream.Position = 0; // Reset stream position
        var request = new DetectLabelsRequest
        {
            Image = new Amazon.Rekognition.Model.Image { Bytes = imageStream },
            MaxLabels = 10,
            MinConfidence = 75
        };

        var response = await rekognitionClient.DetectLabelsAsync(request);
        return response.Labels;
    }

    private async Task<List<TextDetection>> DetectTextAsync(MemoryStream imageStream)
    {
        imageStream.Position = 0; // Reset stream position
        var request = new DetectTextRequest
        {
            Image = new Amazon.Rekognition.Model.Image { Bytes = imageStream }
        };

        var response = await rekognitionClient.DetectTextAsync(request);
        return response.TextDetections;
    }
}
