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
    vDesc.Add(new() { Name = "v1", Description = "这个服务版本的接口是用来上传文件，并且记录到数据库中。" });
    vDesc.Add(new() { Name = "v2", Description = "这个服务版本的接口是用来上传文件，并且记录到数据库中。\r\n增加了统一类型的请求报文头和响应报文头。" });
};
builder.AddDevelopment(vInfos);

var app = builder.Build();

app.UseDevelopment(vInfos);

app.UseStaticFiles(); //启动静态文件服务器（里面的文件在服务器上不经过任何处理就直接发送到客户端，默认在wwwroot目录下查找）
app.UseInitializer(initializerOptions);

app.MapControllers();
app.Run();