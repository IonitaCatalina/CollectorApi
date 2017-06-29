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
            //The image here will be high contrast, inverted. Flatten Image does this for us. We are hunting for markings and ink on a white paper. White, in logic, means 1. means presence of an object. Which, in our case is background paper, which is converse of an object
            //which also is supposed to be logical 1. means white, means brighter. So, we have inverted the image.

            //Step 1
            // lock the image. Otherwise, Bitmap itself takes much time to be processed
            BitmapData bitmapData = PreparedImage.LockBits(
                new Rectangle(0, 0, PreparedImage.Width, PreparedImage.Height),
                ImageLockMode.ReadWrite, PreparedImage.PixelFormat);

            // step 2 - locating objects
            //The only things we Need to find in the image are the four corner marks. Once the marks have been identified, and their centres calculated, we will Wrap the polygon to a rectangle.
            //this rectangle will be transformed to the aspect ratio of original sheet as specified during the creation of OMR sheet used in this image.
            //
            //PLEASE NOTE:!!! No detection will be performed for any other object or marks on the sheet. We won't need to! Once the cam image has been transformed into similar aspect Ratio as that of
            //the original OMR sheet, the objects and OMR marks block will also become equilent to the original sheet. We would only need to BLINDLY, crop the cam image at the pre-specified locations and we will have our block cut out
            //
            //So, lets get started
            //
            //Initialize a blob counter object. this object will be used to find objects on the bitmap
            BlobCounter blobCounter = new BlobCounter();

            //As we are hunting for small corner marks, we already can estimate a lot of things about them even without knowing what kind of a sheet was used.
            //Like,
            //specify the first safe estimate of minimum size possible of the corner block we are searching for. OMR sheet (hopefully) will never be bigger than an A4 Sized paper.
            //this is just the first estimate, we will make it more precise in the coming steps.
            int minBlobWidHei = (int)(0.001 * PreparedImage.Width);
            //we need the blobs filtered with these parametrs
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = minBlobWidHei;  // both these variables have to be given when calling the
            blobCounter.MinWidth = minBlobWidHei;   // method

            //lets call very low lights a line threshhold. this makes it possible to work will low resolution images which have anti-alising turned on.
            //The image here will already be high contrast, remember.
            blobCounter.BackgroundThreshold = Color.FromArgb(10, 10, 10);
            //let the counter start the job synchronously
            blobCounter.ProcessImage(bitmapData);
            //We are done with the image. Lets unlock it.
            PreparedImage.UnlockBits(bitmapData);

            //lets get the coordinates and sizes of the detected blobs
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            //this will transfer the images to the blob objects. Its a requirement of the library
            Blob[] blobs = blobCounter.GetObjects((new Bitmap(PreparedImage)), false);

            //ass we decided earlier, we need to be more precceise about our estimates about the sizes of target objects
            // this helps filtering out too small and too larger blobs depending upon the size of image.
            //We are blind about what ratio does the target have with the original sheet. This has to be asked from the creator of this sheet. Lets check the user manual i.e. The Access DB created with the sheet
            //Minimum blob to sheet ratio. contains tolerance of upto 40%
            double minbr = pattern.MinSizeRatio; //trebuie setate in baza de date!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! read more about it
            //maximum blob to sheet ratio. contains tolerance of upto 40%
            double maxbr = pattern.MaxSizeRatio;

            // Store sheet corner locations in this list (in any order) . . . (if anyone is detected )
            List<IntPoint> quad = new List<IntPoint>();

            //separate those which are the most suspected.
            List<Blob> suspectBlobs = new List<Blob>();

            //now, check each blob and apply a more precise size and location filter
            for (int i = 0; i < blobs.Length; i++)
            {
                //area of blob
                int a = blobs[i].Image.Width * blobs[i].Image.Height;
                //in VS 2012 and below, image debugger worked. so this line was usefull
                //System.Drawing.Image imgt = blobs[i].Image.ToManagedImage();
                if (
                    //only if it doesn't have insanely wrong size
                    ((double)a) / ((double)PreparedImage.Width * PreparedImage.Height) > minbr &&
                        ((double)a) / ((double)PreparedImage.Width * PreparedImage.Height) < maxbr
                        &&
                        //And it doesn't have a total inappropriate ASpect Ratio
                        ((double)blobs[i].Rectangle.Width / (double)blobs[i].Rectangle.Height < 1.3 &&
                            (double)(double)blobs[i].Rectangle.Width / (double)blobs[i].Rectangle.Height > .7)
                    )// filters out blobs having insanely wrong aspect size and )
                {
                    suspectBlobs.Add(blobs[i]);
                    //in VS 2012 and below, image debugger worked. so this line was usefull
                    //System.Drawing.Image imgt2 = blobs[i].Image.ToManagedImage();
                }
            }

            //forget about the old blobs. do the remaing filteration only on the remaining
            blobs = suspectBlobs.ToArray();

            //Detection of paper lies within the presence of crossmark printed on the corneres of printed sheet.
            //edge marks are detected by comparison of detected blobs to a original sample
            // First, detect left edge.
            //Create duplicate is used for mutlithreading. 
            //compImg = Comparison Image
            
            System.Drawing.Image compImg = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath("~/omrtemp/lc.jpg"));


            // lc.jpg = Mirrored image sample as located on the corner of printed sheet
            UnmanagedImage compUMImg = UnmanagedImage.FromManagedImage((Bitmap)compImg);

            //lets create a graphics object in case the process fails to extract in future and we would want to draw some diagnosis marks on the image to be shown to the caller.
            Graphics g = Graphics.FromImage(PreparedImage);
            //A yellow Pen. How gentle!
            Pen yellowPen = new Pen(Color.Yellow, 2);
            //A red one. Red means danger here.
            Pen redPen = new Pen(Color.Red, 2);

            foreach (Blob blob in blobs)
            {
                if (blob.Rectangle.X < (PreparedImage.Width) / 2) // filters out blobs located at very different position
                {
                    //compUMImg = Comparison Unmanaged Image
                    //resize the comparison image to match the size of detected suspect
                    compUMImg = UnmanagedImage.FromManagedImage(PreprocessingHelper.ResizeImage(compImg, blob.Rectangle.Width, blob.Rectangle.Height));
                    //this method just does a pixel to to pixel comparison of two same sized images. If the images are similar, they must also have very good pixel match, regardless of the shape.
                    //this may not apply to some images, but the type of our suspect blob is compatible with this hypothsis
                    if (blob.Image.IsSameAs(compUMImg))
                    {
                        // to give the feedback, draw a rectangle around the blob. Don't send the main object back to the user. It won't be as usable as sending the blob image itself.                       
                        //add the corner mark to the array. Arranement is not significant. We will deal with that later
                        quad.Add(new IntPoint((int)blob.CenterOfGravity.X, (int)blob.CenterOfGravity.Y));
                    }                   
                }
            }


            //It is when we start to arrange the blobs in right sequence to conform with the rest of the algorithm
            // Sort out the list in right sequence, UpperLeft,LowerLeft,LowerRight,upperRight

            //we no assume that only 4 blobs will be added to the quad. Because there is no filteration we can perform now. to detect the left edge.
            //if this assumption is wrong and there are lesser or more blobs, we can  do anything but to declare an extraction failure.
            //2ndly, with this assumption, first two blobs must have bee detected as a left edge corner mark. there X component is not significant, only Y is
            //to  conform with the sequence we decided, do the following check
            //check count only to avoid an indexOutOfRangeException
            if (quad.Count > 1)
            {           
                if (quad[0].Y > quad[1].Y)
                {
                    IntPoint tp = quad[0];
                    quad[0] = quad[1];
                    quad[1] = tp;
                }
            }

            //do the same routine for right side corner marks
            //compUMImg = comparison Unmannaged Image, remember?

            compImg = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath("~/omrtemp/rc.jpg"));
            compUMImg = UnmanagedImage.FromManagedImage((Bitmap)compImg);            

            //this is same as what we did for left edge
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

            //2 more have been added to quad (persumably)
            //here, the lower blob needs to be first
            if (quad.Count > 3)
            {
               if (quad[2].Y < quad[3].Y)
                {
                    IntPoint tp = quad[2];
                    quad[2] = quad[3];
                    quad[3] = tp;
                }
            }
            
            //close the graphics object
            redPen.Dispose();
            yellowPen.Dispose();
            g.Flush();
            g.Dispose();
            //For the last time, check if wrong blobs pretended to our corner marks...


            if (quad.Count == 4)// means, //if our assumption of 4 blobs was true
            {
                if (((double)quad[1].Y - (double)quad[0].Y) / ((double)quad[2].Y - (double)quad[3].Y) < .75 ||
                    ((double)quad[1].Y - (double)quad[0].Y) / ((double)quad[2].Y - (double)quad[3].Y) > 1.25)
                    quad.Clear(); // clear if, both edges have insanely wrong lengths
                else if (quad[0].X > PreparedImage.Width / 2 || quad[1].X > PreparedImage.Width / 2 || quad[2].X < PreparedImage.Width / 2 || quad[3].X < PreparedImage.Width / 2)
                    quad.Clear(); // clear if, sides appear to be "wrong sided"
            }

            //sort the edges for wrap operation. Its required by the Aforge Wrap Operation
            IntPoint tp2 = quad[3];
            quad[3] = quad[1];
            quad[1] = tp2;

                return quad;                                   
        }
    }

}
