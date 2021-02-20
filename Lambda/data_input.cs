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
using System.Reflection;

namespace Lambda
{
    public partial class new_element : Form
    {
        XmlDocument lib = new XmlDocument();
        XmlDocument xDoc = new XmlDocument();

        public static Main_window form1;

        Dictionary<string,double> parametrs = new Dictionary<string, double>();
        Dictionary<string, string> input = new Dictionary<string, string>();

        TextBox amount = new TextBox();
        TextBox position = new TextBox();
        ComboBox name = new ComboBox();
        ComboBox Metric_prefix = new ComboBox();
        int Metric_prefix_initial;

        List<Label> labels = new List<Label>();
        List<ComboBox> boxes = new List<ComboBox>();
        List<Button> research = new List<Button>();

        int shift=30, x=300, label_x=100, top_shift =25, boxwidth = 600;

        string temperature, cur_Type;

        bool is_chosen = false;
        bool is_edit = false;

        // удаление предварительно созданного элемента из главного списка при закрытии 
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && is_edit == false) form1.elements.RemoveAt(form1.elements.Count-1);
            form1.Show();
        }

        public new_element(string type, Main_window x)
        {
            InitializeComponent();
            form1 = x;
            form1.Hide();
            temperature = form1.T;
            cur_Type = type;
            Metric_prefix.Visible = false;

            xDoc.Load(Directory.GetCurrentDirectory() + "\\111.xml");
            lib.Load("Library.xml");
            List<string> prefixes = new List<string>() { "Гига-,10E9", "Мега-,10E6", "Кило-,10E3", "10E0", "Милли-,10E-3", "Микро-,10E-6", "Нано-,10E-9", "Пико-,10E-12" };

            var nod = xDoc.SelectSingleNode(type);

            Text = nod.Attributes["name"].Value;
            //общий цикл
            int count = 0;
            foreach (XmlNode param in nod.ChildNodes)//C1,C2,Pt...
            {
                boxes.Add(new ComboBox());
                boxes.Last().Tag = count;
                // Если параметр выбираемый
                if (param.HasChildNodes)
                {
                    boxes[count].Name = (-count - 1).ToString();
                    boxes[count].FormattingEnabled = true;
                    boxes[count].Location = new System.Drawing.Point(this.x, top_shift + count * shift);
                    boxes[count].Size = new System.Drawing.Size(boxwidth, 21);

                    foreach (XmlNode childnode in param.ChildNodes)
                    {
                        boxes[count].Items.Add(childnode.Attributes.GetNamedItem("text").Value);
                    }
                    boxes[count].SelectedIndex = 0;
                }

                // Если параметр вводится
                else
                {
                    boxes[count].FormattingEnabled = true;
                    boxes[count].Location = new System.Drawing.Point(this.x, top_shift + count * shift);
                    boxes[count].Size = new System.Drawing.Size(boxwidth, 21);

                    // бокс для выбора приставки СИ
                    if (param.Attributes["default"] != null)
                    {
                        boxes[count].Width = boxwidth - 100;
                        boxes[count].Tag = "prefix";

                        Metric_prefix.Visible = true;
                        Metric_prefix.Location = new Point(this.x+500, top_shift + count * shift);
                        Metric_prefix.Size = new Size(100, 21);
                        Controls.Add(Metric_prefix);
                        Metric_prefix_initial = Convert.ToInt32(param.Attributes["default"].Value);
                        foreach (string prfx in prefixes)
                        {
                            Metric_prefix.Items.Add(prfx);
                        }
                        Metric_prefix.SelectedIndex = Convert.ToInt32(param.Attributes["default"].Value);
                    }
                }

                Controls.Add(boxes[count]);
                labels.Add(new Label() { Text = param.Attributes["name"].Value, Location = new Point(20, top_shift + count * shift), AutoSize = true });
                if (param.Attributes["name"].Value == "Температура" || param.Attributes["name"].Value == "Температура перехода" || param.Attributes["name"].Value == "Температура кристалла") boxes[count].Text = temperature;
                Controls.Add(labels[count]);

                // кнопки построения зависимости
                research.Add(new Button());
                research[count].Size = new Size(21,21);
                research[count].Location = new Point(this.x , top_shift + count * shift);
                research[count].Text = "?";
                research[count].FlatStyle = FlatStyle.Flat;
                research[count].FlatAppearance.BorderSize = 0;
                research[count].Tag = count;
                research[count].Click += Help_Handler;
                Controls.Add(research[count]);

                if (labels[count].Size.Width > label_x) label_x = labels[count].Size.Width;
                count++;
            }
            label_x += 50;

            //выравнивание по label
            foreach (ComboBox b in boxes)
            {
                b.Left = label_x;
            }
            foreach (Button b in research)
            {
                b.Left = label_x - 25;
            }
            Metric_prefix.Left = label_x + 500;

            //количество эл-в
            amount.Location = new Point(label_x, top_shift + count * shift);
            amount.Size = new Size(193, 21);
            amount.Name = "num";
            amount.Text = "1";

            amount.Width = boxwidth;
            Controls.Add(amount);
            labels.Add(new Label() { Text = "Количество", Location = new Point(20, top_shift + count * shift), AutoSize = true });
            Controls.Add(labels[count]);

            count++;

            //название
            name.Location = new System.Drawing.Point(label_x, top_shift + count * shift);
            name.Size = new System.Drawing.Size(193, 21);
            name.Name = "name";
            name.Width = boxwidth;
            Controls.Add(name);
            labels.Add(new Label() { Text = "Название", Location = new Point(20, top_shift + count * shift), AutoSize = true });
            Controls.Add(labels[count]);
            count++;

            // позиционное обозначение
            position.Location = new Point(label_x, top_shift + count * shift);
            position.Size = new Size(193, 21);
            position.Name = "position";

            position.Width = boxwidth;
            Controls.Add(position);
            labels.Add(new Label() { Text = "Позиционное \r\nобозначение", Location = new Point(20, top_shift + count * shift), AutoSize = true });
            Controls.Add(labels[count]);

            //предложение элементов
            //name.TextUpdate += new System.EventHandler(this.comboBox1_TextUpdate);
            //name.SelectedIndexChanged += new System.EventHandler(this.offer_accpedted);

            // кнопка сохранения
            button1.Top = top_shift + (count+1) * shift;
            button1.Left = label_x + boxwidth -130;

            // сохранение элемента в библиотеку
            checkBox1.Top = top_shift + (count + 1) * shift;
            checkBox1.Left = label_x + 20;
            checkBox1.Text = "Добавить в библиотеку элементов";
            checkBox1.Checked = true;

            // подгонка формы
            Width = label_x + boxwidth + 50;
            Height = 30 + (count + 3) * shift;
            Show();

            // заполнить поля если элемент редактируется
            if (x.treeView1.SelectedNode.Tag!= null && x.elements[(Int32)x.treeView1.SelectedNode.Tag].type != "")
            {
                is_edit = true;
                var input = x.elements[(Int32)x.treeView1.SelectedNode.Tag].input;

                var inp = input.GetEnumerator();
                foreach (ComboBox box in boxes)
                {
                    inp.MoveNext();
                    var y = inp.Current.Value;
                    box.Text = y;

                    if (box.Tag == "prefix")
                    {
                        // русскую Е на англ.
                        y = y.Replace("Е", "E");

                        box.Text = y.Substring(0, y.IndexOf("E"));

                        foreach (string prefix in prefixes)
                        {
                            var f = y.Substring(y.IndexOf("E"), y.Length - y.IndexOf("E"));
                            if (prefix.Contains(y.Substring(y.IndexOf("E"), y.Length- y.IndexOf("E")))) 
                                Metric_prefix.SelectedIndex = prefixes.IndexOf(prefix);
                        }
                    }
                }
                amount.Text = input["Количество"];
                name.Text = input["Название"];
                position.Text = input["Позиционное обозначение"];
            }
        }
        
        //работа с префиксом
        double Prefix(int index)
        {
            switch (index)
            {
                case 0:
                    return 10E9;
                case 1:
                    return 10E6;
                case 2:
                    return 10E3;
                case 3:
                    return 1;
                case 4:
                    return 10E-3;
                case 5:
                    return 10E-6;
                case 6:
                    return 10E-9;
                case 7:
                    return 10E-12;
            }
            return 0;
        }

        // вызов графика
        void Help_Handler(object sender, EventArgs e)
        {
            Button x = (Button)sender;
            influence(boxes[Convert.ToInt32(x.Tag)],(int)x.Tag);
        }

        // Зафиксировать введенные значения и добавить объект
        private void button1_Click(object sender, EventArgs e)
        {
                int h = -1;
                var nod = xDoc.SelectSingleNode(cur_Type);
                foreach (XmlNode parametr in nod.ChildNodes)
                {
                    h++;
                    double x;
                    string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                    //загрузка выбираемого значения из XML
                    if (parametr.HasChildNodes)
                    {
                        if (double.TryParse(parametr.SelectSingleNode(".//f[@text='" + boxes[h].Text + "']").InnerText.Replace(','.ToString(), sep), out x)) parametrs.Add(parametr.Name,x);
                        else
                        {
                            if (double.TryParse(parametr.SelectSingleNode(".//f[@text='" + boxes[h].Text + "']").InnerText.Replace('.'.ToString(), sep), out x)) parametrs.Add(parametr.Name, x);
                            else
                            {
                                MessageBox.Show("Неверный формат числа в XML");
                                return;
                            }
                        }
                    }
                    // парсим введенное значение
                    else
                    {
                        if (double.TryParse(boxes[h].Text.Replace(','.ToString(), sep), out x)) parametrs.Add(parametr.Name, x);
                        else
                        {
                            if (double.TryParse(boxes[h].Text.Replace('.'.ToString(), sep), out x)) parametrs.Add(parametr.Name, x);
                            else
                            {
                                MessageBox.Show("Неверный формат числа "+ boxes[h].Text);
                                return;
                            }
                        }
                    }

                    //Пересчет с приставкой СИ
                    if (boxes[h].Tag.ToString() == "prefix")
                    {
                        parametrs[parametrs.Last().Key] = parametrs.Last().Value * Prefix(Metric_prefix.SelectedIndex) / Prefix(Metric_prefix_initial);
                    }

                }
                parametrs.Add("Pe",Convert.ToDouble(form1.Pe));

                // Записать входные данные для элемента
                int i = 0;
                while (i < boxes.Count)
                {
                    // Добавление приставки СИ
                    if (boxes[i].Tag.ToString() == "prefix" && Metric_prefix.SelectedIndex != 4) boxes[i].Text =  boxes[i].Text+Metric_prefix.Text.Substring(Metric_prefix.Text.IndexOf('0')+1);
                    input.Add(labels[i].Text, boxes[i].Text);
                    i++;
                }
                input.Add("Количество", amount.Text);
                input.Add("Название", name.Text);
                input.Add("Позиционное обозначение", position.Text);

                // элемент дерева для новых элементов
                TreeNode node = new TreeNode(name.Text);

                int index = 0;

                // для новых элементов
                if (form1.treeView1.SelectedNode.Tag == null )
                {
                    index = form1.elements.Count - 1;
                    node.Tag = index;
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                    form1.treeView1.SelectedNode.Nodes.Add(node);
                }
                // для редактирования
                else 
                {
                    index = (int)form1.treeView1.SelectedNode.Tag;
                    form1.elements[index].input.Clear();
                    form1.elements[index].parametrs.Clear();
                    form1.elements[index] = (Electronics)Activator.CreateInstance(form1.elements[index].GetType());
                }
                form1.elements[index].type = cur_Type;
                form1.elements[index].text_type = Text;
                form1.elements[index].calc_input = parametrs;
                form1.elements[index].input = input;
                form1.elements[index].calc(parametrs);

                Electronics element = form1.elements[form1.elements.Count - 1];
                // добавление в библиотеку элементов
                if (checkBox1.Checked)
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load("Library.xml");
                    XmlElement xmlElement = xDoc.DocumentElement;
                    xmlElement.AppendChild(xDoc.ImportNode(element.ToXML(),true));
                    xDoc.Save("Library.xml");
                }
                form1.Show();
            form1.treeView1_AfterSelect(new object(), new TreeViewEventArgs(form1.treeView1.SelectedNode));
            Dispose();
            
        }
    }
}
