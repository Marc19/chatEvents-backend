using System;

namespace PowerDiary.ChatEvents.Core.Models
{
    public class User : Entity
    {
        public string Name { get; set; }

        public User(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
