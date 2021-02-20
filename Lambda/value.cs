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

// графики зависимости
namespace Lambda
{
    public partial class new_element : Form
    {
        public void influence(ComboBox box, int index)
        {
            string koef = "";
            bool to_clear = false;
            var nod = xDoc.SelectSingleNode(cur_Type);

            if (box.Items.Count == 0)
            {
                koef = box.Text;
                to_clear = true;
                for (int i = 0; i < 10; i++)
                {
                    box.Items.Add(i * 10);
                }
                if (labels[index].Text.Contains("Коэффициент"))
                {
                    box.Items.Clear();
                    for (int i = 1; i < 10; i++)
                    {
                        box.Items.Add((double)i / 10);
                    }
                }
            }

            string[] Lambda = new string[box.Items.Count];
            string[] argument = new string[box.Items.Count];

            for (int i = 0; i < box.Items.Count; i++)
            {
                parametrs.Clear();
                foreach (ComboBox comboBox in boxes)
                {
                    if (comboBox.Items.Count == 0 && comboBox.Text == "")
                    {
                        comboBox.Text = "5.001";
                        if (comboBox.Tag.ToString() != "prefix")
                            if (labels[(int)comboBox.Tag].Text.Contains("Коэффициент")) comboBox.Text = "0.5";
                    }
                }
                box.SelectedIndex = i;

                double x;
                string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                int h = -1;
                foreach (XmlNode parametr in nod.ChildNodes)
                {
                    h++;
                    //загрузка выбираемого значения из XML
                    if (parametr.HasChildNodes)
                    {
                        if (double.TryParse(parametr.SelectSingleNode(".//f[@text='" + boxes[h].Text + "']").InnerText.Replace(','.ToString(), sep), out x)) parametrs.Add(parametr.Name, x);
                        else
                        {
                            if (double.TryParse(nod.SelectSingleNode(".//f[@text='" + boxes[h].Text + "']").InnerText.Replace('.'.ToString(), sep), out x)) parametrs.Add(parametr.Name, x);
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
                                MessageBox.Show("Неверный формат числа " + boxes[h].Text);
                                return;
                            }
                        }
                    }
                }                    
                
                parametrs.Add("Pe",Convert.ToDouble(form1.Pe));
                //Electronics element = (Electronics)form1.elements.Last().Clone();
                Electronics element = (Electronics)Activator.CreateInstance(form1.elements.Last().GetType());
                element.input.Add("Количество","1");
                element.calc(parametrs);
                parametrs.Clear();

                Lambda[i] = (string)element.parametrs["L"].Clone();
                argument[i] = (string)box.Items[i].ToString().Clone();

                //element.parametrs.Clear();
            }
            // исследование зависимости
            new Function(Lambda, argument);

            box.SelectedIndex = 0;
            if (to_clear) 
            { 
                box.Items.Clear();
                box.Text = koef;
            }
            foreach (ComboBox b in boxes)
            {
                if (b.Items.Count == 0 && b.Text == "5.001") b.Text = "";
            }
        }
    }
}
