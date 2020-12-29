using System.Collections.Generic;
using PowerDiary.ChatEvents.Core.Models;

namespace PowerDiary.ChatEvents.Core.Persistence
{
    public interface IInMemoryDB
    {
        public List<User> Users { get; }

        public List<ChatRoom> ChatRooms { get; }

        public List<Event> Events { get; }

        void ClearDB();
    }
}
