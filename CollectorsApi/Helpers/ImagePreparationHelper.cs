using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using CollectorsApi.Helpers;
using CollectorsApi.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

namespace CollectorsApi.Helpers
{
    /// <summary>
    /// source code taken, modified and updated from : https://www.codeproject.com/Articles/884518/Csharp-Optical-Marks-Recognition-OMR-Engine-a-Mar
    /// </summary>
    public static class ImagePreparationHelper
    {
        public static Bitmap PrepareForObjectDetection(this System.Drawing.Image bmpOriginalImage, int whiteLevel, bool overrideWhite)
        {
            Bitmap bmp = (Bitmap)bmpOriginalImage.Clone();

            ColorFiltering colorFilter = new ColorFiltering();

            int white = whiteLevel;
            if (!overrideWhite)
                white = PreprocessingHelper.GetWhiteLevel(bmp);

            colorFilter.Red = new IntRange(0, white / 3);
            colorFilter.Green = new IntRange(0, white / 3);
            colorFilter.Blue = new IntRange(0, white / 3);
            colorFilter.FillOutsideRange = true;

            colorFilter.FillColor = new RGB(Color.White);
            colorFilter.ApplyInPlace(bmp);

            ExtractChannel extract_channel = new ExtractChannel(0);
            bmp = extract_channel.Apply(bmp);

            Invert invert = new Invert();
            invert.ApplyInPlace(bmp);

            Threshold threshholdFilter = new Threshold(10);
            threshholdFilter.ApplyInPlace(bmp);

            bmp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format24bppRgb);

            return bmp;
        }

        public static object ExtractPaperFromPrepapred(this Bitmap PreparedImage, System.Drawing.Image originalImage, bool onlyExtractionPoints, Pattern pattern)
        {
            BitmapData bitmapData = PreparedImage.LockBits(
                new Rectangle(0, 0, PreparedImage.Width, PreparedImage.Height),
                ImageLockMode.ReadWrite, PreparedImage.PixelFormat);

            BlobCounter blobCounter = new BlobCounter();

            int minBlobWidHei = (int)(0.001 * PreparedImage.Width);

            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = minBlobWidHei;  
            blobCounter.MinWidth = minBlobWidHei; 

            blobCounter.BackgroundThreshold = Color.FromArgb(10, 10, 10);
            blobCounter.ProcessImage(bitmapData);
            PreparedImage.UnlockBits(bitmapData);

            Blob[] blobs = blobCounter.GetObjects((new Bitmap(PreparedImage)), false);

            double minbr = pattern.MinSizeRatio; //trebuie setate in baza de date!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! read more about it

            double maxbr = pattern.MaxSizeRatio;

            List<IntPoint> quad = new List<IntPoint>();

            List<Blob> suspectBlobs = new List<Blob>();

            for (int i = 0; i < blobs.Length; i++)
            {
                int area = blobs[i].Image.Width * blobs[i].Image.Height;
              
                if (
                    ((double)area) / ((double)PreparedImage.Width * PreparedImage.Height) > minbr &&
                        ((double)area) / ((double)PreparedImage.Width * PreparedImage.Height) < maxbr
                        &&
                        ((double)blobs[i].Rectangle.Width / (double)blobs[i].Rectangle.Height < 1.3 &&
                            (double)(double)blobs[i].Rectangle.Width / (double)blobs[i].Rectangle.Height > .7)
                    )
                {
                    suspectBlobs.Add(blobs[i]);                  
                }
            }
            blobs = suspectBlobs.ToArray();
            
            System.Drawing.Image compImg = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath("~/omrtemp/lc.jpg"));

            UnmanagedImage compUMImg = UnmanagedImage.FromManagedImage((Bitmap)compImg);

            foreach (Blob blob in blobs)
            {
                if (blob.Rectangle.X < (PreparedImage.Width) / 2) 
                {
                   
                    compUMImg = UnmanagedImage.FromManagedImage(PreprocessingHelper.ResizeImage(compImg, blob.Rectangle.Width, blob.Rectangle.Height));
                    if (blob.Image.IsSameAs(compUMImg))
                    {
                        quad.Add(new IntPoint((int)blob.CenterOfGravity.X, (int)blob.CenterOfGravity.Y));
                    }                   
                }
            }

            if (quad.Count > 1)
            {           
                if (quad[0].Y > quad[1].Y)
                {
                    IntPoint tp = quad[0];
                    quad[0] = quad[1];
                    quad[1] = tp;
                }
            }

            compImg = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath("~/omrtemp/rc.jpg"));
            compUMImg = UnmanagedImage.FromManagedImage((Bitmap)compImg);            
           
            foreach (Blob blob in blobs)
            {
                if (blob.Rectangle.X > (PreparedImage.Width * 3) / 4)
                {

                    compUMImg = UnmanagedImage.FromManagedImage(PreprocessingHelper.ResizeImage(compImg, blob.Rectangle.Width, blob.Rectangle.Height));
                    if (blob.Image.IsSameAs(compUMImg))
                    {                      
                        quad.Add(new IntPoint((int)blob.CenterOfGravity.X, (int)blob.CenterOfGravity.Y));
                    }                   
                }
            }

            if (quad.Count > 3)
            {
               if (quad[2].Y < quad[3].Y)
                {
                    IntPoint tp = quad[2];
                    quad[2] = quad[3];
                    quad[3] = tp;
                }
            }
            
            if (quad.Count == 4)
            {
                if (((double)quad[1].Y - (double)quad[0].Y) / ((double)quad[2].Y - (double)quad[3].Y) < .75 ||
                    ((double)quad[1].Y - (double)quad[0].Y) / ((double)quad[2].Y - (double)quad[3].Y) > 1.25)
                    quad.Clear(); 
                else if (quad[0].X > PreparedImage.Width / 2 || quad[1].X > PreparedImage.Width / 2 || quad[2].X < PreparedImage.Width / 2 || quad[3].X < PreparedImage.Width / 2)
                    quad.Clear(); 
            }
            
            IntPoint tp2 = quad[3];
            quad[3] = quad[1];
            quad[1] = tp2;

                return quad;                                   
        }
    }

}
