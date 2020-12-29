using System;

namespace PowerDiary.ChatEvents.Services.ViewModels
{
    public abstract class EventViewModel
    {
        public DateTime TimeStamp { get; set; }

        public long EventId { get; set; }

        public string EventName { get; set; }

        public UserViewModel User { get; set; }

        public ChatRoomViewModel ChatRoom { get; set; }

    }
}
