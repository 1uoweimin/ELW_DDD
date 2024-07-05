using CommonInitializer;
using CommonInitializer.Options;
using ListeningService.Admin.WebAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var initializerOptions = new InitializerOptions(
    logFilePathOption: "FileLogs:ListeningServiceAdminLog",
    eventBusQueueName: "Listening_Admin_q");
builder.AddInitializer(initializerOptions);
builder.AddListening();
builder.Services.AddEncodingEpisode();
builder.Services.AddSignalR();

Action<List<ApiVersionInfo.Version>> vInfos = vDesc =>
{
    vDesc.Add(new ApiVersionInfo.Version() { Name = "v1", Description = "这个服务版本的接口是用来管理听力资源的。" });
    vDesc.Add(new ApiVersionInfo.Version() { Name = "v2", Description = "这个服务版本的接口是用来管理听力资源的。\r\n增加了统一类型的请求报文头和响应报文头。" });
};
builder.AddDevelopment(vInfos);

var app = builder.Build();

app.UseDevelopment(vInfos);

app.UseInitializer(initializerOptions);
app.MapHub<EpisodeEncodingStatusHub>("/Hubs/EpisodeEncodingStatusHub");

app.MapControllers();
app.Run();
