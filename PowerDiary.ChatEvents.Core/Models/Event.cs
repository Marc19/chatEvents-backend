using System;
using PowerDiary.ChatEvents.Services.Interfaces;

namespace PowerDiary.ChatEvents.Core.Models
{
    public abstract class Event : Entity, IEvent
    {
        public DateTime TimeStamp { get; set; }

        public User User { get; set; }

        public long ChatRoomId { get; set; }

        protected Event(long id)
        {
            Id = id;
        }

        protected Event(User user, long chatRoomId)
        {
            TimeStamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            User = user;
            ChatRoomId = chatRoomId;
        }

        protected Event(long id, User user, long chatRoomId)
        {
            Id = id;
            TimeStamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            User = user;
            ChatRoomId = chatRoomId;
        }

        protected Event(DateTime timestamp, User user, long chatRoomId)
        {
            TimeStamp = timestamp;
            User = user;
            ChatRoomId = chatRoomId;
        }

        protected Event(long id, DateTime timestamp, User user, long chatRoomId)
        {
            Id = id;
            TimeStamp = timestamp;
            User = user;
            ChatRoomId = chatRoomId;
        }
    }
}
