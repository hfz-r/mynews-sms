namespace StockManagementSystem.Core.Domain.Media
{
    public class Picture : BaseEntity
    {
        public string MimeType { get; set; }

        public string AltAttribute { get; set; }

        public string TitleAttribute { get; set; }

        public bool IsNew { get; set; }

        public virtual PictureBinary PictureBinary { get; set; }
    }
}