using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lambda
{
    public partial class Function : Form
    {
        public Function(string[] Lambda, string[] arguement)
        {
            InitializeComponent();
            chart1.Series.Clear();
            //chart1.Titles.Add("");
            chart1.Series.Add("Интенсивность отказов");
            for (int x=0;x<Lambda.Length; x++)
            {
                chart1.Series[0].Points.AddXY(arguement[x],Convert.ToDouble(Lambda[x]));
            }
            chart1.ResetAutoValues();
            Show();
        }
    }
}
