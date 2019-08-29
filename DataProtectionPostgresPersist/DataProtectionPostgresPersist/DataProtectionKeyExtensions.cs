using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace Microsoft.AspNetCore.DataProtection
{
    /// <summary>
    /// DataProtectionKey Extensions
    /// </summary>
    public static class DataProtectionKeyExtensions
    {
        /// <summary>
        /// Persist Data Protection Keys To Postgres
        /// </summary>
        /// <param name="builder">IDataProtectionBuilder</param>
        /// <param name="connectionString">postgres Connection String</param>
        /// <returns></returns>
        public static IDataProtectionBuilder PersistKeysToPostgres(this IDataProtectionBuilder builder, string connectionString)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ApplicationException("connectionString is empty");
            }
            builder.Services.Configure<KeyManagementOptions>(options => options.XmlRepository = new DataProtectionKeyRepository(connectionString));
            return builder;
        }
    }
}
