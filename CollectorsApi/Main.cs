using AForge;
using AForge.Imaging.Filters;
using CollectorsApi.Helpers;
using CollectorsApi.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace CollectorsApi
{
    public static class Main
    {
        private static object lockObject = new object();
        public static PatternsContext db = new PatternsContext();
        
        public static Bitmap ApplyWrap(List<AForge.IntPoint> quad, Bitmap originalIage, double scale, Pattern pattern)
        {
            //pattern witdh and height
            double ratio = Math.Max((double)originalIage.Width / (double)pattern.Width, (double)originalIage.Height / (double)pattern.Height);
            ratio *= scale;
            AForge.Imaging.Filters.QuadrilateralTransformation wrap = new AForge.Imaging.Filters.QuadrilateralTransformation(quad);
            wrap.UseInterpolation = false; //perspective wrap only, no binary.
            Size sr = new Size() {
                Width = pattern.Width,
                Height = pattern.Height
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

        public static int GetTestScore(Pattern pattern, Photo photo, List<PatternAnswerSheet> answerSheet)
        {
            var ms = new MemoryStream(photo.Image);
            PreprocessingHelper.camImg = Image.FromStream(ms);

            answerSheet.Sort((x, y) => x.QuestionNumber.CompareTo(y.QuestionNumber));

            List<AForge.IntPoint> wrapPoints = new List<AForge.IntPoint>();
            Bitmap extd = new Bitmap(1, 1);
            Bitmap extForEB = new Bitmap(1, 1);

            Image flattenedImage = PreprocessingHelper.CamImage.PrepareForObjectDetection(0, false);

            lock (lockObject)
            {
                wrapPoints = (List<AForge.IntPoint>)(new Bitmap(flattenedImage)).ExtractPaperFromPrepapred(PreprocessingHelper.CamImage, true, pattern);
            }


            extd = ApplyWrap(wrapPoints, new Bitmap(PreprocessingHelper.CamImage), PreprocessingHelper.ImageProcessScale, pattern);

            //extracted sheet
            var recogImg = (Image)extd;

            lock (lockObject)
            {
                for (int i = 0; i < pattern.AnswerBlocks.Count; i++)
                {
                    var list = pattern.AnswerBlocks.ToList();
                    var BinaryMaskedOMs = new bool[list[i].Rows, list[i].AnswerOptionsNumber];

                    var CroppedImage = CutOutBlockImage(recogImg, new Size(pattern.Width, pattern.Height), new RectangleF(list[i].CoordinateX, list[i].CoordinateY, list[i].Width, list[i].Height));
                    var CroppedAnswerLines = SliceOsMarkBlock(CroppedImage, list[i].Rows);

                    //rate the marks according to the dimensions collected from database and image extracted/cropped
                    for (int j = 0; j < list[i].Rows; j++)
                    {
                        //initialize a temp array for current line of current block. (see current depth of 2 for loops)
                        bool[] tempSlice = GetBinaryMaskedScore(new Bitmap(CroppedAnswerLines[j]), list[i].AnswerOptionsNumber, 0, false);
                        //rate current mark of current line of current block. (see current depth of 3 for loops)
                        for (int k = 0; k < tempSlice.Length; k++)
                        {
                            BinaryMaskedOMs[j, k] = tempSlice[k];
                        }
                    }

                    var partialSheet = answerSheet.GetRange(list[i].FirstQuestionIndex-1, list[i].Rows).ToList();

                    var scores = list[i].RateScores(partialSheet, BinaryMaskedOMs, false);
                }
            }

            return 10;
        }

        public static List<List<bool>> ProcessSheetAnswers(List<PatternAnswerSheet> partialSheet)
        {
            List<List<bool>> answers = new List<List<bool>>();

            for (var i = 0; i < partialSheet.Count; i++)
            {
                answers.Add(new List<bool>());
                for (var j = 0; j < partialSheet[i].Answer.Length; j++)
                {
                    var answerBubble = partialSheet[i].Answer[j].ToString();

                    answers[i].Add(answerBubble == "1" ? true : false);
                }
            }

            return answers;
        }

        public static double[] RateScores(this AnswerBlock ansBlock, List<PatternAnswerSheet> partialSheet, bool[,] BinaryMaskedOMs, bool multipleAnswers)
        {
            double[] scores = new double[ansBlock.Rows];

            var answerArray = ProcessSheetAnswers(partialSheet);

            for (int i = 0; i < partialSheet.Count; i++)
            {
                if (multipleAnswers)
                {
                    bool allRight = true;
                    bool hasMarked = false;
                    for (int j = 0; j < partialSheet[i].Answer.Length; j++)
                    {
                        if (BinaryMaskedOMs[i, j] != answerArray[i][j])
                            allRight = false;
                        if (BinaryMaskedOMs[i, j])
                            hasMarked = true;
                    }
                    if (!hasMarked)
                        scores[i] = 0;
                    else if (allRight)
                        scores[i] = 4;
                    else
                        scores[i] = -1;
                }
                else
                {
                    double match = 0;
                    var CorrectAnswersNumber = 0;

                    for (int j = 0; j < answerArray[i].Count; j++)
                    {
                        if (BinaryMaskedOMs[i, j] == answerArray[i][j])
                        {
                            if (answerArray[i].Where(x => x == true).Count() > 1)
                            {
                                match += 1;
                                CorrectAnswersNumber++;                                
                            }
                            if (match == answerArray[i].Where(x => x == true).Count())
                                match = 1;
                            else if (match < answerArray[i].Where(x => x == true).Count() && CorrectAnswersNumber == answerArray[i].Where(x => x == true).Count())
                                match = 0.5;

                            else
                                match = 1;

                        }
                        else match = 0;
                    }

                    if (match > 0)
                        scores[i] = match;
                    else
                        scores[i] = 0;
                   
                }
            }

            return scores;
        }

        public static System.Drawing.Image CutOutBlockImage(System.Drawing.Image image, Size sBounds, RectangleF block)
        {
            return CutOutBlockImage(image, sBounds, new Rectangle(
                (int)Math.Round(block.X),
                (int)Math.Round(block.Y),
                (int)Math.Round(block.Width),
                (int)Math.Round(block.Height)));
        }

        public static System.Drawing.Image CutOutBlockImage(System.Drawing.Image image, Size sBounds, Rectangle block)
        {
            double xScale = (double)image.Width / sBounds.Width;
            double yScale = (double)image.Height / sBounds.Height;

            sBounds = new Size(
                (int)Math.Round((double)sBounds.Width * xScale),
                (int)Math.Round((double)sBounds.Height * yScale));
            block = new Rectangle(
                (int)Math.Round((double)block.X * xScale),
                (int)Math.Round((double)block.Y * yScale),
                (int)Math.Round((double)block.Width * xScale),
                (int)Math.Round((double)block.Height * yScale));

            List<IntPoint> quad = new List<IntPoint>();
            quad.Add(new IntPoint(block.X, block.Y));
            quad.Add(new IntPoint(block.X + block.Width, block.Y));
            quad.Add(new IntPoint(block.X + block.Width, block.Y + block.Height));
            quad.Add(new IntPoint(block.X, block.Y + block.Height));
            var sr = block.Size;
            QuadrilateralTransformation wrap = new QuadrilateralTransformation(quad);
            wrap.UseInterpolation = false; //perspective wrap only, no binary.
            wrap.AutomaticSizeCalculaton = false;
            wrap.NewWidth = sr.Width;
            wrap.NewHeight = sr.Height;
            System.Drawing.Image img = (System.Drawing.Image)wrap.Apply(new Bitmap(image)); // wrap
            return img;
        }

        public static Bitmap[] SliceOsMarkBlock(System.Drawing.Image croppedBlock, int slices)
        {
            List<Rectangle> cropRects = new List<Rectangle>();
            for (int i = 0; i < slices; i++)
            {
                cropRects.Add(new Rectangle(
                    0,
                    (int)Math.Round((double)i * (double)croppedBlock.Height / (double)slices),
                    croppedBlock.Width, (int)Math.Round((double)croppedBlock.Height / (double)slices)));
            }
            Bitmap[] bmps = new Bitmap[slices];
            Bitmap src = (Bitmap)croppedBlock;
            for (int i = 0; i < cropRects.Count; i++)
            {
                Rectangle cropRect = cropRects[i];
                bmps[i] = new Bitmap(cropRect.Width, cropRect.Height);

                using (Graphics g = Graphics.FromImage(bmps[i]))
                {
                    g.DrawImage(src, new Rectangle(0, 0, bmps[i].Width, bmps[i].Height),
                                     cropRect,
                                     GraphicsUnit.Pixel);
                }
            }
            return bmps;
        }

        public static bool[] GetBinaryMaskedScore(Bitmap slice, int OMCount, int sheetWhite, bool overrideWhite)
        {
            //make grayscale
            Grayscale gsf = new Grayscale(0.2989, 0.5870, 0.1140);
            slice = gsf.Apply(slice);
            //initialize members
            Rectangle[] cropRects = new Rectangle[OMCount];
            Bitmap[] marks = new Bitmap[OMCount];
            bool[] answers = new bool[OMCount];
            //sub-devide line into options count (horizontal only)
            for (int i = 0; i < OMCount; i++)
            {
                cropRects[i] = new Rectangle(i * slice.Width / OMCount, 0, slice.Width / OMCount, slice.Height);
            }
            //the user specified that the line is actually multiple o Marks. lets crop them
            for (int i = 0; i < OMCount; i++)
            {
                marks[i] = new Bitmap(cropRects[i].Width, cropRects[i].Height);
                using (Graphics g = Graphics.FromImage(marks[i]))
                {
                    g.DrawImage(slice, new Rectangle(0, 0, marks[i].Width, marks[i].Height),
                                     cropRects[i],
                                     GraphicsUnit.Pixel);
                }
            }
            if (!overrideWhite)
                sheetWhite = PreprocessingHelper.GetWhiteLevel(new Bitmap(slice));
            for (int i = 0; i < OMCount; i++)
            {
                int temp = 0;
                temp = Math.Max(PreprocessingHelper.AvgColor((System.Drawing.Image)marks[i], sheetWhite / 2), 0);
                temp = (int)Math.Round(100 - (double)temp / (double)255 * 100D);
                //the lesser the temp var, the more sensative the detection becomes. This Value can be calibrated with further experimentation
                answers[i] = temp > 20;
            }
            return answers;
        }
    }
}
