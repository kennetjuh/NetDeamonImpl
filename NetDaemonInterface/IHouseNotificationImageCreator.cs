using System;
using System.Collections.Generic;

namespace NetDaemonInterface
{
    public interface IHouseNotificationImageCreator
    {
        void AddConditionalImage(int x, int y, int width, int heigth, byte[] image, Func<bool>? condition);
        void AddFormattedText(int x, int y, int size, string label, Func<string?> variables);
        void AddFormattedText(int x, int y, int size, string label, List<Func<string?>> variables);
        void CreateImage();
        string GetImagePath();
    }
}