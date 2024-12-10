using NetDaemonImpl.Modules;
using System.IO;
using Xunit;

namespace NetDaemonTest
{
    public class HouseNotificationImageCreatorTest
    {
        [Fact]
        public void Contructor_NoExceptions()
        {
            // Arrange 

            // Act
            _ = new HouseNotificationImageCreator();

            // Assert
        }

        [Fact]
        public void GetImagePath()
        {
            // Arrange
            var sut = new HouseNotificationImageCreator();

            // Assert
            Assert.Equal("/local/HouseImage.jpg", sut.GetImagePath());
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
            Assert.True(size == 617 || size == 749);
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
            Assert.Equal(1067, new FileInfo(sut.GetPrivate<string>("path")).Length);
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
