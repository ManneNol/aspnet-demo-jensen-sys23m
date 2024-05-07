namespace Server;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

public class Auth
{
    public record LoginData(string Email, string Password);

    public static async Task<IResult> Login(LoginData user, State state, HttpContext ctx)
    {
        var cmd = state.DB.CreateCommand("select user_id, role from users where email = $1 and password = $2");
        cmd.Parameters.AddWithValue(user.Email);
        cmd.Parameters.AddWithValue(user.Password);
        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            string id = reader.GetInt32(0).ToString();
            string role = reader.GetString(1);

            await ctx.SignInAsync("sys23m.teachers.aspnetdemo", new ClaimsPrincipal(
                        new ClaimsIdentity(new Claim[]{
                            new Claim(ClaimTypes.NameIdentifier, id),
                            new Claim(ClaimTypes.Role, role),
                            },
                            "sys23m.teachers.aspnetdemo"
                            )
                        ));
            return TypedResults.Ok("Signed in");
        }
        else
        {
            return TypedResults.Problem("No such user exists");
        }
    }
}
