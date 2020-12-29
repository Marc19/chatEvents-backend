using System.Linq;
using PowerDiary.ChatEvents.Services.ViewModels;

namespace PowerDiary.ChatEvents.Core.Models
{
    public class ViewModelConverter
    {
        public static EventViewModel ConvertToEventViewModel(Event @event, string chatRoomName)
        {
            return @event switch
            {
                EnterTheRoomEvent enterTheRoomEvent => ConvertToEnterTheRoomViewModel(enterTheRoomEvent, chatRoomName),
                LeaveTheRoomEvent leaveTheRoomEvent => ConvertToLeaveTheRoomViewModel(leaveTheRoomEvent, chatRoomName),
                CommentEvent commentEvent => ConvertToCommentViewModel(commentEvent, chatRoomName),
                HighFiveEvent highFiveEvent => ConvertToHighFiveViewModel(highFiveEvent, chatRoomName),
                _ => null,
            };
        }

        private static EnterTheRoomViewModel ConvertToEnterTheRoomViewModel(EnterTheRoomEvent enterTheRoomEvent, string chatRoomName)
        {
            EnterTheRoomViewModel enterTheRoomViewModel = new EnterTheRoomViewModel()
            {
                EventId = enterTheRoomEvent.Id,
                EventName = GetEventName(enterTheRoomEvent),
                TimeStamp = enterTheRoomEvent.TimeStamp,
                User = new UserViewModel()
                {
                    UserId = enterTheRoomEvent.User.Id,
                    UserName = enterTheRoomEvent.User.Name
                },
                ChatRoom = new ChatRoomViewModel()
                {
                    ChatRoomId = enterTheRoomEvent.ChatRoomId,
                    ChatRoomName = chatRoomName
                }
            };

            return enterTheRoomViewModel;
        }

        private static LeaveTheRoomViewModel ConvertToLeaveTheRoomViewModel(LeaveTheRoomEvent leaveTheRoomEvent, string chatRoomName)
        {
            LeaveTheRoomViewModel leaveTheRoomViewModel = new LeaveTheRoomViewModel()
            {
                EventId = leaveTheRoomEvent.Id,
                EventName = GetEventName(leaveTheRoomEvent),
                TimeStamp = leaveTheRoomEvent.TimeStamp,
                User = new UserViewModel()
                {
                    UserId = leaveTheRoomEvent.User.Id,
                    UserName = leaveTheRoomEvent.User.Name
                },
                ChatRoom = new ChatRoomViewModel()
                {
                    ChatRoomId = leaveTheRoomEvent.ChatRoomId,
                    ChatRoomName = chatRoomName
                }
            };

            return leaveTheRoomViewModel;
        }

        private static CommentViewModel ConvertToCommentViewModel(CommentEvent commentEvent, string chatRoomName)
        {
            CommentViewModel commentViewModel = new CommentViewModel()
            {
                EventId = commentEvent.Id,
                EventName = GetEventName(commentEvent),
                TimeStamp = commentEvent.TimeStamp,
                User = new UserViewModel()
                {
                    UserId = commentEvent.User.Id,
                    UserName = commentEvent.User.Name
                },
                ChatRoom = new ChatRoomViewModel()
                {
                    ChatRoomId = commentEvent.ChatRoomId,
                    ChatRoomName = chatRoomName
                },
                Text = commentEvent.Text
            };

            return commentViewModel;
        }

        private static HighFiveViewModel ConvertToHighFiveViewModel(HighFiveEvent highFiveEvent, string chatRoomName)
        {
            HighFiveViewModel highFiveViewModel = new HighFiveViewModel()
            {
                EventId = highFiveEvent.Id,

                EventName = GetEventName(highFiveEvent),
                TimeStamp = highFiveEvent.TimeStamp,
                User = new UserViewModel()
                {
                    UserId = highFiveEvent.User.Id,
                    UserName = highFiveEvent.User.Name
                },
                ChatRoom = new ChatRoomViewModel()
                {
                    ChatRoomId = highFiveEvent.ChatRoomId,
                    ChatRoomName = chatRoomName
                },
                OtherUser = new UserViewModel()
                {
                    UserId = highFiveEvent.OtherUser.Id,
                    UserName = highFiveEvent.OtherUser.Name
                }
            };

            return highFiveViewModel;
        }

        private static string GetEventName(Event @event)
        {
            return @event.GetType().Name.Replace("Event", "");
        }

        public static UserViewModel ConvertToUserViewModel(User user)
        {
            if (user == null) return null;

            return new UserViewModel() { UserId = user.Id, UserName = user.Name };
        }

        public static ChatRoomViewModel ConvertToChatRoomViewModel(ChatRoom chatRoom)
        {
            if (chatRoom == null) return null;

            return new ChatRoomViewModel()
            {
                ChatRoomId = chatRoom.Id,
                ChatRoomName = chatRoom.Name,
                Users = chatRoom.Users.Select(u => ConvertToUserViewModel(u)).ToList()
            };
        }
    }
}
