using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using PowerDiary.ChatEvents.Core.Models;
using PowerDiary.ChatEvents.Core.Persistence;
using PowerDiary.ChatEvents.Services.Interfaces;
using PowerDiary.ChatEvents.Services.ViewModels;

namespace PowerDiary.ChatEvents.Core.Repositories
{
    public class ChatEventRepository : IChatEventRepository
    {
        private readonly IInMemoryDB _inMemoryDB;

        public ChatEventRepository(IInMemoryDB inMemoryDB)
        {
            _inMemoryDB = inMemoryDB;
        }

        public Result AddEvent(IEvent @event)
        {
            if(@event is EnterTheRoomEvent || @event is LeaveTheRoomEvent || @event is CommentEvent || @event is HighFiveEvent)
            {
                Event eventObj = (Event)@event;
                eventObj.Id = GetNextEventId();
                _inMemoryDB.Events.Add(eventObj);

                return Result.Success();
            }
            else
            {
                return Result.Failure("Unrecognizable Event!");
            }
        }

        public Result<IEnumerable<EventViewModel>> GetChatEvents(int chatRoomId, DateTime? dateFrom, DateTime? dateTo)
        {
            ChatRoom chatRoom = _inMemoryDB.ChatRooms.FirstOrDefault(cr => cr.Id == chatRoomId);

            Result validationResult = ValidateChatRoom(chatRoom);
            if (validationResult.IsFailure)
            {
                return Result.Failure<IEnumerable<EventViewModel>>(validationResult.Error);
            }

            var result = _inMemoryDB.Events
                .Where(e => e.ChatRoomId == chatRoomId && IsInRange(e.TimeStamp, dateFrom, dateTo))
                .OrderBy(e => e.TimeStamp)
                .Select(e => ViewModelConverter.ConvertToEventViewModel(e, chatRoom.Name));

            return Result.Success(result);
        }

        public Result<IEnumerable<EventStatsViewModel>> GetChatEventStats(int chatRoomId, int granularity, DateTime? dateFrom, DateTime? dateTo)
        {
            ChatRoom chatRoom = _inMemoryDB.ChatRooms.FirstOrDefault(cr => cr.Id == chatRoomId);

            Result validationResult = ValidateChatRoom(chatRoom);
            if (validationResult.IsFailure)
            {
                return Result.Failure<IEnumerable<EventStatsViewModel>>(validationResult.Error);
            }

            var result = _inMemoryDB.Events
                .Where(e => e.ChatRoomId == chatRoomId && IsInRange(e.TimeStamp, dateFrom, dateTo))
                .OrderBy(e => e.TimeStamp)
                .GroupBy(e =>
                {
                    var stamp = e.TimeStamp;
                    stamp = stamp.AddHours(-(stamp.Hour % granularity));
                    stamp = stamp.AddMinutes(-stamp.Minute);
                    stamp = stamp.AddMilliseconds(-stamp.Millisecond - 1000 * stamp.Second);
                    return stamp;
                })
                .Select(g => new EventStatsViewModel
                {
                    Hour = g.Key,
                    PeopleEnteredCount = g.Where(e => e is EnterTheRoomEvent).Select(e => e.User).Distinct().Count(),
                    PeopleLeftCount = g.Where(e => e is LeaveTheRoomEvent).Select(e => e.User).Distinct().Count(),
                    CommentCount = g.Count(e => e is CommentEvent),
                    PeopleHighFivingCount = g.Where(e => e is HighFiveEvent).Select(e => e.User).Distinct().Count(),
                    PeopleHighFivedCount = g.Where(e => e is HighFiveEvent).Select(e => (e as HighFiveEvent).OtherUser).Distinct().Count()
                });

            return Result.Success(result);
        }

        private Result ValidateChatRoom(ChatRoom chatRoom)
        {
            if (chatRoom == null)
            {
                return Result.Failure("Chat room does not exist!");
            }

            return Result.Success();
        }

        private bool IsInRange(DateTime date, DateTime? dateFrom, DateTime? dateTo)
        {
            if (dateFrom == null && dateTo == null)
            {
                return true;
            }

            if (dateFrom == null)
            {
                return date < dateTo;
            }

            if (dateTo == null)
            {
                return date > dateFrom;
            }

            return date < dateTo && date > dateFrom;
        }

        private long GetNextEventId()
        {
            long maxId = _inMemoryDB.Events.Count == 0 ? 0 : _inMemoryDB.Events.Max(e => e.Id);
            return maxId + 1;
        }
    }
}
