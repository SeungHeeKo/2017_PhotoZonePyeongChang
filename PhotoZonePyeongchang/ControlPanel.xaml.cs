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
        //Chromakey chromakey = new Chromakey();
        ImageConverter imageConverter = new ImageConverter();

        public ControlPanel()
        {
            InitializeComponent();
            setRedValue_Low(0);
            setRedValue_High(148);
            setGreenValue_Low(180);
            setGreenValue_High(255);
            setBlueValue_Low(0);
            setBlueValue_High(160);
        }

        public void SetCaptureImage(Image<Bgr, byte> frame)
        {
            originalImage = frame;
            img_capture.Source = imageConverter.BitmapToImageSource(originalImage.Bitmap);
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

        }
    }
}
