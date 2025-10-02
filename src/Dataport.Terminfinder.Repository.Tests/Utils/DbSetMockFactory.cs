namespace Dataport.Terminfinder.Repository.Tests.Utils;

public static class DbSetMockFactory
{
    public static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(set => set.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<T>>().Setup(set => set.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(set => set.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>()
            .Setup(set => set.GetEnumerator())
            .Returns(() => queryable.GetEnumerator());
        
        return mockSet;
    }
}