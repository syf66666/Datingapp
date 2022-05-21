using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entity;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
       private readonly DataContext context;

       private readonly ITokenService tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            this.context = context;
            this.tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO){

            if(await UserExits(registerDTO.UserName))
                //inherite from ControllerBase
                return BadRequest("username already exists");
            
            //TODO: what does using means?
            using var hmac = new HMACSHA512();

            var user = new AppUser{
                UserName = registerDTO.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };   

            this.context.Users.Add(user);
            await this.context.SaveChangesAsync();

            return new UserDTO{
                UserName = user.UserName,
                Token = this.tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDTO loginDTO){
            //FindOrDefault is also ok, but if there are more than one options, single will throw an exception, find will return the first one
            var user = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);

            if(user == null) return Unauthorized("no user find; pls register first");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for(int i = 0; i<ComputeHash.Length; i++)
                if(ComputeHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid password");
            return user;
        }

        private async Task<bool> UserExits(string username){
            return await this.context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}