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
        public static int index=0;
        private Main_window f1;
        public conditions(Main_window f)
        {
            Show();
            InitializeComponent();
            f1 = f;
            comboBox1.SelectedIndex = index;
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

        public static string get_Pe_name()
        {
            switch (index)
            {
                case 0: return "1.1";
                case 1: return "1.2";
                case 2: return "1.3-1.10";
                case 3: return "2.1.1, 2.1.2, 2.3.1, 2.3.2";
                case 4: return "2.1.3, 2.3.3";
                case 5: return "2.1.5, 2.3.5";
                case 6: return "2.2, 2.4, 2.1.4, 2.3.4";
                case 7: return "3.1";
                case 8: return "3.2";
                case 9: return "3.3, 3.4";
                case 10: return "4.1 - 4.9 в условиях запуска";
                case 11: return "4.1 - 4.9 в условиях свободного полета";
                case 12: return "4.6 в условиях бреющего полета";
                case 13: return "5.1, 5.2";
            }
            return "-1";   

        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            f1.T = textBox2.Text;
            f1.Pe = get_Pe(comboBox1.SelectedIndex);
            index = comboBox1.SelectedIndex;
            f1.recalc();
            f1.treeView1.SelectedNode = f1.treeView1.Nodes[0];
        }
    }
}
