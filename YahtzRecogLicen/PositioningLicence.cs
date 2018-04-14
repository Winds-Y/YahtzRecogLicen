using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace YahtzRecogLicen
{
    internal static  class PositioningLicence
    {
        private static Bitmap _bitmap;
        private static int _width;
        private static int _height;
        private static int[,] _grayMat;
        public static void SetBitmap(Bitmap bitmap)
        {
            _bitmap = bitmap;
            _width = bitmap.Width;
            _height = bitmap.Height;
            _grayMat = new int[_width, _height];
            for(var i = 0; i < _width; i++)
            {
                for(var j = 0; j < _height; j++)
                {
                    _grayMat[i, j] = bitmap.GetPixel(i, j).R;
                }
            }
        }
        public static Bitmap GetBitmap()
        {
            return _bitmap;
        }
        public static void WriteToFile(string str)
        {
            const string fileName = "output\\log.txt";
            /*if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }*/
            var streamWriter=new StreamWriter(fileName,true);
            streamWriter.Write(str);
            streamWriter.Flush();
            streamWriter.Close();
            streamWriter.Dispose();
        }
        
        /*private static bool corrosion_isSetBlack(int row,int col)
        {
            if (row == 0 || col == 0) return false;
            var left = row - 1;
            var up = col - 1;
            return _bitmap.GetPixel(row, col).ToArgb() == Color.Black.ToArgb() && _bitmap.GetPixel(left, col).ToArgb() == Color.Black.ToArgb()
                                                                               && _bitmap.GetPixel(row, up).ToArgb() == Color.Black.ToArgb();
        }
        private static bool dilate_isSetBlack(int row,int col)
        {

            if(row!=0 && col != 0 )
            {
                return _bitmap.GetPixel(row, col).ToArgb() == Color.Black.ToArgb();
            }
            return false;
        }*/

        /*public static void dilate()
        {
            var listBlack = new List<Point>();
            var listWhite = new List<Point>();
            for(var i = 0; i < _bitmap.Width-1; i++)
            {
                for(var j = 0; j < _bitmap.Height-1; j++)
                {
                    if (dilate_isSetBlack(i, j))
                    {
                        listBlack.Add(new Point(i, j));
                    }
                    else
                    {
                        listWhite.Add(new Point(i, j));
                    }
                }
            }
            foreach(var point in listBlack)
            {
                _bitmap.SetPixel(point.X, point.Y, Color.Black);
                _bitmap.SetPixel(point.X + 1, point.Y, Color.Black);
                _bitmap.SetPixel(point.X, point.Y + 1, Color.Black);
            }
            foreach(var point in listWhite)
            {
                //Color color = Color.FromArgb(1, 1, 1);
                _bitmap.SetPixel(point.X, point.Y, Color.White);
            }
        }*/
        
        private static double Distance(int x1,int y1,int x2,int y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
        /*private static bool circle_isSetBlack(int x,int y,double r)
        {
            var left = (int) (x - r < 0 ? 0 : x - r);
            var right = (int)(x + r < _width ? x + r : _width);
            var up = (int) (y - r < 0 ? 0 : y - r);
            var down = (int) (y + r < _height ? y + r : _height);
            var flag=true;
            for(var i = left; i < right; i++)
            {
                for(var j = up; j < down; j++)
                {
                    if (!(Distance(x, y, i, j) <= r)) continue;
                    if (_bitmap.GetPixel(i, j).ToArgb() != Color.White.ToArgb()) continue;
                    flag = false;
                    break;
                }
            }
            return flag;
        }
        public static void circle_corrosion()
        {
            const double r = 4;
            var listWhite = new List<Point>();
            var listBlack = new List<Point>();
            for(var i = 0; i < _width; i++)
            {
                for(var j = 0; j < _height; j++)
                {
                    if (_bitmap.GetPixel(i, j).ToArgb() != Color.Black.ToArgb()) continue;
                    if (circle_isSetBlack(i, j, r))
                    {
                        listBlack.Add(new Point(i, j));
                    }
                    else
                        listWhite.Add(new Point(i, j));

                }
            }
            foreach(var point in listBlack)
            {
                _bitmap.SetPixel(point.X, point.Y, Color.Black);
            }
            foreach(var point in listWhite)
            {
                _bitmap.SetPixel(point.X, point.Y, Color.White);
            }
        }*/
       
        public static void gray_corrosion()
        {
            //Console.WriteLine("niiiiii");
            //int r = 16;
            //int[,] xios = new int[width, height];
            //Rectangle rectangle = new Rectangle(0, 0, width, height);
            //BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite,bitmap.PixelFormat);
            //IntPtr intPtr = bitmapData.Scan0;
            //int size = Math.Abs(bitmapData.Stride) * height;
            //byte[] rgb = new byte[size];
            //byte[] container = new byte[size];
            //System.Runtime.InteropServices.Marshal.Copy(intPtr, rgb, 0, size);
            //for(int i = 0; i < rgb.Length; i += 3)
            //{
            //    int index = i / 3;
            //    int x = index / width;
            //    int y = index % width;
            //    byte min = 255;
            //    Console.WriteLine(x + " " + y);
            //    int left = x - r >= 0 ? x - r : 0;
            //    int right = x + r < rgb.Length / 3 ? x + r : rgb.Length / 3;
            //    int up = y - r >= 0 ? y - r : 0;
            //    int down = y + r < height ? y + r : height;
            //    int start = (left * width + up)*3;
            //    int end = (right * width + down)*3;
            //    for (int j = start; j < end; j+=3)
            //    {
            //        int sub_index = j / 3;
            //        int m = sub_index / width;
            //        int n = sub_index % width;
            //        if (distance(x, y, m, n) <= r)
            //            min = Math.Min(min, rgb[j]);
            //    }
            //    container[i] = min;
            //}
            //for(int i = 0; i < rgb.Length; i+=3)
            //{
            //    rgb[i] = container[i];
            //}
            //bitmap.UnlockBits(bitmapData);
            const int r = 16;
            var xios = new byte[_width, _height];
            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    var left = i - r >= 0 ? i - r : 0;
                    var right = i + r < _width ? i + r : _width;
                    var up = j - r >= 0 ? j - r : 0;
                    var down = j + r < _height ? j + r : _height;
                    byte min = 255;
                    for (var m = left; m < right; m++)
                    {
                        for (var n = up; n < down; n++)
                        {
                            if (!(Distance(i, j, m, n) <= r)) continue;
                            //min = Math.Min(min, bitmap.GetPixel(m, n).R);
                            var value = _bitmap.GetPixel(m, n).R;
                            min = min > value ? value : min;

                        }
                    }
                    xios[i, j] = min;

                }

            }

            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    int x = xios[i, j];
                    _bitmap.SetPixel(i, j, Color.FromArgb(x, x, x));
                }
            }
        }
        public static void gray_dilate()
        {
            //int r = 16;
            //int[,] xios = new int[width, height];
            //Rectangle rectangle = new Rectangle(0, 0, width, height);
            //BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            //IntPtr intPtr = bitmapData.Scan0;
            //int size = Math.Abs(bitmapData.Stride) * height;
            //byte[] rgb = new byte[size];
            //byte[] container = new byte[size];
            //System.Runtime.InteropServices.Marshal.Copy(intPtr, rgb, 0, size);
            //for (int i = 0; i < rgb.Length; i += 3)
            //{
            //    int index = i / 3;
            //    int x = index / width;
            //    int y = index % width;
            //    Console.WriteLine(x + " " + y);
            //    byte max = 0;
            //    for (int j = 0; j < rgb.Length; j+=3)
            //    {
            //        int sub_index = j / 3;
            //        int m = sub_index / width;
            //        int n = sub_index % width;
            //        if (distance(x, y, m, n) <= r)
            //            max = Math.Max(max, rgb[j]);
            //    }
            //    container[i] = max;
            //}
            //for (int i = 0; i < rgb.Length; i += 3)
            //{
            //    rgb[i] = container[i];
            //}
            //bitmap.UnlockBits(bitmapData);

            const int r = 16;
            var xios = new byte[_width, _height];
            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    var left = i - r >= 0 ? i - r : 0;
                    var right = i + r < _width ? i + r : _width;
                    var up = j - r >= 0 ? j - r : 0;
                    var down = j + r < _height ? j + r : _height;
                    byte max = 0;
                    for (var m = left; m < right; m++)
                    {
                        for (var n = up; n < down; n++)
                        {
                            if (!(Distance(i, j, m, n) <= r)) continue;
                            //max = Math.Max(max, bitmap.GetPixel(m, n).R);
                            var value = _bitmap.GetPixel(m, n).R;
                            max = max < value ? value : max;

                        }
                    }

                    xios[i, j] = max;
                }
            }
            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    int x = xios[i, j];
                    _bitmap.SetPixel(i, j, Color.FromArgb(x, x, x));
                }
            }
        }

        public static void Imsubtract()
        {
            for(var i = 0; i < _width; i++)
            {
                for(var j = 0; j < _height; j++)
                {
                    var x = _grayMat[i, j] - _bitmap.GetPixel(i, j).R;
                    x = Math.Max(0, x);
                    x = Math.Min(255, x);
                    _bitmap.SetPixel(i, j, Color.FromArgb(x, x, x));
                }
            }
        }

        public static void corrosion_SE(int row,int col)
        {
            var xios = new byte[_width, _height];
            row /= 2;
            col /= 2;
            
            for(var i = 0; i < _width; i++)
            {
                for(var j = 0; j < _height; j++)
                {
                    var left = i - col >= 0 ? i - col : 0;
                    var right = i + col < _width ? i + col : _width;
                    var up = j - row >= 0 ? j - row : 0;
                    var down = j + row < _height ? j + row : _height;
                    byte min = 255;
                    for (var m = left; m < right; m++)
                    {
                        for (var n = up; n < down; n++)
                        {
                            var value = _bitmap.GetPixel(m, n).R;
                            min = min > value ? value : min;
                        }
                    }
                    xios[i, j] = min;
                }
            }

            for(var i = 0; i < _width; i++)
            {
                for(var j = 0; j < _height; j++)
                {
                    var value = xios[i, j];
                    _bitmap.SetPixel(i, j, Color.FromArgb(value, value, value));
                }
            }
        }
        public static void dalite_SE(int row,int col)
        {
            var xios = new byte[_width, _height];
            row /= 2;
            col /= 2;

            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    var left = i - col >= 0 ? i - col : 0;
                    var right = i + col < _width ? i + col : _width;
                    var up = j - row >= 0 ? j - row : 0;
                    var down = j + row < _height ? j + row : _height;
                    byte max = 0;
                    for (var m = left; m < right; m++)
                    {
                        for (var n = up; n < down; n++)
                        {
                            var value = _bitmap.GetPixel(m, n).R;
                            max = max < value ? value : max;
                        }
                    }
                    xios[i, j] = max;
                    
                }
            }

            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    var value = xios[i, j];
                    _bitmap.SetPixel(i, j, Color.FromArgb(value, value, value));
                }
            }
        }        
        


        /// <summary>
        /// 该函数用于对图像进行细化运算。要求目标图像为灰度图像
        /// </summary>
        /// <param name="dgGrayValue"></param>
        public static void ThiningPic(int dgGrayValue)
        {
            var bmpobj = _bitmap.Clone(new Rectangle(0, 0, _width, _height), PixelFormat.DontCare);
            var lWidth = bmpobj.Width;
            var lHeight = bmpobj.Height;
            //   Bitmap newBmp = new Bitmap(lWidth, lHeight);

            //Color pixel;    //像素颜色值

            //四个条件

            var neighbour = new int[5, 5];    //5×5相邻区域像素值



            var bModified = true;
            while (bModified)
            {
                bModified = false;

                //由于使用5×5的结构元素，为防止越界，所以不处理外围的几行和几列像素
                int j;            //循环变量
                for (j = 2; j < lHeight - 2; j++)
                {
                    int i;            //循环变量
                    for (i = 2; i < lWidth - 2; i++)
                    {
                        var bCondition1 = false;
                        var bCondition2 = false;
                        var bCondition3 = false;
                        var bCondition4 = false;

                        if (bmpobj.GetPixel(i, j).R > dgGrayValue)
                        {
                            if (bmpobj.GetPixel(i, j).R < 255)
                                bmpobj.SetPixel(i, j, Color.White);
                            continue;
                        }

                        //获得当前点相邻的5×5区域内像素值，白色用0代表，黑色用1代表
                        int m;            //循环变量
                        for (m = 0; m < 5; m++)
                        {
                            int n;            //循环变量
                            for (n = 0; n < 5; n++)
                            {
                                neighbour[m, n] = bmpobj.GetPixel(i + m - 2, j + n - 2).R < dgGrayValue ? 1 : 0;
                            }
                        }

                        //逐个判断条件。
                        //判断2<=NZ(P1)<=6
                        var nCount = neighbour[1, 1] + neighbour[1, 2] + neighbour[1, 3]
                                     + neighbour[2, 1] + neighbour[2, 3] +
                                     +neighbour[3, 1] + neighbour[3, 2] + neighbour[3, 3];    //计数器
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

            _bitmap = bmpobj.Clone(new Rectangle(0, 0, _width, _height), PixelFormat.DontCare);
            // 复制细化后的图像
            //    bmpobj = newBmp;
        }

        public static Bitmap find_plant(Bitmap primeBitmap)
        {
            var blueY = new int[_height];

            for (var j = 0; j < _height; j++)
            {
                for (var i = 0; i < _width; i++)
                {
                    if (_bitmap.GetPixel(i, j).ToArgb() == Color.White.ToArgb())
                    {
                        blueY[j]++;
                    }
                }
            }
            var maxIndexY = -1;
            var max = -1;
            for (var i = 0; i < blueY.Length; i++)
            {
                if (max >= blueY[i]) continue;
                max = blueY[i];
                maxIndexY = i;
            }
            var up = maxIndexY;
            const int th = 5;
            while (up > 0 && blueY[up] > th)
            {
                up--;
            }
            var down = maxIndexY;
            while (down < blueY.Length && blueY[down] > th)
            {
                down++;
            }
            var blueX = new int[_width];
            for (var i = 0; i < _width; i++)
            {
                for (var j = up; j < down; j++)
                {
                    if (_bitmap.GetPixel(i, j).ToArgb() == Color.White.ToArgb())
                    {
                        blueX[i]++;
                    }
                }
            }
            var left = 0;
            const int xh = 3;
            while (left < _width && blueX[left] < xh)
            {
                left++;
            }
            var right = _width - 1;
            while (right > 0 && blueX[right] < xh)
            {
                right--;
            }
            Console.WriteLine(left + @" " + right + @" " + up + @" " + down);
            left = left - 5 >= 0 ? left - 5 : 0;
            //up = up - 5 >= 0 ? up - 5 : 0;
            var bmp = primeBitmap.Clone(new Rectangle(left, up, right - left, down - up), PixelFormat.DontCare);
            return bmp;
        }
        
    }
}
