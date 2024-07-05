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
    vDesc.Add(new ApiVersionInfo.Version() { Name = "v1", Description = "这个服务版本的接口是用来搜索原文的。" });
    vDesc.Add(new ApiVersionInfo.Version() { Name = "v2", Description = "这个服务版本的接口是用来搜索原文的。\r\n增加了统一类型的请求报文头和响应报文头。" });
};
builder.AddDevelopment(vInfos);

var app = builder.Build();

app.UseDevelopment(vInfos);

app.UseInitializer(initializerOptions);

app.MapControllers();
app.Run();
