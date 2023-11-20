using System.Collections.Generic;
using ClosedXML.Excel;
using Zaraye.Core.Domain.Localization;
using Zaraye.Services.ExportImport.Help;

namespace Zaraye.Services.ExportImport
{
    public class WorkbookMetadata<T>
    {
        public List<PropertyByName<T, Language>> DefaultProperties { get; set; }

        public List<PropertyByName<T, Language>> LocalizedProperties { get; set; }

        public IXLWorksheet DefaultWorksheet { get; set; }

        public List<IXLWorksheet> LocalizedWorksheets { get; set; }
    }
}
