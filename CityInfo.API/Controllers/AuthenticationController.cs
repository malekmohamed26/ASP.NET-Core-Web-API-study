using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        //We won't use this outside of this class, so we can scope it to this namespace
        public class AuthenticationRequestBody
        {
            public string? userName {  get; set; }
            public string? password { get; set; }
        }

        private class CityInfoUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(
                int userId,
                string userName,
                string firstName,
                string lastName,
                string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(
            AuthenticationRequestBody authenticationRequestBody)
        {
            //Step 1 : We validate credentials (username/password)
            var user = ValidateUserCredentials(
                authenticationRequestBody.userName,
                authenticationRequestBody.password);

            if (user == null)
            {
                return Unauthorized();
            }

            //Step 2 : We create a token
            var securityKey = new SymmetricSecurityKey(
                Convert.FromBase64String(_configuration["Authentication:SecretForKey"]));

            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256 );
                
                //We'll create claims for token
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("given_name", user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));

            //Now we create the actual token
            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"], //We pass the issuer values in appsettings.Development.json
                _configuration["Authentication:Audience"], //We pass the audience values in appsettings.Development.json
                claimsForToken, //We pass the claims
                DateTime.UtcNow, //The value that indicates the start of token validity
                DateTime.UtcNow.AddHours(1), //The value that indicates the end of token validity
                signingCredentials);
            
            //Step 3 : Now we'll return the token we created
            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);
            return Ok(tokenToReturn);
        }

        private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        {
            //We don't have a user database or table yet, so
            return new CityInfoUser(
                1,
                userName ?? "",
                "Malek",
                "Mohamed",
                "Antwerp");
        }
    }
}
