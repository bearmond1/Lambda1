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
    public partial class add_Node : Form
    {
        TreeView x;
        public add_Node(TreeView _x)
        {
            InitializeComponent();
            x = _x;
            Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            x.SelectedNode.Nodes.Add(new TreeNode(textBox1.Text));
            Close();
        }
    }
}
