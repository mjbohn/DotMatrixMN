using System.Drawing;
using System.Windows.Forms;

namespace DotMatrixMN
{
    public class C64Screen : UserControl
    {
        List<Matrix[]> screen = new List<Matrix[]>();
        int _border;

        public C64Screen() : base()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

            BackColor = Color.FromArgb(0x80, 0xff, 0xff);
            _border = 15;
            SetupScreen(_border);

        }

        // avoid rezising of control in designer
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, 640 + (2 * _border), 400 + (2 * _border), specified);
        }

        private void SetupScreen() { SetupScreen(0); }
        private void SetupScreen(int border)
        {
            screen.Clear();

            Size = new Size(640 + (2 * border), 400 + (2 * border));

            for (int i = 0; i < 25; i++)
            {
                Matrix[] row = new Matrix[40];

                for (int j = 0; j < 40; j++)
                {
                    Matrix matrix = new Matrix();
                    
                    matrix.ScreenRow = i;
                    matrix.ScreenColumn = j;
                    matrix.Location = new Point(j * matrix.Width + border, i * matrix.Height + border);

                    row[j] = matrix;
                    Controls.Add(matrix);
                }

                screen.Add(row);

            }

        }

        public void WriteCharAtLocation(Point location, int[] charToDisplay)
        {
            screen[location.X][location.Y].SetMatrix(charToDisplay);
        }

        public void WriteCharAtLocation(int row, int column, int[] charToDisplay)
        {
            screen[row][column].SetMatrix(charToDisplay);
        }

    }
}
