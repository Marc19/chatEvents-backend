using System.Collections.Generic;
using CSharpFunctionalExtensions;
using PowerDiary.ChatEvents.Services.DTOs;
using PowerDiary.ChatEvents.Services.ViewModels;

namespace PowerDiary.ChatEvents.Services.Interfaces
{
    public interface IChatEventService
    {
        Result EnterTheRoom(UserRoomDTO userRoomDTO);

        Result LeaveTheRoom(UserRoomDTO userRoomDTO);

        Result Comment(CommentDTO commentDTO);

        Result HighFive(HighFiveDTO highFiveDTO);

        Result<IEnumerable<EventViewModel>> GetChatEvents(int chatRoomId, string from, string to);

        Result<IEnumerable<EventStatsViewModel>> GetChatEventStats(int chatRoomId, int granularity, string from, string to);

        Result<IEnumerable<UserViewModel>> GetUsers();

        Result<IEnumerable<ChatRoomViewModel>> GetChatRooms();
    }
}
