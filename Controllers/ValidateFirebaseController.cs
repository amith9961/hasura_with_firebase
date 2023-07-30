using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin;

namespace FirebaseIntegration.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class ValidateFirebaseController : ControllerBase
    {
        [HttpPost("createUser")]
        public async Task<IActionResult> SignInAnonymously()
        {
            try
            {
                var auth = FirebaseAuth.DefaultInstance;
                var user = await auth.CreateUserAsync(new UserRecordArgs
                {
                    DisplayName = "Anonymous User"
                });
                var idToken = await auth.CreateCustomTokenAsync(user.Uid);

                return Ok(new { IdToken = idToken });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("CreateCustomClaim")]
        public async Task<IActionResult> SetCustomClaim(string userId, string claimName, bool claimValue)
        {
            try
            {
                var auth = FirebaseAuth.DefaultInstance;
                var user = await auth.GetUserAsync(userId);

                // Add or update the custom claim for the user.
                var claims = new Dictionary<string, object>
            {
                { claimName, claimValue }
            };
                await auth.SetCustomUserClaimsAsync(userId, claims);
                var userTokenAfterClaims = await auth.GetUserAsync(userId);
                var idToken = await auth.CreateCustomTokenAsync(userTokenAfterClaims.Uid);
                return Ok(new { IdToken = idToken });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("VerifyCustomClaim")]
        public async Task<IActionResult> VerifyCustomClaim(string userId, string claimName, bool claimValue)
        {
            try
            {
                var auth = FirebaseAuth.DefaultInstance;
                var user = await auth.GetUserAsync(userId);

                // Check if the user has the required custom claim.
                if (user.CustomClaims.TryGetValue(claimName, out var actualClaimValue)
                    && (bool)actualClaimValue == claimValue)
                {
                    return Ok("Custom claim verified.");
                }

                return Unauthorized("Custom claim verification failed.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("validate-firebase-token")]
        public async Task<IActionResult> ValidateFirebaseToken([FromBody] string firebaseToken)
        {
            try
            {
                // Validate the Firebase token using Firebase Admin SDK
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);

                // Extract user information from the token
                string uid = decodedToken.Uid;
                string email = decodedToken.Claims["email"].ToString();

                // You can perform additional actions based on the user information if needed

                return Ok(new { UserId = uid, Email = email });
            }
            catch (FirebaseAuthException ex)
            {
                // Token verification failed. Handle the error appropriately.
                return BadRequest($"Token verification failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Token validation failed, handle the error appropriately
                return BadRequest("Invalid Firebase token.");
            }
            
        }

    }
}
