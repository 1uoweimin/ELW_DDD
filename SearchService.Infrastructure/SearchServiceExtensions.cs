using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using SearchService.DomainService;
using SearchService.Infrastructure;
using SearchService.Infrastructure.Options;
using System.Net;
using System.Text;

namespace Microsoft.AspNetCore.Builder;

public static class SearchServiceExtensions
{
    public static void AddSearch(this WebApplicationBuilder builder, string elasticSearchOpt)
    {
        var services = builder.Services;

        #region 使用 Nest 库
        //services.AddScoped<IElasticClient>(sp =>
        //{
        //    ElasticSearchOptions options = builder.Configuration.GetSection(elasticSearchOption).Get<ElasticSearchOptions>()!;
        //    string host = options.Url;
        //    string userName = options.UserName;
        //    string password = options.Password;

        //    string[] temp = host.Split("//");
        //    if (temp.Length != 2) throw new InputFormatterException();
        //    string urlString = $"{temp[0]}//{userName}:{password}@{temp[1]}";

        //    var settings = new ConnectionSettings(new Uri(urlString));
        //    return new ElasticClient(settings);
        //});
        //services.AddScoped<ISearchRepository, SearchRepository_v7>();
        #endregion

        #region 使用 Elastic.Clients.Elasticsearch 库
        services.AddScoped<ElasticsearchClient>(sp =>
        {
            ElasticSearchOptions options = builder.Configuration.GetSection(elasticSearchOpt).Get<ElasticSearchOptions>()!;

            var settings = new ElasticsearchClientSettings(new Uri(options.Url))
                .CertificateFingerprint(options.C_Fingerprint) // 该值查看kibana的配置文件 kibana/config/fibana.yml 中 ca_trusted_fingerprint 的值。
                .Authentication(new BasicAuthentication(options.UserName, options.Password));

            return new ElasticsearchClient(settings);
        });
        services.AddScoped<ISearchRepository, SearchRepository_v8>();
        #endregion
    }
}