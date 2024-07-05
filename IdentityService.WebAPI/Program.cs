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
    vDesc.Add(new() { Name = "v1", Description = "�������汾�Ľӿ���ʵ��Ȩ��У��ġ�" });
    vDesc.Add(new() { Name = "v2", Description = "�������汾�Ľӿ���ʵ��Ȩ��У��ġ�\r\n������ͳһ���͵�������ͷ����Ӧ����ͷ��" });
};
builder.AddDevelopment(vInfos);

var app = builder.Build();

app.UseDevelopment(vInfos);

app.UseInitializer(initializerOptions);

app.MapControllers();
app.Run();