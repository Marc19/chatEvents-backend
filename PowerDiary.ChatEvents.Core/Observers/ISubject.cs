using PowerDiary.ChatEvents.Services.Interfaces;

namespace PowerDiary.ChatEvents.Core.Observers
{
    public interface ISubject
    {
        void RegisterObserver(IChatRoomObserver chatRoomObserver);

        void UnregisterObserver(IChatRoomObserver chatRoomObserver);

        void NotifyObservers(IEvent @event);
    }
}
