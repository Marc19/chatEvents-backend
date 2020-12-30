using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using PowerDiary.ChatEvents.Core.Models;
using PowerDiary.ChatEvents.Core.Persistence;
using PowerDiary.ChatEvents.Services.DTOs;
using PowerDiary.ChatEvents.Services.Interfaces;
using PowerDiary.ChatEvents.Services.ViewModels;
using Xunit;

namespace PowerDiary.ChatEvents.UnitTests.Tests
{
    public class EventQueryTests : IClassFixture<DbFixture>, IDisposable
    {
        private readonly IInMemoryDB _inMemoryDB;
        private readonly IChatActionRepository _chatActionRepository;
        private readonly IChatEventRepository _chatEventRepository;
        private readonly IChatEventService _chatEventService;

        public EventQueryTests(DbFixture fixture)
        {
            _chatActionRepository = fixture.ServiceProvider.GetService<IChatActionRepository>();
            _chatEventRepository = fixture.ServiceProvider.GetService<IChatEventRepository>();
            _chatEventService = fixture.ServiceProvider.GetService<IChatEventService>();
            _inMemoryDB = fixture.ServiceProvider.GetService<IInMemoryDB>();
        }

        [Fact]
        public void GetChatEvents_WrongFormat_For_FromDate_ShouldFail()
        {
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result result = _chatEventService.GetChatEvents(5, "25/12/2020-wrong-13:49:17", "25/12/2020T13:49:17");

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void GetChatEvents_WrongFormat_For_ToDate_ShouldFail()
        {
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result result = _chatEventService.GetChatEvents(5, "25/12/2020T13:49:17", "25/12/2020-wrong-13:49:17");

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void GetChatEvents_WrongFormat_For_FromAndToDate_ShouldFail()
        {
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result result = _chatEventService.GetChatEvents(5, "25/12/2020-wrong-13:49:17", "25/12/2020-wrong-13:49:17");

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void GetChatEvents_FromDate_GreaterThan_ToDate_ShouldFail()
        {
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result result = _chatEventService.GetChatEvents(5, "25/12/2020T13:49:17", "23/12/2020T13:49:17");

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void GetChatEvents_ShouldFail_If_ChatRoomDoesNotExist()
        {
            Result result = _chatEventService.GetChatEvents(1, "25/12/2020T13:49:17", "26/12/2020T13:49:17");

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void GetChatEvents_ShouldGetAllEvents_If_NoDatesWereSupplied()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result userEntryResult = _chatActionRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherUserEntryResult = _chatActionRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result commentResult = _chatActionRepository.Comment(new CommentDTO() { UserId = 3, ChatRoomId = 5, Text = "Some Comment." });
            Result highFiveResult = _chatActionRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });

            Result<IEnumerable<EventViewModel>> eventsResult = _chatEventRepository.GetChatEvents(5, null, null);

            Assert.True(userEntryResult.IsSuccess);
            Assert.True(otherUserEntryResult.IsSuccess);
            Assert.True(commentResult.IsSuccess);
            Assert.True(highFiveResult.IsSuccess);
            Assert.True(eventsResult.IsSuccess);

            Assert.Equal(4, _inMemoryDB.Events.Count);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[0]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[1]);
            Assert.IsType<CommentEvent>(_inMemoryDB.Events[2]);
            Assert.IsType<HighFiveEvent>(_inMemoryDB.Events[3]);

            Assert.Equal(4, eventsResult.Value.Count());
        }

        [Fact]
        public void GetChatEvents_ShouldGetNoEvents_Before_FromDate()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result userEntryResult = _chatActionRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherUserEntryResult = _chatActionRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result commentResult = _chatActionRepository.Comment(new CommentDTO() { UserId = 3, ChatRoomId = 5, Text = "Some Comment." });
            Result highFiveResult = _chatActionRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });

            DateTime oneDayInTheFutureDate = DateTime.Now.AddDays(1);
            Result<IEnumerable<EventViewModel>> firstEventsResult = _chatEventRepository.GetChatEvents(5, oneDayInTheFutureDate, null);

            Assert.True(userEntryResult.IsSuccess);
            Assert.True(otherUserEntryResult.IsSuccess);
            Assert.True(commentResult.IsSuccess);
            Assert.True(highFiveResult.IsSuccess);
            Assert.True(firstEventsResult.IsSuccess);

            Assert.Equal(4, _inMemoryDB.Events.Count);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[0]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[1]);
            Assert.IsType<CommentEvent>(_inMemoryDB.Events[2]);
            Assert.IsType<HighFiveEvent>(_inMemoryDB.Events[3]);

            Assert.Empty(firstEventsResult.Value);

            Result firstLeavingResult = _chatActionRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result secondLeavingResult = _chatActionRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });

            _inMemoryDB.Events[4].TimeStamp = DateTime.Now.AddDays(2);
            _inMemoryDB.Events[5].TimeStamp = DateTime.Now.AddDays(2);
            Result<IEnumerable<EventViewModel>> secondEventsResult = _chatEventRepository.GetChatEvents(5, oneDayInTheFutureDate, null);

            Assert.True(firstLeavingResult.IsSuccess);
            Assert.True(secondLeavingResult.IsSuccess);

            Assert.Equal(6, _inMemoryDB.Events.Count);

            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[4]);
            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[5]);

            Assert.Equal(2, secondEventsResult.Value.Count());
        }

        [Fact]
        public void GetChatEvents_ShouldGetNoEvents_After_ToDate()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result userEntryResult = _chatActionRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherUserEntryResult = _chatActionRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result commentResult = _chatActionRepository.Comment(new CommentDTO() { UserId = 3, ChatRoomId = 5, Text = "Some Comment." });
            Result highFiveResult = _chatActionRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });

            Result<IEnumerable<EventViewModel>> firstEventsResult = _chatEventRepository.GetChatEvents(5, null, new DateTime(2020, 12, 25));

            Assert.True(userEntryResult.IsSuccess);
            Assert.True(otherUserEntryResult.IsSuccess);
            Assert.True(commentResult.IsSuccess);
            Assert.True(highFiveResult.IsSuccess);
            Assert.True(firstEventsResult.IsSuccess);

            Assert.Equal(4, _inMemoryDB.Events.Count);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[0]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[1]);
            Assert.IsType<CommentEvent>(_inMemoryDB.Events[2]);
            Assert.IsType<HighFiveEvent>(_inMemoryDB.Events[3]);

            Assert.Empty(firstEventsResult.Value);

            Result firstLeavingResult = _chatActionRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result secondLeavingResult = _chatActionRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });

            DateTime olderDate = new DateTime(2020, 12, 20);
            _inMemoryDB.Events[4].TimeStamp = olderDate;
            _inMemoryDB.Events[5].TimeStamp = olderDate;
            Result<IEnumerable<EventViewModel>> secondEventsResult = _chatEventRepository.GetChatEvents(5, null, new DateTime(2020, 12, 25));

            Assert.True(firstLeavingResult.IsSuccess);
            Assert.True(secondLeavingResult.IsSuccess);

            Assert.Equal(6, _inMemoryDB.Events.Count);

            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[4]);
            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[5]);

            Assert.Equal(2, secondEventsResult.Value.Count());
        }

        [Fact]
        public void GetChatEvents_ShouldGetNoEvents_OutOfRange()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result userEntryResult = _chatActionRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherUserEntryResult = _chatActionRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result commentResult = _chatActionRepository.Comment(new CommentDTO() { UserId = 3, ChatRoomId = 5, Text = "Some Comment." });
            Result highFiveResult = _chatActionRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });
            Result firstLeavingResult = _chatActionRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result secondLeavingResult = _chatActionRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });

            _inMemoryDB.Events[0].TimeStamp = new DateTime(2020, 12, 20);
            _inMemoryDB.Events[1].TimeStamp = new DateTime(2020, 12, 21);
            _inMemoryDB.Events[2].TimeStamp = new DateTime(2020, 12, 22);
            _inMemoryDB.Events[3].TimeStamp = new DateTime(2020, 12, 25);
            _inMemoryDB.Events[4].TimeStamp = new DateTime(2020, 12, 26);
            _inMemoryDB.Events[5].TimeStamp = new DateTime(2020, 12, 27);

            Result<IEnumerable<EventViewModel>> firstEventsResult = _chatEventRepository.GetChatEvents(5, new DateTime(2020,12,19), new DateTime(2020, 12, 23));
            Result<IEnumerable<EventViewModel>> secondEventsResult = _chatEventRepository.GetChatEvents(5, new DateTime(2020, 12, 24), new DateTime(2020, 12, 28));

            Assert.True(userEntryResult.IsSuccess);
            Assert.True(otherUserEntryResult.IsSuccess);
            Assert.True(commentResult.IsSuccess);
            Assert.True(highFiveResult.IsSuccess);
            Assert.True(firstEventsResult.IsSuccess);
            Assert.True(firstLeavingResult.IsSuccess);
            Assert.True(secondLeavingResult.IsSuccess);

            Assert.Equal(6, _inMemoryDB.Events.Count);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[0]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[1]);
            Assert.IsType<CommentEvent>(_inMemoryDB.Events[2]);
            Assert.IsType<HighFiveEvent>(_inMemoryDB.Events[3]);
            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[4]);
            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[5]);

            var firstResultList = firstEventsResult.Value.ToList();
            Assert.Equal(3, firstEventsResult.Value.Count());
            Assert.IsType<EnterTheRoomViewModel>(firstResultList[0]);
            Assert.IsType<EnterTheRoomViewModel>(firstResultList[1]);
            Assert.IsType<CommentViewModel>(firstResultList[2]);

            var secondResultList = secondEventsResult.Value.ToList();
            Assert.Equal(3, secondEventsResult.Value.Count());
            Assert.IsType<HighFiveViewModel>(secondResultList[0]);
            Assert.IsType<LeaveTheRoomViewModel>(secondResultList[1]);
            Assert.IsType<LeaveTheRoomViewModel>(secondResultList[2]);
        }

        public void Dispose()
        {
            _inMemoryDB.ClearDB();
        }
    }
}
