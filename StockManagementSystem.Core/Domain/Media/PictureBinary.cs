namespace StockManagementSystem.Core.Domain.Media
{
    public class PictureBinary : BaseEntity
    {
        public byte[] BinaryData { get; set; }

        public int PictureId { get; set; }

        public virtual Picture Picture { get; set; }
    }
}