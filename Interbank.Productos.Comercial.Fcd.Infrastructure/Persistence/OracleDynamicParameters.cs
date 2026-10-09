using Dapper;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace IInterbank.Productos.Comercial.Fcd.Infrastructure.Persistence
{
    public class OracleDynamicParameters : SqlMapper.IDynamicParameters
    {
        private readonly DynamicParameters dynamicParameters = new DynamicParameters();
        private readonly List<OracleParameter> oracleParameters = new List<OracleParameter>();

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            ((SqlMapper.IDynamicParameters)dynamicParameters).AddParameters(command, identity);


            if (command is OracleCommand oracleCommand) oracleCommand.Parameters.AddRange(oracleParameters.ToArray());
        }

#nullable disable
        public void Add(string name, OracleDbType oracleDbType, ParameterDirection direction, object value = null,
            int? size = null)
        {
            OracleParameter oracleParameter;

            if (value != null)
            {
                if (size.HasValue)
                {
                    oracleParameter = new OracleParameter(name, oracleDbType, size.Value, value, direction);
                }
                else
                {
                    oracleParameter = new OracleParameter(name, oracleDbType, value, direction);
                }
            }
            else
            {
                oracleParameter = new OracleParameter(name, oracleDbType, direction);
            }

            oracleParameters.Add(oracleParameter);
        }
        public void Add(string name, OracleDbType oracleDbType, ParameterDirection direction, object value,
           byte precision, byte scale)
        {
            OracleParameter oracleParameter;
            oracleParameter = new OracleParameter(name, oracleDbType, value, direction)
            {
                Precision = precision,
                Scale = scale
            };
            oracleParameters.Add(oracleParameter);
        }

        public T Get<T>(string parameterName)
        {
            var param = oracleParameters.SingleOrDefault(p => p.ParameterName == parameterName);
            if (param == null || param.Value == DBNull.Value)
            {
                // Si T es no-nullable y no es clase ni nullable, lanzamos excepción
                if (!typeof(T).IsClass && Nullable.GetUnderlyingType(typeof(T)) == null)
                    throw new InvalidOperationException($"Attempting to cast DBNull to non-nullable type {typeof(T).Name}");
                return default!;
            }

            // Manejo especial OracleDecimal
            if (param.Value is OracleDecimal od)
                return (T)Convert.ChangeType(od.Value, typeof(T));

            // Manejo OracleString
            if (param.Value is OracleString os)
                return (T)Convert.ChangeType(os.Value, typeof(T));

            // Manejo general
            if (param.Value is IConvertible)
                return (T)Convert.ChangeType(param.Value, typeof(T));

            // Si no es convertible, cast directo
            return (T)param.Value;
        }

        public T Get<T>(int index)
        {
            var param = oracleParameters[index];
            if (param == null || param.Value == DBNull.Value)
                return default!;

            if (param.Value is OracleDecimal od)
                return (T)Convert.ChangeType(od.Value, typeof(T));

            if (param.Value is OracleString os)
                return (T)Convert.ChangeType(os.Value, typeof(T));

            if (param.Value is IConvertible)
                return (T)Convert.ChangeType(param.Value, typeof(T));

            return (T)param.Value;
        }

        public void Set<T>(string parameterName, T value)
        {
            var param = oracleParameters.SingleOrDefault(p => p.ParameterName == parameterName);
            if (param == null)
                throw new ArgumentException($"No parameter found with name '{parameterName}'");

            param.Value = value;
        }



    }
#nullable restore
}
