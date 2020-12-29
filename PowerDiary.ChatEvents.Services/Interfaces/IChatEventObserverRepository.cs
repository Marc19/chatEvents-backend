using System;
using CSharpFunctionalExtensions;

namespace PowerDiary.ChatEvents.Services.Interfaces
{
    public interface IChatEventObserverRepository
    {
        Result AddEvent(IEvent @event); 
    }
}
