using System;
using System.Collections.Generic;
using System.Linq;
using PowerDiary.ChatEvents.Core.Observers;
using PowerDiary.ChatEvents.Services.Interfaces;

namespace PowerDiary.ChatEvents.Core.Models
{
    public class ChatRoom : Entity, ISubject
    {
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<Event> Events { get; set; }

        public List<User> Users { get; set; }

        public List<Comment> Comments { get; set; }

        public List<HighFive> HighFives { get; set; }

        public ChatRoom(long id, string name)
        {
            Id = id;
            Name = name;
            CreatedAt = DateTime.Now;
            Events = new List<Event>();
            Users = new List<User>();
            Comments = new List<Comment>();
            HighFives = new List<HighFive>();
        }

        public void AddUser(User user)
        {
            if (!UserExistsInChatRoom(user.Id))
            {
                Users.Add(user);
                NotifyObservers(new EnterTheRoomEvent(user, Id));
            }
        }

        public void RemoveUser(long userId)
        {
            User user = Users.FirstOrDefault(u => u.Id == userId);

            if(user != null)
            {
                Users.Remove(user);
                NotifyObservers(new LeaveTheRoomEvent(user, Id));
            }
        }

        public void AddComment(string text, User user, ChatRoom chatRoom)
        {
            if(user != null && chatRoom != null)
            {
                Comment comment = new Comment(text, user, chatRoom);
                Comments.Add(comment);
                NotifyObservers(new CommentEvent(user, Id, text));
            }
        }

        public void AddHighFive(User user, User otherUser, ChatRoom chatRoom)
        {
            if (user != null && chatRoom != null)
            {
                HighFive highFive = new HighFive(user, otherUser, chatRoom);
                HighFives.Add(highFive);
                NotifyObservers(new HighFiveEvent(user, Id, otherUser));
            }
        }

        public bool UserExistsInChatRoom(long userId)
        {
            User user = Users.FirstOrDefault(u => u.Id == userId);

            if(user != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal readonly List<IChatRoomObserver> Observers = new List<IChatRoomObserver>();

        public void RegisterObserver(IChatRoomObserver chatRoomObserver)
        {
            Observers.Add(chatRoomObserver);
        }

        public void UnregisterObserver(IChatRoomObserver chatRoomObserver)
        {
            Observers.Remove(chatRoomObserver);
        }

        public void NotifyObservers(IEvent @event)
        {
            Observers.ForEach(o => o.Update(@event));
        }
    }
}
