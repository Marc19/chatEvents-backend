using System;
using PowerDiary.ChatEvents.Services.ViewModels;

namespace PowerDiary.ChatEvents.API.CustomJSONSerializer
{
    public class EventViewModelJsonConverter : DerivedTypeJsonConverter<EventViewModel>
    {
        protected override Type NameToType(string typeName)
        {
            return typeName switch
            {
                // map string values to types
                nameof(EnterTheRoomViewModel) => typeof(EnterTheRoomViewModel),
                nameof(LeaveTheRoomViewModel) => typeof(LeaveTheRoomViewModel),
                nameof(CommentViewModel) => typeof(CommentViewModel),
                nameof(HighFiveViewModel) => typeof(HighFiveViewModel),
                _ => null
            };
        }

        protected override string TypeToName(Type type)
        {
            // map types to string values
            if (type == typeof(EnterTheRoomViewModel)) return nameof(EnterTheRoomViewModel);
            if (type == typeof(LeaveTheRoomViewModel)) return nameof(LeaveTheRoomViewModel);
            if (type == typeof(CommentViewModel)) return nameof(CommentViewModel);
            if (type == typeof(HighFiveViewModel)) return nameof(HighFiveViewModel);

            return null;
        }
    }
}
