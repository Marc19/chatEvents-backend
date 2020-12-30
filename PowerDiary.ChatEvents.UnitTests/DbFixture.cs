using Microsoft.Extensions.DependencyInjection;
using PowerDiary.ChatEvents.Core.Persistence;
using PowerDiary.ChatEvents.Services.Interfaces;
using PowerDiary.ChatEvents.Core.Repositories;
using PowerDiary.ChatEvents.Core.Observers;
using PowerDiary.ChatEvents.Services.Services;

namespace PowerDiary.ChatEvents.UnitTests
{
    public class DbFixture
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public DbFixture()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<IChatActionRepository, ChatActionRepository>();
            serviceCollection.AddTransient<IChatActionService, ChatActionService>();
            serviceCollection.AddTransient<IChatEventRepository, ChatEventRepository>();
            serviceCollection.AddTransient<IChatEventService, ChatEventService>();
            serviceCollection.AddSingleton<IChatRoomObserver, ChatRoomEventObserver>();
            serviceCollection.AddSingleton<IInMemoryDB, InMemoryTestDB>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
