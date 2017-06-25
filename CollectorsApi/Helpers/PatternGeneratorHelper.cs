
using System;
using System.Drawing;
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

        public static Bitmap AddAnswerBlock(Bitmap sheet, int options, int answers, int startIndex, int coordX, int coordY)
        {
            var blockSize = new Size(options, answers);
            var indexingFont = new Font("Arial", 5000 / 70);
            var answerBlockBitmap = new Bitmap(1, 1);
            var g = Graphics.FromImage(answerBlockBitmap);
            int maxWid = 0;

            if (startIndex > 0)
            {
                for (int i = 0; i < answers; i++)
                {
                    int tw = (int)g.MeasureString((i + startIndex).ToString(), indexingFont).Width;
                    if (tw > maxWid)
                        maxWid = tw;
                }
            }

            var indBlockWid = maxWid + 5;

            answerBlockBitmap = new Bitmap(indBlockWid + options * 150 + 50, answers * 150 + 50);
            g = Graphics.FromImage(answerBlockBitmap);
            g.Clear(Color.White);

            g.DrawRectangle(new Pen(Brushes.Black, 4), new Rectangle(indBlockWid, 0, answerBlockBitmap.Width - indBlockWid, answerBlockBitmap.Height));

            int boxWid = (int)Math.Round((double)(answerBlockBitmap.Width - indBlockWid) / options);
            int lineMargin = (int)Math.Round(boxWid * 0.4) / 2;
            int boxHei = (int)Math.Round((double)answerBlockBitmap.Height / answers);
            int lineHei = (int)Math.Round(boxHei * 0.6);

            for (int j = 0; j < answers; j++)
            {
                if (j < answers - 1)
                {
                    g.DrawLine(new Pen(Brushes.Black, 2), indBlockWid + 1, boxHei * (j + 1), answerBlockBitmap.Width - 1, boxHei * (j + 1));
                }

                for (int i = 0; i < options; i++)
                {
                    if (i < options - 1)
                    {
                        g.DrawLine(new Pen(Brushes.Black, 2),
                            new PointF(indBlockWid + (i + 1) * boxWid, boxHei * j + lineMargin),
                            new PointF(indBlockWid + (i + 1) * boxWid, boxHei * j + lineMargin + lineHei));
                    }

                }

                if (startIndex > 0)
                {
                    string ansNum = (startIndex + j).ToString();
                    int length = (int)g.MeasureString(ansNum, indexingFont).Width;
                    int indX = indBlockWid - length - 5;
                    int indTextY = (150 - indexingFont.Height) / 2;
                    g.DrawString(ansNum, indexingFont, Brushes.Black, new Point(indX, indTextY + 150 * j + 50 / 2));
                }
            }

            g = Graphics.FromImage(sheet);

            g.DrawImage((Image)answerBlockBitmap, coordX, coordY, answerBlockBitmap.Width, answerBlockBitmap.Height);

            return sheet;
        }
    }
}