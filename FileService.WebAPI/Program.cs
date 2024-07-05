using CommonInitializer;
using CommonInitializer.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var initializerOptions = new InitializerOptions(
    logFilePathOption: "FileLogs:FileServiceLog",
    eventBusQueueName: null,
    isRedis: false);
builder.AddInitializer(initializerOptions);
builder.AddFileService("FileService:SMB", "FileService:Endpoint");

Action<List<ApiVersionInfo.Version>> vInfos = vDesc =>
{
    vDesc.Add(new() { Name = "v1", Description = "�������汾�Ľӿ��������ϴ��ļ������Ҽ�¼�����ݿ��С�" });
    vDesc.Add(new() { Name = "v2", Description = "�������汾�Ľӿ��������ϴ��ļ������Ҽ�¼�����ݿ��С�\r\n������ͳһ���͵�������ͷ����Ӧ����ͷ��" });
};
builder.AddDevelopment(vInfos);

var app = builder.Build();

app.UseDevelopment(vInfos);

app.UseStaticFiles(); //������̬�ļ���������������ļ��ڷ������ϲ������κδ����ֱ�ӷ��͵��ͻ��ˣ�Ĭ����wwwrootĿ¼�²��ң�
app.UseInitializer(initializerOptions);

app.MapControllers();
app.Run();