using NetDaemonImpl;
using Xunit;

namespace NetDaemonTest;

public class ActionCollectionTest
{
    [Fact]
    public void AddOrUpdate_ExecuteActions_AllActionsExecuted()
    {
        // Arrange
        var collection = new ActionCollection();
        var count1 = 0;
        var count2 = 0;
        collection.AddOrUpdate("entity1", () => count1++);
        collection.AddOrUpdate("entity2", () => count2++);

        // Act
        collection.ExecuteActions();

        // Assert
        Assert.Equal(1, count1);
        Assert.Equal(1, count2);
    }

    [Fact]
    public void AddOrUpdate_SameKey_OverwritesAction()
    {
        // Arrange
        var collection = new ActionCollection();
        var count1 = 0;
        var count2 = 0;
        collection.AddOrUpdate("entity1", () => count1++);
        collection.AddOrUpdate("entity1", () => count2++);

        // Act
        collection.ExecuteActions();

        // Assert
        Assert.Equal(0, count1);
        Assert.Equal(1, count2);
    }

    [Fact]
    public void Delete_RemovesAction()
    {
        // Arrange
        var collection = new ActionCollection();
        var count = 0;
        collection.AddOrUpdate("entity1", () => count++);

        // Act
        collection.Delete("entity1");
        collection.ExecuteActions();

        // Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public void Delete_NonExistingKey_DoesNotThrow()
    {
        // Arrange
        var collection = new ActionCollection();

        // Act & Assert
        collection.Delete("nonexistent");
    }

    [Fact]
    public void ExecuteActions_Empty_DoesNotThrow()
    {
        // Arrange
        var collection = new ActionCollection();

        // Act & Assert
        collection.ExecuteActions();
    }
}
