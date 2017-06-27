using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using FluentValidation.Attributes;
using Nop.Plugin.Widgets.RecentPurchases.Validators;

namespace Nop.Plugin.Widgets.RecentPurchases.Models
{
    [Validator(typeof(ConfigurationValidator))]
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        public ConfigurationModel()
        {
            AvailableZones = new List<SelectListItem>();
            AvailableWindowTypes = new List<SelectListItem>();
        }

        [NopResourceDisplayName(Constants.LocaleResources.RecentPurchases_Fields_WindowSize)]
        public int WindowSize { get; set; }
        public bool WindowSize_OverrideForStore { get; set; }

        [NopResourceDisplayName(Constants.LocaleResources.RecentPurchases_Fields_WindowType)]
        public int WindowTypeId { get; set; }
        public IList<SelectListItem> AvailableWindowTypes { get; set; }
        public bool WindowTypeId_OverrideForStore { get; set; }

        [NopResourceDisplayName(Constants.LocaleResources.RecentPurchases_Fields_Zone)]
        public string ZoneId { get; set; }
        public IList<SelectListItem> AvailableZones { get; set; }
        public bool ZoneId_OverrideForStore { get; set; }

        [NopResourceDisplayName(Constants.LocaleResources.RecentPurchases_Fields_ShowProductImage)]
        public bool ShowProductImage { get; set; }
        public bool ShowProductImage_OverrideForStore { get; set; }

        [NopResourceDisplayName(Constants.LocaleResources.RecentPurchases_Fields_ShowQuantity)]
        public bool ShowQuantity { get; set; }
        public bool ShowQuantity_OverrideForStore { get; set; }
    }
}