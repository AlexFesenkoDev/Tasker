using Microsoft.EntityFrameworkCore;
using Tasker.Data;

namespace Tasker.Tests.TestUtils
{
    public static class DbContextHelper
    {
        public static AppDbContext CreateInMemoryContext(string? dbName = null)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}
