using System;

namespace PowerDiary.ChatEvents.Core.Models
{
    public class HighFive : Entity
    {
        public User User { get; set; }

        public User OtherUser { get; set; }

        public ChatRoom ChatRoom { get; set; }

        public DateTime TimeStamp { get; set; }

        public HighFive()
        {

        }

        public HighFive(User user, User otherUser, ChatRoom chatRoom)
        {
            User = user;
            OtherUser = otherUser;
            ChatRoom = chatRoom;
            TimeStamp = DateTime.Now;
        }
    }
}
