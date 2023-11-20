using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Humanizer;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zaraye.Framework.Extensions
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
        /// </summary>
        /// <param name="source">Source (UTC format)</param>
        /// <param name="languageCode">Language culture code</param>
        /// <returns>Formatted date and time string</returns>
        public static string RelativeFormat(this DateTime source, string languageCode = "en-US")
        {
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - source.Ticks);
            var delta = ts.TotalSeconds;

            CultureInfo culture;
            try
            {
                culture = new CultureInfo(languageCode);
            }
            catch (CultureNotFoundException)
            {
                culture = new CultureInfo("en-US");
            }
            return TimeSpan.FromSeconds(delta).Humanize(precision: 1, culture: culture, maxUnit: TimeUnit.Year);
        }
    }
}