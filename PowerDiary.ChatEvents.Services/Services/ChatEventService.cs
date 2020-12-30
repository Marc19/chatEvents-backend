using System;
using System.Collections.Generic;
using System.Globalization;
using CSharpFunctionalExtensions;
using PowerDiary.ChatEvents.Services.Interfaces;
using PowerDiary.ChatEvents.Services.ViewModels;

namespace PowerDiary.ChatEvents.Services.Services
{
    public class ChatEventService : IChatEventService
    {
        private readonly IChatEventRepository _chatEventRepository;

        public ChatEventService(IChatEventRepository chatEventRepository)
        {
            _chatEventRepository = chatEventRepository;
        }

        public Result<IEnumerable<EventViewModel>> GetChatEvents(int chatRoomId, string from, string to)
        {
            Result<DateTime?> fromDateResult = ConvertToDateTime(from, "From");
            Result<DateTime?> toDateResult = ConvertToDateTime(to, "To");
            Result dateRangesResult =
                ValidateDateRanges(fromDateResult.IsSuccess ? fromDateResult.Value : null, toDateResult.IsSuccess ? toDateResult.Value : null);

            Result datesResult = Result.Combine(fromDateResult, toDateResult, dateRangesResult);

            if (datesResult.IsFailure)
            {
                return Result.Failure<IEnumerable<EventViewModel>>(datesResult.Error);
            }

            return _chatEventRepository.GetChatEvents(chatRoomId, fromDateResult.Value, toDateResult.Value);
        }

        public Result<IEnumerable<EventStatsViewModel>> GetChatEventStats(int chatRoomId, int granularity, string from, string to)
        {
            Result<DateTime?> fromDateResult = ConvertToDateTime(from, "From");
            Result<DateTime?> toDateResult = ConvertToDateTime(to, "To");
            Result dateRangesResult =
                ValidateDateRanges(fromDateResult.IsSuccess ? fromDateResult.Value : null, toDateResult.IsSuccess ? toDateResult.Value : null);
            Result granularityResult = ValidateGranularity(granularity);

            Result datesResult = Result.Combine(fromDateResult, toDateResult, dateRangesResult, granularityResult);

            if (datesResult.IsFailure)
            {
                return Result.Failure<IEnumerable<EventStatsViewModel>>(datesResult.Error);
            }

            return _chatEventRepository.GetChatEventStats(chatRoomId, granularity, fromDateResult.Value, toDateResult.Value);
        }

        private Result<DateTime?> ConvertToDateTime(string dateString, string errorPrefix)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(dateString))
                {
                    DateTime? date = DateTime.ParseExact(dateString, "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);
                    return Result.Success(date);
                }
                else
                {
                    return Result.Success<DateTime?>(null);
                }
            }
            catch (Exception)
            {
                return Result.Failure<DateTime?>(errorPrefix + " Date is not valid!");
            }
        }

        private Result ValidateDateRanges(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate != null && toDate != null)
            {
                if (fromDate > toDate)
                {
                    return Result.Failure("From Date cannot be greater than To Date!");
                }
            }

            return Result.Success();
        }

        private Result ValidateGranularity(int granularity)
        {
            if (24 % granularity == 0)
            {
                return Result.Success();
            }

            return Result.Failure("Granularity value must be one of: [1, 2, 3, 4, 6, 8, 12, 24]");
        }

        
    }
}
