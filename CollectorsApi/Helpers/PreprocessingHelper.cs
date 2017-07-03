using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace CollectorsApi.Helpers
{
    /// <summary>
    /// source code : https://www.codeproject.com/Articles/884518/Csharp-Optical-Marks-Recognition-OMR-Engine-a-Mar
    /// </summary>
    public static class PreprocessingHelper
    {
        public static System.Drawing.Image camImg { get; set; }

        public static System.Drawing.Image CamImage
        {
            get
            {
                //set image from db
                System.Drawing.Image value = camImg;
                if (value.Width * value.Height > 10000000)
                {
                    int maxRes = (int)Math.Round(10D * 1000000D);
                    //lock (lockObject)
                    {
                        camImg = ResizeImage(value, (maxRes) / value.Height, (maxRes) / value.Width);
                    }
                }
                else
                    camImg = value;
                return camImg;

            }
            set
            {
                camImg = value;
                if (value.Width * value.Height > 10000000)
                {
                    int maxRes = (int)Math.Round(10D * 1000000D);
                    var lockObject = new object();

                    lock (lockObject)
                    {
                        camImg = ResizeImage(value, (maxRes) / value.Height, (maxRes) / value.Width);
                    }
                }
            }
        }

        public static double ImageProcessScale
        {
            get
            {
                int pixel = camImg.Width * camImg.Height;
                int minPixForP = 1500000;
                int maxPixForP = 10000000;
                if (pixel <= minPixForP)
                    return (double)minPixForP / (double)pixel;
                if (pixel >= maxPixForP)
                {
                    return (double)pixel / (double)maxPixForP;
                }
                int reqPix = minPixForP + (int)Math.Round((double)80 / 100D * (double)(pixel - minPixForP));
                return (double)reqPix / (double)pixel;
            }
        }

        public static byte[] ToByteArray(this System.Drawing.Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }

        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            //a holder for the result
            Bitmap result = new Bitmap(width, height);
            // set the resolutions the same to avoid cropping due to resolution differences
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return result;
        }

        public static int GetWhiteLevel(Bitmap bmp)
        {
            int[] histo = SmoothHistogram(Histogram(bmp));
            int[] maximas = MaximasOfHistoGram(histo);
            try
            {
                return Math.Min((int)(maximas[maximas.Length - 1] * 0.8 + maximas[maximas.Length - 2] * 0.2), 255);
            }
            catch { }
            if (maximas.Length == 0)
                return 127;
            else if (maximas.Length == 1)
                return maximas[maximas.Length - 1];
            throw new Exception();
        }

        public static int[] SmoothHistogram(int[] histogram)
        {
            int[] ans = new int[256];
            for (int i = 0; i < 256; i++)
            {
                ans[i] = histogram[i];
            }
            for (int r = 0; r < 50; r++)
            {
                for (int i = 1; i < 255; i++)
                {
                    ans[i] = (histogram[i + 1] + histogram[i - 1] + histogram[i]) / 3;
                }
                ans[0] = Math.Min(Math.Max(histogram[1] - (histogram[2] - histogram[1]), 0), 255);
                ans[255] = Math.Min(Math.Max(histogram[254] - (histogram[253] - histogram[254]), 0), 255);
                for (int i = 0; i < 256; i++)
                {
                    histogram[i] = ans[i];
                }
            }
            return ans;
        }

        static int[] MaximasOfHistoGram(int[] histo)
        {
            List<int> maximas = new List<int>();
            int[] dOfF = new int[256], d2OfF = new int[256];

            for (int i = 0; i < 255; i++)
            {
                if (histo[i + 1] < histo[i])
                    dOfF[i] = -1;
                else if (histo[i + 1] > histo[i])
                    dOfF[i] = 1;
                else
                    dOfF[i] = 0;
            }
            dOfF[255] = dOfF[254];

            for (int i = 0; i < 255; i++)
            {
                if (dOfF[i + 1] < dOfF[i])
                    d2OfF[i] = -1;
                else if (dOfF[i + 1] > dOfF[i])
                    d2OfF[i] = 1;
                else
                    d2OfF[i] = 0;
            }
            d2OfF[255] = d2OfF[254];

            bool lookingForDown = true;
            int startInd = 0;
            for (int i = 0; i < 256; i++)
            {
                if (dOfF[i] == -1) // downhill start
                {
                    if (lookingForDown)
                    {
                        maximas.Add(((i + 1) + startInd) / 2);
                        lookingForDown = false;
                    }
                }
                if (dOfF[i] == 1)
                {
                    lookingForDown = true;
                    startInd = i;
                }
                if (i == 255)
                {
                    if (lookingForDown)
                    {
                        maximas.Add((i + startInd) / 2);
                    }
                    else
                    {
                        if (dOfF[i] == 1)
                            maximas.Add((i + startInd) / 2);
                    }
                }
            }
            return maximas.ToArray();

        }

        public static int[] Histogram(System.Drawing.Image img)
        {

            int[] ans = new int[256];

            AForge.Imaging.UnmanagedImage umimg = AForge.Imaging.UnmanagedImage.FromManagedImage(new Bitmap(img));
            for (int y = 0; y < umimg.Height; y++)
                for (int x = 0; x < umimg.Width; x++)
                {
                    Color c = umimg.GetPixel(x, y);
                    ans[(c.R + c.G + c.B) / 3]++;
                }
            int max = ans.Max();
            for (int i = 0; i < 256; i++)
            {
                ans[i] = (int)Math.Round((double)ans[i] / (double)max * 255D, 0);
            }
            return ans;
        }

        public static bool IsSameAs(this UnmanagedImage imageToBeCompared, UnmanagedImage referenceImage)
        {
            Grayscale gsf = new Grayscale(0.2989, 0.5870, 0.1140);
            imageToBeCompared = gsf.Apply(imageToBeCompared);
            referenceImage = gsf.Apply(referenceImage);

            Bitmap bmp1 = imageToBeCompared.ToManagedImage(), bmp2 = referenceImage.ToManagedImage();

            int count = 0, tcount = referenceImage.Width * referenceImage.Height;

            for (int y = 0; y < imageToBeCompared.Height; y++)
                for (int x = 0; x < imageToBeCompared.Width; x++)
                {
                    Color c1 = imageToBeCompared.GetPixel(x, y), c2 = referenceImage.GetPixel(x, y);

                    int a1 = (c1.R + c1.G + c1.B) / 3; 
                    int a2 = (c2.R + c2.G + c2.B) / 3; 

                    if ((a1 < 127) == (a2 < 127))
                    {
                        if (a2 > 127)
                            count++;  
                        else 
                            tcount--;
                    }
                    else
                       
                        count--;
                }
           
            count += tcount;
            count /= 2;   
           
            bool returnValue = (count * 100) / tcount >= 50;


            return returnValue;
        }

        public static int AvgColor(System.Drawing.Image img, int forceZeroBelow)
        {
            long total = 0;
            AForge.Imaging.UnmanagedImage umimg = AForge.Imaging.UnmanagedImage.FromManagedImage(new Bitmap(img));
            for (int y = 0; y < umimg.Height; y++)
                for (int x = 0; x < umimg.Width; x++)
                {
                    Color c = umimg.GetPixel(x, y);
                    total += forceZeroBelow > ((c.R + c.G + c.B) / 3) ? 0 : 255;
                }
            return (int)((long)total / ((long)umimg.Width * (long)umimg.Height));
        }
    }
}