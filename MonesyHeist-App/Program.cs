using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MonesyHeist_App.Data;
using MonesyHeist_App.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer("Data Source=KUKICRO\\SQLEXPRESS;Initial Catalog=MoneyHeist;Integrated Security=True;Pooling=False")
);
builder.Services.AddScoped<MemberService>();


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
