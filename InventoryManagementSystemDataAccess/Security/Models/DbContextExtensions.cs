using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SecurityApplicationDataAccess.Models
{
    /*
    public static class DbContextExtensions
    {
        public static async Task<List<T>> SqlQueryAsync<T>(this DbContext db, string sql, object[] parameters = null, CancellationToken cancellationToken = default)
           where T : class
        {
            if (parameters is null)
            {
                parameters = new object[] { };
            }

            if (typeof(T).GetProperties().Any())
            {
                return await db.Database
                    .SqlQueryRaw<T>(sql, parameters)
                    .ToListAsync(cancellationToken);
            }
            else
            {
                await db.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
                return default;
            }
        }
    }

    public class OutputParameter<TValue>
    {
        private bool _valueSet = false;

        public TValue _value;

        public TValue Value
        {
            get
            {
                if (!_valueSet)
                    throw new InvalidOperationException("Value not set.");

                return _value;
            }
        }

        internal void SetValue(object value)
        {
            _valueSet = true;

            _value = null == value || Convert.IsDBNull(value) ? default(TValue) : (TValue)value;
        }
    }
    */
}

