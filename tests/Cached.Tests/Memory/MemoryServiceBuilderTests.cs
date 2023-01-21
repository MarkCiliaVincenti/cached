﻿namespace Cached.Tests.Memory
{
    using System.Threading.Tasks;
    using Cached.Caching;
    using Configuration;
    using MemoryCache;
    using MemoryCache.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class MemoryServiceBuilderTests : ServiceBuilderTestsBase<IMemory>
    {
        public MemoryServiceBuilderTests() : base(new MemoryCacheServiceBuilder())
        {
        }

        [Fact]
        public void AddFunctionMethod_Adds_Valid_Service_Correctly()
        {
            // Arrange
            ServiceBuilder.AddFunction<string, string>(
                arg => "",
                (provider, key, arg) => Task.FromResult(""));
            var services = new ServiceCollection();

            // Act
            ServiceBuilder.GetBuild()(services);

            // Assert
            Assert.Equal(9, services.Count); // Includes MemoryCache and all options instances.
            Assert.Equal(typeof(ICache<IMemory>), services[6].ServiceType); // ICache service added before ICached
            Assert.Equal(typeof(ICached<string, string>), services[8].ServiceType); // ICached service added last.
        }
    }
}