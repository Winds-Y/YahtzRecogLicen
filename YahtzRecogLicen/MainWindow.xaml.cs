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
    }
}
