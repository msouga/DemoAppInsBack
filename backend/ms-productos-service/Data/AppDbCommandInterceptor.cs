using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ms_productos_service.Data
{
    public class AppDbCommandInterceptor : IDbCommandInterceptor
    {
        private readonly TelemetryClient _telemetryClient;

        public AppDbCommandInterceptor(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        // Interceptación para comandos de lectura (Reader)
        public InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            TrackDependency(command, eventData, "SQL", "ReaderExecuting");
            return result;
        }

        public async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            TrackDependency(command, eventData, "SQL", "ReaderExecutingAsync");
            return result;
        }

        // Interceptación para comandos que no son de lectura (NonQuery)
        public InterceptionResult<int> NonQueryExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<int> result)
        {
            TrackDependency(command, eventData, "SQL", "NonQueryExecuting");
            return result;
        }

        public async ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            TrackDependency(command, eventData, "SQL", "NonQueryExecutingAsync");
            return result;
        }

        // Interceptación para comandos escalares (Scalar)
        public InterceptionResult<object> ScalarExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<object> result)
        {
            TrackDependency(command, eventData, "SQL", "ScalarExecuting");
            return result;
        }

        public async ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<object> result,
            CancellationToken cancellationToken = default)
        {
            TrackDependency(command, eventData, "SQL", "ScalarExecutingAsync");
            return result;
        }

        // Métodos de post-ejecución (no necesitamos modificarlos para este objetivo, pero los incluimos)
        public DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result) => result;
        public async ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default) => result;
        public int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result) => result;
        public async ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default) => result;
        public object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result) => result;
        public async ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result, CancellationToken cancellationToken = default) => result;

        // Método auxiliar para registrar la dependencia
        private void TrackDependency(DbCommand command, CommandEventData eventData, string type, string commandType)
        {
            var dependencyTelemetry = new Microsoft.ApplicationInsights.DataContracts.DependencyTelemetry
            {
                Type = type,
                Target = command.Connection?.DataSource, // O el nombre de la base de datos si está disponible
                Name = commandType, // Por ejemplo, "ReaderExecutingAsync"
                Data = command.CommandText, // La consulta SQL
                Duration = TimeSpan.Zero, // La duración real se capturará automáticamente por la integración de AI con EF Core
                ResultCode = "0", // O un código de resultado apropiado si se pudiera determinar aquí
                Success = true, // Asumimos éxito antes de la ejecución; el seguimiento automático de AI ajustará esto
                Timestamp = eventData.StartTime.UtcDateTime // Usar el tiempo de inicio del evento
            };

            // Añadir parámetros como propiedades personalizadas
            if (command.Parameters != null)
            {
                for (int i = 0; i < command.Parameters.Count; i++)
                {
                    var param = command.Parameters[i];
                    // Evitar registrar valores de parámetros sensibles si es necesario
                    // Por ahora, registramos todos los parámetros
                    dependencyTelemetry.Properties[$"Parameter_{param.ParameterName}"] = param.Value?.ToString() ?? "NULL";
                }
            }

            _telemetryClient.TrackDependency(dependencyTelemetry);
        }
    }
}