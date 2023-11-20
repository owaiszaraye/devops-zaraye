using System.Collections.Generic;
using FluentMigrator;
using Zaraye.Core.Configuration;
using Zaraye.Core.Infrastructure;
using Zaraye.Data.Migrations;
using Zaraye.Framework.WebOptimizer;

namespace Zaraye.Framework.Migrations.UpgradeTo450
{

    [NopMigration("2021-10-07 00:00:00", "Pseudo-migration to update appSettings.json file", MigrationProcessType.Update)]
    public class AppSettingsMigration : MigrationBase
    {
        public override void Up()
        {
            var fileProvider = EngineContext.Current.Resolve<IZarayeFileProvider>();

            var rootDir = fileProvider.MapPath("~/");

            var config = new WebOptimizerConfig {
                EnableTagHelperBundling = false,
                EnableCaching = true,
                EnableDiskCache = true,
                AllowEmptyBundle = true,
                CacheDirectory = fileProvider.Combine(rootDir, @"wwwroot\bundles")
            };

            AppSettingsHelper.SaveAppSettings(new List<IConfig> { config }, fileProvider);
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}