using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml;
using System.Globalization;
using System.Threading;

namespace Lambda
{
    public partial class find_element : Form
    {
        Main_window main_Window;
        XmlDocument xDoc = new XmlDocument();
        public find_element(Main_window mw)
        {
            main_Window = mw;
            InitializeComponent();
            xDoc.Load("Library.xml");
            Show();
        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach(XmlNode childnode in xDoc.DocumentElement.ChildNodes)
            {
                if (childnode.Attributes["name"].Value.Contains(comboBox1.Text))
                {
                    comboBox1.Items.Add(childnode.Attributes["name"].Value);
                }
            }
            if (comboBox1.Items.Count != 0) comboBox1.DroppedDown = true;
            comboBox1.SelectionLength = 0;
            comboBox1.SelectionStart = comboBox1.Text.Length;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (xDoc.DocumentElement.SelectSingleNode("device[@name='" + comboBox1.Text + "']") == null) return;

            TreeNode tree_node = new TreeNode(comboBox1.Text);
            tree_node.Tag = main_Window.elements.Count;

            main_Window.treeView1.SelectedNode.Nodes.Add(tree_node);
            main_Window.elements.Add(Electronics.getElectronics(xDoc.DocumentElement.SelectSingleNode("device[@name='" + comboBox1.Text + "']")));
              
        }
    }
}
