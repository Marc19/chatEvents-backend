using System.Collections.Generic;

namespace PowerDiary.ChatEvents.Services.ViewModels
{
    public class ChatRoomViewModel
    {
        public long ChatRoomId { get; set; }

        public string ChatRoomName { get; set; }

        public List<UserViewModel> Users { get; set; }
    }
}
