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
    public class CommentTests : IClassFixture<DbFixture>, IDisposable
    {
        private readonly IInMemoryDB _inMemoryDB;
        private readonly IChatEventRepository _chatEventRepository;

        public CommentTests(DbFixture fixture)
        {
            _chatEventRepository = fixture.ServiceProvider.GetService<IChatEventRepository>();
            _inMemoryDB = fixture.ServiceProvider.GetService<IInMemoryDB>();
        }

        [Fact]
        public void Comment_ShouldFail_If_UserDoesNotExist()
        {
            Result result = _chatEventRepository.Comment(new CommentDTO() { UserId = 3, Text = "Some Comment." });

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Comment_ShouldFail_If_ChatRoomDoesNotExist()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));

            Result result = _chatEventRepository.Comment(new CommentDTO() { UserId = 3, ChatRoomId = 5, Text = "Some Comment." });

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Comment_ShouldFail_If_UserIsNotInTheChatRoom()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result result = _chatEventRepository.Comment(new CommentDTO() { UserId = 3, ChatRoomId = 5, Text = "Some Comment." });

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Comment_ShouldSucceed_If_UserIsInChatRoom()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result entryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result commentResult = _chatEventRepository.Comment(new CommentDTO() { UserId = 3, ChatRoomId = 5, Text = "Some Comment." });

            Assert.True(entryResult.IsSuccess);
            Assert.True(commentResult.IsSuccess);
        }

        [Fact]
        public void CommentEvent_ShouldBeCreated_If_CommentSucceeded()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result entryesult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result commentResult = _chatEventRepository.Comment(new CommentDTO() { UserId = 3, ChatRoomId = 5, Text = "Some Comment." });

            Assert.True(entryesult.IsSuccess);
            Assert.True(commentResult.IsSuccess);

            Assert.Equal(2, _inMemoryDB.Events.Count);

            Assert.Equal(_inMemoryDB.Users[0].Id, _inMemoryDB.Events[0].User.Id);
            Assert.Equal(_inMemoryDB.ChatRooms[0].Id, _inMemoryDB.Events[0].ChatRoomId);

            Assert.Equal(_inMemoryDB.Users[0].Id, _inMemoryDB.Events[0].User.Id);
            Assert.Equal(_inMemoryDB.ChatRooms[0].Id, _inMemoryDB.Events[0].ChatRoomId);

            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[0]);
            Assert.IsType<CommentEvent>(_inMemoryDB.Events[1]);
        }

        public void Dispose()
        {
            _inMemoryDB.ClearDB();
        }
    }
}
