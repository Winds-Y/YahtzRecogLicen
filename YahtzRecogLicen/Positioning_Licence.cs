using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV.UI;

namespace YahtzRecogLicen
{
    class Positioning_Licence
    {
        private static Bitmap bitmap;
        private static int width;
        private static int height;
        private static int[,] gray_mat;
        public static void setBitmap(Bitmap bitmap)
        {
            Positioning_Licence.bitmap = bitmap;
            width = bitmap.Width;
            height = bitmap.Height;
            gray_mat = new int[width, height];
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    gray_mat[i, j] = bitmap.GetPixel(i, j).R;
                }
            }
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
            Bitmap bmp = bitmap.Clone(new Rectangle(left, up, right-left, down-up), System.Drawing.Imaging.PixelFormat.DontCare);
            return bmp;
        }
        private static double distance(int x1,int y1,int x2,int y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
        private static bool circle_isSetBlack(int x,int y,double r)
        {
            int left, right, up, down;
            left =(int) (x - r < 0 ? 0 : x - r);
            right = (int)(x + r < width ? x + r : width);
            up =(int) (y - r < 0 ? 0 : y - r);
            down =(int) (y + r < height ? y + r : height);
            bool flag=true;
            for(int i = left; i < right; i++)
            {
                for(int j = up; j < down; j++)
                {
                    if (distance(x, y, i, j) <= r)
                    {
                        if (bitmap.GetPixel(i, j).ToArgb() == Color.White.ToArgb())
                        {
                            flag = false;
                            break;
                        }
                    }
                }
            }
            return flag;
        }
        public static void circle_corrosion()
        {
            double r = 4;
            List<Point> list_white = new List<Point>();
            List<Point> list_black = new List<Point>();
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    if (bitmap.GetPixel(i, j).ToArgb() == Color.Black.ToArgb())
                    {
                        if (circle_isSetBlack(i, j, r))
                        {
                            list_black.Add(new Point(i, j));
                        }
                        else
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
                bitmap.SetPixel(point.X, point.Y, Color.White);
            }
        }
       
        


        public static void gray_corrosion()
        {
            int r = 16;
            int[,] xios = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    int left = i - r >= 0 ? i - r : 0;
                    int right = i + r < width ? i + r : width;
                    int up = j - r >= 0 ? j - r : 0;
                    int down = j + r < height ? j + r : height;
                    int min = 256;
                    for (int m = left; m < right; m++)
                    {
                        for(int n = up; n < down; n++)
                        {
                            if(distance(i,j,m,n)<=r)
                                min = Math.Min(min, bitmap.GetPixel(m, n).R);
                        }
                    }
                    xios[i, j] = min;
                    
                }

            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int x = xios[i, j];
                    bitmap.SetPixel(i, j, Color.FromArgb(x, x, x));
                }
            }
        }
        public static void gray_dilate()
        {

            int r = 16;
            int[,] xios = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int left = i - r >= 0 ? i - r : 0;
                    int right = i + r < width ? i + r : width;
                    int up = j - r >= 0 ? j - r : 0;
                    int down = j + r < height ? j + r : height;
                    int max = 0;
                    for (int m = left; m < right; m++)
                    {
                        for (int n = up; n < down; n++)
                        {
                            if (distance(i, j, m, n) <= r)
                                max = Math.Max(max, bitmap.GetPixel(m, n).R);
                        }
                    }

                    xios[i, j] = max;
                }
            }
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    int  x= xios[i, j];
                    bitmap.SetPixel(i, j, Color.FromArgb(x, x, x));
                }
            }
        }
        public static void imsubtract()
        {
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    int x = gray_mat[i, j] - bitmap.GetPixel(i, j).R;
                    x = Math.Max(0, x);
                    x = Math.Min(255, x);
                    bitmap.SetPixel(i, j, Color.FromArgb(x, x, x));
                }
            }
        }

        /// <summary>
        /// 该函数用于对图像进行腐蚀运算。结构元素为水平方向或垂直方向的三个点，
        /// 中间点位于原点；或者由用户自己定义3×3的结构元素。
        /// </summary>
        /// <param name="dgGrayValue">前后景临界值</param>
        /// <param name="nMode">腐蚀方式：0表示水平方向，1垂直方向，2自定义结构元素。</param>
        /// <param name="structure"> 自定义的3×3结构元素</param>
        public static Bitmap ErosionPic(int dgGrayValue, int nMode, bool[,] structure)
        {
            Bitmap bmpobj = bitmap.Clone(new Rectangle(0, 0, width, height), System.Drawing.Imaging.PixelFormat.DontCare);
            int lWidth = bmpobj.Width;
            int lHeight = bmpobj.Height;
            Bitmap newBmp = new Bitmap(lWidth, lHeight);

            int i, j, n, m;            //循环变量
            Color pixel;    //像素颜色值

            if (nMode == 0)
            {
                //使用水平方向的结构元素进行腐蚀
                // 由于使用1×3的结构元素，为防止越界，所以不处理最左边和最右边
                // 的两列像素
                for (j = 0; j < lHeight; j++)
                {
                    for (i = 1; i < lWidth - 1; i++)
                    {
                        //目标图像中的当前点先赋成黑色
                        newBmp.SetPixel(i, j, Color.Black);

                        //如果源图像中当前点自身或者左右有一个点不是黑色，
                        //则将目标图像中的当前点赋成白色
                        if (bmpobj.GetPixel(i - 1, j).R > dgGrayValue ||
                            bmpobj.GetPixel(i, j).R > dgGrayValue ||
                            bmpobj.GetPixel(i + 1, j).R > dgGrayValue)
                            newBmp.SetPixel(i, j, Color.White);
                    }
                }
            }
            else if (nMode == 1)
            {
                //使用垂真方向的结构元素进行腐蚀
                // 由于使用3×1的结构元素，为防止越界，所以不处理最上边和最下边
                // 的两行像素
                for (j = 1; j < lHeight - 1; j++)
                {
                    for (i = 0; i < lWidth; i++)
                    {
                        //目标图像中的当前点先赋成黑色
                        newBmp.SetPixel(i, j, Color.Black);

                        //如果源图像中当前点自身或者左右有一个点不是黑色，
                        //则将目标图像中的当前点赋成白色
                        if (bmpobj.GetPixel(i, j - 1).R > dgGrayValue ||
                            bmpobj.GetPixel(i, j).R > dgGrayValue ||
                            bmpobj.GetPixel(i, j + 1).R > dgGrayValue)
                            newBmp.SetPixel(i, j, Color.White);
                    }
                }
            }
            else
            {
                if (structure.Length != 9)  //检查自定义结构
                    return bmpobj;
                //使用自定义的结构元素进行腐蚀
                // 由于使用3×3的结构元素，为防止越界，所以不处理最左边和最右边
                // 的两列像素和最上边和最下边的两列像素
                for (j = 1; j < lHeight - 1; j++)
                {
                    for (i = 1; i < lWidth - 1; i++)
                    {
                        //目标图像中的当前点先赋成黑色
                        newBmp.SetPixel(i, j, Color.Black);
                        //如果原图像中对应结构元素中为黑色的那些点中有一个不是黑色，
                        //则将目标图像中的当前点赋成白色
                        for (m = 0; m < 3; m++)
                        {
                            for (n = 0; n < 3; n++)
                            {
                                if (!structure[m, n])
                                    continue;
                                if (bmpobj.GetPixel(i + m - 1, j + n - 1).R > dgGrayValue)
                                {
                                    newBmp.SetPixel(i, j, Color.White);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            bmpobj = newBmp;
            return bmpobj;
        }


        /// <summary>
        /// 该函数用于对图像进行细化运算。要求目标图像为灰度图像
        /// </summary>
        /// <param name="dgGrayValue"></param>
        public void ThiningPic(int dgGrayValue)
        {
            Bitmap bmpobj = bitmap.Clone(new Rectangle(0, 0, width, height), System.Drawing.Imaging.PixelFormat.DontCare);
            int lWidth = bmpobj.Width;
            int lHeight = bmpobj.Height;
            //   Bitmap newBmp = new Bitmap(lWidth, lHeight);

            bool bModified;            //脏标记
            int i, j, n, m;            //循环变量
            Color pixel;    //像素颜色值

            //四个条件
            bool bCondition1;
            bool bCondition2;
            bool bCondition3;
            bool bCondition4;

            int nCount;    //计数器
            int[,] neighbour = new int[5, 5];    //5×5相邻区域像素值



            bModified = true;
            while (bModified)
            {
                bModified = false;

                //由于使用5×5的结构元素，为防止越界，所以不处理外围的几行和几列像素
                for (j = 2; j < lHeight - 2; j++)
                {
                    for (i = 2; i < lWidth - 2; i++)
                    {
                        bCondition1 = false;
                        bCondition2 = false;
                        bCondition3 = false;
                        bCondition4 = false;

                        if (bmpobj.GetPixel(i, j).R > dgGrayValue)
                        {
                            if (bmpobj.GetPixel(i, j).R < 255)
                                bmpobj.SetPixel(i, j, Color.White);
                            continue;
                        }

                        //获得当前点相邻的5×5区域内像素值，白色用0代表，黑色用1代表
                        for (m = 0; m < 5; m++)
                        {
                            for (n = 0; n < 5; n++)
                            {
                                neighbour[m, n] = bmpobj.GetPixel(i + m - 2, j + n - 2).R < dgGrayValue ? 1 : 0;
                            }
                        }

                        //逐个判断条件。
                        //判断2<=NZ(P1)<=6
                        nCount = neighbour[1, 1] + neighbour[1, 2] + neighbour[1, 3]
                                + neighbour[2, 1] + neighbour[2, 3] +
                                +neighbour[3, 1] + neighbour[3, 2] + neighbour[3, 3];
                        if (nCount >= 2 && nCount <= 6)
                        {
                            bCondition1 = true;
                        }

                        //判断Z0(P1)=1
                        nCount = 0;
                        if (neighbour[1, 2] == 0 && neighbour[1, 1] == 1)
                            nCount++;
                        if (neighbour[1, 1] == 0 && neighbour[2, 1] == 1)
                            nCount++;
                        if (neighbour[2, 1] == 0 && neighbour[3, 1] == 1)
                            nCount++;
                        if (neighbour[3, 1] == 0 && neighbour[3, 2] == 1)
                            nCount++;
                        if (neighbour[3, 2] == 0 && neighbour[3, 3] == 1)
                            nCount++;
                        if (neighbour[3, 3] == 0 && neighbour[2, 3] == 1)
                            nCount++;
                        if (neighbour[2, 3] == 0 && neighbour[1, 3] == 1)
                            nCount++;
                        if (neighbour[1, 3] == 0 && neighbour[1, 2] == 1)
                            nCount++;
                        if (nCount == 1)
                            bCondition2 = true;

                        //判断P2*P4*P8=0 or Z0(p2)!=1
                        if (neighbour[1, 2] * neighbour[2, 1] * neighbour[2, 3] == 0)
                        {
                            bCondition3 = true;
                        }
                        else
                        {
                            nCount = 0;
                            if (neighbour[0, 2] == 0 && neighbour[0, 1] == 1)
                                nCount++;
                            if (neighbour[0, 1] == 0 && neighbour[1, 1] == 1)
                                nCount++;
                            if (neighbour[1, 1] == 0 && neighbour[2, 1] == 1)
                                nCount++;
                            if (neighbour[2, 1] == 0 && neighbour[2, 2] == 1)
                                nCount++;
                            if (neighbour[2, 2] == 0 && neighbour[2, 3] == 1)
                                nCount++;
                            if (neighbour[2, 3] == 0 && neighbour[1, 3] == 1)
                                nCount++;
                            if (neighbour[1, 3] == 0 && neighbour[0, 3] == 1)
                                nCount++;
                            if (neighbour[0, 3] == 0 && neighbour[0, 2] == 1)
                                nCount++;
                            if (nCount != 1)
                                bCondition3 = true;
                        }

                        //判断P2*P4*P6=0 or Z0(p4)!=1
                        if (neighbour[1, 2] * neighbour[2, 1] * neighbour[3, 2] == 0)
                        {
                            bCondition4 = true;
                        }
                        else
                        {
                            nCount = 0;
                            if (neighbour[1, 1] == 0 && neighbour[1, 0] == 1)
                                nCount++;
                            if (neighbour[1, 0] == 0 && neighbour[2, 0] == 1)
                                nCount++;
                            if (neighbour[2, 0] == 0 && neighbour[3, 0] == 1)
                                nCount++;
                            if (neighbour[3, 0] == 0 && neighbour[3, 1] == 1)
                                nCount++;
                            if (neighbour[3, 1] == 0 && neighbour[3, 2] == 1)
                                nCount++;
                            if (neighbour[3, 2] == 0 && neighbour[2, 2] == 1)
                                nCount++;
                            if (neighbour[2, 2] == 0 && neighbour[1, 2] == 1)
                                nCount++;
                            if (neighbour[1, 2] == 0 && neighbour[1, 1] == 1)
                                nCount++;
                            if (nCount != 1)
                                bCondition4 = true;
                        }

                        if (bCondition1 && bCondition2 && bCondition3 && bCondition4)
                        {
                            bmpobj.SetPixel(i, j, Color.White);
                            bModified = true;
                        }
                        else
                        {
                            bmpobj.SetPixel(i, j, Color.Black);
                        }
                    }
                }
            }
           
            
            // 复制细化后的图像
            //    bmpobj = newBmp;
        }
        
    }
}
