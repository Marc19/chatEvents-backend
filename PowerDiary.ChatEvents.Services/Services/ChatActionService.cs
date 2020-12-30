using System;
using System.Collections.Generic;
using System.Globalization;
using CSharpFunctionalExtensions;
using PowerDiary.ChatEvents.Services.DTOs;
using PowerDiary.ChatEvents.Services.Interfaces;
using PowerDiary.ChatEvents.Services.ViewModels;

namespace PowerDiary.ChatEvents.Services.Services
{
    public class ChatActionService : IChatActionService
    {
        private readonly IChatActionRepository _chatActionRepository;

        public ChatActionService(IChatActionRepository chatActionRepository)
        {
            _chatActionRepository = chatActionRepository;
        }

        public Result EnterTheRoom(UserRoomDTO userRoomDTO)
        {
            return _chatActionRepository.EnterTheRoom(userRoomDTO); 
        }

        public Result LeaveTheRoom(UserRoomDTO userRoomDTO)
        {
            return _chatActionRepository.LeaveTheRoom(userRoomDTO);
        }

        public Result Comment(CommentDTO commentDTO)
        {
            return _chatActionRepository.Comment(commentDTO);
        }

        public Result HighFive(HighFiveDTO highFiveDTO)
        {
            return _chatActionRepository.HighFive(highFiveDTO);
        }

        public Result<IEnumerable<UserViewModel>> GetUsers()
        {
            return _chatActionRepository.GetUsers();
        }

        public Result<IEnumerable<ChatRoomViewModel>> GetChatRooms()
        {
            return _chatActionRepository.GetChatRooms();
        }
    }
}
