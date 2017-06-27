
using Nop.Core.Configuration;
using Nop.Plugin.Widgets.RecentPurchases.Models;

namespace Nop.Plugin.Widgets.RecentPurchases
{
    public class RecentPurchasesSettings : ISettings
    {
        /// <summary>
        /// The maximum length of time units before the last purchase.
        /// </summary>
        public int WindowSize { get; set; }

        /// <summary>
        /// The window type identifier.
        /// </summary>
        public int WindowTypeId { get; set; }

        /// <summary>
        /// The window type.
        /// </summary>
        public WindowType WindowType
        {
            get { return (WindowType)WindowTypeId; }
            set { WindowTypeId = (int)value; }
        }

        /// <summary>
        /// The widget zone.
        /// </summary>
        public string WidgetZone { get; set; }

        /// <summary>
        /// A value indicating whether to show product image.
        /// </summary>
        public bool ShowProductImage { get; set; }

        /// <summary>
        /// A value indicating whether to show quantity purchased.
        /// </summary>
        public bool ShowQuantity { get; set; }
    }
}