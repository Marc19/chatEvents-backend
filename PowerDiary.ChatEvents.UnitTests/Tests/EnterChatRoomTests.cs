using System;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using PowerDiary.ChatEvents.Core.Models;
using PowerDiary.ChatEvents.Core.Persistence;
using PowerDiary.ChatEvents.Services.DTOs;
using PowerDiary.ChatEvents.Services.Interfaces;
using Xunit;

namespace PowerDiary.ChatEvents.UnitTests.Tests
{
    public class EnterChatRoomTests : IClassFixture<DbFixture>, IDisposable
    {
        private readonly IInMemoryDB _inMemoryDB;
        private readonly IChatEventRepository _chatEventRepository;

        public EnterChatRoomTests(DbFixture fixture)
        {
            _chatEventRepository = fixture.ServiceProvider.GetService<IChatEventRepository>();
            _inMemoryDB = fixture.ServiceProvider.GetService<IInMemoryDB>();
        }

        [Fact]
        public void EnterTheRoom_ShouldFail_If_UserDoesNotExist()
        {
            Result result = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3 });

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void EnterTheRoom_ShouldFail_If_ChatRoomDoesNotExist()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));

            Result result = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void EnterTheRoom_ShouldFail_If_UserIsAlreadyInTheChatRoom()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result firstEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result secondEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });

            Assert.True(firstEntryResult.IsSuccess);
            Assert.True(secondEntryResult.IsFailure);
        }

        [Fact]
        public void EnterTheRoom_ShouldSucceed_If_UserAndChatRoomExist()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result result = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void EnterTheRoomEvent_ShouldBeCreated_If_EnterTheRoomSucceeded()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result result = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });

            Assert.True(result.IsSuccess);

            Assert.Single(_inMemoryDB.Events);

            Assert.Equal(_inMemoryDB.Users[0].Id, _inMemoryDB.Events[0].User.Id);
            Assert.Equal(_inMemoryDB.ChatRooms[0].Id, _inMemoryDB.Events[0].ChatRoomId);

            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[0]);
        }

        public void Dispose()
        {
            _inMemoryDB.ClearDB();
        }

    }
}
