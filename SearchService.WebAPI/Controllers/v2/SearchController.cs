using CommonInitializer.ActionDatas;
using Microsoft.AspNetCore.Mvc;
using SearchService.DomainService;
using SearchService.DomainService.Entities;
using SearchService.WebAPI.Controllers.v2.Requests;
using SearchService.WebAPI.Controllers.v2.Responses;
using Zack.EventBus;

namespace SearchService.WebAPI.Controllers.v2;

[Route("api/v2/[controller]/[action]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v2")] // 版本控制
public class SearchController : ControllerBase
{
    private readonly IEventBus _eventBus;
    private readonly ISearchRepository _searchRepository;
    public SearchController(IEventBus eventBus, ISearchRepository searchRepository)
    {
        _eventBus = eventBus;
        _searchRepository = searchRepository;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<SearchEpisodesRsp>>> SearchEpisodes([FromQuery] ApiRequest<SearchEpisodesReq> req)
    {
        (IEnumerable<Episode> episodes, long totalCount) = await _searchRepository.SearchEpisodesAsync(req.ReqData.KeyWord, req.ReqData.PageIndex, req.ReqData.PageSize);
        return ApiResponse<SearchEpisodesRsp>.Succeed(new(episodes, totalCount));
    }

    [HttpGet]
    public IActionResult ReIndexAll()
    {
        //避免耦合，这里发送ReIndexAll的集成事件
        //所有向搜索系统贡献数据的系统都可以响应这个事件，重新贡献数据
        _eventBus.Publish("SearchService.ReIndexAll", null);
        return Ok();
    }
}