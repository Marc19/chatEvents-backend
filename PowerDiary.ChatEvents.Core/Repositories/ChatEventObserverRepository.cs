using System.Linq;
using CSharpFunctionalExtensions;
using PowerDiary.ChatEvents.Core.Models;
using PowerDiary.ChatEvents.Core.Persistence;
using PowerDiary.ChatEvents.Services.Interfaces;

namespace PowerDiary.ChatEvents.Core.Repositories
{
    public class ChatEventObserverRepository : IChatEventObserverRepository
    {
        private readonly IInMemoryDB _inMemoryDB;

        public ChatEventObserverRepository(IInMemoryDB inMemoryDB)
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
        
        private long GetNextEventId()
        {
            long maxId = _inMemoryDB.Events.Count == 0 ? 0 : _inMemoryDB.Events.Max(e => e.Id);
            return maxId + 1;
        }
    }
}
