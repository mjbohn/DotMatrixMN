using System.Drawing;
using System.Windows.Forms;

namespace DotMatrixMN
{
    public delegate void MatrixChangedEventHandler(int[] dots);
    
    public class Matrix : Control
    {
        private Pixel[] pixels;
        private int _pixelHeight = 10;
        private int _pixelWidth = 10;

        #region Properties

        private Color _enabledColor;
        private Color _disabledColor;
        private bool _showBorders;
        public enum Style { Dots, Rectagles };
        private Style _displayStyle;
        private bool _teachMode;
        private bool _showNumbers;
        private int _matrixWidth;
        private int _matrixHeight;
        public bool TeachMode
        {
            get { return _teachMode; }
            set
            {
                _teachMode = value;
                this.Invalidate();
            }
        }
        public bool ShowNumbers
        {
            get { return _showNumbers; }
            set
            {
                _showNumbers = value;
                this.Invalidate();
            }
        }
        public Style DisplayStyle
        {
            get { return _displayStyle; }
            set
            {
                _displayStyle = value;
                this.Invalidate();
            }
        }
        public bool ShowBorders
        {
            get { return _showBorders; }
            set
            {
                _showBorders = value;
                this.Invalidate();
            }
        }
        public Color ColorEnabled
        {
            get { return _enabledColor; }
            set
            {
                _enabledColor = value;
                this.Invalidate();
            }
        }
        public Color ColorDisabled
        {
            get { return _disabledColor; }
            set
            {
                _disabledColor = value;
                this.Invalidate();
            }
        }

        public int PixelHeight { get => _pixelHeight; set => _pixelHeight = value; }
        public int PixelWidth { get => _pixelWidth; set => _pixelWidth = value; }
        public int MatrixWidth 
        { 
            get { return _matrixWidth; }
            private set { _matrixWidth = value; }
        }
        public int MatrixHeight 
        { 
            get {return _matrixHeight; }
            private set { _matrixHeight = value; }
        }

        //public int MatrixWidth { get; private set; }
        //public int MatrixHeight { get; private set; }

        #endregion

        public event MatrixChangedEventHandler? MatrixChanged;
        protected virtual void onMatrixChanged()
        {
            MatrixChanged?.Invoke(GetDots());
        }

        public Matrix() : this(8,8){}
        public Matrix(int height, int width) : base()
        {
            MatrixWidth = width;
            MatrixHeight = height;

            pixels = new Pixel[height * width];

            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.Size = new Size(80, 80);
            this.BackColor = SystemColors.ControlDarkDark;
            this.ForeColor = Color.Lime;
            this.ColorDisabled = Color.DarkRed;
            this.ColorEnabled = Color.Red;
            this.ShowBorders = false;
            this.DisplayStyle = Style.Rectagles;
            this.Margin = new Padding(0);
            this.TeachMode = false;
            this.ShowNumbers = false;

            BuildPixelArray(MatrixHeight, MatrixWidth);
        }

        private void BuildPixelArray(int height, int width)
        {
            int row = 0;
            int column = 0;
            for (int i = 0; i < (height * width); i++)
            {
                pixels[i] = new Pixel();
                pixels[i].Id = i;

                if (i % width == 0 && i != 0)
                {
                    column = 0;
                    row++;
                }
                pixels[i].Column = column++;
                pixels[i].Row = row;
                pixels[i].BinaryValue = width - 1 - pixels[i].Column;

                pixels[i].Rect = new Rectangle(PixelWidth * pixels[i].Column, PixelHeight * pixels[i].Row, PixelWidth, PixelHeight);

            }
        }

        private int[] GetDots()
        {
            int[] dots = new int[MatrixWidth];
            for (int i = 0; i < (MatrixWidth * MatrixHeight); i++)
            {
                for (int j = 0; j < MatrixHeight; j++)
                {
                    if (pixels[i].Row == j)
                    {
                        if (pixels[i].Enabled)
                        {
                            dots[j] += (int)Math.Pow(2, pixels[i].BinaryValue);
                        }
                    }
                }
            }

            return dots;
        }

        public Bitmap getBitmap()
        {
            Bitmap bmp = new Bitmap(MatrixWidth, MatrixHeight);

            for (int i = 0; i < (MatrixWidth * MatrixHeight); i++)
            {
                Color color = pixels[i].Color;
                bmp.SetPixel(pixels[i].Column, pixels[i].Row, pixels[i].Color);
            }
            return bmp;
        }

        public void setMatrix(int[] charToDisplay)
        {
            if (charToDisplay.Length != MatrixHeight)
            {
                throw new ArgumentException("PixelCount doesn't match MatrixHeight of "+ MatrixHeight.ToString(),nameof(charToDisplay));
            }
            for (int i = 0; i < (MatrixWidth * MatrixHeight); i++)  // all Dots
            {

                for (int j = 0; j < MatrixHeight; j++) //  Rows
                {
                    if (pixels[i].Row == j)
                    {
                        if (charToDisplay[j] > Math.Pow(2, MatrixWidth))
                        {
                            throw new ArgumentException("PixelCount doesn't match MatrixWidth of " + MatrixWidth.ToString(), nameof(charToDisplay));
                        }

                        if (pixels[i].BinaryValue < 8) // LowByte
                        {
                            byte value = (byte)(Math.Pow(2, pixels[i].BinaryValue));

                            byte mask = (byte)charToDisplay[j];
                            byte check = (byte)(value & mask);

                            pixels[i].Enabled = (check == value);


                        }
                        else  // HighByte
                        {
                            int value = (int)(Math.Pow(2, pixels[i].BinaryValue)) >> 8;
                            int mask = charToDisplay[j] >> 8;

                            byte check = (byte)(value & mask);

                            pixels[i].Enabled = (check == value);
                        }
                    }
                }
            }
            this.Invalidate();
            onMatrixChanged();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics gr = e.Graphics;
            Rectangle rect = ClientRectangle;
            Brush brushEnabeld = new SolidBrush(ColorEnabled);
            Brush brushDisabled = new SolidBrush(ColorDisabled);

            int rw = rect.Width;
            int refw = PixelWidth * MatrixWidth;


            for (int i = 0; i < (MatrixHeight * MatrixWidth); i++)
            {

                Rectangle rtmp = pixels[i].Rect;

                if (this.ShowBorders)
                {
                    rtmp.Inflate(-1, -1);
                }


                if (pixels[i].Enabled)
                {
                    pixels[i].Color = ColorEnabled;
                    switch (DisplayStyle)
                    {
                        case Style.Dots:
                            gr.FillEllipse(brushEnabeld, rtmp);
                            break;
                        case Style.Rectagles:
                            gr.FillRectangle(brushEnabeld, rtmp);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    pixels[i].Color = ColorDisabled;
                    switch (DisplayStyle)
                    {
                        case Style.Dots:
                            gr.FillEllipse(brushDisabled, rtmp);
                            break;
                        case Style.Rectagles:
                            gr.FillRectangle(brushDisabled, rtmp);
                            break;
                        default:
                            break;
                    }
                }
                if (_teachMode && _showNumbers)
                {
                    gr.DrawString((pixels[i].Column + 1).ToString(), this.Font, new SolidBrush(Color.Black), pixels[i].Rect.X + 5, pixels[i].Rect.Y + 5);
                }

            }

            onMatrixChanged();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            PixelHeight = this.Height / MatrixHeight;
            PixelWidth = this.Width / MatrixWidth;
            //this.Width = this.Height;

            for (int i = 0; i < (MatrixHeight * MatrixWidth); i++)
            {
                if (pixels[i] != null)
                {
                    pixels[i].Rect = new Rectangle(PixelWidth * pixels[i].Column, PixelHeight * pixels[i].Row, PixelWidth, PixelHeight);
                }
            }

        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            this.Height = PixelHeight * MatrixHeight;
            this.Width = PixelWidth * MatrixWidth;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_teachMode)
            {
                foreach (Pixel p in pixels)
                {
                    if (p.Rect.Contains(e.X, e.Y))
                    {
                        p.Enabled = !p.Enabled;
                        break;
                    }
                }
                this.Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_teachMode)
            {
                foreach (Pixel p in pixels)
                {
                    if (p.Rect.Contains(e.X, e.Y))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            p.Enabled = true;
                        }
                        if (e.Button == MouseButtons.Right)
                        {
                            p.Enabled = false;
                        }
                        break;
                    }
                }
                this.Invalidate();
            }

        }
    }

    public class Pixel
    {
        public Pixel()
        {

        }

        private Rectangle _rect;
        private int _id;
        private bool _enabled;
        private int _row;
        private int _col;
        private int _binaryValue;
        private Color _color;

        public Rectangle Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }
        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }
        public int Column
        {
            get { return _col; }
            set { _col = value; }
        }
        public int BinaryValue
        {
            get { return _binaryValue; }
            set { _binaryValue = value; }
        }
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }


    }
}