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

var builder = WebApplication.CreateBuilder(args);

builder.Host
.ConfigureLogging((hostingContext, builder) =>
{
    builder.AddFilter("System", LogLevel.Error);
    builder.AddFilter("Microsoft", LogLevel.Error);
    builder.SetMinimumLevel(LogLevel.Error);
    builder.AddLog4Net(Path.Combine(Directory.GetCurrentDirectory(), "log4net.config"));
});

ILoggerRepository loggerRepository = LogManager.CreateRepository("NETCoreRepository");
Log4NetHelper.SetConfig(loggerRepository, "log4net.config");

// Add services to the container.

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "swaggerapi API文档", Description = "swaggerapi API的使用说明" });
    //options.SwaggerDoc("", new Microsoft.OpenApi.Models.OpenApiInfo 
    //{ 
    //    Version = "v1",
    //    Title = "swaggerapi API文档",
    //    Description = "swaggerapi API的使用说明"
    //});

    // 为 Swagger JSON and UI设置xml文档注释路径
    // 获取应用程序所在目录（绝对路径，不受工作目录影响，建议采用此方法获取路径）
    // 此方式适用于Windows/Linux 平台
    //var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
    //var xmlPath = Path.Combine(basePath, "NetCore.Swagger.xml");
    //options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("bearerAuth", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "请输入token进行验证"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
            },
            new string[] {}
        }
    });

    options.OperationFilter<SwaggerFilter>();

});

builder.Services.AddSingleton(new JwtHelper(builder.Configuration));
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuer = true, //是否验证Issuer
        ValidIssuer = builder.Configuration["Jwt:Issuer"], //发行人Issuer
        ValidateAudience = true, //是否验证Audience
        ValidAudience = builder.Configuration["Jwt:Audience"], //订阅人Audience
        ValidateIssuerSigningKey = true, //是否验证SecurityKey
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])), //SecurityKey
        ValidateLifetime = true, //是否验证失效时间
        ClockSkew = TimeSpan.FromSeconds(30), //过期时间容错值，解决服务器端时间不同步问题（秒）
        RequireExpirationTime = true,
    };
});

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
