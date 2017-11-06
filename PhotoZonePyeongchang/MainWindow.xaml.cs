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
     * GIT HUB ADDRESS : 
     * 개발환경 : WPF (C#)
     * 개발 내용 : 크로마키 배경으로 사진을 찍은 후 인물만 추출하며 미리 저장된 이미지와 합성한 후 후면 디스플레이에 보여줌.
     * 콘텐츠 내용 : 크로마키 된 사진 + 배경 = 합성사진  
     * 핵심개발자 : 고승희 
     * 개발시작일 : 2017년 10월 31일 
     * **/
    public partial class MainWindow : System.Windows.Window
    {
        private System.Timers.Timer timer;
        public Image<Bgr, byte> frame;

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

        public MainWindow()
        {
            InitializeComponent();

            chromakey = new Chromakey();
            imageConverter = new OpenCV.ImageConverter();
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //capture.FlipHorizontal = true;

            //bgImg = new Image<Bgr, byte>(BitmapImage2Bitmap(backgroundImage));
            ////orgImg = new Image<Bgr, byte>(BitmapImage2Bitmap(originalImage));
            //////applyChromakey();
            OpenWebCam();
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
            chromakey.applyChromakey(bgImg, orgImg);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseWebCam();
        }

        private void button_capture_Click(object sender, RoutedEventArgs e)
        {
            controlPanel = new ControlPanel();
            controlPanel.SetCaptureImage(frame);
            controlPanel.Show();
            //MessageBox.Show("Clicked");
            //applyChromakey();
        }


        public void OpenWebCam()
        {
            capture = new Capture(0);
            StartPreview();
        }
        public void StartPreview()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 15;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            using (frame = capture.QueryFrame())
            {
                if (frame != null)
                {
                    var bmp = frame.Bitmap;
                    bmp.RotateFlip(RotateFlipType.Rotate180FlipY);

                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)(delegate
                    {
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

    }

}
