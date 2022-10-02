﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sheca.Dtos;
using Sheca.Error;
using Sheca.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sheca.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public DataContext _context { get; set; }
        public AuthService(IConfiguration configuration, DataContext context, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
        }
        public string CreateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", user.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string ValidateToken(string token)
        {
            if (token == null)
                return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
                return userId;
            }
            catch
            {
                return null;
            }
        }
        public async Task<(User, string token)> Login(UserDTO userDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(userDTO.Email.ToLower()));
            if (user == null)
            {
                throw new ApiException("User not found!", 400);
            } else if(user.Password != userDTO.Password)
            {
                throw new ApiException("Wrong password!", 400);
            }
            return (user, CreateToken(user));
        }
        public async Task<(User, string token)> Register(UserDTO userDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(userDTO.Email.ToLower()));
            if (user != null)
            {
                throw new ApiException("Email have already existed!", 400);
            }
            User _user = _mapper.Map<User>(userDTO);
            _context.Add(_user);
            _context.SaveChanges();
            return (_user, CreateToken(_user));
        }
        public async Task<bool> FindUserByEmai(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == email.Trim().ToLower()) == null ? false : true;
        }
        public async Task ResetPassword(UserDTO userDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == userDTO.Email.Trim().ToLower());
            if (user == null)
            {
                throw new ApiException("Invalid User",400);
            }
            user.Password = userDTO.Password;
            _context.Users.Update(user);
            _context.SaveChanges();
            return;
        }
    }
}
