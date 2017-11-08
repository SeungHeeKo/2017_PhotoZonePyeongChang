using Emgu.CV;
using Emgu.CV.Structure;
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
using System.Windows.Shapes;
using PhotoZonePyeongchang.OpenCV;

namespace PhotoZonePyeongchang
{
    /// <summary>
    /// ControlPanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ControlPanel : Window
    {
        public Image<Bgr, byte> originalImage;
        Chromakey chromakey = new Chromakey();
        ImageConverter imageConverter = new ImageConverter();

        public ControlPanel()
        {
            InitializeComponent();
            setRedValue_Low(0);
            setRedValue_High(190);
            setGreenValue_Low(155);
            setGreenValue_High(255);
            setBlueValue_Low(0);
            setBlueValue_High(130);

            if (backgroundImage == null || bgImg == null)
                LoadResourceImage();

        }

        Image<Bgr, byte> bgImg;
        BitmapImage backgroundImage;

        public void SetCaptureImage(Image<Bgr, byte> frame)
        {
            if (backgroundImage == null || bgImg == null)
                LoadResourceImage();
            originalImage = frame.Clone();
            SetChromakeyColor(getRedValue_Low(), getRedValue_High(), getGreenValue_Low(), getGreenValue_High(), getBlueValue_Low(), getBlueValue_High());
            applyChromakey(bgImg, originalImage);
            img_capture.Source = imageConverter.BitmapToImageSource(dst.Bitmap);

        }

        private void LoadResourceImage()
        {
            backgroundImage = new BitmapImage(new Uri("Resources/bg_1920.jpeg", UriKind.Relative));
            bgImg = new Image<Bgr, byte>(imageConverter.BitmapImage2Bitmap(backgroundImage));
        }
        public byte getRedValue_Low()
        {
            return (byte)slColorR_L.Value;
        }
        public byte getRedValue_High()
        {
            return (byte)slColorR_H.Value;
        }
        public byte getGreenValue_Low()
        {
            return (byte)slColorG_L.Value;
        }
        public byte getGreenValue_High()
        {
            return (byte)slColorG_H.Value;
        }
        public byte getBlueValue_Low()
        {
            return (byte)slColorB_L.Value;
        }
        public byte getBlueValue_High()
        {
            return (byte)slColorB_H.Value;
        }

        public void setRedValue_Low(double value)
        {
            slColorR_L.Value = value;
        }
        public void setRedValue_High(double value)
        {
            slColorR_H.Value = value;
        }
        public void setGreenValue_Low(double value)
        {
            slColorG_L.Value = value;
        }
        public void setGreenValue_High(double value)
        {
            slColorG_H.Value = value;
        }
        public void setBlueValue_Low(double value)
        {
            slColorB_L.Value = value;
        }
        public void setBlueValue_High(double value)
        {
            slColorB_H.Value = value;
        }
        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Color color = Color.FromRgb(getRedValue_Low(), getGreenValue_Low(), getBlueValue_Low());
            setRedValue_Low(slColorR_L.Value);
            setRedValue_High(slColorR_H.Value);
            setGreenValue_Low(slColorG_L.Value);
            setGreenValue_High(slColorG_H.Value);
            setBlueValue_Low(slColorB_L.Value);
            setBlueValue_High(slColorB_H.Value);
            ColorControlPanel.Background = new SolidColorBrush(color);
            
            SetChromakeyColor(getRedValue_Low(), getRedValue_High(), getGreenValue_Low(), getGreenValue_High(), getBlueValue_Low(), getBlueValue_High());

            if (backgroundImage == null || bgImg == null)
                LoadResourceImage();
            if(originalImage == null)
                return;
            applyChromakey(bgImg, originalImage);
            img_capture.Source = imageConverter.BitmapToImageSource(dst.Bitmap);
        }






        const int red_low_max = 255;
        const int red_high_max = 255;
        int red_low, red_high;
        double red_l, red_h;

        const int green_low_max = 255;
        const int green_high_max = 255;
        int green_low, green_high;
        double green_l, green_h;

        const int blue_low_max = 255;
        const int blue_high_max = 255;
        int blue_low, blue_high;

        private void img_capture_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(img_capture);
            int y = Convert.ToInt32(pos.Y);
            int x = Convert.ToInt32(pos.X);
            
            textBox_Pixel.Text = "R : " + originalImage.Data[y, x, 2].ToString();
            textBox_Pixel.Text += "\nG : " + originalImage.Data[y, x, 1].ToString();
            textBox_Pixel.Text += "\nB : " + originalImage.Data[y, x, 0].ToString();
        }

        double blue_l, blue_h;

        public Image<Bgr, byte> dst;

        public void SetChromakeyColor(int redLow, int redHigh, int greenLow, int greenHigh, int blueLow, int blueHigh)
        {
            red_l = redLow;
            red_h = redHigh;

            green_l = greenLow;
            green_h = greenHigh;

            blue_l = blueLow;
            blue_h = blueHigh;

        }
        public void applyChromakey(Image<Bgr, byte> background, Image<Bgr, byte> original)   // original : webcam
        {

            dst = new Image<Bgr, byte>(background.Size);

            // chromakey_mask
            Image<Gray, byte> mask = new Image<Gray, byte>(original.Size);   // Scalar(100, 200, 185)    , OpenCvSharp.Scalar color

            for (int y = 0; y < original.Rows; y++)
            {
                for (int x = 0; x < original.Cols; x++)
                {
                    ////cout << "R : " << over.at<Vec3b>(y, x)[0] << "  G : " << over.at<Vec3b>(y, x)[1] << "  B : " << over.at<Vec3b>(y, x)[2] << std::endl;
                    if (original.Data[y, x, 2] >= red_l && original.Data[y, x, 2] <= red_h && original.Data[y, x, 1] >= green_l && original.Data[y, x, 1] <= green_h && original.Data[y, x, 0] >= blue_l && original.Data[y, x, 0] <= blue_h)
                    {
                        mask.Data[y, x, 0] = 0;
                    }
                    else
                    {
                        mask.Data[y, x, 0] = 255;
                    }
                }
            }
            mask.Erode(2);

            for (int y = 0; y < background.Rows; y++)
            {
                for (int x = 0; x < background.Cols; x++)
                {
                    ////cout << "R : " << over.at<Vec3b>(y, x)[0] << "  G : " << over.at<Vec3b>(y, x)[1] << "  B : " << over.at<Vec3b>(y, x)[2] << std::endl;
                    if (mask.Data[y, x, 0] == 0)
                    {
                        dst.Data[y, x, 0] = background.Data[y, x, 0];
                        dst.Data[y, x, 1] = background.Data[y, x, 1];
                        dst.Data[y, x, 2] = background.Data[y, x, 2];
                    }
                    else
                    {
                        dst.Data[y, x, 0] = original.Data[y, x, 0];
                        dst.Data[y, x, 1] = original.Data[y, x, 1];
                        dst.Data[y, x, 2] = original.Data[y, x, 2];
                    }
                }
            }

            //Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)(delegate
            //{
            //    img_webcam.Source = imageConverter.BitmapToImageSource(dst.Bitmap);
            //}));


        }

    }
}
