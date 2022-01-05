using Microsoft.EntityFrameworkCore;
using ProiectP3_BackendApp.Models;
using ProiectP3_BackendApp.Trees;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. 
builder.Services.AddControllers();
builder.Services.AddDbContext<UserContext>(opt =>
    opt.UseInMemoryDatabase("UserList"));
builder.Services.AddDbContext<ReviewContext>(opt =>
    opt.UseInMemoryDatabase("ReviewList"));
builder.Services.AddDbContext<MenuItemContext>(opt =>
    opt.UseInMemoryDatabase("MenuItemList"));
builder.Services.AddDbContext<OrderContext>(opt =>
    opt.UseInMemoryDatabase("OrderList"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
