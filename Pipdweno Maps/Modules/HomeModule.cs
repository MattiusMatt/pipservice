using Nancy;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace Pipdweno_Maps.Modules
{
    public class HomeModule : NancyModule
    {
        private const string GOOGLE_MAPS_URL = @"https://maps.googleapis.com/maps/api/staticmap";
        private const string GOOGLE_MAPS_KEY = @"AIzaSyAhMaZDXg2er-2xQacrvLUB496yl2O9OQ8";
        private const int LOCAL_MAP_ZOOM = 15;
        private const int WORLD_MAP_ZOOM = 9;

        public HomeModule()
        {
            Get["/local"] = parameters => {
                Response response;

                HomeModule.CleanupOldFiles();

                string imagePath = this.AquirePipMap(Request.Query.lat, Request.Query.lon, LOCAL_MAP_ZOOM);
                response = Response.AsImage(imagePath);

                return response;
            };

            Get["/world"] = parameters => {
                Response response;

                HomeModule.CleanupOldFiles();

                string imagePath = this.AquirePipMap(Request.Query.lat, Request.Query.lon, WORLD_MAP_ZOOM);
                response = Response.AsImage(imagePath);

                return response;
            };
        }

        private static void CleanupOldFiles()
        {
            string[] imageFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), @"*.bmp");

            foreach (string imageFile in imageFiles)
            {
                try
                {
                    File.Delete(imageFile);
                }
                catch (IOException)
                {

                }
            }
        }

        private string AquirePipMap(string latitude, string longditude, int zoom)
        {
            string path = Directory.GetCurrentDirectory();
            string fileName = Guid.NewGuid().ToString();
            string originalImagePath = string.Format(@"{0}\{1}.png", path, fileName);
            string newImagePath = string.Format(@"{0}\{1}.bmp", path, fileName);
            string googleMapsUrl = string.Format(
                "{0}?center={1},{2}&zoom={3}&size=300x215&maptype=hybrid&key={4}",
                GOOGLE_MAPS_URL,
                latitude,
                longditude,
                zoom,
                GOOGLE_MAPS_KEY);

            // Download map
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(
                    googleMapsUrl,
                    originalImagePath);
            }

            // Convert map to pipgreen bitmap
            using (Bitmap originalImage = new Bitmap(originalImagePath))
            {
                int width = originalImage.Width;
                int height = originalImage.Height;

                using (Bitmap pipImage = new Bitmap(width, height - 20, PixelFormat.Format24bppRgb))
                {
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

            File.Delete(originalImagePath);

            return newImagePath;
        }
    }
}
