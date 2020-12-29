using System;

namespace PowerDiary.ChatEvents.Core.Models
{
    public class CommentEvent : Event
    {
        public string Text { get; set; }

        public CommentEvent(long id, string text) : base(id)
        {
            Text = text;
        }

        public CommentEvent(User user, long chatRoomId, string text) : base(user, chatRoomId)
        {
            Text = text;
        }

        public CommentEvent(long id, User user, long chatRoomId, string text) : base(id, user, chatRoomId)
        {
            Text = text;
        }
        
        public CommentEvent(long id, DateTime timestamp, User user, long chatRoomId, string text) : base(id, timestamp,user, chatRoomId)
        {
            Text = text;
        }
    }
}
