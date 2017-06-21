using CollectorsApi;
using CollectorsApi.Controllers;
using CollectorsApi.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

namespace CollectorsApi
{
    public static class Main
    {
        private static object lockObject = new object();

        public static Bitmap ApplyWrap(List<AForge.IntPoint> quad, Bitmap originalIage, double scale)
        {
            //pattern witdh and height
            double ratio = Math.Max((double)originalIage.Width / (double)578, (double)originalIage.Height / (double)839);
            ratio *= scale;
            AForge.Imaging.Filters.QuadrilateralTransformation wrap = new AForge.Imaging.Filters.QuadrilateralTransformation(quad);
            wrap.UseInterpolation = false; //perspective wrap only, no binary.
            Size sr = new Size() {
                Width = 578,
                Height = 839
            };

            sr = new Size(
                (int)Math.Round((double)sr.Width * ratio),
                (int)Math.Round((double)sr.Height * ratio)
                );
            wrap.AutomaticSizeCalculaton = false;
            wrap.NewWidth = sr.Width;
            wrap.NewHeight = sr.Height;
            return wrap.Apply(originalIage); // wrap 
        }

        public static int GetTestScore(Image BitmapStream)
        {                   
            List<AForge.IntPoint> wrapPoints = new List<AForge.IntPoint>();
            Bitmap extd = new Bitmap(1, 1);
            Bitmap extForEB = new Bitmap(1, 1);

            //get it from db tho
            var sheetBounds = new Size(578, 839);


            Image flattenedImage = PreprocessingHelper.CamImage.PrepareForObjectDetection(0, false);

            lock (lockObject)
            {
                wrapPoints = (List<AForge.IntPoint>)(new Bitmap(flattenedImage)).ExtractPaperFromPrepapred(PreprocessingHelper.CamImage, true);
            }

           
            extd = ApplyWrap(wrapPoints, new Bitmap(PreprocessingHelper.CamImage), PreprocessingHelper.ImageProcessScale);

            //extracted sheet
            var recogImg = (Image)extd;

            if (/*number of answer blocks */ 3 > 0)
                extForEB = ApplyWrap(wrapPoints, new Bitmap(PreprocessingHelper.CamImage), 1);

            var test = new PhotosController();

            test.Post(new Models.Photo { Name="test2", Image = extForEB.ToByteArray(ImageFormat.Jpeg)});
            test.Post(new Models.Photo { Name = "testImage", Image = (System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath("~/omrtemp/testImage.jpg"))).ToByteArray(ImageFormat.Jpeg) });

            return 10;
        }
    }
}
