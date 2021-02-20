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
    public partial class conditions : Form
    {
        private static Main_window f1;
        public conditions(Main_window f)
        {
            InitializeComponent();
            f1 = f;
            comboBox1.SelectedIndex = Convert.ToInt32(f1.Pe);
            textBox2.Text = f1.T;
        }

        public static string get_Pe(int index)
        {
            switch (index)
            {
                case 0:
                    return "1";
                case 1:
                    return "6";
                case 2:
                case 3:
                    return "9";
                case 4:
                case 5:
                case 6:
                    return "19";
                case  7:
                    return "24";
                case 8:
                    return "13";
                case 9:
                    return "29";
                case 10:
                    return "32";
                case 11:
                case 12:
                    return "14";
                case 13:
                    return "0.5";
                    
            }
            return "-1";

        }

        public string get_Pe_name()
        {
            int x = Convert.ToInt32(f1.Pe);
            return comboBox1.Items[x].ToString();
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            f1.T = textBox2.Text;
            f1.Pe = comboBox1.SelectedIndex.ToString();
            //index = comboBox1.SelectedIndex;
            f1.recalc();
            f1.treeView1_AfterSelect(new object(), new TreeViewEventArgs(f1.treeView1.SelectedNode));
        }
    }
}
