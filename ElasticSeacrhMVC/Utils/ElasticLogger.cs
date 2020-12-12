using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchMVC.Utils
{
    public class ElasticLogger
    {
        private static Serilog.Core.Logger _log;
        public bool IsConfigured { get; set; } = false;

        #region Singleton Section
        private static readonly Lazy<ElasticLogger> _instance = new Lazy<ElasticLogger>(() => new ElasticLogger());

        private ElasticLogger()
        {
            Configure();
        }

        public static ElasticLogger Instance => _instance.Value;
        #endregion

        public void Error(Exception ex, string messageTemplate)
        {
            if (IsConfigured)
                _log.Error(ex, messageTemplate);
        }

        public void Info(string messageTemplate)
        {
            if (IsConfigured)
                _log.Information(messageTemplate);
        }

        private void Configure()
        {
            string elasticUri = Environment.GetEnvironmentVariable("ELASTIC_URI");
            string format = Environment.GetEnvironmentVariable("LOG_INDEX_FORMAT");  // ex.  "serilog-{0:yyyy.MM.dd}"

            if (string.IsNullOrEmpty(elasticUri) || string.IsNullOrEmpty(format))
                throw new ArgumentNullException($"env:ELASTIC_URI ve env:LOG_INDEX_FORMAT cannot be null!");

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri(elasticUri))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                        TemplateName = "serilog-events-template",
                        IndexFormat = format
                    });

            _log = loggerConfiguration.CreateLogger();

            IsConfigured = true;
        }
    }
}
