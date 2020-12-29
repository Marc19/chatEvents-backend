using System;

namespace PowerDiary.ChatEvents.Core.Models
{
    public class EnterTheRoomEvent : Event
    {
        public EnterTheRoomEvent(long id) : base(id)
        {

        }

        public EnterTheRoomEvent(User user, long chatRoomId) : base(user, chatRoomId)
        {

        }

        public EnterTheRoomEvent(long id, User user, long chatRoomId) : base(id, user, chatRoomId)
        {

        }

        public EnterTheRoomEvent(long id, DateTime timestamp, User user, long chatRoomId) : base(id, timestamp, user, chatRoomId)
        {

        }
    }
}
