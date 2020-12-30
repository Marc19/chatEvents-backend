using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using PowerDiary.ChatEvents.Core.Models;
using PowerDiary.ChatEvents.Core.Observers;
using PowerDiary.ChatEvents.Core.Persistence;
using PowerDiary.ChatEvents.Services.DTOs;
using PowerDiary.ChatEvents.Services.Interfaces;
using PowerDiary.ChatEvents.Services.ViewModels;

namespace PowerDiary.ChatEvents.Core.Repositories
{
    public class ChatActionRepository : IChatActionRepository
    {
        private readonly IChatRoomObserver _chatRoomObserver;
        private readonly IInMemoryDB _inMemoryDB;

        public ChatActionRepository(IChatRoomObserver chatRoomObserver, IInMemoryDB inMemoryDB)
        {
            _chatRoomObserver = chatRoomObserver;
            _inMemoryDB = inMemoryDB;
        }

        public Result EnterTheRoom(UserRoomDTO userRoomDTO)
        {
            User user = _inMemoryDB.Users.FirstOrDefault(u => u.Id == userRoomDTO.UserId);
            ChatRoom chatRoom = _inMemoryDB.ChatRooms.FirstOrDefault(cr => cr.Id == userRoomDTO.ChatRoomId);

            Result validationResult = ValidateEnterTheRoom(user, chatRoom);
            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            chatRoom.RegisterObserver(_chatRoomObserver);
            chatRoom.AddUser(user);
            chatRoom.UnregisterObserver(_chatRoomObserver);

            return Result.Success();
        }

        public Result LeaveTheRoom(UserRoomDTO userRoomDTO)
        {
            User user = _inMemoryDB.Users.FirstOrDefault(u => u.Id == userRoomDTO.UserId);
            ChatRoom chatRoom = _inMemoryDB.ChatRooms.FirstOrDefault(cr => cr.Id == userRoomDTO.ChatRoomId);

            Result validationResult = ValidateLeaveTheRoom(user, chatRoom);
            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            chatRoom.RegisterObserver(_chatRoomObserver);
            chatRoom.RemoveUser(user.Id);
            chatRoom.UnregisterObserver(_chatRoomObserver);

            return Result.Success();
        }

        public Result Comment(CommentDTO commentDTO)
        {
            User user = _inMemoryDB.Users.FirstOrDefault(u => u.Id == commentDTO.UserId);
            ChatRoom chatRoom = _inMemoryDB.ChatRooms.FirstOrDefault(cr => cr.Id == commentDTO.ChatRoomId);

            Result validationResult = ValidateComment(user, chatRoom);
            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            chatRoom.RegisterObserver(_chatRoomObserver);
            chatRoom.AddComment(commentDTO.Text, user, chatRoom);
            chatRoom.UnregisterObserver(_chatRoomObserver);

            return Result.Success();
        }

        public Result HighFive(HighFiveDTO highFiveDTO)
        {
            User user = _inMemoryDB.Users.FirstOrDefault(u => u.Id == highFiveDTO.UserId);
            User otherUser = _inMemoryDB.Users.FirstOrDefault(u => u.Id == highFiveDTO.OtherUserId);
            ChatRoom chatRoom = _inMemoryDB.ChatRooms.FirstOrDefault(cr => cr.Id == highFiveDTO.ChatRoomId);

            Result validationResult = ValidateHighFive(user, chatRoom, otherUser);
            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            chatRoom.RegisterObserver(_chatRoomObserver);
            chatRoom.AddHighFive(user, otherUser, chatRoom);
            chatRoom.UnregisterObserver(_chatRoomObserver);

            return Result.Success();
        }

        public Result<IEnumerable<UserViewModel>> GetUsers()
        {
            return Result.Success(_inMemoryDB.Users.Select(u => ViewModelConverter.ConvertToUserViewModel(u)));
        }

        public Result<IEnumerable<ChatRoomViewModel>> GetChatRooms()
        {
            return Result.Success(_inMemoryDB.ChatRooms.Select(cr => ViewModelConverter.ConvertToChatRoomViewModel(cr)));
        }

        private Result ValidateUser(User user)
        {
            if (user == null)
            {
                return Result.Failure("User does not exist!");
            }

            return Result.Success();
        }

        private Result ValidateChatRoom(ChatRoom chatRoom)
        {
            if (chatRoom == null)
            {
                return Result.Failure("Chat room does not exist!");
            }

            return Result.Success();
        }

        private Result ValidateUserChatRoom(User user, ChatRoom chatRoom)
        {
            Result userValidationResult = ValidateUser(user);
            Result chatRoomValidationResult = ValidateChatRoom(chatRoom);

            Result result = Result.Combine(userValidationResult, chatRoomValidationResult);

            return result;
        }

        private Result ValidateEnterTheRoom(User user, ChatRoom chatRoom)
        {
            Result userRoomValidationResult = ValidateUserChatRoom(user, chatRoom);

            if (userRoomValidationResult.IsFailure)
            {
                return userRoomValidationResult;
            }

            bool userAlreadyExists = chatRoom.UserExistsInChatRoom(user.Id);
            Result userAlreadyExistsResult;

            if (userAlreadyExists)
            {
                userAlreadyExistsResult = Result.Failure("User Already exists in chat room!");
            }
            else
            {
                userAlreadyExistsResult = Result.Success();
            }

            Result result = Result.Combine(userRoomValidationResult, userAlreadyExistsResult);
            return result;
        }

        private Result ValidateLeaveTheRoom(User user, ChatRoom chatRoom)
        {
            Result userRoomValidationResult = ValidateUserChatRoom(user, chatRoom);

            if (userRoomValidationResult.IsFailure)
            {
                return userRoomValidationResult;
            }

            bool userAlreadyExists = chatRoom.UserExistsInChatRoom(user.Id);
            Result userAlreadyExistsResult;

            if (!userAlreadyExists)
            {
                userAlreadyExistsResult = Result.Failure("User is not in the chat room!");
            }
            else
            {
                userAlreadyExistsResult = Result.Success();
            }

            Result result = Result.Combine(userRoomValidationResult, userAlreadyExistsResult);
            return result;
        }

        private Result ValidateComment(User user, ChatRoom chatRoom)
        {
            Result userRoomValidationResult = ValidateUserChatRoom(user, chatRoom);

            if (userRoomValidationResult.IsFailure)
            {
                return userRoomValidationResult;
            }

            bool userAlreadyExists = chatRoom.UserExistsInChatRoom(user.Id);

            if (!userAlreadyExists)
            {
                return Result.Failure("User can't comment because they are not in the chat room!");
            }

            return Result.Success();
        }

        private Result ValidateHighFive(User user, ChatRoom chatRoom, User otherUser)
        {
            Result userRoomValidationResult = ValidateUserChatRoom(user, chatRoom);

            if (userRoomValidationResult.IsFailure)
            {
                return userRoomValidationResult;
            }

            if (otherUser == null)
            {
                return Result.Failure("The User you are trying to high five does not exist!");
            }

            if(user.Id == otherUser.Id)
            {
                return Result.Failure("You can't self five, but Barney Stinson can ;");
            }

            bool userExistsInChatRoom = chatRoom.UserExistsInChatRoom(user.Id);
            bool otherUserExistsInChatRoom = chatRoom.UserExistsInChatRoom(otherUser.Id);

            if (!userExistsInChatRoom)
            {
                return Result.Failure("User can't High Five because they are not in the chat room!");
            }

            if (!otherUserExistsInChatRoom)
            {
                return Result.Failure("Other User can't be High Fived because they are not in the chat room!");
            }

            return Result.Success();
        }
    }
}
