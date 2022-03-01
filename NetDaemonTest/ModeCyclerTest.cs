using NetDaemonImpl;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NetDaemonTest
{
    public class ModeCyclerTest
    {

        [Fact]
        public void Contructor_NoExceptions()
        {
            // Arrange 

            // Act
            _ = new ModeCycler(TimeSpan.Zero);

            // Assert
        }

        [Fact]
        public void ModeCycler_2ActionsCycle2Times_BothActionsExecuted()
        {
            // Arrange
            var ModesTest = new List<int>();

            var modeCycler = new ModeCycler(TimeSpan.FromMilliseconds(10),
                () =>
                {
                    ModesTest.Add(1);
                },
                () =>
                {
                    ModesTest.Add(2);
                });

            // Act
            modeCycler.Cycle();
            modeCycler.Cycle();

            // Assert
            Assert.Equal(1, ModesTest.Count(x => x == 1));
            Assert.Equal(1, ModesTest.Count(x => x == 2));
        }

        [Fact]
        public async Task ModeCycler_2ActionsCycle2TimesWithDelay_FirstActionExecutedTwiceAsync()
        {
            // Arrange
            var ModesTest = new List<int>();

            var modeCycler = new ModeCycler(TimeSpan.FromMilliseconds(1),
                () =>
                {
                    ModesTest.Add(1);
                },
                () =>
                {
                    ModesTest.Add(2);
                });

            // Act
            modeCycler.Cycle();
            await Task.Delay(TimeSpan.FromMilliseconds(1));
            modeCycler.Cycle();

            // Assert
            Assert.Equal(2, ModesTest.Count(x => x == 1));
            Assert.Equal(0, ModesTest.Count(x => x == 2));
        }

        [Fact]
        public void ModeCycler_1ActionCycle2Times_FirstActionExecutedTwice()
        {
            // Arrange
            var ModesTest = new List<int>();

            var modeCycler = new ModeCycler(TimeSpan.FromMilliseconds(10),
                () =>
                {
                    ModesTest.Add(1);
                }
              );

            // Act
            modeCycler.Cycle();
            modeCycler.Cycle();

            // Assert
            Assert.Equal(2, ModesTest.Count(x => x == 1));
        }
    }
}
