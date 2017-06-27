using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.RecentPurchases.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Services;
using System.Web;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Widgets.RecentPurchases.Controllers
{
    public class WidgetsRecentPurchasesController : BasePluginController
    {
        #region Const

        private const string RECENTLY_PURCHASED_ITEM_COOKIE_NAME = "Nop.recentpurchases.product";

        #endregion

        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        private readonly ICategoryService _categoryService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPictureService _pictureService;

        #endregion

        #region Ctor

        public WidgetsRecentPurchasesController(HttpContextBase httpContext,
            IWorkContext workContext,
            IStoreContext storeContext,
            IStoreService storeService,
            ISettingService settingService,
            IOrderService orderService,
            ILogger logger,
            ICategoryService categoryService,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IGenericAttributeService genericAttributeService,
            IPictureService pictureService)
        {
            this._httpContext = httpContext;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._orderService = orderService;
            this._logger = logger;
            this._categoryService = categoryService;
            this._productAttributeParser = productAttributeParser;
            this._productService = productService;
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._genericAttributeService = genericAttributeService;
            this._pictureService = pictureService;
        }

        #endregion

        #region Utilities

        protected virtual HttpCookie GetRecentlyPurchasedItemCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[RECENTLY_PURCHASED_ITEM_COOKIE_NAME];
        }

        protected virtual void SetRecentlyPurchasedItemCookie(int orderItemId)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(RECENTLY_PURCHASED_ITEM_COOKIE_NAME);
                cookie.HttpOnly = true;
                cookie.Value = orderItemId.ToString();
                if (orderItemId <= 0)
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    int cookieExpires = 24 * 365; //TODO make configurable
                    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }

                _httpContext.Response.Cookies.Remove(RECENTLY_PURCHASED_ITEM_COOKIE_NAME);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        private Order GetLastOrder()
        {
            //load settings for a chosen store scope
            var recentPurchasesSettings = _settingService.LoadSetting<RecentPurchasesSettings>(_storeContext.CurrentStore.Id);
            var createdFromUtc = DateTime.UtcNow;

            switch (recentPurchasesSettings.WindowType)
            {
                case WindowType.Seconds:
                    createdFromUtc = createdFromUtc.AddSeconds(-1 * recentPurchasesSettings.WindowSize);
                    break;
                case WindowType.Minutes:
                    createdFromUtc = createdFromUtc.AddMinutes(-1 * recentPurchasesSettings.WindowSize);
                    break;
                case WindowType.Hours:
                    createdFromUtc = createdFromUtc.AddHours(-1 * recentPurchasesSettings.WindowSize);
                    break;
                case WindowType.Days:
                    createdFromUtc = createdFromUtc.AddDays(-1 * recentPurchasesSettings.WindowSize);
                    break;
                case WindowType.Weeks:
                    createdFromUtc = createdFromUtc.AddDays(-7 * recentPurchasesSettings.WindowSize);
                    break;
                case WindowType.Months:
                    createdFromUtc = createdFromUtc.AddMonths(-1 * recentPurchasesSettings.WindowSize);
                    break;
                case WindowType.Years:
                    createdFromUtc = createdFromUtc.AddYears(-1 * recentPurchasesSettings.WindowSize);
                    break;
                default:
                    createdFromUtc = createdFromUtc.AddHours(-1);
                    break;
            }

            var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                createdFromUtc: createdFromUtc,
                pageSize: 1).FirstOrDefault();

            if (order == null || order.OrderItems?.Count == 0)
                return null;

            if (order.CustomerId == _workContext.CurrentCustomer.Id)
                return null;

            return order;
        }

        
        #endregion

        #region Methods

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var recentPurchasesSettings = _settingService.LoadSetting<RecentPurchasesSettings>(storeScope);
            var model = new ConfigurationModel();
            model.WindowSize = recentPurchasesSettings.WindowSize;
            model.ShowProductImage = recentPurchasesSettings.ShowProductImage;
            model.ShowQuantity = recentPurchasesSettings.ShowQuantity;

            model.WindowTypeId = recentPurchasesSettings.WindowTypeId;
            model.AvailableWindowTypes = WindowType.Days.ToSelectList().ToList();

            model.ZoneId = recentPurchasesSettings.WidgetZone;
            model.AvailableZones.Add(new SelectListItem() { Text = "Before left side column start html tag", Value = "left_side_column_before" });
            model.AvailableZones.Add(new SelectListItem() { Text = "After left side column end html tag", Value = "left_side_column_after" });
            model.AvailableZones.Add(new SelectListItem() { Text = "Before left side column category navigation start html tag", Value = "left_side_column_before_category_navigation" });
            model.AvailableZones.Add(new SelectListItem() { Text = "After left side column category navigation end html tag", Value = "left_side_column_after_category_navigation" });
            model.AvailableZones.Add(new SelectListItem() { Text = "Before main column start html tag", Value = "main_column_before" });
            model.AvailableZones.Add(new SelectListItem() { Text = "After main column end html tag", Value = "main_column_after" });

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.WindowSize_OverrideForStore = _settingService.SettingExists(recentPurchasesSettings, x => x.WindowSize, storeScope);
                model.ShowProductImage_OverrideForStore = _settingService.SettingExists(recentPurchasesSettings, x => x.ShowProductImage, storeScope);
                model.ShowQuantity_OverrideForStore = _settingService.SettingExists(recentPurchasesSettings, x => x.ShowQuantity, storeScope);
                model.WindowTypeId_OverrideForStore = _settingService.SettingExists(recentPurchasesSettings, x => x.WindowTypeId, storeScope);
                model.ZoneId_OverrideForStore = _settingService.SettingExists(recentPurchasesSettings, x => x.WidgetZone, storeScope);
            }

            return View("~/Plugins/Widgets.RecentPurchases/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var recentPurchasesSettings = _settingService.LoadSetting<RecentPurchasesSettings>(storeScope);
            recentPurchasesSettings.WindowSize = model.WindowSize;
            recentPurchasesSettings.ShowProductImage = model.ShowProductImage;
            recentPurchasesSettings.ShowQuantity = model.ShowQuantity;
            recentPurchasesSettings.WindowTypeId = model.WindowTypeId;
            recentPurchasesSettings.WidgetZone = model.ZoneId;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(recentPurchasesSettings, x => x.WindowSize, model.WindowSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(recentPurchasesSettings, x => x.ShowProductImage, model.ShowProductImage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(recentPurchasesSettings, x => x.ShowQuantity, model.ShowQuantity_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(recentPurchasesSettings, x => x.WindowTypeId, model.WindowTypeId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(recentPurchasesSettings, x => x.WidgetZone, model.ZoneId_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            PublicInfoModel model = null;
            var routeData = ((System.Web.UI.Page)this.HttpContext.CurrentHandler).RouteData;

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var recentPurchasesSettings = _settingService.LoadSetting<RecentPurchasesSettings>(storeScope);

            try
            {
                var controller = routeData.Values["controller"];
                var action = routeData.Values["action"];

                if (controller == null || action == null)
                    return Content("");

                var order = GetLastOrder();
                if (order.OrderItems == null)
                    return Content("");

                var custormer = _customerService.GetCustomerById(order.CustomerId);
                if (custormer == null)
                    return Content("");

                int lastDisplayedItemId = -1;
                var cookie = GetRecentlyPurchasedItemCookie();
                if (cookie != null)
                    int.TryParse(cookie.Value, out lastDisplayedItemId);

                var orderItem = order.OrderItems
                    .Where(x => x.Id > lastDisplayedItemId)
                    .OrderBy(x => x.Id)
                    .FirstOrDefault();

                if (orderItem == null)
                    return Content("");

                SetRecentlyPurchasedItemCookie(orderItem.Id);// set item as recently viewed.
                
                var product = _productService.GetProductById(orderItem.ProductId);
                if (product == null)
                    return Content("");

                var orderItemPicture = orderItem.Product.GetProductPicture(orderItem.AttributesXml, _pictureService, _productAttributeParser);
                
                model = new PublicInfoModel();
                model.CustomerName = custormer.FormatUserName();
                model.ProductName = product.Name;
                model.ProductSeName = product.GetSeName(0, true, false);
                model.Quantity = orderItem.Quantity;
                model.ShowQuantity = recentPurchasesSettings.ShowQuantity;

                if (orderItemPicture != null && recentPurchasesSettings.ShowProductImage)
                {
                    var pictureThumbnailUrl = _pictureService.GetPictureUrl(orderItemPicture, 75, true);
                    model.ShowProductImage = true;
                    model.Picture.ImageUrl = pictureThumbnailUrl;
                    model.Picture.AlternateText = orderItemPicture.AltAttribute;
                    model.Picture.Title = orderItemPicture.TitleAttribute;
                }               
            }
            catch (Exception ex)
            {
                _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, "Error creating content for other customers' recent purchases", ex.ToString());
            }

            if (model == null)
                return Content("");

            return View("~/Plugins/Widgets.RecentPurchases/Views/PublicInfo.cshtml", model);
        }
        
        #endregion
    }
}