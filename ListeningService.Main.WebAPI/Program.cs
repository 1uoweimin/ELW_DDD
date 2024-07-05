using CommonInitializer;
using CommonInitializer.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var initializerOptions = new InitializerOptions(
    logFilePathOption: "FileLogs:ListeningServiceMainLog",
    eventBusQueueName: "Listening_Main_q");
builder.AddInitializer(initializerOptions);
builder.AddListening();

Action<List<ApiVersionInfo.Version>> vInfos = vDesc =>
{
    vDesc.Add(new ApiVersionInfo.Version() { Name = "v1", Description = "�������汾�Ľӿ�����������������Դ�ġ�" });
    vDesc.Add(new ApiVersionInfo.Version() { Name = "v2", Description = "�������汾�Ľӿ�����������������Դ�ġ�\r\n������ͳһ���͵�������ͷ����Ӧ����ͷ��" });
};
builder.AddDevelopment(vInfos);

var app = builder.Build();

app.UseDevelopment(vInfos);

app.UseInitializer(initializerOptions);

app.MapControllers();
app.Run();
