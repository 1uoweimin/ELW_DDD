using CommonInitializer;
using CommonInitializer.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var initializerOptions = new InitializerOptions(
    logFilePathOption: "FileLogs:SearchServiceLog",
    eventBusQueueName: "SearchService_q");
builder.AddInitializer(initializerOptions);
builder.AddSearch("ElasticSearch");

Action<List<ApiVersionInfo.Version>> vInfos = vDesc =>
{
    vDesc.Add(new ApiVersionInfo.Version() { Name = "v1", Description = "�������汾�Ľӿ�����������ԭ�ĵġ�" });
    vDesc.Add(new ApiVersionInfo.Version() { Name = "v2", Description = "�������汾�Ľӿ�����������ԭ�ĵġ�\r\n������ͳһ���͵�������ͷ����Ӧ����ͷ��" });
};
builder.AddDevelopment(vInfos);

var app = builder.Build();

app.UseDevelopment(vInfos);

app.UseInitializer(initializerOptions);

app.MapControllers();
app.Run();
