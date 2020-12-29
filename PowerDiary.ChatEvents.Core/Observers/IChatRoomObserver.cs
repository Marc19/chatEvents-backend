using PowerDiary.ChatEvents.Services.Interfaces;

namespace PowerDiary.ChatEvents.Core.Observers
{
    public interface IChatRoomObserver
    {
        void Update(IEvent @event);
    }
}
