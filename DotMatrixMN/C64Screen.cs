using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotMatrixMN
{
    public class C64Screen : UserControl
    {
        List<Matrix[]> screen = new List<Matrix[]>();
        public C64Screen() : base()
        {
            BackColor = Color.Yellow;
            
            SetupScreen();
           
        }

        private void SetupScreen()
        {
            for (int i = 0; i < 25; i++)
            {
                Matrix[] row = new Matrix[40];

                for (int j = 0; j < 40; j++)
                {
                    Matrix matrix = new Matrix();
                    matrix.ScreenRow = i;
                    matrix.ScreenColumn = j;
                    matrix.Location = new Point(j * matrix.Width, i * matrix.Height);
                    //if (j % 2 == 0) { matrix.ColorDisabled = Color.Red; }

                    row[j] = matrix;
                    this.Controls.Add(matrix);
                }

                screen.Add(row);
                
            }

        }

       public void WriteCharAtLocation(Point location, int[] charToDisplay)
        {
            screen[location.X][location.Y].setMatrix(charToDisplay);
        }

    }
}
