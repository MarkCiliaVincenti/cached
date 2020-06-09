﻿namespace Cached.Tests.Caching
{
    using System;
    using System.Threading.Tasks;
    using Cached.Caching;
    using Moq;
    using Xunit;

    public class CachedTests
    {
        internal class FakeProvider : ICacheProvider
        {
            public Task Set<TValue>(string key, TValue value)
            {
                throw new NotImplementedException();
            }

            public Task<ValueResult<TValue>> TryGet<TValue>(string key)
            {
                throw new NotImplementedException();
            }

            public Task Remove(string key)
            {
                throw new NotImplementedException();
            }
        }

        internal class FakeHandler : ICache<FakeProvider>
        {
            public FakeProvider Provider { get; }
            public Task<TValue> GetOrFetchAsync<TValue>(string key, Func<string, Task<TValue>> fetchFactory)
            {
                throw new NotImplementedException();
            }
        }

        public sealed class Constructor
        {
            public sealed class Throws
            {
                [Fact]
                public void If_Cacher_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() =>
                        new Cached<object, object, FakeProvider>(
                            null,
                            o => "",
                            (s, o) => Task.FromResult(o)));
                }

                [Fact]
                public void If_FetchFactory_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() =>
                        new Cached<object, object, FakeProvider>(
                            new FakeHandler(), 
                            o => "",
                            null));
                }

                [Fact]
                public void If_KeyFactory_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() =>
                        new Cached<object, object, FakeProvider>(
                            new FakeHandler(), 
                            null,
                            (key, arg) => Task.FromResult(arg)));
                }
            }
        }

        public sealed class GetOrFetchAsyncMethod
        {
            //        [Fact]
            //        public async Task Fetch_Value_From_Cacher()
            //        {
            //            // Arrange
            //            var cacherMock = new Mock<ICacher>();
            //            cacherMock.Setup(c => c.GetOrFetchAsync(It.IsAny<string>(), It.IsAny<Func<string, Task<string>>>()))
            //                .Returns((string key, Func<string, Task<string>> fetch) =>
            //                    Task.FromResult(fetch(key).Result + key));

            //            var memoryCached = new Cached<string, int>(
            //                cacherMock.Object,
            //                arg => "key_" + arg,
            //                (key, arg) => Task.FromResult("fetch_" + arg));

            //            // Act
            //            var response = await memoryCached.GetOrFetchAsync(22);

            //            // Assert
            //            Assert.Equal("fetch_22key_22", response);
            //            cacherMock.Verify(
            //                c =>
            //                    c.GetOrFetchAsync(It.IsAny<string>(),
            //                        It.IsAny<Func<string, Task<string>>>()),
            //                Times.Once);
            //        }

            //        [Fact]
            //        public async Task Passes_Key_To_FetchFactory()
            //        {
            //            // Arrange
            //            var cacherMock = new Mock<ICacher>();
            //            var keyFromCachedCall = string.Empty;
            //            cacherMock.Setup(c => c.GetOrFetchAsync(It.IsAny<string>(), It.IsAny<Func<string, Task<string>>>()))
            //                .Returns((string key, Func<string, Task<string>> fetch) =>
            //                {
            //                    keyFromCachedCall = key;
            //                    return Task.FromResult(key);
            //                });

            //            var memoryCached = new Cached<string, int>(
            //                cacherMock.Object,
            //                arg => arg.ToString(),
            //                (key, arg) => Task.FromResult(key));

            //            // Act
            //            var response = await memoryCached.GetOrFetchAsync(221);

            //            // Assert
            //            Assert.Equal("221", response);
            //            Assert.Equal("221", keyFromCachedCall);
            //        }
            //    }
            }
        }
}