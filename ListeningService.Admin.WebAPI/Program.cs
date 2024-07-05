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
    vDesc.Add(new ApiVersionInfo.Version() { Name = "v1", Description = "�������汾�Ľӿ�����������������Դ�ġ�" });
    vDesc.Add(new ApiVersionInfo.Version() { Name = "v2", Description = "�������汾�Ľӿ�����������������Դ�ġ�\r\n������ͳһ���͵�������ͷ����Ӧ����ͷ��" });
};
builder.AddDevelopment(vInfos);

var app = builder.Build();

app.UseDevelopment(vInfos);

app.UseInitializer(initializerOptions);
app.MapHub<EpisodeEncodingStatusHub>("/Hubs/EpisodeEncodingStatusHub");

app.MapControllers();
app.Run();
