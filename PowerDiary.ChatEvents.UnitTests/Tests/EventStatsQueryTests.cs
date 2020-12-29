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
    public class EventStatsQueryTests : IClassFixture<DbFixture>, IDisposable
    {
        private readonly IInMemoryDB _inMemoryDB;
        private readonly IChatEventRepository _chatEventRepository;
        private readonly IChatEventService _chatEventService;

        public EventStatsQueryTests(DbFixture fixture)
        {
            _chatEventRepository = fixture.ServiceProvider.GetService<IChatEventRepository>();
            _chatEventService = fixture.ServiceProvider.GetService<IChatEventService>();
            _inMemoryDB = fixture.ServiceProvider.GetService<IInMemoryDB>();
        }

        [Fact]
        public void GetChatEventStats_WrongFormat_For_FromDate_ShouldFail()
        {
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result result = _chatEventService.GetChatEventStats(5, 1, "25-12-2020-wrong-13:49:17", "25-12-2020T13:49:17");

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void GetChatEventStats_WrongFormat_For_ToDate_ShouldFail()
        {
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result result = _chatEventService.GetChatEventStats(5, 1, "25-12-2020T13:49:17", "25-12-2020-wrong-13:49:17");

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void GetChatEventStats_WrongFormat_For_FromAndToDate_ShouldFail()
        {
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result result = _chatEventService.GetChatEventStats(5, 1, "25-12-2020-wrong-13:49:17", "25-12-2020-wrong-13:49:17");

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void GetChatEventStats_FromDate_GreaterThan_ToDate_ShouldFail()
        {
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result result = _chatEventService.GetChatEventStats(5, 1, "25-12-2020T13:49:17", "23-12-2020T13:49");

            Assert.True(result.IsFailure);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(17)]
        [InlineData(18)]
        [InlineData(19)]
        [InlineData(20)]
        [InlineData(21)]
        [InlineData(22)]
        [InlineData(23)]
        public void GetChatEventStats_ShouldFail_If_GranularityIsNotDivisibleBy24(int granularity)
        {
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result result = _chatEventService.GetChatEventStats(5, granularity, null, null);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void GetChatEventStats_ShouldFail_If_ChatRoomDoesNotExist()
        {
            Result result = _chatEventService.GetChatEventStats(1, 1, "25-12-2020T13:49:17", "26-12-2020T13:49:17");

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void GetChatEventStats_ShouldGetAllCorrectEventStats_If_NoDatesWereSupplied()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result userEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherUserEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result commentResult = _chatEventRepository.Comment(new CommentDTO() { UserId = 3, ChatRoomId = 5, Text = "Some Comment." });
            Result highFiveResult = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });

            Result<IEnumerable<EventStatsViewModel>> eventsResult = _chatEventRepository.GetChatEventStats(5, 1, null, null);

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

            var eventsResultList = eventsResult.Value.ToList();
            Assert.Single(eventsResult.Value);
            Assert.Equal(2, eventsResultList[0].PeopleEnteredCount);
            Assert.Equal(1, eventsResultList[0].CommentCount);
            Assert.Equal(1, eventsResultList[0].PeopleHighFivingCount);
            Assert.Equal(1, eventsResultList[0].PeopleHighFivedCount);
            Assert.Equal(0, eventsResultList[0].PeopleLeftCount);
        }

        [Fact]
        public void GetChatEventStats_ShouldGetCorrectEventStats_After_FromDate()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result userEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherUserEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result commentResult = _chatEventRepository.Comment(new CommentDTO() { UserId = 3, ChatRoomId = 5, Text = "Some Comment." });
            Result highFiveResult = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });

            DateTime oneDayInTheFutureDate = GetTodaysDateWithYearsAdded(1);
            Result<IEnumerable<EventStatsViewModel>> firstEventsResult = _chatEventRepository.GetChatEventStats(5, 1, oneDayInTheFutureDate, null);

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

            Result firstLeavingResult = _chatEventRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result secondLeavingResult = _chatEventRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });

            _inMemoryDB.Events[4].TimeStamp = GetTodaysDateWithYearsAdded(2);
            _inMemoryDB.Events[5].TimeStamp = GetTodaysDateWithYearsAdded(2);
            Result<IEnumerable<EventStatsViewModel>> secondEventsResult = _chatEventRepository.GetChatEventStats(5, 1, oneDayInTheFutureDate, null);

            Assert.True(firstLeavingResult.IsSuccess);
            Assert.True(secondLeavingResult.IsSuccess);

            Assert.Equal(6, _inMemoryDB.Events.Count);

            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[4]);
            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[5]);

            var secondEventsResultList = secondEventsResult.Value.ToList();
            Assert.Single(secondEventsResult.Value);
            Assert.Equal(0, secondEventsResultList[0].PeopleEnteredCount);
            Assert.Equal(0, secondEventsResultList[0].CommentCount);
            Assert.Equal(0, secondEventsResultList[0].PeopleHighFivingCount);
            Assert.Equal(0, secondEventsResultList[0].PeopleHighFivedCount);
            Assert.Equal(2, secondEventsResultList[0].PeopleLeftCount);
        }

        [Fact]
        public void GetChatEventStats_ShouldGetCorrectEventStats_Before_ToDate()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result userEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherUserEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result commentResult = _chatEventRepository.Comment(new CommentDTO() { UserId = 3, ChatRoomId = 5, Text = "Some Comment." });
            Result highFiveResult = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });

            Result<IEnumerable<EventStatsViewModel>> firstEventsResult = _chatEventRepository.GetChatEventStats(5, 1, null, new DateTime(2020, 12, 25));

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

            Result firstLeavingResult = _chatEventRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result secondLeavingResult = _chatEventRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });

            DateTime olderDate = new DateTime(2020, 12, 20);
            _inMemoryDB.Events[4].TimeStamp = olderDate;
            _inMemoryDB.Events[5].TimeStamp = olderDate;
            Result<IEnumerable<EventStatsViewModel>> secondEventsResult = _chatEventRepository.GetChatEventStats(5, 1, null, new DateTime(2020, 12, 25));

            Assert.True(firstLeavingResult.IsSuccess);
            Assert.True(secondLeavingResult.IsSuccess);

            Assert.Equal(6, _inMemoryDB.Events.Count);

            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[4]);
            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[5]);

            var secondEventsResultList = secondEventsResult.Value.ToList();
            Assert.Single(secondEventsResult.Value);
            Assert.Equal(0, secondEventsResultList[0].PeopleEnteredCount);
            Assert.Equal(0, secondEventsResultList[0].CommentCount);
            Assert.Equal(0, secondEventsResultList[0].PeopleHighFivingCount);
            Assert.Equal(0, secondEventsResultList[0].PeopleHighFivedCount);
            Assert.Equal(2, secondEventsResultList[0].PeopleLeftCount);
        }

        [Fact]
        public void GetChatEventStats_ShouldGetCorrectEvents_InRange()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result userEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherUserEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result commentResult = _chatEventRepository.Comment(new CommentDTO() { UserId = 3, ChatRoomId = 5, Text = "Some Comment." });
            Result highFiveResult = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });
            Result firstLeavingResult = _chatEventRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result secondLeavingResult = _chatEventRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });

            _inMemoryDB.Events[0].TimeStamp = new DateTime(2020, 12, 20, 13, 30, 00);
            _inMemoryDB.Events[1].TimeStamp = new DateTime(2020, 12, 20, 13, 35, 00);
            _inMemoryDB.Events[2].TimeStamp = new DateTime(2020, 12, 20, 13, 30, 00);
            _inMemoryDB.Events[3].TimeStamp = new DateTime(2020, 12, 25, 15, 10, 00);
            _inMemoryDB.Events[4].TimeStamp = new DateTime(2020, 12, 25, 15, 15, 00);
            _inMemoryDB.Events[5].TimeStamp = new DateTime(2020, 12, 25, 15, 20, 00);

            Result<IEnumerable<EventStatsViewModel>> firstEventsResult = _chatEventRepository.GetChatEventStats(5, 1, new DateTime(2020, 12, 19), new DateTime(2020, 12, 23));
            Result<IEnumerable<EventStatsViewModel>> secondEventsResult = _chatEventRepository.GetChatEventStats(5, 1, new DateTime(2020, 12, 24), new DateTime(2020, 12, 28));

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
            Assert.Single(firstResultList);
            Assert.Equal(2, firstResultList[0].PeopleEnteredCount);
            Assert.Equal(1, firstResultList[0].CommentCount);
            Assert.Equal(0, firstResultList[0].PeopleHighFivingCount);
            Assert.Equal(0, firstResultList[0].PeopleHighFivedCount);
            Assert.Equal(0, firstResultList[0].PeopleLeftCount);

            var secondResultList = secondEventsResult.Value.ToList();
            Assert.Single(secondResultList);
            Assert.Equal(0, secondResultList[0].PeopleEnteredCount);
            Assert.Equal(0, secondResultList[0].CommentCount);
            Assert.Equal(1, secondResultList[0].PeopleHighFivingCount);
            Assert.Equal(1, secondResultList[0].PeopleHighFivedCount);
            Assert.Equal(2, secondResultList[0].PeopleLeftCount);
        }

        [Theory]
        [InlineData(1, 4)]
        [InlineData(2, 2)]
        [InlineData(3, 2)]
        [InlineData(4, 1)]
        [InlineData(6, 1)]
        public void GetChatEventStats_ShouldGetCorrectStats_BasedOn_Granularity(int granularity, int expectedGroups)
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.Users.Add(new User(8, "Angela"));
            _inMemoryDB.Users.Add(new User(9, "Sophie"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result user1EntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result user2EntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result commentResult = _chatEventRepository.Comment(new CommentDTO() { UserId = 3, ChatRoomId = 5, Text = "Some Comment." });
            Result highFiveResult = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, OtherUserId = 6, ChatRoomId = 5 });
            Result firstLeavingResult = _chatEventRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result secondLeavingResult = _chatEventRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result user3EntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 8, ChatRoomId = 5 });
            Result user4EntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 9, ChatRoomId = 5 });

            _inMemoryDB.Events[0].TimeStamp = new DateTime(2020, 12, 20, 12, 30, 00);
            _inMemoryDB.Events[1].TimeStamp = new DateTime(2020, 12, 20, 12, 35, 00);
            _inMemoryDB.Events[2].TimeStamp = new DateTime(2020, 12, 20, 13, 30, 00);
            _inMemoryDB.Events[3].TimeStamp = new DateTime(2020, 12, 20, 13, 10, 00);
            _inMemoryDB.Events[4].TimeStamp = new DateTime(2020, 12, 20, 14, 15, 00);
            _inMemoryDB.Events[5].TimeStamp = new DateTime(2020, 12, 20, 14, 20, 00);
            _inMemoryDB.Events[6].TimeStamp = new DateTime(2020, 12, 20, 15, 25, 00);
            _inMemoryDB.Events[7].TimeStamp = new DateTime(2020, 12, 20, 15, 30, 00);


            Result<IEnumerable<EventStatsViewModel>> eventsResult = _chatEventRepository.GetChatEventStats(5, granularity, new DateTime(2020, 12, 19), new DateTime(2020, 12, 23));

            Assert.True(user1EntryResult.IsSuccess);
            Assert.True(user2EntryResult.IsSuccess);
            Assert.True(commentResult.IsSuccess);
            Assert.True(highFiveResult.IsSuccess);
            Assert.True(eventsResult.IsSuccess);
            Assert.True(firstLeavingResult.IsSuccess);
            Assert.True(secondLeavingResult.IsSuccess);
            Assert.True(user3EntryResult.IsSuccess);
            Assert.True(user4EntryResult.IsSuccess);

            Assert.Equal(8, _inMemoryDB.Events.Count);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[0]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[1]);
            Assert.IsType<CommentEvent>(_inMemoryDB.Events[2]);
            Assert.IsType<HighFiveEvent>(_inMemoryDB.Events[3]);
            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[4]);
            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[5]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[6]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[7]);

            var resultList = eventsResult.Value.ToList();
            Assert.Equal(resultList.Count, expectedGroups);

            switch (granularity)
            {
                case 1:
                    Assert.Equal(2, resultList[0].PeopleEnteredCount);
                    Assert.Equal(0, resultList[0].CommentCount);
                    Assert.Equal(0, resultList[0].PeopleHighFivingCount);
                    Assert.Equal(0, resultList[0].PeopleHighFivedCount);
                    Assert.Equal(0, resultList[0].PeopleLeftCount);

                    Assert.Equal(0, resultList[1].PeopleEnteredCount);
                    Assert.Equal(1, resultList[1].CommentCount);
                    Assert.Equal(1, resultList[1].PeopleHighFivingCount);
                    Assert.Equal(1, resultList[1].PeopleHighFivedCount);
                    Assert.Equal(0, resultList[1].PeopleLeftCount);

                    Assert.Equal(0, resultList[2].PeopleEnteredCount);
                    Assert.Equal(0, resultList[2].CommentCount);
                    Assert.Equal(0, resultList[2].PeopleHighFivingCount);
                    Assert.Equal(0, resultList[2].PeopleHighFivedCount);
                    Assert.Equal(2, resultList[2].PeopleLeftCount);

                    Assert.Equal(2, resultList[3].PeopleEnteredCount);
                    Assert.Equal(0, resultList[3].CommentCount);
                    Assert.Equal(0, resultList[3].PeopleHighFivingCount);
                    Assert.Equal(0, resultList[3].PeopleHighFivedCount);
                    Assert.Equal(0, resultList[3].PeopleLeftCount);
                    break;

                case 2:
                    Assert.Equal(2, resultList[0].PeopleEnteredCount);
                    Assert.Equal(1, resultList[0].CommentCount);
                    Assert.Equal(1, resultList[0].PeopleHighFivingCount);
                    Assert.Equal(1, resultList[0].PeopleHighFivedCount);
                    Assert.Equal(0, resultList[0].PeopleLeftCount);

                    Assert.Equal(2, resultList[1].PeopleEnteredCount);
                    Assert.Equal(0, resultList[1].CommentCount);
                    Assert.Equal(0, resultList[1].PeopleHighFivingCount);
                    Assert.Equal(0, resultList[1].PeopleHighFivedCount);
                    Assert.Equal(2, resultList[1].PeopleLeftCount);
                    break;

                case 3:
                    Assert.Equal(2, resultList[0].PeopleEnteredCount);
                    Assert.Equal(1, resultList[0].CommentCount);
                    Assert.Equal(1, resultList[0].PeopleHighFivingCount);
                    Assert.Equal(1, resultList[0].PeopleHighFivedCount);
                    Assert.Equal(2, resultList[0].PeopleLeftCount);

                    Assert.Equal(2, resultList[1].PeopleEnteredCount);
                    Assert.Equal(0, resultList[1].CommentCount);
                    Assert.Equal(0, resultList[1].PeopleHighFivingCount);
                    Assert.Equal(0, resultList[1].PeopleHighFivedCount);
                    Assert.Equal(0, resultList[1].PeopleLeftCount);
                    break;

                case 4:
                    Assert.Equal(4, resultList[0].PeopleEnteredCount);
                    Assert.Equal(1, resultList[0].CommentCount);
                    Assert.Equal(1, resultList[0].PeopleHighFivingCount);
                    Assert.Equal(1, resultList[0].PeopleHighFivedCount);
                    Assert.Equal(2, resultList[0].PeopleLeftCount);
                    break;
            }
           
            
        }

        [Fact]
        public void GetChatEventStats_ShouldGetDistinctPeopleWhoEntered()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result userFirstEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherFirstUserEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result userLeavingResult = _chatEventRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherUserLeavingResult = _chatEventRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result userSecondEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherSecondUserEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });

            Result<IEnumerable<EventStatsViewModel>> eventsResult = _chatEventRepository.GetChatEventStats(5, 1, null, null);

            Assert.True(userFirstEntryResult.IsSuccess);
            Assert.True(otherFirstUserEntryResult.IsSuccess);
            Assert.True(userLeavingResult.IsSuccess);
            Assert.True(otherUserLeavingResult.IsSuccess);
            Assert.True(eventsResult.IsSuccess);
            Assert.True(userSecondEntryResult.IsSuccess);
            Assert.True(otherSecondUserEntryResult.IsSuccess);

            Assert.Equal(6, _inMemoryDB.Events.Count);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[0]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[1]);
            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[2]);
            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[3]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[4]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[5]);

            var eventsResultList = eventsResult.Value.ToList();
            Assert.Single(eventsResult.Value);
            Assert.Equal(2, eventsResultList[0].PeopleEnteredCount);
            Assert.Equal(0, eventsResultList[0].CommentCount);
            Assert.Equal(0, eventsResultList[0].PeopleHighFivingCount);
            Assert.Equal(0, eventsResultList[0].PeopleHighFivedCount);
            Assert.Equal(2, eventsResultList[0].PeopleLeftCount);
        }

        [Fact]
        public void GetChatEventStats_ShouldGetDistinctPeopleWhoLeft()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result userFirstEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherFirstUserEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result userFirstLeavingResult = _chatEventRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherFirstUserLeavingResult = _chatEventRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result userSecondEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherSecondUserEntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result userSecondLeavingResult = _chatEventRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result otherUserSecondLeavingResult = _chatEventRepository.LeaveTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });

            Result<IEnumerable<EventStatsViewModel>> eventsResult = _chatEventRepository.GetChatEventStats(5, 1, null, null);

            Assert.True(userFirstEntryResult.IsSuccess);
            Assert.True(otherFirstUserEntryResult.IsSuccess);
            Assert.True(userFirstLeavingResult.IsSuccess);
            Assert.True(otherFirstUserLeavingResult.IsSuccess);
            Assert.True(eventsResult.IsSuccess);
            Assert.True(userSecondEntryResult.IsSuccess);
            Assert.True(otherSecondUserEntryResult.IsSuccess);
            Assert.True(userSecondLeavingResult.IsSuccess);
            Assert.True(otherUserSecondLeavingResult.IsSuccess);

            Assert.Equal(8, _inMemoryDB.Events.Count);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[0]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[1]);
            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[2]);
            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[3]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[4]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[5]);
            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[6]);
            Assert.IsType<LeaveTheRoomEvent>(_inMemoryDB.Events[7]);

            var eventsResultList = eventsResult.Value.ToList();
            Assert.Single(eventsResult.Value);
            Assert.Equal(2, eventsResultList[0].PeopleEnteredCount);
            Assert.Equal(0, eventsResultList[0].CommentCount);
            Assert.Equal(0, eventsResultList[0].PeopleHighFivingCount);
            Assert.Equal(0, eventsResultList[0].PeopleHighFivedCount);
            Assert.Equal(2, eventsResultList[0].PeopleLeftCount);
        }

        [Fact]
        public void GetChatEventStats_ShouldGetDistinctPeopleWhoHighFived()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.Users.Add(new User(8, "Angela"));
            _inMemoryDB.Users.Add(new User(9, "Sophie"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result user1EntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result user2EntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result user3EntryLeavingResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 8, ChatRoomId = 5 });
            Result user4EntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 9, ChatRoomId = 5 });
            Result user1HighFivingUser2 = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, ChatRoomId = 5, OtherUserId = 6 });
            Result user1HighFivingUser3 = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, ChatRoomId = 5, OtherUserId = 8 });
            Result user1HighFivingUser4 = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 3, ChatRoomId = 5, OtherUserId = 9 });


            Result<IEnumerable<EventStatsViewModel>> eventsResult = _chatEventRepository.GetChatEventStats(5, 1, null, null);

            Assert.True(user1EntryResult.IsSuccess);
            Assert.True(user2EntryResult.IsSuccess);
            Assert.True(user3EntryLeavingResult.IsSuccess);
            Assert.True(user4EntryResult.IsSuccess);
            Assert.True(eventsResult.IsSuccess);
            Assert.True(user1HighFivingUser2.IsSuccess);
            Assert.True(user1HighFivingUser3.IsSuccess);
            Assert.True(user1HighFivingUser4.IsSuccess);

            Assert.Equal(7, _inMemoryDB.Events.Count);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[0]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[1]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[2]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[3]);
            Assert.IsType<HighFiveEvent>(_inMemoryDB.Events[4]);
            Assert.IsType<HighFiveEvent>(_inMemoryDB.Events[5]);
            Assert.IsType<HighFiveEvent>(_inMemoryDB.Events[6]);

            var eventsResultList = eventsResult.Value.ToList();
            Assert.Single(eventsResult.Value);
            Assert.Equal(4, eventsResultList[0].PeopleEnteredCount);
            Assert.Equal(0, eventsResultList[0].CommentCount);
            Assert.Equal(1, eventsResultList[0].PeopleHighFivingCount);
            Assert.Equal(3, eventsResultList[0].PeopleHighFivedCount);
            Assert.Equal(0, eventsResultList[0].PeopleLeftCount);
        }

        [Fact]
        public void GetChatEventStats_ShouldGetDistinctPeopleWhoGotHighFived()
        {
            _inMemoryDB.Users.Add(new User(3, "Brad"));
            _inMemoryDB.Users.Add(new User(6, "Luke"));
            _inMemoryDB.Users.Add(new User(8, "Angela"));
            _inMemoryDB.Users.Add(new User(9, "Sophie"));
            _inMemoryDB.ChatRooms.Add(new ChatRoom(5, "CollegeBuddies"));

            Result user1EntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 3, ChatRoomId = 5 });
            Result user2EntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 6, ChatRoomId = 5 });
            Result user3EntryLeavingResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 8, ChatRoomId = 5 });
            Result user4EntryResult = _chatEventRepository.EnterTheRoom(new UserRoomDTO() { UserId = 9, ChatRoomId = 5 });
            Result user2HighFivingUser1 = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 6, ChatRoomId = 5, OtherUserId = 3 });
            Result user3HighFivingUser1 = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 8, ChatRoomId = 5, OtherUserId = 3 });
            Result user4HighFivingUser1 = _chatEventRepository.HighFive(new HighFiveDTO() { UserId = 9, ChatRoomId = 5, OtherUserId = 3 });

            Result<IEnumerable<EventStatsViewModel>> eventsResult = _chatEventRepository.GetChatEventStats(5, 1, null, null);

            Assert.True(user1EntryResult.IsSuccess);
            Assert.True(user2EntryResult.IsSuccess);
            Assert.True(user3EntryLeavingResult.IsSuccess);
            Assert.True(user4EntryResult.IsSuccess);
            Assert.True(eventsResult.IsSuccess);
            Assert.True(user2HighFivingUser1.IsSuccess);
            Assert.True(user3HighFivingUser1.IsSuccess);
            Assert.True(user4HighFivingUser1.IsSuccess);

            Assert.Equal(7, _inMemoryDB.Events.Count);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[0]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[1]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[2]);
            Assert.IsType<EnterTheRoomEvent>(_inMemoryDB.Events[3]);
            Assert.IsType<HighFiveEvent>(_inMemoryDB.Events[4]);
            Assert.IsType<HighFiveEvent>(_inMemoryDB.Events[5]);
            Assert.IsType<HighFiveEvent>(_inMemoryDB.Events[6]);

            var eventsResultList = eventsResult.Value.ToList();
            Assert.Single(eventsResult.Value);
            Assert.Equal(4, eventsResultList[0].PeopleEnteredCount);
            Assert.Equal(0, eventsResultList[0].CommentCount);
            Assert.Equal(3, eventsResultList[0].PeopleHighFivingCount);
            Assert.Equal(1, eventsResultList[0].PeopleHighFivedCount);
            Assert.Equal(0, eventsResultList[0].PeopleLeftCount);
        }

        // Adding dates in the future by incrementing years, 
        // in order not to deal with end of month problem with days
        private DateTime GetTodaysDateWithYearsAdded(int years)
        {
            return new DateTime((DateTime.Now.Year + years), DateTime.Now.Month, DateTime.Now.Day,
                DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        }

        public void Dispose()
        {
            _inMemoryDB.ClearDB();
        }
    }
}
