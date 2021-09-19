﻿using AccountMicroservice.DBContexts;
using AccountMicroservice.Models;
using AccountMicroservice.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AccountContext _dbcontext;
        private readonly ITokenBuilder _tokenBuilder;

        public AuthenticationController(
            AccountContext dbcontext,
            ITokenBuilder tokenBuilder)
        {
            _dbcontext = dbcontext;
            _tokenBuilder = tokenBuilder;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var dbUser = await _dbcontext
                .Users
                .SingleOrDefaultAsync(u => u.UserName == user.UserName);

            if (dbUser == null)
            {
                return NotFound("User not found.");
            }

            // This is just an example, made for simplicity; do not store plain passwords in the database
            // Always hash and salt your passwords
            var isValid = dbUser.PwdHash == user.PwdHash;

            if (!isValid)
            {
                return BadRequest("Could not authenticate user.");
            }

            var token = _tokenBuilder.BuildToken(user.UserName);

            return Ok(token);
        }

        [HttpGet("verify")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> VerifyToken()
        {
            var username = User
                .Claims
                .SingleOrDefault();

            if (username == null)
            {
                return Unauthorized();
            }

            var userExists = await _dbcontext
                .Users
                .AnyAsync(u => u.UserName == username.Value);

            if (!userExists)
            {
                return Unauthorized();
            }

            return NoContent();
        }
    }
}
