using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace YahtzRecogLicen
{
    class Positioning_Licence
    {
        private static Bitmap bitmap;
        private static int width;
        private static int height;
        public static void setBitmap(Bitmap bitmap)
        {
            Positioning_Licence.bitmap = bitmap;
            width = bitmap.Width;
            height = bitmap.Height;
        }
        public static Bitmap getBitmap()
        {
            return bitmap;
        }

        public static bool corrosion_isSetBlack(int row,int col)
        {
            if (row != 0 && col != 0)
            {
                int left = row - 1;
                int up = col - 1;
                if (bitmap.GetPixel(row, col).ToArgb() == Color.Black.ToArgb() && bitmap.GetPixel(left, col).ToArgb() == Color.Black.ToArgb()
                    && bitmap.GetPixel(row, up).ToArgb() == Color.Black.ToArgb())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static bool dilate_isSetBlack(int row,int col)
        {
            
            if(row!=0 && col != 0 )
            {
                if (bitmap.GetPixel(row, col).ToArgb() == Color.Black.ToArgb())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else{
                return false;
            }
        }
        public static void corrosion()
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            List<Point> list_black = new List<Point>();
            List<Point> list_white = new List<Point>();
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    if (corrosion_isSetBlack(i, j))
                    {
                        list_black.Add(new Point(i, j));
                    }
                    else
                    {
                        list_white.Add(new Point(i, j));
                    }
                }
            }
            foreach(Point point in list_black)
            {
                bitmap.SetPixel(point.X, point.Y, Color.Black);
            }
            foreach(Point point in list_white)
            {
                //Color color = Color.FromArgb(1, 1, 1);
                bitmap.SetPixel(point.X, point.Y, Color.White);
            }
            
        }
        public static void dilate()
        {
            List<Point> list_black = new List<Point>();
            List<Point> list_white = new List<Point>();
            for(int i = 0; i < bitmap.Width-1; i++)
            {
                for(int j = 0; j < bitmap.Height-1; j++)
                {
                    if (dilate_isSetBlack(i, j))
                    {
                        list_black.Add(new Point(i, j));
                    }
                    else
                    {
                        list_white.Add(new Point(i, j));
                    }
                }
            }
            foreach(Point point in list_black)
            {
                bitmap.SetPixel(point.X, point.Y, Color.Black);
                bitmap.SetPixel(point.X + 1, point.Y, Color.Black);
                bitmap.SetPixel(point.X, point.Y + 1, Color.Black);
            }
            foreach(Point point in list_white)
            {
                //Color color = Color.FromArgb(1, 1, 1);
                bitmap.SetPixel(point.X, point.Y, Color.White);
            }
        }
        public static Bitmap find_plant()
        {
            int[] Blue_y = new int[height];

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (bitmap.GetPixel(i, j).ToArgb() == Color.White.ToArgb())
                    {
                        Blue_y[j]++;
                    }
                }
            }
            int max_index_y = -1;
            int max = -1;
            for(int i = 0; i < Blue_y.Length; i++)
            {
                if (max < Blue_y[i])
                {
                    max = Blue_y[i];
                    max_index_y = i;
                }
            }
            int up = max_index_y;
            int th = 5;
            while(up > 0 &&  Blue_y[up]>th )
            {
                up--;
            }
            int down = max_index_y;
            while(down < Blue_y.Length && Blue_y[down]>th)
            {
                down++;
            }
            int[] Blue_x = new int[width];
            for(int i = 0; i < width; i++)
            {
                for(int j = up; j < down; j++)
                {
                    if (bitmap.GetPixel(i, j).ToArgb() == Color.White.ToArgb())
                    {
                        Blue_x[i]++;
                    }
                }
            }
            int left = 0;
            int xh = 3;
            while(left<width && Blue_x[left] < xh)
            {
                left++;
            }
            int right = width - 1;
            while(right>0 && Blue_x[right] < xh)
            {
                right--;
            }
            Console.WriteLine(left + " " + right + " " + up + " " + down);
            Bitmap bmp = bitmap.Clone(new Rectangle(left, up, right-left+1, down-up+1), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            return bmp;
        }
    }
}
