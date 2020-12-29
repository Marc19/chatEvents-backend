using System;
using System.Collections.Generic;
using PowerDiary.ChatEvents.Core.Models;

namespace PowerDiary.ChatEvents.Core.Persistence
{
    public class InMemoryDB : IInMemoryDB
    {
        public List<User> Users { get; private set; }

        public List<ChatRoom> ChatRooms { get; private set; }

        public List<Event> Events { get; private set; }

        public InMemoryDB()
        {
            Users = new List<User>()
            {
                new User(1, "Bob"),
                new User(2, "Kate"),
                new User(3, "Alice"),
                new User(4, "Jake")
            };

            ChatRooms = new List<ChatRoom>()
            {
                new ChatRoom(1, "BestBuddies")
            };

            Events = new List<Event>()
            {
                new EnterTheRoomEvent(1, new DateTime(2020,12,20,12,0,0) , Users[0], 1),
                new EnterTheRoomEvent(2, new DateTime(2020,12,20,12,20,0) ,Users[1], 1),
                new EnterTheRoomEvent(3, new DateTime(2020,12,20,12,30,0) ,Users[2], 1),
                new LeaveTheRoomEvent(4, new DateTime(2020,12,20,12,40,0) ,Users[2], 1),
                new CommentEvent(5, new DateTime(2020,12,20,12,45,0), Users[0], 1, "This is a comment"),
                new HighFiveEvent(6, new DateTime(2020,12,20,12,50,0), Users[1], 1, Users[0]),
                new HighFiveEvent(7, new DateTime(2020,12,20,12,50,0), Users[1], 1, Users[0]),

                new EnterTheRoomEvent(8, new DateTime(2020,12,20,13,0,0) ,Users[2], 1),
                new EnterTheRoomEvent(9, new DateTime(2020,12,20,13,20,0) ,Users[3], 1),
                new CommentEvent(10, new DateTime(2020,12,20,13,45,0), Users[2], 1, "This is also a comment"),
                new HighFiveEvent(11, new DateTime(2020,12,20,13,50,0), Users[3], 1, Users[2]),
                new HighFiveEvent(12, new DateTime(2020,12,20,13,50,0), Users[3], 1, Users[1]),
                new HighFiveEvent(13, new DateTime(2020,12,20,13,50,0), Users[3], 1, Users[0]),
                new HighFiveEvent(14, new DateTime(2020,12,20,13,50,0), Users[1], 1, Users[0]),
                new HighFiveEvent(15, new DateTime(2020,12,20,13,50,0), Users[1], 1, Users[3]),

                new LeaveTheRoomEvent(16, new DateTime(2020,12,20,13,55,0) ,Users[0], 1),
                new LeaveTheRoomEvent(17, new DateTime(2020,12,20,13,55,0) ,Users[1], 1),
                new LeaveTheRoomEvent(18, new DateTime(2020,12,20,13,55,0) ,Users[2], 1),
                new LeaveTheRoomEvent(19, new DateTime(2020,12,20,13,55,0) ,Users[3], 1),

                new EnterTheRoomEvent(20, new DateTime(2020,11,20,13,0,0) ,Users[0], 1),
                new EnterTheRoomEvent(21, new DateTime(2020,11,20,13,20,0) ,Users[1], 1),
                new EnterTheRoomEvent(22, new DateTime(2020,11,20,13,30,0) ,Users[2], 1),
                new LeaveTheRoomEvent(23, new DateTime(2020,11,20,13,40,0) ,Users[2], 1),
                new CommentEvent(24, new DateTime(2020,11,20,13,45,0), Users[0], 1, "This is a comment"),
                new HighFiveEvent(25, new DateTime(2020,11,20,13,50,0), Users[1], 1, Users[0]),
                new HighFiveEvent(26, new DateTime(2020,11,20,13,50,0), Users[1], 1, Users[0]),

                new EnterTheRoomEvent(27, new DateTime(2020,11,20,14,0,0) ,Users[2], 1),
                new EnterTheRoomEvent(28, new DateTime(2020,11,20,14,20,0) ,Users[3], 1),
                new CommentEvent(29, new DateTime(2020,11,20,14,45,0), Users[2], 1, "This is also a comment"),
                new HighFiveEvent(30, new DateTime(2020,11,20,14,50,0), Users[3], 1, Users[2]),
                new HighFiveEvent(31, new DateTime(2020,11,20,14,50,0), Users[3], 1, Users[1]),
                new HighFiveEvent(32, new DateTime(2020,11,20,14,50,0), Users[3], 1, Users[0]),
                new HighFiveEvent(33, new DateTime(2020,11,20,14,50,0), Users[1], 1, Users[0]),
                new HighFiveEvent(34, new DateTime(2020,11,20,14,50,0), Users[1], 1, Users[3]),
            };
        }

        public void ClearDB()
        {
            Users.RemoveAll(_ => true);
            ChatRooms.RemoveAll(_ => true);
            Events.RemoveAll(_ => true);
        }
    }
}
