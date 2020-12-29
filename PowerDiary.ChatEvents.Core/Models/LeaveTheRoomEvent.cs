using System;

namespace PowerDiary.ChatEvents.Core.Models
{
    public class LeaveTheRoomEvent : Event
    {
        public LeaveTheRoomEvent(long id) : base(id)
        {

        }

        public LeaveTheRoomEvent(User user, long chatRoomId) : base(user, chatRoomId)
        {

        }

        public LeaveTheRoomEvent(long id, User user, long chatRoomId) : base(id, user, chatRoomId)
        {
            
        }

        public LeaveTheRoomEvent(long id, DateTime timestamp, User user, long chatRoomId) : base(id, timestamp, user, chatRoomId)
        {
            
        }
    }
}
