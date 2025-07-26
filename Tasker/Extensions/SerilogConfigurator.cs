using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace Tasker.Extensions
{
    public static class SerilogConfigurator
    {
        public static LoggerConfiguration Configure(IConfiguration configuration)
        {
            var columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter() },
            { "MessageTemplate", new MessageTemplateColumnWriter() },
            { "Level", new LevelColumnWriter() },
            { "TimeStamp", new TimestampColumnWriter() },
            { "Exception", new ExceptionColumnWriter() },
            { "Properties", new LogEventSerializedColumnWriter() }
        };

            return new LoggerConfiguration()
                //.Enrich.FromLogContext()
                //.Enrich.WithEnvironmentName()
                //.WriteTo.Console()
                .WriteTo.PostgreSQL(
                    connectionString: configuration.GetConnectionString("DefaultConnection"),
                    tableName: "logs",
                    columnOptions: columnWriters,
                    needAutoCreateTable: false,
                    useCopy: false)
                .MinimumLevel.Error();
        }
    }
}
