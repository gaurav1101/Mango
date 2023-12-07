using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Extensions;
using Mango.Services.EmailAPI.Messaging;
using Mango.Services.EmailAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>(); //current statement
builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

//from line-28 to 30 explanations
// Because We want to consume and prosess our emails from azureServiceBus we have created AzureServiceBusConsumer.cs
// and in lie 16 we have scoped it singleton bcoz we want it up to consume email everytime and save email to db.
//so to consume ApplicationDbContext into AzureServiceBusConsumer.cs we need an singleton scopped appDbcontext as well
//which is done below, and is consumed in singleton EmailService.
//EmailService is consumed in AzureServiceBusConsumer.

var optionBuilder =new DbContextOptionsBuilder<ApplicationDBContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(new EmailService(optionBuilder.Options));
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
app.UseAzureServiceBusConsumer();
app.Run();
