using System;

namespace PowerDiary.ChatEvents.Core.Models
{
    public class Comment : Entity
    {
        public string Text{ get; set; }

        public User User { get; set; }

        public ChatRoom ChatRoom { get; set; }

        public DateTime TimeStamp { get; set; }

        public Comment(string text)
        {
            Text = text;
        }

        public Comment(string text, User user, ChatRoom chatRoom)
        {
            Text = text;
            User = user;
            ChatRoom = chatRoom;
            TimeStamp = DateTime.Now;
        }

    }
}
