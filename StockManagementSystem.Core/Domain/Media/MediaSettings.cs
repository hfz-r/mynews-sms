using StockManagementSystem.Core.Configuration;

namespace StockManagementSystem.Core.Domain.Media
{
    public class MediaSettings : ISettings
    {
        /// <summary>
        /// Picture size of customer avatars (if enabled)
        /// </summary>
        public int AvatarPictureSize { get; set; }

        /// <summary>
        /// Maximum allowed picture size. If a larger picture is uploaded, then it'll be resized
        /// </summary>
        public int MaximumImageSize { get; set; }

        /// <summary>
        /// Gets or sets a default quality used for image generation
        /// </summary>
        public int DefaultImageQuality { get; set; }
    }
}