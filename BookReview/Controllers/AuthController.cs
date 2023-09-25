using AutoMapper;
using BookReview.Dto;
using BookReview.Models;
using BookReview.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BookReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: Controller
    {
        
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public AuthController(IConfiguration configuration, IMapper mapper, IUserRepository userRepository)
        {
            _configuration = configuration;
            _mapper = mapper;
            _userRepository = userRepository;
           
        }

        [HttpPost("register")]
        public ActionResult<User> Register(UserDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(ModelState);
                }

                var country = _userRepository.GetUsers().Where(c => c.Username.Trim().ToUpper() == request.Username.TrimEnd().ToUpper()).FirstOrDefault();

                if (country != null)
                {
                    ModelState.AddModelError("", "Username already exists");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

                var userMap = _mapper.Map<User>(request);
                userMap.Username = request.Username;
                userMap.PasswordHash = passwordHash;
                userMap.PasswordSalt = passwordSalt;

                _userRepository.CreateUser(userMap);

                return Ok(userMap);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpPost("login")]
        public ActionResult<string> Login(UserDto request)
        {
            try
            {
                var user = _userRepository.GetUser(request.Username);
                if (user.Username != request.Username)
                {
                    return BadRequest("User not found.");
                }

                if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return BadRequest("Wrong password.");
                }

                string token = CreateToken(user);

                return Ok(token);

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }
            
        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
               
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
