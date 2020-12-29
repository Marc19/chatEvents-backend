using PowerDiary.ChatEvents.Services.Interfaces;

namespace PowerDiary.ChatEvents.Core.Observers
{
    public class ChatRoomEventObserver : IChatRoomObserver
    {
        private readonly IChatEventObserverRepository _chatEventObserverRepository;

        public ChatRoomEventObserver(IChatEventObserverRepository chatEventObserverRepository)
        {
            _chatEventObserverRepository = chatEventObserverRepository;
        }

        public void Update(IEvent @event)
        {
            _chatEventObserverRepository.AddEvent(@event);
        }
    }
}
