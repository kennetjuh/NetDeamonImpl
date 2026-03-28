using NetDaemonImpl.Modules;
using System.IO;
using Xunit;

namespace NetDaemonTest
{
    public class HouseNotificationImageCreatorTest
    {
        [Fact]
        public void GetImagePath()
        {
            // Arrange
            var sut = new HouseNotificationImageCreator();

            // Assert
            Assert.StartsWith("/local/HouseImage.jpg?t=", sut.GetImagePath());
        }

        [Fact]
        public void CreateEmptyImage_CheckSize()
        {
            // Arrange
            var sut = new HouseNotificationImageCreator();
            sut.SetPrivate("path", "HouseImage.png");

            // Act
            sut.CreateImage();

            // Assert
            Assert.Equal(356, new FileInfo(sut.GetPrivate<string>("path")).Length);
        }

        [Fact]
        public void CreateImage_Text_CheckSize()
        {
            // Arrange
            var sut = new HouseNotificationImageCreator();
            sut.SetPrivate("path", "HouseImage.png");
            sut.AddFormattedText(5, 10, 10, "Test", () => "Test");

            // Act
            sut.CreateImage();

            // Assert
            var size = new FileInfo(sut.GetPrivate<string>("path")).Length;
            Assert.True(size > 356, $"Expected image with text to be larger than empty image (356 bytes), but was {size} bytes");
        }

        [Fact]
        public void CreateImage_Image_CheckSize()
        {
            // Arrange
            var sut = new HouseNotificationImageCreator();
            sut.SetPrivate("path", "HouseImage.png");
            sut.AddConditionalImage(150, 50, 50, 50, File.ReadAllBytes("../../../../NetDaemonImpl/Resources/Home.png"), null);

            // Act
            sut.CreateImage();

            // Assert
            Assert.Equal(1061, new FileInfo(sut.GetPrivate<string>("path")).Length);
        }

        [Fact]
        public void CreateImage_ImageWithFalseCondition_CheckSize()
        {
            // Arrange
            var sut = new HouseNotificationImageCreator();
            sut.SetPrivate("path", "HouseImage.png");
            sut.AddConditionalImage(150, 50, 50, 50, File.ReadAllBytes(@"../../../../NetDaemonImpl/Resources/Home.png"), () => false);

            // Act
            sut.CreateImage();

            // Assert
            Assert.Equal(356, new FileInfo(sut.GetPrivate<string>("path")).Length);
        }
    }
}
