using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SlideSimply
{
    public partial class MainForm : Form
    {
        public string ImagesDirectory { get; set; }
        private Image[] Images = null;
        private int CurrentImageIndex;
        private int BlackPanelAlpha = 0;
        private Config config;
        private bool _IsReady = false;
        public bool IsReady { get { return _IsReady; } }

        public MainForm()
        {
            InitializeComponent();

            _IsReady = true;
            config = Config.Load("slidesimply.xml");
            fileSystemWatcher1.Path = ImagesDirectory = config.Directory;
            if (!Directory.Exists(ImagesDirectory)) Application.Exit();
            timerImageSwitch.Interval = config.SlideInterval * 1000;
            if (!LoadImages()) _IsReady = false;
            Left = Top = 0;
            Size = Screen.PrimaryScreen.Bounds.Size;
        }

        private bool LoadImages()
        {
            if (Images != null) for (int i = 0; i < Images.Length; i++) Images[i].Dispose();
            var filenames = GetImagesInDirectory(ImagesDirectory);
            if (filenames.Length == 0) return false;
            Images = new Image[filenames.Length];
            try
            {
                for (int i = 0; i < filenames.Length; i++) Images[i] = Image.FromFile(filenames[i]);
                CurrentImageIndex = 0;
                Refresh();
                return true;
            }
            catch { }
            return false;
        }

        public string[] GetImagesInDirectory(string path)
        {
            var images = new List<string>();
            foreach (var f in Directory.GetFiles(path))
            {
                var ext = Path.GetExtension(f).ToLower();
                if (ext == ".bmp" || ext == ".gif" || ext == ".jpg" || ext == ".png") images.Add(f);
            }
            return images.ToArray();
        }

        #region Change image index methods
        private int ChangeImageIndex(int d)
        {
            CurrentImageIndex += d;
            if (CurrentImageIndex >= Images.Length) CurrentImageIndex = 0;
            if (CurrentImageIndex < 0) CurrentImageIndex = Images.Length - 1;
            //BlackPanelAlpha = 100;
            Refresh();
            return CurrentImageIndex;
        }

        private int SetNextImageIndex()
        {
            return ChangeImageIndex(1);
        }

        private int SetPreviousImageIndex()
        {
            return ChangeImageIndex(-1);
        }

        #endregion

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.Black);
            var rectScreen = new Rectangle(0, 0, Width, Height);
            g.DrawImage(Images[CurrentImageIndex], 
                        config.ScaleOnFullScreen ? rectScreen : GetImageRect(Images[CurrentImageIndex].Size));

            var brush = new SolidBrush(Color.FromArgb(BlackPanelAlpha, Color.Black));
            g.FillRectangle(brush, 0, 0, Width, Height);
        }

        private Rectangle GetImageRect(Size imageSize)
        {
            double aspectRatioI = (double)imageSize.Width / imageSize.Height;
            bool wd = imageSize.Width >= imageSize.Height;
            int w = 0, h = 0;
            if (wd)
            {
                w = Size.Width;
                h = (int)(w / aspectRatioI);
            }
            else
            {
                h = Size.Height;
                w = (int)(h * aspectRatioI);
            }

            int x = (Size.Width - w) / 2;
            int y = (Size.Height - h) / 2;

            Rectangle rect = new Rectangle(x, y, w, h);
            return rect;
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Left:
                    SetPreviousImageIndex();
                    break;

                case Keys.Right:
                    SetNextImageIndex();
                    break;

                case Keys.Escape:
                    Close();
                    break;
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void timerImageSwitch_Tick(object sender, EventArgs e)
        {
            SetNextImageIndex();
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            timerImageSwitch.Stop();
            LoadImages();
            timerImageSwitch.Start();
        }
    }
}
