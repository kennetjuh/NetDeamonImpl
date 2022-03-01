using Microsoft.AspNetCore.Identity;
using NetDaemonInterface;
using SkiaSharp;
using System.Collections.Generic;
using System.IO;

namespace NetDaemonImpl.Modules
{
    internal class HouseNotificationTextEntry
    {
        private readonly int x;
        private readonly int y;
        private readonly int size;
        private readonly string label;
        private readonly List<Func<string?>> variables;

        public HouseNotificationTextEntry(int x, int y, int size, string label, List<Func<string?>> variables)
        {
            this.x = x;
            this.y = y;
            this.size = size;
            this.label = label;
            this.variables = variables;
        }

        public void Draw(SKCanvas canvas, SKFont font, SKPaint paint)
        {
            font.Size = size;
            var text = label;
            for (var i = 0; i < variables.Count; i++)
            {
                var variable = variables[i]();
                text = text.Replace($"{{{i}}}", variable ?? "NULL");
            }

            canvas.DrawText(text, new SKPoint(x, y), font, paint);
        }
    }

    internal class HouseNotificationImageEntry
    {
        private readonly int x;
        private readonly int y;
        private readonly int width;
        private readonly int heigth;
        private readonly byte[] image;
        private readonly Func<bool>? condition;

        public HouseNotificationImageEntry(int x, int y, int width, int heigth, byte[] image, Func<bool>? condition)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.heigth = heigth;
            this.image = image;
            this.condition = condition;
        }

        public void Draw(SKCanvas canvas)
        {
            if (condition != null && !condition())
            {
                return;
            }

            var image = SKBitmap.Decode(this.image);
            image = image.Resize(new SKSizeI(width, heigth), SKSamplingOptions.Default);
            canvas.DrawBitmap(image, new SKPoint(x, y));
        }
    }


    public class HouseNotificationImageCreator : IHouseNotificationImageCreator
    {
        private readonly string path;
        private readonly List<HouseNotificationTextEntry> textEntries;
        private readonly List<HouseNotificationImageEntry> imageEntries;
        private const string imageName = "HouseImage.jpg";

        public HouseNotificationImageCreator()
        {
            var destfolder = Path.GetFullPath("../www");
            if (!Directory.Exists(destfolder))
            {
                destfolder = Path.GetFullPath(@".\");
            }
            path = Path.Combine(destfolder, imageName);

            textEntries = new List<HouseNotificationTextEntry>();
            imageEntries = new List<HouseNotificationImageEntry>();
        }

        public string GetImagePath()
        {
            return $"/local/{imageName}";
        }

        public void CreateImage()
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var font = Resource.arial;
            using var fontStream = new MemoryStream(font);

            var imageInfo = new SKImageInfo(200, 100);
            using var surface = SKSurface.Create(imageInfo);
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.Black);
            using var paint = new SKPaint();
            paint.IsAntialias = true;
            paint.Color = SKColors.White;

            using var skfont = new SKFont(SKTypeface.FromStream(fontStream), 10);

            foreach (var textEntry in textEntries)
            {
                textEntry.Draw(canvas, skfont, paint);
            }

            foreach (var imageEntry in imageEntries)
            {
                imageEntry.Draw(canvas);
            }

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var file = File.OpenWrite(path);
            data.SaveTo(file);
        }

        public void AddFormattedText(int x, int y, int size, string label, List<Func<string?>> variables)
        {
            textEntries.Add(new HouseNotificationTextEntry(x, y, size, label, variables));
        }

        public void AddFormattedText(int x, int y, int size, string label, Func<string?> variables)
        {
            textEntries.Add(new HouseNotificationTextEntry(x, y, size, label, new List<Func<string?>> { variables }));
        }

        public void AddConditionalImage(int x, int y, int width, int heigth, byte[] image, Func<bool>? condition)
        {
            imageEntries.Add(new HouseNotificationImageEntry(x, y, width, heigth, image, condition));
        }
    }
}
