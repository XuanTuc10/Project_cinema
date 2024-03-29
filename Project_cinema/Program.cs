using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Project_cinema.Constants;
using Project_cinema.Payloads.Converters;
using Project_cinema.Payloads.DataResponses.DataToken;
using Project_cinema.Payloads.DataResponses.DataUser;
using Project_cinema.Payloads.Responses;
using Project_cinema.Services.Implements;
using Project_cinema.Services.Interfaces;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System;
using Project_cinema.DataContexts;
using Project_cinema.Payloads.DataResponses.DataCinema;
using Project_cinema.Payloads.DataResponses.DataFood;
using Project_cinema.Payloads.DataResponses.DataMovie;
using Project_cinema.Payloads.DataResponses.DataRoom;
using Project_cinema.Payloads.DataResponses.DataSeat;
using Project_Room.Payloads.Converters;
using Project_cinema.Payloads.DataResponses.DataOrder;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Add services to the container.

builder.Services.AddControllersWithViews()
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddCors();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "NS.Core API", Version = "v1" });
    option.CustomSchemaIds(type => type.ToString());
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICinemaService, CinemaService>();
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ResponseObject<DataResponseUser>>();
builder.Services.AddScoped<ResponseObject<DataResponseToken>>();
builder.Services.AddScoped<ResponseObject<DataResponseCinema>>();
builder.Services.AddScoped<ResponseObject<DataResponseFood>>();
builder.Services.AddScoped<ResponseObject<DataResponseMovie>>();
builder.Services.AddScoped<ResponseObject<DataResponseRoom>>();
builder.Services.AddScoped<ResponseObject<DataResponseSeat>>();
builder.Services.AddScoped<ResponseObject<DataResponseOrder>>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserConverter>();
builder.Services.AddScoped<CinemaConverter>();
builder.Services.AddScoped<FoodConverter>();
builder.Services.AddScoped<MovieConverter>();
builder.Services.AddScoped<RoomConverter>();
builder.Services.AddScoped<ScheduleConverter>();
builder.Services.AddScoped<SeatConverter>();
builder.Services.AddScoped<TicketConverter>();
builder.Services.AddScoped<OrderConverter>();
builder.Services.AddScoped<BillFoodConverter>();
builder.Services.AddScoped<BillTicketConverter>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration.GetSection(AppSettingsKeys.AUTH_SECRET).Value!))
    };
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
 
/*app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());*/
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthorization();

app.MapControllers();

app.Run();
