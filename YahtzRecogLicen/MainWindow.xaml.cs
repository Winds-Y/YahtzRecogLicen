using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
//using System.Windows.Forms;
using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Linq;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;


namespace YahtzRecogLicen
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private Bitmap _primeBitmap;
        private int _bitmapHeight;
        private int _bitmapWidth;
        private Bitmap _grayBitmap;
        private Bitmap _bitmapLicence;
        private Bitmap smallBitmap;
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void btn_open_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Jpeg文件(*.jpg)|*.jpg|Bitmap文件(*.bmp)|*.bmp|所有合适文件(*.bmp/*.jpg)|*.bmp;*.jpg",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName=openFileDialog.FileName;
                var tempBitmap= (Bitmap)Image.FromFile(fileName, false);
                _primeBitmap = tempBitmap.Clone(new Rectangle(0, 0, tempBitmap.Width, tempBitmap.Height), PixelFormat.DontCare);
                var intPtr = _primeBitmap.GetHbitmap();
                ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                img_load.Source = imageSource;
            }
            _bitmapHeight = _primeBitmap.Height;
            _bitmapWidth = _primeBitmap.Width;
        }
        private void show_pic()
        {
            var intPtr = _primeBitmap.GetHbitmap();
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            img_load.Source = imageSource;
        }
        
        private void show_pic(Bitmap currentBitmap)
        {
            var intPtr = currentBitmap.GetHbitmap();
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            img_load.Source = imageSource;
        }
        private void t_show_pic(Bitmap currentBitmap)
        {
            var intPtr = currentBitmap.GetHbitmap();
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            image_t.Source = imageSource;
        }
        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Jpeg文件(*.jpg)|*.jpg|Bitmap文件(*.bmp)|*.bmp|所有合适文件(*.bmp/*.jpg)|*.bmp;*.jpg",
                FilterIndex = 0,
                RestoreDirectory = true,
                Title = "选择保存文件路径"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                _grayBitmap.Save(saveFileDialog.FileName);
            }
        }


        private void btn_to_gray_Click(object sender, RoutedEventArgs e)
        {
            if (_primeBitmap == null)
            {
                MessageBox.Show("请选择图片");
                return;
            }
            _grayBitmap = _primeBitmap.Clone(new Rectangle(0, 0, _primeBitmap.Width, _primeBitmap.Height), PixelFormat.DontCare);
            for (var i = 0; i < _primeBitmap.Height; i++)
            {
                for(var j = 0; j < _primeBitmap.Width; j++)
                {
                    var color = _primeBitmap.GetPixel(j, i);
                    int r = color.R;
                    int g = color.G;
                    int b = color.B;
                    var x = (byte)(.299 * r + .587 * g + .114 * b);
                    //int y = color.ToArgb();
                    //MessageBox.Show(y + "");
                    color = Color.FromArgb(x,x,x);
                    _grayBitmap.SetPixel(j,i,color);
                    
                    //MessageBox.Show(color.ToString());
                }
            }
            
            show_pic(_grayBitmap);

            //for (int i = 0; i < prime_bitmap.Height; i++)
            //{
            //    for (int j = 0; j < prime_bitmap.Width; j++)
            //    {
            //        System.Drawing.Color color = prime_bitmap.GetPixel(j, i);
            //        MessageBox.Show(color.ToString());
            //    }
            //}
        }


        private void btn_equalization_Click(object sender, RoutedEventArgs e)
        {
            if (_primeBitmap == null)
            {
                MessageBox.Show("请选择图片");
                return;
            }
            var numPixel = new int[256];
            for (var i = 0; i < _primeBitmap.Height; i++)
            {
                for (var j = 0; j < _primeBitmap.Width; j++)
                {
                    int x = _primeBitmap.GetPixel(j, i).R;
                    numPixel[x]++;
                }
            }
            for (var i = 0; i < 256; i++)
            {
                Console.WriteLine(numPixel[i]);
            }
            var probPixel = new double[256];
            for(var i = 0; i < 256; i++)
            {
                probPixel[i] = numPixel[i] / (_primeBitmap.Height * _primeBitmap.Width * 1.0);
            }
            var cumuPixel = new double[256];
            for(var i = 0; i < 256; i++)
            {
                if (i == 0)
                {
                    cumuPixel[i] = probPixel[i];
                }
                else
                {
                    cumuPixel[i] = cumuPixel[i - 1] + probPixel[i];
                }
            }
            for(var i = 0; i < 256; i++)
            {
                cumuPixel[i]=cumuPixel[i] * 256 + 0.5;
                Console.WriteLine(@"pixel is :" + cumuPixel[i]);
            }
            for(var i = 0; i < _primeBitmap.Height; i++)
            {
                for(var j = 0; j < _primeBitmap.Width; j++)
                {
                    int x = _primeBitmap.GetPixel(j, i).R;
                    var value = (int)cumuPixel[x];
                    if (value > 255) value = 255;
                    var color = Color.FromArgb(value, value, value);
                    _primeBitmap.SetPixel(j, i, color);
                }
            }
            //for (int i = 0; i < prime_bitmap.Height; i++)
            //{
            //    for (int j = 0; j < prime_bitmap.Width; j++)
            //    {
            //        System.Drawing.Color color = prime_bitmap.GetPixel(j, i);
            //        MessageBox.Show(color.ToString());
            //    }
            //}
            show_pic();
        }

        private static int[,] get_submat(int s,int e,int k, int[,] xios)
        {
            var submat= new int[k, k];
            var cntI=0;
            for(var i = s; i < s + k; i++)
            {
                var cntJ=0;
                for(var j = e; j < e + k; j++)
                {
                    if (cntJ >= 3) break;
                    submat[cntI, cntJ++] = xios[j, i];
                }
                cntI++;
            }
            return submat;
        }
        private static int Median(int[,] submat)
        {
            int x;
            var row = submat.GetLength(0);
            var col = submat.GetLength(1);
            var temp = new int[row * col];
            var cnt = 0;
            for(var i = 0; i < row; i++)
            {
                for(var j = 0; j < col; j++)
                {
                    temp[cnt++] = submat[i, j];

                }
            }
            Array.Sort(temp);

            //Console.WriteLine("取中位数一维数组如下：");
            //for(int i = 0; i < temp.Length; i++)
            //{
            //    Console.Write(temp[i] + " ");
            //}Console.WriteLine();

            var mideum = row * col / 2;
            if (row*col % 2==0)
            {               
                x = (temp[mideum - 1] + temp[mideum]) / 2;
            }
            else
            {
                x = temp[mideum];
            }
            return x;
        }

        private int[,] get_bitmap_mat()
        {
            var height = _primeBitmap.Height;
            var width = _primeBitmap.Width;
            var bitmapMat = new int[width, height];
            
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    int x = _grayBitmap.GetPixel(j, i).R;
                    bitmapMat[j, i] = x;
                }
            }
            return bitmapMat;
        }
        private void btn_wave_filter_Click(object sender, RoutedEventArgs e)
        {
            if (_primeBitmap == null)
            {
                MessageBox.Show("请选择图片");
                return;
            }
            var height = _primeBitmap.Height;
            var width = _primeBitmap.Width;
            const int n = 3;
            var bitmapMat = get_bitmap_mat();
            for (var i = 0; i < height-n+1; i++)
            {
                for(var j = 0; j < width-n+1; j++)
                {
                    var submat = get_submat(i, j,n, bitmapMat);
                    //Console.WriteLine("子矩阵如下：");
                    //Console.WriteLine("行：" + submat.GetLength(0) + "列：" + submat.GetLength(1));
                    //for(int m = 0; m < submat.GetLength(0);m++)
                    //{
                    //    for(int l = 0; l < submat.GetLength(1); l++)
                    //    {
                    //        Console.Write(submat[m, l] + " ");
                    //    }
                    //    Console.WriteLine();
                    //}
                    var mid=Median(submat);
                    //Console.WriteLine("子矩阵中值：" + mid);
                    bitmapMat[j + (n - 1) / 2, i + (n - 1) / 2] = mid;
                    
                }
            }
            for(var i = 0; i < height; i++)
            {
                for(var j = 0; j < width; j++)
                {
                    var value = bitmapMat[j, i];
                    var color = Color.FromArgb(value, value, value);
                    _primeBitmap.SetPixel(j, i, color);
                }
            
            }
            show_pic();
        }

        private static int get_sobel_value(int row,int col, int[,] sobelBitmapMat)
        {
            /*int[,] sobel =
            {
                {-1,-2,-1 },
                {0,0,0 },
                {1,2,1 }
            };*/
            var subMat = new int[3, 3];
            var rowCnt = 0;
            for(var i = row; i <row+3; i++)
            {
                var colCnt = 0;
                for(var j =col; j < col+3; j++)
                {
                    subMat[rowCnt, colCnt++] = sobelBitmapMat[i, j];
                }
                rowCnt++;
            }
            var sobelValue = Math.Abs(subMat[0, 0] + 2 * subMat[0, 1] + subMat[0, 2] - subMat[2, 0] - 2 * subMat[2, 1] - subMat[2, 2]) + Math.Abs(subMat[0, 2] + 2 * subMat[1, 2] + subMat[2, 2] - subMat[0, 0] - 2 * subMat[1, 0] - subMat[2, 0]);
            return sobelValue;
        }
        private void set_prime_bitmap(int [,] bitmapMat)
        {
            for (var i = 0; i < _primeBitmap.Height; i++)
            {
                for (var j = 0; j < _primeBitmap.Width; j++)
                {
                    var x = bitmapMat[j, i];
                    //int y = color.ToArgb();
                    //MessageBox.Show(y + "");
                    if (x > 255) x = 255;
                    var color = Color.FromArgb(x, x, x);
                    _grayBitmap.SetPixel(j, i, color);

                    //MessageBox.Show(color.ToString());
                }
            }
        }
        
        private void btn_edge_detect_Click(object sender, RoutedEventArgs e)
        {
            var bitmapMat = get_bitmap_mat();
            var height = bitmapMat.GetLength(0);
            var width = bitmapMat.GetLength(1);
            var sobelBitmapMat = new int[height + 2, width + 2];
            for (var i = 0; i < height+2; i++)
            {
                for (var j = 0; j < width+2; j++)
                {
                    if (i == 0 || i == height + 1)
                    {
                        sobelBitmapMat[i, j] = 0;
                    }else if (j == 0 || j == width + 1)
                    {
                        sobelBitmapMat[i, j] = 0;
                    }
                    else
                    {
                        sobelBitmapMat[i, j] = bitmapMat[i - 1,j - 1];
                    }
                }
            }
            //for(int i = 0; i < height + 2; i++)
            //{
            //    for(int j = 0; j < width + 2; j++)
            //    {
            //        Console.Write(sobel_bitmap_mat[i, j]+" ");
            //    }
            //    Console.WriteLine();
            //}
            for(var i = 0; i < height; i++)
            {
                for(var j = 0; j < width; j++)
                {
                    var sobelValue = get_sobel_value(i, j, sobelBitmapMat);
                    bitmapMat[i, j] = sobelValue;
                }
            }
            set_prime_bitmap(bitmapMat);
            show_pic(_grayBitmap);
        }
        
        //average to get binarization
        /*private void bitmap_binarization()
        {
            var average = 0;
            var width = _primeBitmap.Width;
            var height = _primeBitmap.Height;
            for(var i = 0; i <width; i++)
            {
                for(var j = 0; j < height; j++)
                {
                    var color = _primeBitmap.GetPixel(i, j);
                    average += color.B;
                }
            }
            average = average / (width * height);
            for(var i = 0; i < width; i++)
            {
                for(var j = 0; j < height; j++)
                {
                    var color = _primeBitmap.GetPixel(i, j);
                    int x = color.B;
                    var value = x > average ? 255 : 0;
                    color = Color.FromArgb(value, value, value);
                    _primeBitmap.SetPixel(i, j, color);
                }
            }
            show_pic();
        }*/

        private void output_bitmap()
        {
            const string fileName = "output\\bitmaps.txt";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            var streamWriter = new StreamWriter(fileName, true);
            for (var j = 0; j < _bitmapHeight; j++)
            {               
                for (var i = 0; i < _bitmapWidth; i++)
                {
                    var b = _grayBitmap.GetPixel(i, j).B;
                    streamWriter.Write(b + " ");
                }
                streamWriter.WriteLine();
            }
            streamWriter.Flush();
            streamWriter.Close();
            streamWriter.Dispose();
        }
        
        private void output_bitmap(Bitmap bitmap)
        {
            const string fileName = "output\\bitmaps.txt";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            var streamWriter = new StreamWriter(fileName, true);
            for (var i= 0; i < bitmap.Width; i++)
            {               
                for (var j = 0; j < bitmap.Height; j++)
                {
                    var b = bitmap.GetPixel(i, j).B;
                    streamWriter.Write(b + " ");
                }
                streamWriter.WriteLine();
            }
            streamWriter.Flush();
            streamWriter.Close();
            streamWriter.Dispose();
        }
        

        //三分点法，取灰度范围的三分之二点
        private void binarization_class()
        {
            int max = 0, min = 0;
            for(var i = 0; i < _bitmapWidth; i++)
            {
                for(var j = 0; j < _bitmapHeight; j++)
                {
                    int value = _grayBitmap.GetPixel(i, j).R;
                    max = max < value ? value : max;
                    min = min > value ? value : min;
                }
            }
            var t = max - (max - min) / 3;
            Console.WriteLine(@"阈值：" + t);
            for(var i = 0; i < _bitmapWidth; i++)
            {
                for(var j = 0; j < _bitmapHeight; j++)
                {
                    int value = _grayBitmap.GetPixel(i, j).R;
                    var n = value > t ? 255 : 0;
                    _grayBitmap.SetPixel(i, j, Color.FromArgb(n, n, n));
                }
            }
            show_pic(_grayBitmap);
            
        }

        private void btn_binarization_Click(object sender, RoutedEventArgs e)
        {
            binarization_class();
        }

        private void btn_stretch_Click(object sender, RoutedEventArgs e)
        {
            int max = 0, min = 0;
            for (var i = 0; i < _bitmapWidth; i++)
            {
                for (var j = 0; j < _bitmapHeight; j++)
                {
                    int value = _primeBitmap.GetPixel(i, j).R;
                    max = max < value ? value : max;
                    min = min > value ? value : min;
                }
            }
            Console.WriteLine(@"max is " + max + @"min is: " + min);
            for(var i = 0; i < _bitmapWidth; i++)
            {
                for(var j = 0; j < _bitmapHeight; j++)
                {
                    int value = _primeBitmap.GetPixel(i, j).R;
                    var x = 255 / (max - min) * value - 255 * min / (max - min);
                    _primeBitmap.SetPixel(i, j, Color.FromArgb(x, x, x));
                }
            }
            show_pic();
        }

        private void btn_imopen_Click(object sender, RoutedEventArgs e)
        {
            PositioningLicence.SetBitmap(_grayBitmap);
            PositioningLicence.gray_corrosion();
            PositioningLicence.gray_dilate();         
            show_pic(_grayBitmap);
            output_bitmap();
        }

        private void btn_enhance_Click(object sender, RoutedEventArgs e)
        {
            PositioningLicence.Imsubtract();
            show_pic(_grayBitmap);
        }

        private void imclose_f_Click(object sender, RoutedEventArgs e)
        {
            if (PositioningLicence.GetBitmap() == null)
            {
                MessageBox.Show("请设置Position_Licence.bitmap");
            }
            PositioningLicence.dalite_SE(5, 19);
            PositioningLicence.corrosion_SE(5, 19);
            show_pic(_grayBitmap);
        }

        private void imopen_s_Click(object sender, RoutedEventArgs e)
        {
            if (PositioningLicence.GetBitmap() == null)
            {
                MessageBox.Show("请设置Position_Licence.bitmap");
            }
            PositioningLicence.corrosion_SE(5, 19);
            PositioningLicence.dalite_SE(5, 19);
            PositioningLicence.corrosion_SE(11, 5);
            PositioningLicence.dalite_SE(11, 5);
            show_pic(_grayBitmap);
        }

       /*private void bwareaopen_Click(object sender, RoutedEventArgs e)
        {
            if (PositioningLicence.GetBitmap() == null)
            {
                MessageBox.Show("请设置Position_Licence.bitmap");
            }
            PositioningLicence.ThiningPic(10);
            show_pic(_grayBitmap);
        }*/

        private void btn_positioning_licence_Click(object sender, RoutedEventArgs e)
        {
            var bitmapPlant = PositioningLicence.find_plant(_primeBitmap);
            _bitmapLicence = bitmapPlant.Clone(new Rectangle(0, 0, bitmapPlant.Width, bitmapPlant.Height),
                PixelFormat.DontCare);
            
            t_show_pic(bitmapPlant);
        }

        private void PlantGray_OnClick(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < _bitmapLicence.Width; i++)
            {
                for (var j = 0; j < _bitmapLicence.Height; j++)
                {
                    var r = _bitmapLicence.GetPixel(i, j).R;
                    var g = _bitmapLicence.GetPixel(i, j).G;
                    var b = _bitmapLicence.GetPixel(i, j).B;
                    var x =(int)(.299 * r + .587 * g + .114 * b);
                    _bitmapLicence.SetPixel(i,j,Color.FromArgb(x,x,x));            
                }
            }
            t_show_pic(_bitmapLicence);
        }

        private void Plant_binary_OnClick(object sender, RoutedEventArgs e)
        {
            const int n = 256;
            var plantPixelCount=new int[n];
            for (var i = 0; i < _bitmapLicence.Width; i++)
            {
                for (var j = 0; j < _bitmapLicence.Height; j++)
                {
                    var x = _bitmapLicence.GetPixel(i, j).R;
                    plantPixelCount[x]++;
                }
            }

            var totalValue = plantPixelCount.Select((t, i) => i * t).Sum();

            //MessageBox.Show("totalValue is :" + totalValue+Environment.NewLine);

            PositioningLicence.WriteToFile("totalValue is :"+totalValue+Environment.NewLine);
            double sum0 = 0;
            var w0 = 0;
            double maximun = 0;
            var total = _bitmapLicence.Width * _bitmapLicence.Height;
            byte level = 0;
            for (byte i = 0; i < plantPixelCount.Length; i++)
            {
                w0 += plantPixelCount[i];
                if(w0==0)
                    continue;
                var w1 = total - w0;
                if(w1==0)
                    break;
                sum0 += i * plantPixelCount[i];
                var m0 = sum0 / w0;         
                var m1 = (totalValue - sum0) / w1;
                var icv = w0 * w1 * (m0 - m1) * (m0 - m1);
                if (!(icv >= maximun)) continue;
                level = i;
                maximun = icv;
                PositioningLicence.WriteToFile(m0+" "+m1+" "+icv+" "+Environment.NewLine);

            }

            //MessageBox.Show(@"level is: "+level);
            PositioningLicence.WriteToFile(@"level is: "+level+Environment.NewLine);
            for (var i = 0; i < _bitmapLicence.Width; i++)
            {
                for (var j = 0; j < _bitmapLicence.Height; j++)
                {
                    int x = _bitmapLicence.GetPixel(i, j).R;
                    var value = x > level ? 255 : 0;
                    _bitmapLicence.SetPixel(i,j,Color.FromArgb(value,value,value));
                }
            }
            t_show_pic(_bitmapLicence);
            var cnt = 0;
            var str = "";
            foreach (var b in plantPixelCount)
            {
                Console.Write(b+@" ");
                str += b + " ";
                if (cnt++ % 20 ==0 &&cnt-1!=0)
                    str += Environment.NewLine;
                    
            }
            PositioningLicence.WriteToFile(str);
        }


        private void SavePlant_OnClick(object sender, RoutedEventArgs e)
        {
            if (_bitmapLicence == null)
            {
                MessageBox.Show("the licence is null");
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Jpeg文件(*.jpg)|*.jpg|Bitmap文件(*.bmp)|*.bmp|所有合适文件(*.bmp/*.jpg)|*.bmp;*.jpg",
                FilterIndex = 0,
                RestoreDirectory = true,
                Title = "选择保存文件路径"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                _bitmapLicence.Save(saveFileDialog.FileName);
            }
        }

        private static void SetImages(System.Windows.Controls.Image image, Bitmap bitmap)
        {
            var intPtr = bitmap.GetHbitmap();
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            image.Source = imageSource;
        }

        private void CutPlant_OnClick(object sender, RoutedEventArgs e)
        {
            output_bitmap(_bitmapLicence);
            var rowSum = new int[_bitmapLicence.Height];       

            for (var j = 0; j < _bitmapLicence.Height; j++)
            {
                var sum = 0;
                for (var i = 0; i < _bitmapLicence.Width; i++)
                {
                    int x = _bitmapLicence.GetPixel(i, j).R;
                    x = x == 0 ? 0 : 1;
                    sum += x;
                }
                rowSum[j] = sum;
            }
            int py0 = 0, py1=_bitmapLicence.Width-1;
            for (var i = 0; i < _bitmapLicence.Height; i++)
            {
                var rate =(double) rowSum[i] / _bitmapLicence.Width*1.0;
                if (rate > 0.3 && rate < 0.6)
                    break;
                py0 = i;
            }

            for (var i = _bitmapLicence.Height-1; i>=0; i--)
            {
                var rate = (double) rowSum[i] / _bitmapLicence.Width * 1.0;
                if (rate > 0.3 && rate < 0.6)
                    break;
                py1 = i;
            }
            PositioningLicence.WriteToFile("Py0 is:"+py0+Environment.NewLine+"Py1 is:"+py1+Environment.NewLine);
            PositioningLicence.WriteToFile("height is:"+_bitmapLicence.Height+Environment.NewLine);
            
            var colSum = new int[_bitmapLicence.Width];
            for (var i = 0; i < _bitmapLicence.Width; i++)
            {
                var sum = 0;
                for (var j = py0; j<= py1; j++)
                {
                    int x = _bitmapLicence.GetPixel(i, j).R;
                    x = x == 0 ? 0 : 1;
                    sum += x;
                }
                colSum[i] = sum;
            }

            var x0 = 0;
            var x1 = _bitmapLicence.Width-1;
            while (x0 < _bitmapLicence.Height && (colSum[x0] == 0 || colSum[x0] == _bitmapLicence.Height))
            {
                x0++;
            }
            while (x0 < _bitmapLicence.Height && (colSum[x0] == 0 || colSum[x0] == py1 - py0))
            {
                x0++;
            }

            while (x1>=0 &&(colSum[x1]==0 || colSum[x1]==_bitmapLicence.Height))
            {
                x1--;
            }
            while (x1>=0 &&(colSum[x1]==0 || colSum[x1]==py1 - py0))
            {
                x1--;
            }
            PositioningLicence.WriteToFile("x0 is:"+x0+Environment.NewLine+"x1 is:"+x1+Environment.NewLine);
            PositioningLicence.WriteToFile("width is:"+_bitmapLicence.Width+Environment.NewLine);
            
            smallBitmap = _bitmapLicence.Clone(new Rectangle(x0, py0, x1 - x0, py1 - py0), PixelFormat.DontCare);
            t_show_pic(smallBitmap);

            var subColSum = new int[smallBitmap.Width];
            for (var i = 0; i < smallBitmap.Width; i++)
            {
                var sum = 0;
                for (var j = 0; j < smallBitmap.Height; j++)
                {
                    int x = smallBitmap.GetPixel(i, j).R;
                    x = x == 0 ? 0 : 1;
                    sum += x;
                }
                subColSum[i] = sum;
            }

            var len = subColSum.Count(t => t == 0);
            var mark = new int[len];
            var ct = 0;
            for (var i = 0; i < smallBitmap.Width; i++)
            {
                if (subColSum[i] == 0)
                {
                    mark[ct++] = i;                   
                }
            }

            var c=0;
            var list = new List<List<int>>();
            while (c < len)
            {
                var j = c + 1;
                while (j < len)
                {
                    if (mark[j] - mark[j - 1] == 1)
                    {
                        j++;
                    }
                    else
                    {
                        break;
                    }
                }
                var li=new List<int>{mark[c],mark[j-1]};
                list.Add(li);
                c = j;
            }

            len = smallBitmap.Width;
            var templist = new List<int>();
            foreach (var lir in list)
            {
                foreach (var i in lir)
                {
                    templist.Add(i);
                }
            }
            templist.Insert(0,1);
            templist.Add(len);
            var charsub=new List<List<int>>();
            for (var i = 0; i < templist.Count-1; i += 2)
            {
                charsub.Add(new List<int>{templist[i]-1,templist[i+1]-1});
            }
            var bitmaplist=new List<Bitmap>();
            //MessageBox.Show(smallBitmap.Width + "  " + smallBitmap.Height);
            //var tpt = smallBitmap.Clone(new Rectangle(0, 4, smallBitmap.Width, smallBitmap.Height), PixelFormat.DontCare);
            foreach (var fir in charsub)
            {
                var left = fir[0];
                var right = fir[1];
                var tempbitmap = smallBitmap.Clone(new Rectangle(left, 0, right - left, smallBitmap.Height),
                    PixelFormat.DontCare);
                bitmaplist.Add(tempbitmap);
            }

            var cnt = 0;
            foreach (var image in images.Children.OfType<System.Windows.Controls.Image>())
            {
                SetImages(image,bitmaplist[cnt++]);
                if(cnt>bitmaplist.Count)
                    break;
            }
        }

        private void OpenPlant_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Jpeg文件(*.jpg)|*.jpg|Bitmap文件(*.bmp)|*.bmp|所有合适文件(*.bmp/*.jpg)|*.bmp;*.jpg",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() != true) return;
            var fileName=openFileDialog.FileName;
            var tempBitmap= (Bitmap)Image.FromFile(fileName, false);
            _bitmapLicence = tempBitmap.Clone(new Rectangle(0, 0, tempBitmap.Width, tempBitmap.Height), PixelFormat.DontCare);
            t_show_pic(_bitmapLicence);
        }
    }
}
