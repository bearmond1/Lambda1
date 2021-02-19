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
    public partial class Closing_warning : Form
    {
        FormClosingEventArgs e;
        Main_window main_Window;
        public Closing_warning(FormClosingEventArgs e, Main_window main_Window)
        {
            InitializeComponent();
            this.e = e;
            this.main_Window = main_Window;
        }
        public Closing_warning(Main_window main_Window)
        {
            InitializeComponent();
            this.main_Window = main_Window;
        }
        // сохранить
        public void button1_Click(object sender, EventArgs e)
        {
            main_Window.сохранитьToolStripMenuItem_Click(sender,e);
            main_Window.elements.Clear();
            main_Window.treeView1.Nodes.Clear();
            main_Window.treeView1.Nodes.Add("Аппаратура");
            main_Window.dataGridView1.Rows.Clear();
            Close();
        }
        // не сохранять
        public void button2_Click(object sender, EventArgs e)
        {
            main_Window.elements.Clear();
            main_Window.treeView1.Nodes.Clear();
            main_Window.treeView1.Nodes.Add("Аппаратура");
            main_Window.dataGridView1.Rows.Clear();
            Close();
        }
        // отмена
        public void button3_Click(object sender, EventArgs e)
        {
            if (this.e != null) this.e.Cancel = true;
            Close();
        }
    }
}
