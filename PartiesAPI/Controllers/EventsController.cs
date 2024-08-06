﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;
using PartiesAPI.Data;
using PartiesAPI.Models;

namespace PartiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private PartyDbContext _context;

        public EventsController(PartyDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateEvent(Event ev)
        {
            try
            {
                _context.Events.Add(ev);
                _context.SaveChanges();
                return Ok(ev);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
