using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using System.Windows.Forms;
using Microsoft.Win32;
using System.Drawing;

namespace YahtzRecogLicen
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap prime_bitmap;
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void btn_open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Jpeg文件(*.jpg)|*.jpg|Bitmap文件(*.bmp)|*.bmp|所有合适文件(*.bmp/*.jpg)|*.bmp;*.jpg";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                string file_name=openFileDialog.FileName;
                prime_bitmap = (Bitmap)Bitmap.FromFile(file_name, false);
                IntPtr intPtr = prime_bitmap.GetHbitmap();
                ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                img_load.Source = imageSource;
            }
        }
        private void show_pic()
        {
            IntPtr intPtr = prime_bitmap.GetHbitmap();
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            img_load.Source = imageSource;
        }
        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Jpeg文件(*.jpg)|*.jpg|Bitmap文件(*.bmp)|*.bmp|所有合适文件(*.bmp/*.jpg)|*.bmp;*.jpg";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "选择保存文件路径";
            if (saveFileDialog.ShowDialog() == true)
            {
                prime_bitmap.Save(saveFileDialog.FileName);
            }
        }


        private void btn_to_gray_Click(object sender, RoutedEventArgs e)
        {
            if (prime_bitmap == null)
            {
                MessageBox.Show("请选择图片");
                return;
            }
            
            for(int i = 0; i < prime_bitmap.Height; i++)
            {
                for(int j = 0; j < prime_bitmap.Width; j++)
                {
                    System.Drawing.Color color = prime_bitmap.GetPixel(j, i);
                    int r = color.R;
                    int g = color.G;
                    int b = color.B;
                    int x = (int)(.299 * r + .587 * g + .114 * b);
                    //int y = color.ToArgb();
                    //MessageBox.Show(y + "");
                    color = System.Drawing.Color.FromArgb(x,x,x);
                    prime_bitmap.SetPixel(j,i,color);
                    
                    //MessageBox.Show(color.ToString());
                }
            }

            show_pic();

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
            if (prime_bitmap == null)
            {
                MessageBox.Show("请选择图片");
                return;
            }
            int[] num_pixel = new int[256];
            for (int i = 0; i < prime_bitmap.Height; i++)
            {
                for (int j = 0; j < prime_bitmap.Width; j++)
                {
                    int x = prime_bitmap.GetPixel(j, i).R;
                    num_pixel[x]++;
                }
            }
            for (int i = 0; i < 256; i++)
            {
                Console.WriteLine(num_pixel[i]);
            }
            double[] prob_pixel = new double[256];
            for(int i = 0; i < 256; i++)
            {
                prob_pixel[i] = num_pixel[i] / (prime_bitmap.Height * prime_bitmap.Width * 1.0);
            }
            double[] cumu_pixel = new double[256];
            for(int i = 0; i < 256; i++)
            {
                if (i == 0)
                {
                    cumu_pixel[i] = prob_pixel[i];
                }
                else
                {
                    cumu_pixel[i] = cumu_pixel[i - 1] + prob_pixel[i];
                }
            }
            for(int i = 0; i < 256; i++)
            {
                cumu_pixel[i]=cumu_pixel[i] * 256 + 0.5;
                Console.WriteLine("pixel is :" + cumu_pixel[i]);
            }
            for(int i = 0; i < prime_bitmap.Height; i++)
            {
                for(int j = 0; j < prime_bitmap.Width; j++)
                {
                    int x = prime_bitmap.GetPixel(j, i).R;
                    int value = (int)cumu_pixel[x];
                    if (value > 255) value = 255;
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(value, value, value);
                    prime_bitmap.SetPixel(j, i, color);
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

        private int[,] get_submat(int s,int e,int k, int[,] xios)
        {
            int[,]submat= new int[k, k];
            int cnt_i=0;
            int cnt_j=0;
            for(int i = s; i < s + k; i++)
            {
                cnt_j = 0;
                for(int j = e; j < e + k; j++)
                {
                    if (cnt_j >= 3) break;
                    submat[cnt_i, cnt_j++] = xios[j, i];
                }
                cnt_i++;
            }
            return submat;
        }
        private int median(int[,] submat)
        {
            int x=0;
            int row = submat.GetLength(0);
            int col = submat.GetLength(1);
            int[] temp = new int[row * col];
            int cnt = 0;
            for(int i = 0; i < row; i++)
            {
                for(int j = 0; j < col; j++)
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

            int mideum = row * col / 2;
            if (row*col % 2==0)
            {               
                x = (int)(temp[mideum - 1] + temp[mideum]) / 2;
            }
            else
            {
                x = temp[mideum];
            }
            return x;
        }

        private int[,] get_bitmap_mat()
        {
            int height = prime_bitmap.Height;
            int width = prime_bitmap.Width;
            int[,] bitmap_mat = new int[width, height];
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int x = prime_bitmap.GetPixel(j, i).R;
                    bitmap_mat[j, i] = x;
                }
            }
            return bitmap_mat;
        }
        private void btn_wave_filter_Click(object sender, RoutedEventArgs e)
        {
            if (prime_bitmap == null)
            {
                MessageBox.Show("请选择图片");
                return;
            }
            int height = prime_bitmap.Height;
            int width = prime_bitmap.Width;
            int n = 3;
            int[,] bitmap_mat = get_bitmap_mat();
            for (int i = 0; i < height-n+1; i++)
            {
                for(int j = 0; j < width-n+1; j++)
                {
                    int[,] submat = get_submat(i, j,n, bitmap_mat);
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
                    int mid=median(submat);
                    //Console.WriteLine("子矩阵中值：" + mid);
                    bitmap_mat[j + (n - 1) / 2, i + (n - 1) / 2] = mid;
                    
                }
            }
            for(int i = 0; i < height; i++)
            {
                for(int j = 0; j < width; j++)
                {
                    int value = bitmap_mat[j, i];
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(value, value, value);
                    prime_bitmap.SetPixel(j, i, color);
                }
            
            }
            show_pic();
        }

        private int get_sobel_value(int row,int col, int[,] sobel_bitmap_mat)
        {
            int sobel_value = -1;
            int[,] sobel =
            {
                {-1,-2,-1 },
                {0,0,0 },
                {1,2,1 }
            };
            int[,] sub_mat = new int[3, 3];
            int row_cnt = 0;
            int col_cnt = 0;
            for(int i = row; i <row+3; i++)
            {
                col_cnt = 0;
                for(int j =col; j < col+3; j++)
                {
                    sub_mat[row_cnt, col_cnt++] = sobel_bitmap_mat[i, j];
                }
                row_cnt++;
            }
            sobel_value = Math.Abs(sub_mat[0, 0] + 2 * sub_mat[0, 1] + sub_mat[0, 2] - sub_mat[2, 0] - 2 * sub_mat[2, 1] - sub_mat[2, 2]) + Math.Abs(sub_mat[0, 2] + 2 * sub_mat[1, 2] + sub_mat[2, 2] - sub_mat[0, 0] - 2 * sub_mat[1, 0] - sub_mat[2, 0]);
            return sobel_value;
        }
        private void set_prime_bitmap(int [,] bitmap_mat)
        {
            for (int i = 0; i < prime_bitmap.Height; i++)
            {
                for (int j = 0; j < prime_bitmap.Width; j++)
                {
                    System.Drawing.Color color;
                    int x = bitmap_mat[j, i];
                    //int y = color.ToArgb();
                    //MessageBox.Show(y + "");
                    if (x > 255) x = 255;
                    color = System.Drawing.Color.FromArgb(x, x, x);
                    prime_bitmap.SetPixel(j, i, color);

                    //MessageBox.Show(color.ToString());
                }
            }
        }
        
        private void btn_edge_detect_Click(object sender, RoutedEventArgs e)
        {
            int[,] bitmap_mat = get_bitmap_mat();
            int height = bitmap_mat.GetLength(0);
            int width = bitmap_mat.GetLength(1);
            int[,] sobel_bitmap_mat = new int[height + 2, width + 2];
            for (int i = 0; i < height+2; i++)
            {
                for (int j = 0; j < width+2; j++)
                {
                    if (i == 0 || i == height + 1)
                    {
                        sobel_bitmap_mat[i, j] = 0;
                    }else if (j == 0 || j == width + 1)
                    {
                        sobel_bitmap_mat[i, j] = 0;
                    }
                    else
                    {
                        sobel_bitmap_mat[i, j] = bitmap_mat[i - 1,j - 1];
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
            for(int i = 0; i < height; i++)
            {
                for(int j = 0; j < width; j++)
                {
                    int sobel_value = get_sobel_value(i, j, sobel_bitmap_mat);
                    bitmap_mat[i, j] = sobel_value;
                }
            }
            set_prime_bitmap(bitmap_mat);
            show_pic();
        }
    }
}
