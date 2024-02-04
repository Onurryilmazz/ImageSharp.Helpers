using ImageCreator.Business.Interfaces;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageCreator.Business.Services;

public class ImageCreator : IImageCreator
{
    public Image CreateEmptyImage(int width, int height, Color? backgroundColor = null)
    {
        return new Image<Rgba32>(width, height, backgroundColor ?? Color.White);
    }
    
    public void ResizeImage(Image image, int width, int height)
    { 
        image.Mutate(x => x.Resize(width, height));
    }   

    public void ResizeImage(Image image, int ratio)
    {
        image.Mutate(x => x.Resize(image.Width * ratio, image.Height * ratio));
    }
    
    public Image AddBackgroundImage(Image backgroundImage, Image mainImage, int? backgroundPointX, int? backgroundPointY, bool isResizeBackground = true)
    {
        if (isResizeBackground)
        {
            ResizeImage(backgroundImage, mainImage.Width, mainImage.Height);
        }
        mainImage.Mutate(x => x.DrawImage(backgroundImage, new Point(backgroundPointX ?? 0, backgroundPointY ?? 0), 1f));
        return mainImage;
    }
    
    public void RotateImage(Image image, float angle)
    {
        image.Mutate(x => x.Rotate(angle));
    }
    
    public void DrawTextOnImage(Image image, string text, Font font, int pointX, int pointY,Color? color)
    {
        image.Mutate(x => x.DrawText(
            text,
            font,
            Brushes.Solid(Color.White),
            new PointF(pointX , pointY)));
        
    }
    
    public void DrawTextOnImageTorectangle(Image image, string text, Font font, int rectangleWidth, int textPointX = 0, int textPointY= 0, Color? color = null)
    {
        var vocabularyList = text.Split(' ');

        var lineSize = 0;
        var lineText = "";
        var lineOffset = 0; 
            
        foreach (var vocab in vocabularyList)
        {
            var textSize = TextMeasurer.MeasureSize(vocab, new TextOptions(font));
                
            if (lineSize + textSize.Width > rectangleWidth)
            {
                image.Mutate(x => x.DrawText(
                    lineText,
                    font,
                    Brushes.Solid(color ?? Color.White),
                    new PointF(textPointX , textPointY + lineOffset)));
                lineOffset += (int)textSize.Height + 5; // 5 changeable
                lineSize = (int)textSize.Width;
                lineText = vocab + " ";
                continue;
            }
            lineText += vocab + " ";
            lineSize += (int)textSize.Width;
        }
            
        if (lineText.Length > 0)
        {
            image.Mutate(x => x.DrawText(
                lineText,
                font,
                Brushes.Solid(color ?? Color.White),
                new PointF(textPointX, textPointY + lineOffset)));
        }
    }
    
    public Font GetFontWithName(string path, int fontSize, FontStyle style = FontStyle.Regular)
    {
        FontCollection collection = new();
        FontFamily family = collection.Add(path);
        return family.CreateFont(fontSize,style);
    }

    public string ImageToBase64(Image image)
    {
        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        var imageBytes = ms.ToArray();
        var base64String = Convert.ToBase64String(imageBytes);
        
        ms.Close();
        
        return base64String;
    }

    public Image Base64ToImage(string base64String)
    {
        var imageBytes = Convert.FromBase64String(base64String);
        var image = Image.Load(imageBytes);
        return image;
    }
    
    public Image GetImageFromUrl(string url)
    {
        var imageBytes = DownloadImageWithUrl(url);
        if (imageBytes == null)
        {
            return null;
        }
        var image = Image.Load(imageBytes);
        return image;
    }
    
    public byte[] DownloadImageWithUrl(string imageUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                var response = client.GetAsync(imageUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsByteArrayAsync().Result;
                }
                else
                {
                    Console.WriteLine($"HTTP Hata Kodu: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }

            return null;
        }
    }
}