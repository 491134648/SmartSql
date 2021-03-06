﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SmartSql;
using SmartSql.Abstractions;
using System;
using System.IO;
namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmartSqlExtensions
    {
        public static void AddSmartSql(this IServiceCollection services)
        {
            services.AddSingleton<ISmartSqlMapper>(sp =>
            {
                var options = new SmartSqlOptions();
                InitOptions(sp, options);
                return MapperContainer.Instance.GetSqlMapper(options);
            });
            AddOthers(services);
        }

        public static void AddSmartSql(this IServiceCollection services, Func<IServiceProvider, SmartSqlOptions> setupOptions)
        {
            services.AddSingleton((sp =>
            {
                var options = setupOptions(sp);
                InitOptions(sp, options);
                return MapperContainer.Instance.GetSqlMapper(options);
            }));
            AddOthers(services);
        }

        private static void InitOptions(IServiceProvider sp, SmartSqlOptions options)
        {
            if (String.IsNullOrEmpty(options.ConfigPath))
            {
                var env = sp.GetService<IHostingEnvironment>();
                if (env != null && !env.IsProduction())
                {
                    options.ConfigPath = $"SmartSqlMapConfig.{env.EnvironmentName}.xml";
                }
                if (!File.Exists(options.ConfigPath))
                {
                    options.ConfigPath = Consts.DEFAULT_SMARTSQL_CONFIG_PATH;
                }
            }
            options.LoggerFactory = options.LoggerFactory ?? sp.GetService<ILoggerFactory>();
        }

        private static void AddOthers(IServiceCollection services)
        {
            services.AddSingleton<ISmartSqlMapperAsync>(sp =>
            {
                return sp.GetRequiredService<ISmartSqlMapper>();
            });
            services.AddSingleton<ITransaction>(sp =>
            {
                return sp.GetRequiredService<ISmartSqlMapper>();
            });
            services.AddSingleton<ISession>(sp =>
            {
                return sp.GetRequiredService<ISmartSqlMapper>();
            });
        }
    }
}
