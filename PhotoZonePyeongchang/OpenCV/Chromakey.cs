using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PhotoZonePyeongchang.OpenCV
{
    class Chromakey
    {
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
        double blue_l, blue_h;

        public Image<Bgr, byte> dst;

        ControlPanel controlPanel = new ControlPanel();
        ImageConverter imageConverter = new ImageConverter();

        public void getColorMinMax()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                //controlPanel = new ControlPanel();

                red_l = controlPanel.getRedValue_Low();
                red_h = controlPanel.getRedValue_High();

                green_l = controlPanel.getGreenValue_Low();
                green_h = controlPanel.getGreenValue_High();

                blue_l = controlPanel.getBlueValue_Low();
                blue_h = controlPanel.getBlueValue_High();
            }));
        }
        //public void getMaskImage(Image<Bgr, byte> over) // over : webcam image
        //{
        //    Image<Gray, byte> mask = new Image<Gray, byte>(over.Size);   // Scalar(100, 200, 185)    , OpenCvSharp.Scalar color

        //    for (int y = 0; y < over.Rows; y++)
        //    {
        //        for (int x = 0; x < over.Cols; x++)
        //        {
        //            ////cout << "R : " << over.at<Vec3b>(y, x)[0] << "  G : " << over.at<Vec3b>(y, x)[1] << "  B : " << over.at<Vec3b>(y, x)[2] << std::endl;
        //            if (over.Data[y, x, 2] >= red_l && over.Data[y, x, 2] <= red_h && over.Data[y, x, 1] >= green_l && over.Data[y, x, 1] <= green_h && over.Data[y, x, 0] >= blue_l && over.Data[y, x, 0] <= blue_h)
        //            {
        //                mask.Data[y, x, 0] = 0;
        //            }
        //            else
        //            {
        //                mask.Data[y, x, 0] = 255;
        //            }
        //        }
        //    }
        //    mask.Erode(1);
        //}

        public void applyChromakey(Image<Bgr, byte> background, Image<Bgr, byte> original)   // original : webcam
        {
            getColorMinMax();
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

            //Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            //{
                ((MainWindow)System.Windows.Application.Current.MainWindow).img_webcam.Source = imageConverter.BitmapToImageSource(dst.Bitmap);
            //}));


        }
    }
}
