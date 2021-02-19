using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Lambda
{
    public partial class Main_window : Form
    {
        public List<Electronics> elements = new List<Electronics>();

        public string Pe="1", T="25";
        public int image_index;
        string path;

        public Main_window()
        {
            InitializeComponent();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader;
            dataGridView1.DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            dataGridView1.EnableHeadersVisualStyles = false;
            treeView1.Nodes.Add(new TreeNode("Аппаратура"));
            treeView1.SelectedNode = treeView1.Nodes[0];

            try
            {
                using (StreamReader sr = new StreamReader((Directory.GetCurrentDirectory()+"\\hta.txt")))
                {
                    if (sr.ReadToEnd() != "") path = sr.ReadToEnd();
                    else path = Directory.GetCurrentDirectory();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        // проверка на выбранный узел
        private void check()
        {
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Tag != null)
                {
                    treeView1.SelectedNode = treeView1.SelectedNode.Parent;
                }
            }
            else treeView1.SelectedNode = treeView1.Nodes[0];
        }
        // добавить узел
        private void button2_Click(object sender, EventArgs e)
        {
            check();
            new add_Node(treeView1);
        }
        // добавить элемент
        private void button3_Click(object sender, EventArgs e)
        {
            check();
            new pick_element(this);
        }
        // показать элемент
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            dataGridView1.Columns.Clear();

            if (treeView1.SelectedNode.Tag != null)
            {
                int index = Convert.ToInt32(treeView1.SelectedNode.Tag);

                for (int i = 0; i < elements[index].parametrs.Count; i++)
                {
                    dataGridView1.Columns.Add("", "");
                }

                List<string> title = new List<string>();
                List<string> value = new List<string>();

                List<string> input_title = new List<string>();
                List<string> input_value = new List<string>();

                foreach(KeyValuePair<string,string> pair in elements[index].parametrs)
                {
                    title.Add(pair.Key);
                    value.Add(pair.Value);
                }
                dataGridView1.Rows.Add(title.ToArray());
                dataGridView1.Rows.Add(value.ToArray());

                dataGridView1.Rows.Add();

                foreach (KeyValuePair<string, string> pair in elements[index].input)
                {
                    input_title.Add(pair.Key);
                    input_value.Add(pair.Value);
                }

                dataGridView1.Rows.Add(input_title.ToArray());
                dataGridView1.Rows.Add(input_value.ToArray());
            }
        }
        // удалить элемент
        private void button4_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Text != "Root")
                {

                    if (treeView1.SelectedNode.Tag != null)
                    {
                        int index = Convert.ToInt32(treeView1.SelectedNode.Tag) ;
                        elements.RemoveAt(Convert.ToInt32(treeView1.SelectedNode.Tag));
                        foreach (TreeNode node in treeView1.Nodes[0].Nodes)
                        {
                            if ((int)node.Tag > index) node.Tag = (int)node.Tag - 1;
                        }
                    }
                    treeView1.SelectedNode.Remove();
                }
            }
        }

        private void Main_window_Activated(object sender, EventArgs e)
        {
            //image_set(treeView1.Nodes[0]);
            treeView1.ExpandAll();
        }
        // картинки для дерева
        private void image_set(TreeNode node)
        {
            if (node.Tag != null)
            {
                node.SelectedImageIndex = 1;
                node.ImageIndex = 1;
            }
            else
            { 
                foreach (TreeNode childnode in node.Nodes)
                {
                    image_set(childnode);
                }
            }
        }        
        // поиск элемента
        private void button5_Click(object sender, EventArgs e)
        {
            check();
            new find_element(this);
        }

        private void Main_window_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (elements.Count != 0)
            {
                Closing_warning warning = new Closing_warning(e,this);
                warning.ShowDialog();
            }
        }
        // папка по умолчанию
        private void button2_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            path = folderBrowserDialog.SelectedPath;
            try
            {
                using (StreamWriter sw = new StreamWriter("hta.txt"))
                {
                    sw.WriteLine(folderBrowserDialog.SelectedPath);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show( ex.Message);
            }
        }
        // редактирование элемента
        private void edit_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag != null)
            {
                new new_element(elements[(int)treeView1.SelectedNode.Tag].type,this);
            }
        }
        // условия эксплуатации
        private void button1_Click(object sender, EventArgs e)
        {
            new conditions(this);
        }

        private void Main_window_Resize(object sender, EventArgs e)
        {
            label1.Width = this.Width - 35;
            label1.Height = 2;
        }

    }
}
