using System;

namespace PowerDiary.ChatEvents.Services.ViewModels
{
    public class EventStatsViewModel
    {
        public DateTime Hour { get; set; }

        public int PeopleEnteredCount { get; set; }

        public int PeopleLeftCount { get; set; }

        public int CommentCount { get; set; }

        public int PeopleHighFivingCount { get; set; }

        public int PeopleHighFivedCount { get; set; }
    }
}
