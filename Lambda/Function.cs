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
            chart1.Series.Add("Интенсивность отказов"); 
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            chart1.Series[0].BorderWidth = 5;

            for (int x=0;x<Lambda.Length; x++)
            {
                chart1.Series[0].Points.AddXY(arguement[x],Convert.ToDouble(Lambda[x]));
            }
            chart1.ResetAutoValues();
            Show();
        }
    }
}
