﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Collections.Generic;
using TaskMenagerAPI.Data;
using TaskMenagerAPI.DTO;
using TaskMenagerAPI.Interfaces;
using TaskMenagerAPI.Models;

namespace TaskMenagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ToDoController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IToDoRepository _toDoRepository;
        private readonly IUserRepository _userRepository;

        public ToDoController(DataContext context, IMapper mapper,IToDoRepository toDoRepository, IUserRepository userRepository )
        {
            _context = context;
            _mapper = mapper;
            _toDoRepository = toDoRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ToDo>))]


        public IActionResult GetAllTodoes() {
            
            
            var todoes = _mapper.Map<List<ToDoDTO>>(_toDoRepository.GetAllToDo());

            return Ok(todoes);
        
        }
        [HttpGet("{todoId}")]
        [ProducesResponseType(200, Type = typeof(ToDo))]
        [ProducesResponseType(400)]
        public IActionResult GetToDo(int todoId)
        {
           
            var todo = _mapper.Map<ToDoDTO>(_toDoRepository.GetTodo(todoId));

            if(!ModelState.IsValid)

            {
                return BadRequest(ModelState);
            }


            return Ok(todo);


        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]

        public IActionResult CreateTodo([FromQuery] int userId, [FromBody] ToDoDTO todoCreate)
        {
            var todoes = _toDoRepository.GetAllToDo()
                .Where(x => x.Title.Trim().ToUpper() ==  todoCreate.Title.Trim().ToUpper())
                .FirstOrDefault();

            if(todoes != null)
            {
                ModelState.AddModelError("", "Task altready exist");
                return StatusCode(422, ModelState);
            }
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var todoMap = _mapper.Map<ToDo>(todoCreate);

            todoMap.User = _userRepository.GetUser(userId);

            if(!_toDoRepository.CreateToDo(todoMap))
            {
                ModelState.AddModelError("", "Something went wrong whith saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Seccefully created");

        }
        [HttpPut("{todoId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateToDo(int todoId, [FromBody] ToDoDTO updatedToDo)
        {
            if (updatedToDo == null)
                return BadRequest(ModelState);

            if (todoId != updatedToDo.Id)
                return BadRequest(ModelState);

            var user = _context.Find<User>(updatedToDo.UserId);

            if (!ModelState.IsValid)
                return BadRequest();


            var todoMap = _mapper.Map<ToDo>(updatedToDo);

            
            if (!_toDoRepository.UpdateToDo(todoMap))
            {
                ModelState.AddModelError("", "Something went wrong updating review");
                return StatusCode(500, ModelState);
            }

            return NoContent();

            
        }
        [HttpDelete("{todoId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public IActionResult DeleteUser(int todoId)
        {

            var todoToDelete = _toDoRepository.GetTodo(todoId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_toDoRepository.DeleteToDo(todoToDelete))
            {
                ModelState.AddModelError("", "Something went wront delete category");
            }

            return NoContent();


        }
    }
    
}