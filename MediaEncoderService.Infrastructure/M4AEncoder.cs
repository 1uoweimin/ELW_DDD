using FFmpeg.NET;
using MediaEncoderService.Domain;

namespace MediaEncoderService.Infrastructure;

internal class M4AEncoder : IMediaEncoder
{
    public bool Support(string format)
        => format.Equals("m4a", StringComparison.OrdinalIgnoreCase);

    public async Task EncodeAsync(FileInfo sourceFile, FileInfo destFile, string destFormat, string[]? arg, CancellationToken cancellationToken)
    {
        //可以用“FFmpeg.AutoGen”，因为他是bingding库，不用启动独立的进程，更靠谱。但是编程难度大，这里重点不是FFMPEG，所以先用命令行实现

        string ffmpegPath = Path.Combine(AppContext.BaseDirectory, "ffmpeg.exe");
        Engine ffmpeg = new Engine(ffmpegPath);

        // 监听错误事件
        string? errorMsg = null;
        ffmpeg.Error += (s, e) =>
        {
            errorMsg = e.Exception.Message;
        };

        // 进行转码
        InputFile inputFile = new InputFile(sourceFile);
        OutputFile outputFile = new OutputFile(destFile);
        await ffmpeg.ConvertAsync(inputFile, outputFile, cancellationToken);

        if (errorMsg != null)
            throw new ApplicationException(errorMsg);
    }
}
