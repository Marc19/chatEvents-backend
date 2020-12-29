using System;
using System.Collections.Generic;
using PowerDiary.ChatEvents.Core.Models;
using PowerDiary.ChatEvents.Core.Persistence;

namespace PowerDiary.ChatEvents.UnitTests
{
    public class InMemoryTestDB : IInMemoryDB
    {
        public List<User> Users { get; private set; }

        public List<ChatRoom> ChatRooms { get; private set; }

        public List<Event> Events { get; private set; }

        public InMemoryTestDB()
        {
            Users = new List<User>();
            ChatRooms = new List<ChatRoom>();
            Events = new List<Event>();
        }

        public void ClearDB()
        {
            Users.RemoveAll(_ => true);
            ChatRooms.RemoveAll(_ => true);
            Events.RemoveAll(_ => true);
        }
    }
}
