using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using PowerDiary.ChatEvents.Services.DTOs;
using PowerDiary.ChatEvents.Services.ViewModels;

namespace PowerDiary.ChatEvents.Services.Interfaces
{
    public interface IChatEventRepository
    {
        Result EnterTheRoom(UserRoomDTO userRoomDTO);

        Result  LeaveTheRoom(UserRoomDTO userRoomDTO);

        Result Comment(CommentDTO commentDTO);

        Result HighFive(HighFiveDTO highFiveDTO);

        Result<IEnumerable<EventViewModel>> GetChatEvents(int chatRoomId, DateTime? dateFrom, DateTime? dateTo);

        Result<IEnumerable<EventStatsViewModel>> GetChatEventStats(int chatRoomId, int granularity, DateTime? dateFrom, DateTime? dateTo);

        Result<IEnumerable<UserViewModel>> GetUsers();

        Result<IEnumerable<ChatRoomViewModel>> GetChatRooms();
    }
}
