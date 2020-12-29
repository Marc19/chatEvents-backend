using System;

namespace PowerDiary.ChatEvents.Core.Models
{
    public class HighFiveEvent : Event
    {
        public User OtherUser { get; set; }

        public HighFiveEvent(long id) : base(id)
        {

        }

        public HighFiveEvent(User user, long chatRoomId, User otherUser) : base(user, chatRoomId)
        {
            OtherUser = otherUser;
        }

        public HighFiveEvent(long id, User user, long chatRoomId, User otherUser) : base(id, user, chatRoomId)
        {
            OtherUser = otherUser;
        }
        
       public HighFiveEvent(long id, DateTime timestamp, User user, long chatRoomId, User otherUser) : base(id, timestamp, user, chatRoomId)
        {
            OtherUser = otherUser;
        }
    }
}
