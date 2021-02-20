using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Threading;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Globalization;

namespace Lambda
{
    public partial class Main_window : Form
    {
        XmlDocument xDoc = new XmlDocument();

        //сохранение подробного отчета
        private void full_rep_Click(object sender, EventArgs e)
        {
            if (elements.Count == 0) return;

            // максимальное кол-во параметров
            int max = 0;
            const int permanent_cells = 6;
            foreach (Electronics element in elements)
            {
                if (element.parametrs.Count > max) max = element.parametrs.Count;
            }

            double lambda = 0;
            recalc();
            Table table = new Table();

            TableProperties tableProperties1 = new TableProperties();
            TableWidth tableWidth1 = new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct };

            TableGrid tableGrid1 = new TableGrid();
            for(int g = 0; g < permanent_cells + max;g++)
            {
                tableGrid1.AppendChild(new GridColumn() { Width = (10296/(permanent_cells + max)).ToString() });
            }

            table.Append(new TableProperties(
                new TableStyle() { Val = "TableGrid" },
                new TableWidth() { Width = "0", Type = TableWidthUnitValues.Auto },
                new TableLook()
                {
                    Val = "04A0",
                    FirstRow = true,
                    LastRow = false,
                    FirstColumn = true,
                    LastColumn = false,
                    NoHorizontalBand = false,
                    NoVerticalBand = true
                }

                ));
            table.Append(tableGrid1);


            List<TableCell> tableCellList = new List<TableCell>();

            for (int index = 0; index < 7; ++index)
                tableCellList.Add(new TableCell(new Paragraph()));
            // paragraph prop
            ParagraphProperties pPr = new ParagraphProperties(new Indentation() { Left = "80" , Right="80"});
            //
            // Borders for cells
            TableCellProperties tclPr = new TableCellProperties();
            TableCellBorders tableCellBorders2 = new TableCellBorders();
            TopBorder topBorder1 = new TopBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };
            LeftBorder leftBorder1 = new LeftBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };
            BottomBorder bottomBorder1 = new BottomBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };
            RightBorder rightBorder2 = new RightBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };

            tableCellBorders2.Append(topBorder1);
            tableCellBorders2.Append(leftBorder1);
            tableCellBorders2.Append(bottomBorder1);
            tableCellBorders2.Append(rightBorder2);
            
            tclPr.AppendChild(tableCellBorders2);

            tableCellList[0].Append((OpenXmlElement)tclPr.Clone(),   new Paragraph((ParagraphProperties)pPr.Clone(), new Run(  new Text("Название")) ));
            tableCellList[1].Append((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone(), new Run(new Text(" Позиционное обозначение "))));
            tableCellList[2].Append((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone(), new Run(new Text(" Тип "))));
            tableCellList[3].Append((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone(), new Run(new Text(" Кол-во "))));
            tableCellList[4].Append((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone(), new Run(new Text(" λ * n "))));
            tableCellList[5].Append((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone(), new Run(new Text(" λ "))));

            TableRow tableRow1 = new TableRow();

            foreach (TableCell tableCell in tableCellList)
                tableRow1.Append( tableCell );
            table.Append(tableRow1);

            foreach (Electronics element in this.elements)
            {
                lambda += Convert.ToDouble(element.LN.Replace(",",".").Replace(".", ",").Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));

                TableRow tableRow2 = new TableRow();
                tableRow2.Append(new TableCell((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone(), new Run(new Text(element.input["Название"])))));
                tableRow2.Append(new TableCell((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone(), new Run(new Text(element.input["Позиционное обозначение"])))));

                if (element.input.ElementAt(0).Key.Contains("Группа"))
                    tableRow2.Append(new TableCell((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone(), new Run(new Text(element.text_type+", " + element.input.ElementAt(0).Value)))));
                else
                    tableRow2.Append(new TableCell((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone(), new Run(new Text(element.text_type)))));

                tableRow2.Append(new TableCell((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone(), new Run(new Text(element.input["Количество"])))));
                tableRow2.Append(new TableCell((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone(), new Run(new Text(element.LN)))));

                foreach(KeyValuePair<string,string> pair in element.parametrs)
                {
                    TableCell tableCell = new TableCell();
                    tableCell.Append((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone(), new Run(new Text(pair.Key + "=" + pair.Value))));
                    tableRow2.Append(tableCell);
                }
                for (int i= element.parametrs.Count; i < max; i++)
                {
                    TableCell tableCell = new TableCell((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone()));
                    tableRow2.Append(tableCell);
                }
                table.Append(tableRow2);
                 
            }
            // создание строки коэффициентов и пустых ячеек
            for (int i = 2; i < table.ChildElements.Count - 1; ++i)
            {
                for (int count = table.ChildElements[i].ChildElements.Count; count < max+ permanent_cells-1; count++)
                {
                        table.ChildElements[i].Append(new TableCell((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone())));
                }
            }
            table.ChildElements[2].ChildElements[permanent_cells].Append(new TableCellProperties(new HorizontalMerge()
            {
                Val = MergedCellValues.Restart
            }));
            for (int i = permanent_cells; i < max+ permanent_cells - 2; ++i)
            {
                table.ChildElements[2].ChildElements[i + 1].Append(new TableCellProperties( new HorizontalMerge()   
                {
                    Val =  MergedCellValues.Continue 
                }));
            }
            table.ChildElements[2].ChildElements[permanent_cells].Append((OpenXmlElement)tclPr.Clone(), new Paragraph((ParagraphProperties)pPr.Clone(), new Run(new Text("Коэффициенты"))) );
            //


            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = this.path;
            saveFileDialog.DefaultExt = "docx";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "Отчет | *.docx";
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)  return;

            WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Create(saveFileDialog.FileName, WordprocessingDocumentType.Document);
            MainDocumentPart mainDocumentPart = wordprocessingDocument.AddMainDocumentPart();
            mainDocumentPart.Document = new Document();
            mainDocumentPart.Document.AppendChild(new Body());
            double num2 = 12240.0;
            double num3 = 15840.0;
            SectionProperties sectionProperties = new SectionProperties();
            PageSize pageSize = new PageSize()
            {
                Width = (uint)num3,
                Height = (uint)num2,
                Orient = PageOrientationValues.Landscape

            }; 
            PageMargin pageMargin = new PageMargin() { Top = 1008, Right = (UInt32Value)500U, Bottom = 500, Left = (UInt32Value)500U, Header = (UInt32Value)720U, Footer = (UInt32Value)720U, Gutter = (UInt32Value)0U };
            sectionProperties.Append( pageSize ); 
            sectionProperties.Append(pageMargin);
            mainDocumentPart.Document.Body.Append(sectionProperties );
            mainDocumentPart.Document.Body.Append(new Paragraph(new Run(new Text("Температура окружающей среды, °С: "+T))));
            var b = new conditions(this);
            mainDocumentPart.Document.Body.Append(new Paragraph(new Run(new Text("Группа аппаратуры по ГОСТ Р В 20.39.304-98 - " + b.get_Pe_name() ))));
                                  
            double m = Math.Pow(10, 3 - Math.Ceiling(Math.Log10(lambda)));
            lambda = ((Math.Round(lambda * m)) / m);

            mainDocumentPart.Document.Body.Append(new Paragraph(new Run(new Text("Расчетная интенсивность отказов: " + lambda+ " 1/ч"))));
            mainDocumentPart.Document.Body.Append( table);
            try
            {
                wordprocessingDocument.MainDocumentPart.Document.Save();
                wordprocessingDocument.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        // загрузка
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlDocument xDoc = new XmlDocument();

            OpenFileDialog openfiledialog1 = new OpenFileDialog();
            openfiledialog1.InitialDirectory = path;

            if (openfiledialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    elements.Clear();
                    treeView1.Nodes.Clear();
                    xDoc.Load(openfiledialog1.FileName);

                    if (xDoc.DocumentElement.Attributes["Pe"] != null)
                    {
                        T = xDoc.DocumentElement.Attributes["T"].Value;
                        Pe = xDoc.DocumentElement.Attributes["Pe"].Value;
                        Pe = conditions.get_Pe(Convert.ToInt32(xDoc.DocumentElement.Attributes["Pe"].Value));
                    }

                    load(xDoc.DocumentElement);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            treeView1.ExpandAll();
        }

        //сохранение
        public void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            recalc();
            xDoc.Load("template.xml");
            xDoc.DocumentElement.Attributes["T"].Value = T;
            xDoc.DocumentElement.Attributes["Pe"].Value = Pe;
            nodes(treeView1.Nodes[0], xDoc.DocumentElement);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = path;
            saveFileDialog1.DefaultExt = "xml";
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.Filter = "Отчет | *.xml";
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            xDoc.Save(saveFileDialog1.FileName);

        }

        // обход treeview для сохранения
        private void nodes(TreeNode node, XmlElement xmlElement)
        {
            // для узлов
            if (node.Tag == null)
            {
                XmlElement parent = xDoc.CreateElement("node");
                XmlAttribute attribute = xDoc.CreateAttribute("name");
                attribute.AppendChild(xDoc.CreateTextNode(node.Text));
                parent.Attributes.Append(attribute);

                xmlElement.AppendChild(parent);
                if (node.Nodes != null)
                {
                    foreach (TreeNode childnode in node.Nodes)
                    {
                        xDoc.CreateElement("node");
                        nodes(childnode, parent);
                    }
                }

            }
            // для элементов
            else
            {
                xmlElement.AppendChild(xDoc.ImportNode(elements[Convert.ToInt32(node.Tag)].ToXML(),true));
            }
        }

        // чтение загружаемого файла
        private void load(XmlNode node)
        {
            // для Root
            if (treeView1.SelectedNode == null)
            {
                treeView1.Nodes.Add(node.FirstChild.Attributes["name"].Value);

                foreach (XmlElement childnode in node.FirstChild.ChildNodes)
                {
                    treeView1.SelectedNode = treeView1.Nodes[0];
                    load(childnode);
                }
            }

            else
            {
                // для узлов
                if (node.Name == "node")
                {
                    TreeNode tree_node = new TreeNode(node.Attributes["name"].Value);
                    treeView1.SelectedNode.Nodes.Add(tree_node);
                    foreach (XmlElement childnode in node.ChildNodes)
                    {
                        treeView1.SelectedNode = tree_node;
                        load(childnode);
                    }
                }
                // для устройств
                else
                {
                    TreeNode tree_node = new TreeNode(node.Attributes["name"].Value);
                    tree_node.Tag = elements.Count;
                    treeView1.SelectedNode.Nodes.Add(tree_node);
                    
                    elements.Add(Electronics.getElectronics(node));
                }
            }
        }

        // Закрыть текущий файл
        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (elements.Count != 0)
            {
                Closing_warning warning = new Closing_warning(this);
                warning.ShowDialog();
            }
        }

        // связывание имени ноды с типом
        static public Type get_type(string type)
        {
            switch (type)
            {
                case ("elements/BDIC"):
                    return new Digital_Bipolar().GetType();
                case ("elements/BAIC"):
                    return new Digital_MOS().GetType();
                case ("elements/PLD_IC"):
                    return new Digital_Bipolar().GetType();
                case ("elements/SAW_DIC"):
                    return new Analog_Bipolar_and_MOS().GetType();
                case ("elements/SAW_AIC"):
                    return new Digital_MOS().GetType();
                case ("elements/SAW_PLD"):
                    return new Analog_Bipolar_and_MOS().GetType();
                case ("elements/SAW_MMC_ROM"):
                    return new Memory().GetType();
                case ("elements/SAW_MMC_PROM"):
                    return new Memory().GetType();
                case ("elements/SAW_DRAM"):
                    return new Memory().GetType();
                case ("elements/SAW_SRAM"):
                    return new Memory().GetType();
                case ("elements/BROM"):
                    return new Memory().GetType();
                case ("elements/BSRAM"):
                    return new Memory().GetType();
                case ("elements/BMPU"):
                    return new Digital_Bipolar().GetType();
                case ("elements/SAWMPU"):
                    return new Analog_Bipolar_and_MOS().GetType();
                case ("elements/CLSI"):
                    return new CLSI().GetType();
                case ("elements/GaAsMMIC_SC"):
                    return new GaAsMMIC_SC().GetType();
                case ("elements/SAW"):
                    return new SAW().GetType();
                case ("elements/LFD"):
                    return new LFD().GetType();
                case ("elements/HFD"):
                    return new HFD().GetType();
                case ("elements/LFT"):
                    return new LFT().GetType();
                case ("elements/LFTSFE"):
                    return new LFTSFE().GetType();
                case ("elements/SPT"):
                    return new SPT().GetType();
                case ("elements/LNHFBT"):
                    return new LNHFBT().GetType();
                case ("elements/PHFBT"):
                    return new PHFBT().GetType();
                case ("elements/HFGaAsFET"):
                    return new HFGaAsFET().GetType();
                case ("elements/HFSiFET"):
                    return new HFSiFET().GetType();
                case ("elements/Thyristor"):
                    return new Thyristor().GetType();
                case ("elements/DOE"):
                    return new DOE().GetType();
                case ("elements/LDD"):
                    return new LDD().GetType();
                case ("elements/LD"):
                    return new LD().GetType();
                case ("elements/R"):
                    return new Resistors().GetType();
                case ("elements/C"):
                    return new Capacitor().GetType();
                case ("elements/Transformer"):
                    return new Transformer().GetType();
                case ("elements/SP"):
                    return new SP().GetType();
                case ("elements/Connectors"):
                    return new Connectors().GetType();
                case ("elements/Relay"):
                    return new Relay().GetType();
                case ("elements/Relay2"):
                    return new Relay2().GetType();
                case ("elements/Lamp"):
                    return new Lamp().GetType();
                case ("elements/QD"):
                    return new QD().GetType();
                case ("elements/Fuse"):
                    return new Fuse().GetType();
            }
            return null;
        }

        // пересчет с условиями T, Pe
        public void recalc()
        {
            foreach (Electronics element in elements)
            {
                element.calc_input["T"] = Convert.ToDouble(T);
                //element.calc_input["Т"] = Convert.ToDouble(T);
                element.calc_input["Pe"] = Convert.ToDouble(Pe);
                element.input["Температура"] = T;
                element.parametrs.Clear();
                element.calc(element.calc_input);
            }
        }
    }
}
