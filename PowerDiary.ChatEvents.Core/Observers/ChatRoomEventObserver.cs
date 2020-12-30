using PowerDiary.ChatEvents.Services.Interfaces;

namespace PowerDiary.ChatEvents.Core.Observers
{
    public class ChatRoomEventObserver : IChatRoomObserver
    {
        private readonly IChatEventRepository _chatEventObserverRepository;

        public ChatRoomEventObserver(IChatEventRepository chatEventObserverRepository)
        {
            _chatEventObserverRepository = chatEventObserverRepository;
        }

        public void Update(IEvent @event)
        {
            _chatEventObserverRepository.AddEvent(@event);
        }
    }
}
