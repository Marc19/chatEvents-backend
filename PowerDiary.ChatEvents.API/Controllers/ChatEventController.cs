using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using PowerDiary.ChatEvents.Services.DTOs;
using PowerDiary.ChatEvents.Services.Interfaces;
using PowerDiary.ChatEvents.Services.ViewModels;

namespace PowerDiary.ChatEvents.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ChatEventController : ControllerBase
    {
        private readonly IChatEventService _chatEventService;

        public ChatEventController(IChatEventService chatEventService)
        {
            _chatEventService = chatEventService;
        }

        [HttpPost]
        public IActionResult EnterTheRoom(UserRoomDTO userRoomDTO)
        {
            Result result = _chatEventService.EnterTheRoom(userRoomDTO);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult LeaveTheRoom(UserRoomDTO userRoomDTO)
        {
            Result result = _chatEventService.LeaveTheRoom(userRoomDTO);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult Comment(CommentDTO commentDTO)
        {
            Result result = _chatEventService.Comment(commentDTO);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult HighFive(HighFiveDTO highFiveDTO)
        {
            Result result = _chatEventService.HighFive(highFiveDTO);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        // https://localhost:5001/api/chatevent/getchatevents?chatroomid=1&from=25-12-2020T13:49:17&to=25/12/2020T15:49:17
        [HttpGet]
        public IActionResult GetChatEvents([FromQuery] int chatRoomId, string from, string to)
        {
            var result = _chatEventService.GetChatEvents(chatRoomId, from, to);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value.ToList());
            
        }

        // https://localhost:5001/api/chatevent/getchateventstats?chatroomid=1&granularity=1
        [HttpGet]
        public IActionResult GetChatEventStats([FromQuery] int chatRoomId, int granularity, string from, string to)
        {
            var result = _chatEventService.GetChatEventStats(chatRoomId, granularity, from, to);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value.ToList());
        }


        [HttpGet]
        public IActionResult GetUsers()
        {
            Result<IEnumerable<UserViewModel>> result = _chatEventService.GetUsers();
            
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value.ToList());
        }

        [HttpGet]
        public IActionResult GetChatRooms()
        {
            Result<IEnumerable<ChatRoomViewModel>> result = _chatEventService.GetChatRooms();

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value.ToList());
        }
    }
}
