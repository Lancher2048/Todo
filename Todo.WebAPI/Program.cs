using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Todo.Commons;
using Todo.Core;
using Todo.WebAPI.App_Start;
using Todo.Commons.Log;
using log4net;
using log4net.Repository;
using System.Text.Json;
using Todo.Commons.Converter;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Todo.Commons.App;

var builder = WebApplication.CreateBuilder(args);

#region 日志
builder.Logging.AddFilter("System", LogLevel.Error)
    .AddFilter("Microsoft", LogLevel.Error)
    .SetMinimumLevel(LogLevel.Error)
    .AddLog4Net(Path.Combine(Directory.GetCurrentDirectory(), "xmlconfig\\log4net.config"));

ILoggerRepository loggerRepository = LogManager.CreateRepository("NETCoreRepository");
Log4NetHelper.SetConfig(loggerRepository, "xmlconfig\\log4net.config");
#endregion

// Add services to the container.

builder.Services.AddSingleton(new Appsettings(builder.Configuration));
builder.Services.AddSwaggerSetup();
builder.Services.AddEFCoreSetup();
builder.Services.AddAuthorizationSetup();


#region 控制器
builder.Services.AddControllers(n =>
{
    n.Filters.Add(typeof(GlobalExceptionsFilter));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

    options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new DateTimeNullableConverter());

    options.JsonSerializerOptions.Converters.Add(new BooleanJsonConverter());

    options.JsonSerializerOptions.Converters.Add(new DecimalJsonConverter());

    options.JsonSerializerOptions.Converters.Add(new IntJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new LongJsonConverter());
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
});

builder.Services.AddEndpointsApiExplorer();
#endregion


builder.Services.AddSingleton(new JwtHelper(builder.Configuration));


// 雪花ID配置
SnowflakeHelper.SetIdWorker(new IdWorkOptions { 
    workId = 1,
    datacenterId = 1
});


var app = builder.Build();

// Configure the HTTP request pipeline.
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
