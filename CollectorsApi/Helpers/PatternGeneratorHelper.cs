
using CollectorsApi.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace CollectorsApi.Helpers
{
    public static class PatternGeneratorHelper
    {
        public static Bitmap AddWrapPoints()
        {
            var sheet = new Bitmap(4200, 5940);
            var g = Graphics.FromImage(sheet);

            g.Clear(Color.White);

            /// images take from open source code : https://www.codeproject.com/Articles/884518/Csharp-Optical-Marks-Recognition-OMR-Engine-a-Mar
            g.DrawImage(Image.FromFile(HttpContext.Current.Server.MapPath("~/omrtemp/LC Prints.jpg")),
                new Rectangle(new Point(100, 100), new System.Drawing.Size(150, 150)));
            g.DrawImage(Image.FromFile(HttpContext.Current.Server.MapPath("~/omrtemp/LC Prints.jpg")),
                new Rectangle(new Point(100, sheet.Height - 250), new System.Drawing.Size(150, 150)));
            g.DrawImage(Image.FromFile(HttpContext.Current.Server.MapPath("~/omrtemp/RC Prints.jpg")),
                new Rectangle(new Point(sheet.Width - 250, 100), new System.Drawing.Size(150, 150)));
            g.DrawImage(Image.FromFile(HttpContext.Current.Server.MapPath("~/omrtemp/RC Prints.jpg")),
                new Rectangle(new Point(sheet.Width - 250, sheet.Height - 250), new System.Drawing.Size(150, 150)));

            return sheet;
        }

        /// <summary>
        /// source code : modified and updated from https://www.codeproject.com/Articles/884518/Csharp-Optical-Marks-Recognition-OMR-Engine-a-Mar
        /// </summary>
        public static Pattern AddAnswerBlock(Pattern pattern)
        {
            var answerBlock = pattern.AnswerBlocks.First();

            var blockSize = new Size(answerBlock.AnswerOptionsNumber, answerBlock.Rows);
            var indexingFont = new Font("Arial", 5000 / 70);
            var answerBlockBitmap = new Bitmap(1, 1);
            var g = Graphics.FromImage(answerBlockBitmap);
            int maxWid = 0;

            if (answerBlock.FirstQuestionIndex > 0)
            {
                for (int i = 0; i < answerBlock.Rows; i++)
                {
                    int tw = (int)g.MeasureString((i + answerBlock.FirstQuestionIndex).ToString(), indexingFont).Width;
                    if (tw > maxWid)
                        maxWid = tw;
                }
            }

            var indBlockWid = maxWid + 5;

            answerBlockBitmap = new Bitmap(indBlockWid + answerBlock.AnswerOptionsNumber * 150 + 50, answerBlock.Rows * 150 + 50);
            g = Graphics.FromImage(answerBlockBitmap);
            g.Clear(Color.White);

            g.DrawRectangle(new Pen(Brushes.Black, 4), new Rectangle(indBlockWid, 0, answerBlockBitmap.Width - indBlockWid, answerBlockBitmap.Height));

            int boxWid = (int)Math.Round((double)(answerBlockBitmap.Width - indBlockWid) / answerBlock.AnswerOptionsNumber);
            int lineMargin = (int)Math.Round(boxWid * 0.4) / 2;
            int boxHei = (int)Math.Round((double)answerBlockBitmap.Height / answerBlock.Rows);
            int lineHei = (int)Math.Round(boxHei * 0.6);

            for (int j = 0; j < answerBlock.Rows; j++)
            {
                if (j < answerBlock.Rows - 1)
                {
                    g.DrawLine(new Pen(Brushes.Black, 2), indBlockWid + 1, boxHei * (j + 1), answerBlockBitmap.Width - 1, boxHei * (j + 1));
                }

                for (int i = 0; i < answerBlock.AnswerOptionsNumber; i++)
                {
                    if (i < answerBlock.AnswerOptionsNumber - 1)
                    {
                        g.DrawLine(new Pen(Brushes.Black, 2),
                            new PointF(indBlockWid + (i + 1) * boxWid, boxHei * j + lineMargin),
                            new PointF(indBlockWid + (i + 1) * boxWid, boxHei * j + lineMargin + lineHei));
                    }

                }

                if (answerBlock.FirstQuestionIndex > 0)
                {
                    string ansNum = (answerBlock.FirstQuestionIndex + j).ToString();
                    int length = (int)g.MeasureString(ansNum, indexingFont).Width;
                    int indX = indBlockWid - length - 5;
                    int indTextY = (150 - indexingFont.Height) / 2;
                    g.DrawString(ansNum, indexingFont, Brushes.Black, new Point(indX, indTextY + 150 * j + 50 / 2));
                }
            }

            var ms = new MemoryStream(pattern.Image);
            var sheet = new Bitmap(ms);
            g = Graphics.FromImage(sheet);


            g.DrawImage((Image)answerBlockBitmap, answerBlock.CoordinateX, answerBlock.CoordinateY, answerBlockBitmap.Width, answerBlockBitmap.Height);

            answerBlock.Width = answerBlockBitmap.Width;
            answerBlock.Height = answerBlockBitmap.Height;

            return new Pattern { Image = sheet.ToByteArray(ImageFormat.Jpeg), AnswerBlocks = new List<AnswerBlock> { answerBlock } };
        }
    }
}