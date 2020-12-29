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
    public class HighFiveTests : IClassFixture<DbFixture>, IDisposable
    {
        private readonly IInMemoryDB _inMemoryDB;
        private readonly IChatEventRepository _chatEventRepository;

        public HighFiveTests(DbFixture fixture)
        {
            _chatEventRepository = fixture.ServiceProvider.GetService<IChatEventRepository>();
            _inMemoryDB = fixture.ServiceProvider.GetService<IInMemoryDB>();
        }

        [Fact]
        public void HighFive_ShouldFail_If_BothUsersDoNotExist()
        {
            Result result = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6 });

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void HighFive_ShouldFail_If_UserDoesNotExist()
        {
            _inMemoryDB.Users.Add(new User(6, "Luke"));

            Result result = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6 });

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void HighFive_ShouldFail_If_OtherUserDoesNotExist()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));

            Result result = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6 });

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void HighFive_ShouldFail_If_ChatRoomDoesNotExist()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));

            Result result = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void HighFive_ShouldFail_If_BothUsersAreNotInTheChatRoom()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result result = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void HighFive_ShouldFail_If_UserIsNotInTheChatRoom()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result otherUserEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result highFiveResult = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });

            Assert.True(otherUserEntryResult.IsSuccess);
            Assert.True(highFiveResult.IsFailure);
        }

        [Fact]
        public void HighFive_ShouldFail_If_OtherUserIsNotInTheChatRoom()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result firstUserEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result highFiveResult = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });

            Assert.True(firstUserEntryResult.IsSuccess);
            Assert.True(highFiveResult.IsFailure);
        }

        [Fact]
        public void HighFive_ShouldFail_If_UserSelfFived()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result userEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result userSelfFiveResult = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, ChatRoomId = 5, OtherUserId = 3 });

            Assert.True(userEntryResult.IsSuccess);
            Assert.True(userSelfFiveResult.IsFailure);
        }

        [Fact]
        public void HighFive_ShouldSucceed_If_BothUsersAreInChatRoom()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result userEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherUserEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result highFiveResult = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });

            Assert.True(userEntryResult.IsSuccess);
            Assert.True(otherUserEntryResult.IsSuccess);
            Assert.True(highFiveResult.IsSuccess);
        }

        [Fact]
        public void HighFiveEvent_ShouldBeCreated_If_HighFiveSucceeded()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result userEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherUserEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result highFiveResult = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });

            Assert.True(userEntryResult.IsSuccess);
            Assert.True(otherUserEntryResult.IsSuccess);
            Assert.True(highFiveResult.IsSuccess);

            Assert.Equal(3, _inMemoryDB.Events.Count);

            Assert.Equal(_inMemoryDB.Users[0].Id, _inMemoryDB.Events[0].User.Id);
            Assert.Equal(_inMemoryDB.ChatRooms[0].Id, _inMemoryDB.Events[0].ChatRoomId);

            Assert.Equal(_inMemoryDB.Users[1].Id, _inMemoryDB.Events[1].User.Id);
            Assert.Equal(_inMemoryDB.ChatRooms[0].Id, _inMemoryDB.Events[1].ChatRoomId);

            Assert.Equal(_inMemoryDB.Users[0].Id, _inMemoryDB.Events[2].User.Id);
            Assert.Equal(_inMemoryDB.Users[1].Id, (_inMemoryDB.Events[2] as HighFiveEvent).OtherUser.Id);
            Assert.Equal(_inMemoryDB.ChatRooms[0].Id, _inMemoryDB.Events[2].ChatRoomId);

            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[0]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[1]);
            Assert.IsType<HighFiveEvent>(_inMemoryDB.Events[2]);
        }

        public void Dispose()
        {
            _inMemoryDB.ClearDB();
        }
    }
}
