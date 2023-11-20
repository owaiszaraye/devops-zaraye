﻿using System;
using System.Collections.Generic;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Orders;
using Zaraye.Services.ExportImport.Help;

namespace Zaraye.Services.ExportImport
{
    public partial class ImportOrderMetadata
    {
        public int EndRow { get; internal set; }

        public PropertyManager<Order, Language> Manager { get; internal set; }

        public IList<PropertyByName<Order, Language>> Properties { get; set; }

        public int CountOrdersInFile { get; set; }

        public PropertyManager<OrderItem, Language> OrderItemManager { get; internal set; }

        public List<Guid> AllOrderGuids { get; set; }

        public List<Guid> AllCustomerGuids { get; set; }
    }
}
