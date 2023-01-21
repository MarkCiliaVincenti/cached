namespace Cached.Tests.Locking
{
    using AsyncKeyedLock;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class KeyBasedLockTests
    {
        public class LockAsyncMethod
        {
            public class Throws
            {
                [Fact]
                public async Task When_Key_Is_Null()
                {
                    await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                        await new AsyncKeyedLocker<string>().LockAsync(null));
                }
            }

            private static async Task<SemaphoreSlim> LockTestTask(AsyncKeyedLocker<string> keyBasedLock)
            {
                SemaphoreSlim lck;
                using (await keyBasedLock.LockAsync("test_lock_stack"))
                {
                    lck = keyBasedLock.Index
                        .FirstOrDefault(l => l.Key.Equals("test_lock_stack"))
                        .Value.SemaphoreSlim;

                    await Task.Delay(10);
                }

                return lck;
            }

            [Fact]
            public async Task Creates_Separate_Locks_Based_On_Key_And_Dispose_Them_Correctly()
            {
                // Arrange
                var keyBasedLock = new AsyncKeyedLocker<string>();
                const string testKey = "test_lock_by_key";

                bool KeyExists(string key) => keyBasedLock.Index
                    .Any(l => l.Key.Equals(key));

                SemaphoreSlim GetByKey(string key) => keyBasedLock.Index
                    .First(l => l.Key.Equals(key)).Value.SemaphoreSlim;

                SemaphoreSlim lck1;
                SemaphoreSlim lck2;

                // Act, Assert
                using (await keyBasedLock.LockAsync(testKey))
                {
                    Assert.True(KeyExists(testKey));
                    lck1 = GetByKey(testKey);
                    using (await keyBasedLock.LockAsync(testKey + "_2"))
                    {
                        Assert.True(KeyExists(testKey));
                        Assert.True(KeyExists(testKey + "_2"));
                        lck2 = GetByKey(testKey + "_2");
                    }

                    Assert.True(KeyExists(testKey));
                    Assert.False(KeyExists(testKey + "_2"));
                }

                Assert.False(KeyExists(testKey));
                Assert.True(lck1.CurrentCount == 1);
                Assert.True(lck2.CurrentCount == 1);
            }

            [Fact]
            public async Task Reuse_Same_Lock_For_All_Queued_Tasks_With_Same_Key()
            {
                // Arrange
                var keyBasedLock = new AsyncKeyedLocker<string>();
                var tasks = Enumerable.Repeat(LockTestTask(keyBasedLock), 10);

                // Act
                var result = await Task.WhenAll(tasks);

                // Assert
                Assert.Equal(10, result.Length);
                Assert.True(result.All(l => l != null));
                Assert.True(result.All(l => l.Equals(result[0])));
                Assert.DoesNotContain(keyBasedLock.Index, l => l.Key.Equals("test_lock_stack"));
            }
        }
    }
}