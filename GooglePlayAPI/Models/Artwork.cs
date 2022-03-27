using GooglePlayApi.Proto;
using System.Text.Json.Serialization;

namespace GooglePlayApi.Models
{
    public class Artwork
    {
        [JsonConstructor]
        public Artwork() { }

        public Artwork(Image image)
        {
            Type = image.ImageType;
            Url = image.ImageUrl;
            UrlAlt = image.ImageUrlAlt;
            AspectRatio = image.Dimension != null ? image.Dimension.AspectRatio : 0;
            Width = image.Dimension != null ? image.Dimension.Width : 0;
            Height = image.Dimension != null ? image.Dimension.Height : 0;
        }

        public int Type { get; set; }
        public string Url { get; set; }
        public string UrlAlt { get; set; }
        public int AspectRatio { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

    }
}