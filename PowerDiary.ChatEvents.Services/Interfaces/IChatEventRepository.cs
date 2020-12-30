using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using PowerDiary.ChatEvents.Services.ViewModels;

namespace PowerDiary.ChatEvents.Services.Interfaces
{
    public interface IChatEventRepository
    {
        Result AddEvent(IEvent @event);

        Result<IEnumerable<EventViewModel>> GetChatEvents(int chatRoomId, DateTime? dateFrom, DateTime? dateTo);

        Result<IEnumerable<EventStatsViewModel>> GetChatEventStats(int chatRoomId, int granularity, DateTime? dateFrom, DateTime? dateTo);
    }
}
