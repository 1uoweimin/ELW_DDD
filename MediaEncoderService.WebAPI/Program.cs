using CommonInitializer;
using CommonInitializer.Options;
using MediaEncoderService.WebAPI.BgService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var initializerOptions = new InitializerOptions(
    logFilePathOption: "FileLogs:MediaEncodingServiceLog",
    eventBusQueueName: "MediaEncoding_q");
builder.AddInitializer(initializerOptions);
builder.AddMediaEncoder("FileService:Endpoint");
builder.Services.AddHttpClient();
builder.Services.AddHostedService<EncodingBgService>();

Action<List<ApiVersionInfo.Version>> vInfos = vDesc =>
{
    vDesc.Add(new ApiVersionInfo.Version() { Name = "v1", Description = "�������汾����������ת���¼��������ת��ġ�" });
};
builder.AddDevelopment(vInfos);

var app = builder.Build();

app.UseDevelopment(vInfos);

app.UseInitializer(initializerOptions);

app.MapControllers();
app.Run();
