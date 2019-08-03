using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for BrailleWindow.xaml
    /// </summary>
    public partial class BrailleWindow : Window
    {
        public BrailleWindow()
        {
            InitializeComponent();
        }

        private void SetImage(string imagePath)
        {
            var og = new Bitmap(imagePath);
            var bitmap =
                new Bitmap(
                    og,
                    Math.Min(og.Width, int.TryParse(WidthTB.Text, out int width) ? width : int.MaxValue) / 2 * 2,
                    Math.Min(og.Height, int.TryParse(HeightTB.Text, out int height) ? height : int.MaxValue) / 4 * 4);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    bitmap.SetPixel(x, y, bitmap.GetPixel(x, y).GetBrightness() > 0.5 ?
                        System.Drawing.Color.White :
                        System.Drawing.Color.Black);
                }
            }

            char[][] charArr = GetCharArr(bitmap);
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < charArr.Length; y++)
            {
                sb.Append(new string(charArr[y]));
                sb.Append(Environment.NewLine);
            }
            new TextWindow(sb.ToString()).Show();
            Dispatcher.Invoke(() =>
            {
                Img.Source = ToWpfBitmap(bitmap);
            });
        }

        private char[][] GetCharArr(Bitmap bitmap)
        {
            char[][] ret = new char[bitmap.Height / 4][];
            for (int retY = 0; retY < ret.Length; retY++)
            {
                ret[retY] = new char[bitmap.Width / 2];
                for (int retX = 0; retX < ret[retY].Length; retX++)
                {
                    ret[retY][retX] = '\u2800';
                    if (IsBlack(retX, retY, 0, 0, bitmap))
                    {
                        ret[retY][retX]++;
                    }

                    if (IsBlack(retX, retY, 0, 1, bitmap))
                    {
                        ret[retY][retX] += (char)2;
                    }

                    if (IsBlack(retX, retY, 0, 2, bitmap))
                    {
                        ret[retY][retX] += (char)4;
                    }

                    if (IsBlack(retX, retY, 0, 3, bitmap))
                    {
                        ret[retY][retX] += (char)8;
                    }

                    if (IsBlack(retX, retY, 1, 0, bitmap))
                    {
                        ret[retY][retX] += (char)16;
                    }

                    if (IsBlack(retX, retY, 1, 1, bitmap))
                    {
                        ret[retY][retX] += (char)32;
                    }

                    if (IsBlack(retX, retY, 1, 2, bitmap))
                    {
                        ret[retY][retX] += (char)64;
                    }

                    if (IsBlack(retX, retY, 1, 3, bitmap))
                    {
                        ret[retY][retX] += (char)128;
                    }
                }
            }

            return ret;
        }

        private bool IsBlack(int retX, int retY, int x, int y, Bitmap bitmap)
        {
            System.Drawing.Color col = bitmap.GetPixel(retX * 2 + x, retY * 4 + y);
            return col.ToArgb() == System.Drawing.Color.Black.ToArgb();
        }

        public static BitmapSource ToWpfBitmap(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        public string FileName { get; set; }
        private void SelectPic_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".png",
                Filter = "(*.jpeg;*.png;*.jpg;*.bmp)|*.jpeg;*.png;*.jpg;*.bmp"
            };
            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                FileName = dlg.FileName;
            }
        }

        private void BraillePic_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                SetImage(FileName);
            }
        }
    }
}
