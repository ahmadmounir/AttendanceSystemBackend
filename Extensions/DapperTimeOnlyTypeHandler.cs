using System;
using System.Data;
using Dapper;

namespace AttendanceSystemBackend.Extensions
{
    public class DapperTimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
    {
        public override void SetValue(IDbDataParameter parameter, TimeOnly value)
        {
            parameter.DbType = DbType.Time;
            parameter.Value = value.ToTimeSpan();
        }

        public override TimeOnly Parse(object value)
        {
            if (value is TimeSpan ts)
                return TimeOnly.FromTimeSpan(ts);

            if (value is DateTime dt)
                return TimeOnly.FromDateTime(dt);

            if (value is string s && TimeOnly.TryParse(s, out var parsed))
                return parsed;

            return TimeOnly.Parse(value.ToString() ?? string.Empty);
        }
    }
}
