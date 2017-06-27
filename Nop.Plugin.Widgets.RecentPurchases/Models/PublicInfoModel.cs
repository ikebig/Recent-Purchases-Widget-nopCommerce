namespace Nop.Plugin.Widgets.RecentPurchases.Models
{
    public class PublicInfoModel
    {
        public PublicInfoModel()
        {
            Picture = new PublicInfoPictureModel();
        }

        public string CustomerName { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        public int Quantity { get; set; }

        public bool ShowProductImage { get; set; }

        public bool ShowQuantity { get; set; }

        public PublicInfoPictureModel Picture { get; set; }

        #region Nested Classes

        public class PublicInfoPictureModel
        {
            public string ImageUrl { get; set; }

            public string Title { get; set; }

            public string AlternateText { get; set; }
        }

        #endregion
    }
}
