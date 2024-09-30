using lastSETIONDEPI.Models.Context;
using lastSETIONDEPI.Models.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region DB

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#endregion

#region Identity

builder.Services.AddIdentity<SchoolUser, IdentityRole>(options =>
{
    options.Password.RequireUppercase = false;  
    options.Password.RequiredLength = 6;       
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

#endregion

#region Authentication

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", options =>
{
    var keyString = builder.Configuration.GetValue<string>("Jwt:SecretKey");
    var keyBytes = Encoding.ASCII.GetBytes(keyString);
    var key = new SymmetricSecurityKey(keyBytes);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = key,
        ValidateIssuer = false,        
        ValidateAudience = false,      
        ValidateLifetime = true,       
        ValidateIssuerSigningKey = true
    };
});

#endregion

#region Authorization


builder.Services.AddAuthorization(options =>
{
 
    options.AddPolicy("TeacherPolicy", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Teacher"));

   
    options.AddPolicy("StudentPolicy", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Student"));
});

#endregion

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();  

app.UseAuthorization();  

app.MapControllers();

app.Run();
