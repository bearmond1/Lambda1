namespace Lambda
{
    partial class pick_element
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(pick_element));
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBox2
            // 
            this.listBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Items.AddRange(new object[] {
            "Биполярные цифровые интегральные схемы (Bipolar  Digital Gate/Logic Arrays)",
            "Биполярные аналоговые интегральные схемы (Bipolar  Linear Gate/Logic Arrays)",
            "Биполярные ПЛМ интегральные схемы (Bipolar PLA/PAL)",
            "МОП Биполярные цифровые интегральные схемы (MOS  Bipolar  Digital Gate/Logic Arra" +
                "ys)",
            "МОП Биполярные аналоговые интегральные схемы (MOS  Bipolar  Linear Gate/Logic Arr" +
                "ays)",
            "МОП Биполярные ПЛМ интегральные схемы (MOS Bipolar PLA/PAL)",
            "ПЗУ МОП (MOS ROM)",
            "ППЗУ, РПЗУ МОП (MOS PROM, UVEPROM, EEPROM, EAPROM)",
            "Динамические ОЗУ (MOS DRAM)",
            "Статические ОЗУ (MOS SRAM (MOS&BiMOS))",
            "ПЗУ, ППЗУ Биполярные (Bipolar  ROM, PROM)",
            "Статические ОЗУ Биполярные (Bipolar  SRAM)",
            "Микропроцессоры Биполярные (Bipolar  Microprocessor)",
            "Микропроцессоры МОП (MOS  Microprocessor)",
            "Микросхемы сверхбольшой степени интеграции  (Microcircuits, vhsic/vhsic-like and " +
                "vlsi cmos)",
            "Микросхемы арсенидогаллиевые (GaAs)  (Microcircuits, GaAs MMIC )",
            "Микросхемы ПАВ  (Microcircuits, SAW DEVICES)",
            "Диоды низкочастотные (Diodes,, Low frequency)",
            "Диоды высокочастотные (Diodes, High frequency)",
            "Транзисторы низкочастотные  (Transistors, Low frequency)",
            "Транзисторы низкочастотные кремниевые полевые  (Transistors, Low frequency Si FE " +
                ")",
            "Транзисторы однопереходные (Transistors, Unijunction)",
            "Транзисторы малошумящие высокочастотные биполярные  (Transistors, Low Noise, High" +
                " Frequency,  Bipolar   (Frequency > 200 MHz, Power < 1W))",
            "Транзисторы мощные высокочастотные биполярные (Transistors, High Power High Frequ" +
                "ency Bipolar)",
            "Транзисторы высокочастотные арсенидогаллиевые полевые (Transistors, High Frequenc" +
                "y,  GaAs FET ( > 1 GHz))",
            "Транзисторы высокочастотные полевые кремниевые (Transistors, High Frequency,  SI " +
                "FET (Avg. Power <300 mW, Freq. > 400 MHz))",
            "Тиристоры (Thyristors and SCRS)",
            "Детекторы, оптопары, излучатели (Optoelectronics, Detectors, Isolators, Emitters)" +
                "",
            "Буквенно-цифровые дисплеи (Optoelectronics, Alphanumeric Displays)",
            "Лазерные диоды (Optoelectronics, Laser Diode (< 25 amps))",
            "Резисторы (Resistors)",
            "Конденсаторы (Capacitor)",
            "Трансформаторы (Inductive Devices, Transformers)",
            "Коммутационные изделия (Switches)",
            "Соеденители (Connectors, General (expect printed circuit board)",
            "Механические реле (Relays, Mechanical)",
            "Твердотельные с временной задержкой реле и гибридные (Relays, Solid State and Tim" +
                "e Delay)",
            "Лампы (Lamps)",
            "Кварцевые приборы (Quartz Crystals)",
            "Предохранители (Fuses)"});
            this.listBox2.Location = new System.Drawing.Point(12, 10);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(759, 537);
            this.listBox2.TabIndex = 4;
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged_1);
            // 
            // pick_element
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 565);
            this.Controls.Add(this.listBox2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "pick_element";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выбор элемента";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListBox listBox2;
    }
}