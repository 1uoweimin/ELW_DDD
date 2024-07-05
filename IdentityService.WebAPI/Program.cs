using CommonInitializer;
using CommonInitializer.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var initializerOptions = new InitializerOptions(
    logFilePathOption: "FileLogs:IdentityServiceLog",
    eventBusQueueName: "IdentityService_q",
    isRedis: false);
builder.AddInitializer(initializerOptions);
builder.AddIdentityService();

Action<List<ApiVersionInfo.Version>> vInfos = vDesc =>
{
    vDesc.Add(new() { Name = "v1", Description = "这个服务版本的接口是实现权限校验的。" });
    vDesc.Add(new() { Name = "v2", Description = "这个服务版本的接口是实现权限校验的。\r\n增加了统一类型的请求报文头和响应报文头。" });
};
builder.AddDevelopment(vInfos);

var app = builder.Build();

app.UseDevelopment(vInfos);

app.UseInitializer(initializerOptions);

app.MapControllers();
app.Run();