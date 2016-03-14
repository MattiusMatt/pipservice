using Nancy;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace Pipdweno_Maps.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = parameters => {
                AquirePipMap(Request.Query.lat, Request.Query.lon, 15);
                return Response.AsImage(@"pipmap.bmp"); ;
            };
        }

        private void AquirePipMap(string latitude, string longditude, int zoom)
        {
            string path = Directory.GetCurrentDirectory();
            string originalImagePath = string.Format(@"{0}\pipmap.png", path);
            string newImagePath = string.Format(@"{0}\pipmap.bmp", path);
            string googleMapsUrl = string.Format(
                "https://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom={2}&size=300x215&maptype=satellite&key=AIzaSyAhMaZDXg2er-2xQacrvLUB496yl2O9OQ8",
                latitude,
                longditude,
                zoom);

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(
                    googleMapsUrl,
                    originalImagePath);
            }

            Bitmap originalImage = new Bitmap(originalImagePath);

            int width = originalImage.Width;
            int height = originalImage.Height;

            Bitmap pipImage = new Bitmap(width, height - 20, PixelFormat.Format24bppRgb);

            for (int y = 0; y < height - 20; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);

                    int a = pixel.A;
                    int g = pixel.G;

                    pipImage.SetPixel(x, y, Color.FromArgb(a, 0, g, 0));
                }
            }

            pipImage.Save(newImagePath, ImageFormat.Bmp);
        }
    }
}
