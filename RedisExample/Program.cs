using RedisExample.Redis;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//REDIS
builder.Services.Configure<RedisConfig>(a => builder.Configuration.GetSection(nameof(RedisConfig)).Bind(a));
builder.Services.AddScoped<ICache,RedisCache>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("RedisConfig").GetValue<string>("Configuration");
    var password = builder.Configuration.GetSection("RedisConfig").GetValue<string>("Password");
    if (!string.IsNullOrEmpty(password))
    {
        options.ConfigurationOptions = new ConfigurationOptions();
        options.ConfigurationOptions.Password = password;
        options.ConfigurationOptions.SyncTimeout = 10000;
        options.ConfigurationOptions.EndPoints.Add(options.Configuration);
    }
});
//REDIS

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