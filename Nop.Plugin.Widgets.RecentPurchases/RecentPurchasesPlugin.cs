using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Plugin.Widgets.RecentPurchases.Models;

namespace Nop.Plugin.Widgets.RecentPurchases
{
    /// <summary>
    /// Recent purchases provider
    /// </summary>
    public class RecentPurchasesPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ISettingService _settingService;
        private readonly RecentPurchasesSettings _recentPurchasesSettings;

        public RecentPurchasesPlugin(ISettingService settingService, RecentPurchasesSettings recentPurchasesSettings)
        {
            this._settingService = settingService;
            this._recentPurchasesSettings = recentPurchasesSettings;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return !string.IsNullOrWhiteSpace(_recentPurchasesSettings.WidgetZone)
                       ? new List<string>() { _recentPurchasesSettings.WidgetZone }
                       : new List<string>() { "left_side_column_after" };
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsRecentPurchases";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.RecentPurchases.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsRecentPurchases";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Widgets.RecentPurchases.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            var settings = new RecentPurchasesSettings
            {
                WidgetZone = "left_side_column_after",
                WindowType = WindowType.Days,
                WindowSize = 1,
                ShowProductImage = true,
                ShowQuantity = true
            };

            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource(Constants.LocaleResources.RecentPurchases_CustomerRecentlyPurchased_Format,
                "<div class=\"nop-plugin-recent-purchases\" style=\"display:none;visibility:hidden\"> <input id=\"nop-plugin-recent-purchases-btn-close\" type=\"button\" value=\"Close\" class=\"button-1 cart-button\" style=\"right:0;top:0\"><b>{0}</b> just purchased<br/><img src=\"{1}\" title=\"{2}\" alt=\"{3}\" /><b>{4}</b> unit(s) of<br/><b>{5}</b></div>");

            this.AddOrUpdatePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Instructions, "Widget instructions comes here.");
            this.AddOrUpdatePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Note, "Configuration note comes here.");
            this.AddOrUpdatePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_WindowSize, "Window size");
            this.AddOrUpdatePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_WindowSize_Hint, "The maximum length of time units before the last purchase.");
            this.AddOrUpdatePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_WindowSize_Required, "Window size is required.");
            this.AddOrUpdatePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_WindowSize_MustBeGreaterThanZero, "Window size must be greater than zero.");
            this.AddOrUpdatePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_WindowType, "Window type");
            this.AddOrUpdatePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_WindowType_Hint, "The window type.");
            this.AddOrUpdatePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_WindowType_Required, "Window type is required.");
            this.AddOrUpdatePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_ShowProductImage, "Show product image");
            this.AddOrUpdatePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_ShowProductImage_Hint, "Check to show product image.");
            this.AddOrUpdatePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_ShowQuantity, "Show quantity purchased");
            this.AddOrUpdatePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_ShowQuantity_Hint, "Check to show quantity purchased.");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<RecentPurchasesSettings>();

            //locales
            this.DeletePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Instructions);
            this.DeletePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Note);
            this.DeletePluginLocaleResource(Constants.LocaleResources.RecentPurchases_CustomerRecentlyPurchased_Format);
            this.DeletePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_WindowSize);
            this.DeletePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_WindowSize_Hint);
            this.DeletePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_WindowSize_Required);
            this.DeletePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_WindowSize_MustBeGreaterThanZero);
            this.DeletePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_WindowType);
            this.DeletePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_WindowType_Hint);
            this.DeletePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_WindowType_Required);
            this.DeletePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_ShowProductImage);
            this.DeletePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_ShowProductImage_Hint);
            this.DeletePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_ShowQuantity);
            this.DeletePluginLocaleResource(Constants.LocaleResources.RecentPurchases_Fields_ShowQuantity_Hint);

            base.Uninstall();
        }
    }
}
