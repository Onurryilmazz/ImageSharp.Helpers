using SixLabors.Fonts;
using SixLabors.ImageSharp;

namespace ImageCreator.Business.Interfaces;

public interface IImageCreator
{
    Image CreateEmptyImage(int width, int height, Color? backgroundColor = null);
    void ResizeImage(Image image, int width, int height);
    void ResizeImage(Image image, int ratio);
    Image AddBackgroundImage(Image backgroundImage, Image mainImage, int? backgroundPointX, int? backgroundPointY,
        bool isResizeBackground = true);
    void RotateImage(Image image, float angle);
    void DrawTextOnImage(Image image, string text, Font font, int pointX, int pointY, Color? color);
    void DrawTextOnImageTorectangle(Image image, string text, Font font, int rectangleWidth, int textPointX = 0,
        int textPointY = 0, Color? color = null);
    Font GetFontWithName(string path, int fontSize, FontStyle style = FontStyle.Regular);
    string ImageToBase64(Image image);
    Image Base64ToImage(string base64String);
    Image GetImageFromUrl(string url);
    byte[] DownloadImageWithUrl(string imageUrl);
}