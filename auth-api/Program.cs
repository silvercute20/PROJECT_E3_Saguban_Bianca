using auth_api.Models;
using auth_api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
   options.UseSqlite("Data Source=authapi.db"));

var jwtKey = builder.Configuration["JwtSettings:Secret"];
if (string.IsNullOrEmpty(jwtKey)) throw new Exception("JWT Secret is missing!");
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactDev");
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/register", async (User user, AppDbContext db) =>
{
    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "User registered successfully" });
});

app.MapPost("/login", async (User loginUser, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginUser.Email);
    if (user == null || !BCrypt.Net.BCrypt.Verify(loginUser.PasswordHash, user.PasswordHash)) return Results.Unauthorized();

    var tokenHandler = new JwtSecurityTokenHandler();
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        }),
        Expires = DateTime.UtcNow.AddMinutes(int.Parse(builder.Configuration["JwtSettings:ExpirationMinutes"]!)),
        Issuer = builder.Configuration["JwtSettings:Issuer"],
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    return Results.Ok(new { token = tokenString });
});

app.MapGet("/protected", () => Results.Ok("Protected data accessed!")).RequireAuthorization();

app.MapGet("/puzzles/next", async (int packId, int userId, AppDbContext db) =>
{
    var solvedPuzzleIds = await db.UserPuzzleProgresses
        .Where(up => up.UserId == userId && up.Solved)
        .Select(up => up.PuzzleId)
        .ToListAsync();

    var puzzle = await db.Puzzles
        .Where(p => p.PackId == packId && !solvedPuzzleIds.Contains(p.Id))
        .OrderBy(p => Guid.NewGuid())
        .FirstOrDefaultAsync();

    if (puzzle == null) return Results.NotFound(new { message = "No puzzles left in this pack." });
    return Results.Ok(puzzle);
}).RequireAuthorization();

app.MapPost("/game/submit", async (GameSubmission submission, AppDbContext db) =>
{
    db.GameSubmissions.Add(submission);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Game submitted successfully!" });
}).RequireAuthorization();

app.MapGet("/packs", async (AppDbContext db) =>
{
    var packs = await db.Packs.ToListAsync();
    return Results.Ok(packs);
});

app.MapPost("/packs", async (Pack pack, AppDbContext db) =>
{
    pack.CreatedAt = DateTime.UtcNow;
    db.Packs.Add(pack);
    await db.SaveChangesAsync();
    return Results.Created($"/packs/{pack.Id}", pack);
});

app.Run();