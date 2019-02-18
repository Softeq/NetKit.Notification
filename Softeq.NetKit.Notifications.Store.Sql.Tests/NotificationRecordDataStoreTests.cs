// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Models;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Domain.Models.NotificationRecord;
using Softeq.NetKit.Notifications.Store.Sql.DataStores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Notifications.Store.Sql.Tests
{
    public class NotificationRecordDataStoreTests : UnitTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task SaveRecordTest()
        {
            var newRecord = new NotificationRecord
            {
                OwnerUserId = Guid.NewGuid().ToString(),
                Event = NotificationEvent.PackageArrived,
                Parameters = new Dictionary<string, object> { { "test", "value" } }
            };

            using (var context = GetContext())
            {
                var store = new NotificationRecordDataStore(context, DefaultMapper);
                var result = await store.SaveAsync(newRecord);
                Assert.NotEqual(Guid.Empty, result.Id);
                Assert.NotEqual(new DateTimeOffset(), result.Created);
            }

            using (var context = GetContext())
            {
                Assert.NotEmpty(context.NotificationRecords);
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task FindRecordAsync()
        {
            var record = new Models.NotificationRecord
            {
                Created = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                Event = NotificationEvent.PackageArrived,
                OwnerUserId = Guid.NewGuid().ToString(),
                Parameters = new Dictionary<string, object> { { "test", "value" } }
            };

            using (var context = GetContext())
            {
                await context.NotificationRecords.AddAsync(record);
                await context.SaveChangesAsync();
            }

            using (var context = GetContext())
            {
                var store = new NotificationRecordDataStore(context, DefaultMapper);
                var result = await store.FindAsync(record.Id);
                Assert.NotNull(result);
                Assert.Equal(record.Created, result.Created);
                Assert.Equal(record.Event, result.Event);
                Assert.Equal(record.OwnerUserId, result.OwnerUserId);
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteAllTest()
        {
            var userId = Guid.NewGuid().ToString();

            using (var context = GetContext())
            {
                await context.NotificationRecords.AddAsync(new Models.NotificationRecord
                {
                    Created = DateTimeOffset.UtcNow,
                    Id = Guid.NewGuid(),
                    Event = NotificationEvent.PackageArrived,
                    OwnerUserId = userId,
                    Parameters = new Dictionary<string, object> { { "test", "value" } }
                });
                await context.SaveChangesAsync();
            }

            using (var context = GetContext())
            {
                var store = new NotificationRecordDataStore(context, DefaultMapper);
                await store.DeleteAllAsync(userId);
                Assert.Empty(context.NotificationRecords);
            }
        }

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData(1, 1)]
        [InlineData(40, 4)]
        public async Task ListRecordsPageSizeTest(int size, int expectedSize)
        {
            var userId = Guid.NewGuid().ToString();

            var records = CreateRecords(userId);

            using (var context = GetContext())
            {
                await context.NotificationRecords.AddRangeAsync(records);
                await context.SaveChangesAsync();
            }

            using (var context = GetContext())
            {
                var store = new NotificationRecordDataStore(context, DefaultMapper);
                var results = await store.ListAsync(userId, size);
                Assert.NotNull(results);
                Assert.Equal(expectedSize, results.Count());

                var expected = records.OrderByDescending(x => x.Created).Take(size).ToList();
                expected.ForEach(x => Assert.Contains(results, y => y.Id == x.Id));

                Assert.Equal(records.Last().Id, results.First().Id);
            }
        }

        [Theory]
        [Trait("Category", "Unit")]
        [MemberData(nameof(TimeFilters))]
        public async Task ListRecordsFilterTest(DateTimeOffset? startTime, DateTimeOffset? endTime, IList<Guid> expectedIds)
        {
            var userId = Guid.NewGuid().ToString();
            var filter = new FilterOptions(startTime, endTime);

            var records = CreateRecords(userId);

            using (var context = GetContext())
            {
                await context.NotificationRecords.AddRangeAsync(records);
                await context.SaveChangesAsync();
            }

            using (var context = GetContext())
            {
                var store = new NotificationRecordDataStore(context, DefaultMapper);
                var results = await store.ListAsync(userId, 5, filter);
                Assert.NotNull(results);
                expectedIds.All(x =>
                {
                    Assert.Contains(results, y => y.Id == x);
                    return true;
                });
            }
        }

        public static IEnumerable<object[]> TimeFilters =>
            new[]
            {
                new object[]
                {
                    new DateTimeOffset(2016, 05,26,07,01,01, TimeSpan.Zero),
                    null,
                    new [] { new Guid("C28057D4-83C5-4C17-A6DE-03D076A20955"), new Guid("CE98B84E-BC47-485E-A5F3-64BA1455A3F7") }
                },
                new object[]
                {
                    null,
                    new DateTimeOffset(2017, 05,26,07,01,01, TimeSpan.Zero),
                    new [] { new Guid("03C35FE2-3F1D-4233-A205-0324618DBC89"), new Guid("8AFA6A0B-C0D9-4435-8D19-086546A8CBA1"), new Guid("C28057D4-83C5-4C17-A6DE-03D076A20955") }
                },
                new object[]
                {
                    new DateTimeOffset(2016, 05,26,07,01,01, TimeSpan.Zero),
                    new DateTimeOffset(2017, 05,26,07,01,01, TimeSpan.Zero),
                    new [] { new Guid("C28057D4-83C5-4C17-A6DE-03D076A20955") }
                },
                new object[]
                {
                    null,
                    null,
                    new [] { new Guid("03C35FE2-3F1D-4233-A205-0324618DBC89"), new Guid("8AFA6A0B-C0D9-4435-8D19-086546A8CBA1"),
                        new Guid("C28057D4-83C5-4C17-A6DE-03D076A20955"), new Guid("CE98B84E-BC47-485E-A5F3-64BA1455A3F7") }
                }
            };

        private static IList<Models.NotificationRecord> CreateRecords(string userId)
        {
            return new List<Models.NotificationRecord>
            {
                new Models.NotificationRecord
                {
                    Created = DateTimeOffset.Parse("25/01/2015"),
                    Id = new Guid("03C35FE2-3F1D-4233-A205-0324618DBC89"),
                    Event = NotificationEvent.PackageArrived,
                    OwnerUserId = userId,
                    Parameters = new Dictionary<string, object>()
                },
                new Models.NotificationRecord
                {
                    Created = DateTimeOffset.Parse("25/01/2016"),
                    Id = new Guid("8AFA6A0B-C0D9-4435-8D19-086546A8CBA1"),
                    Event = NotificationEvent.ArticleCreated,
                    OwnerUserId = userId,
                    Parameters = new Dictionary<string, object>()
                },
                new Models.NotificationRecord
                {
                    Created = DateTimeOffset.Parse("25/01/2017"),
                    Id = new Guid("C28057D4-83C5-4C17-A6DE-03D076A20955"),
                    Event = NotificationEvent.ResetPassword,
                    OwnerUserId = userId,
                    Parameters = new Dictionary<string, object>()
                },
                new Models.NotificationRecord
                {
                    Created = DateTimeOffset.Parse("25/01/2018"),
                    Id = new Guid("CE98B84E-BC47-485E-A5F3-64BA1455A3F7"),
                    Event = NotificationEvent.CommentLiked,
                    OwnerUserId = userId,
                    Parameters = new Dictionary<string, object>()
                }
            };
        }
    }
}
