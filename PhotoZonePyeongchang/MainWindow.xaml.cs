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
using System.Drawing;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;
using System.Media;
using System.Timers;
using System.Windows.Threading;
using PhotoZonePyeongchang.OpenCV;

namespace PhotoZonePyeongchang
{
    /**
     * 프로젝트 이름 : PhotoZonePyeongchang
     * GIT HUB ADDRESS : https://github.com/SeungHeeKo/2017_PhotoZonePyeongChang
     * 개발환경 : WPF (C#)
     * 개발 내용 : 크로마키 배경으로 사진을 찍은 후 인물만 추출하며 미리 저장된 이미지와 합성한 후 후면 디스플레이에 보여줌.
     * 콘텐츠 내용 : 크로마키 된 사진 + 배경 = 합성사진  
     * 핵심개발자 : 고승희 
     * 개발시작일 : 2017년 10월 31일 
     * **/
    public partial class MainWindow : System.Windows.Window
    {
        private System.Timers.Timer timer;
        public Image<Bgr, byte> frame, captureFrame;

        // 4K Ultra HD 화상 통화(최대 4096 x 2160픽셀 @ 30fps)
        // 1080p Full HD 화상 통화(최대 1920 x 1080픽셀 @ 30 또는 60fps)
        // 720p HD 화상 통화(최대 1280 x 720픽셀 @ 30, 60 또는 90fps)
        public Capture capture = default(Capture);



        Image<Bgr, byte> bgImg;
        Image<Bgr, byte> orgImg;

        BitmapImage backgroundImage, originalImage;
        Chromakey chromakey;
        OpenCV.ImageConverter imageConverter;
        ControlPanel controlPanel;

        int _width = 1920;
        int _height = 1080;

        public MainWindow()
        {
            InitializeComponent();

            chromakey = new Chromakey();
            imageConverter = new OpenCV.ImageConverter();
            orgImg = new Image<Bgr, byte>(_width, _height);
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //capture.FlipHorizontal = true;

            //bgImg = new Image<Bgr, byte>(BitmapImage2Bitmap(backgroundImage));
            ////orgImg = new Image<Bgr, byte>(BitmapImage2Bitmap(originalImage));
            //////applyChromakey();


            //textBox.Text = "W : " + SystemParameters.MaximizedPrimaryScreenWidth;
            //textBox.Text += "H : " + SystemParameters.MaximizedPrimaryScreenHeight;
            LoadResourceImage();
            //OpenControlPanel();
            OpenWebCam();
        }
        
        private void LoadResourceImage()
        {
            backgroundImage = new BitmapImage(new Uri("Resources/bg_pc_640.jpg", UriKind.Relative));
            bgImg = new Image<Bgr, byte>(imageConverter.BitmapImage2Bitmap(backgroundImage));
        }
        public Image<Bgr, byte> getCurrentFrame()
        {
            return orgImg;
        }
        public void setCurrentFrame(Image<Bgr, byte> image)
        {
            image.CopyTo(orgImg);
        }


        public Image<Bgr, byte> getBackground()
        {
            return bgImg;
        }
        public void setBackground(Image<Bgr, byte> image)
        {
            image.CopyTo(bgImg);
        }

        public void applyChromakey()
        {
            //chromakey.applyChromakey(bgImg, orgImg);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseWebCam();
        }

        private void button_capture_Click(object sender, RoutedEventArgs e)
        {
            OpenControlPanel();
            //MessageBox.Show("Clicked");
            //applyChromakey();
        }

        public void OpenControlPanel()
        {
            controlPanel = new ControlPanel();
            controlPanel.SetCaptureImage(getCurrentFrame());
            controlPanel.Show();
        }
        public void OpenWebCam()
        {
            capture = new Capture(0);
            capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, _width);
            capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, _height);
            StartPreview();
        }
        public void StartPreview()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 500;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            using (frame = capture.QueryFrame())
            {
                if (frame != null)
                {
                    setCurrentFrame(frame);
                    
                    var bmp = frame.Bitmap;
                    bmp.RotateFlip(RotateFlipType.Rotate180FlipY);
                    
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)(delegate
                    {
                        //if(orgImg.Data != null)
                        //{
                        //SetChromakeyColor(controlPanel.getRedValue_Low(), controlPanel.getRedValue_High(), controlPanel.getGreenValue_Low(), controlPanel.getGreenValue_High(), controlPanel.getBlueValue_Low(), controlPanel.getBlueValue_High());
                        //SetChromakeyColor(0,190,155,255,0,130);
                        //applyChromakey(bgImg, orgImg);

                        //}
                        img_webcam.Source = imageConverter.BitmapToImageSource(bmp);
                        // frame chromakey

                    }));
                }
                //else
                //{
                //    MessageBox.Show("웹캠에 문제가 있습니다.");
                //}
            }
        }

        public void CloseWebCam()
        {
            if (capture != null)
            {
                capture.Dispose();
            }
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
            img_webcam.Source = imageConverter.BitmapToImageSource(dst.Bitmap);
            //}));


        }

    }

}
