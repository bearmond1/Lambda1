﻿using System;
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
    public partial class pick_element : Form
    {
        Main_window x;
        public pick_element(Main_window _x)
        {
            InitializeComponent();
            x = _x;
            Show();
        }
        private void listBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            switch (listBox2.SelectedIndex)
            {
                case (0):
                    new_element form2 = new new_element("elements/BDIC", x);
                    x.elements.Add(new Digital_Bipolar());
                    x.image_index = 0;
                    break;
                case (1):
                    new_element form3 = new new_element("elements/BAIC", x);
                    x.elements.Add(new Digital_MOS());
                    x.image_index = 0;
                    break;
                case (2):
                    new_element form4 = new new_element("elements/PLD_IC", x);
                    x.elements.Add(new Digital_Bipolar());
                    x.image_index = 0;
                    break;
                case (3):
                    new_element form5 = new new_element("elements/SAW_DIC", x);
                    x.elements.Add(new Analog_Bipolar_and_MOS());
                    x.image_index = 0;
                    break;
                case (4):
                    new_element form6 = new new_element("elements/SAW_AIC", x);
                    x.elements.Add(new Digital_MOS());
                    x.image_index = 0;
                    break;
                case (5):
                    new_element form7 = new new_element("elements/SAW_PLD", x);
                    x.elements.Add(new Analog_Bipolar_and_MOS());
                    x.image_index = 0;
                    break;
                case (6):
                    new_element form8 = new new_element("elements/SAW_MMC_ROM", x);
                    x.elements.Add(new Memory());
                    x.image_index = 0;
                    break;
                case (7):
                    new_element form9 = new new_element("elements/SAW_MMC_PROM", x);
                    x.elements.Add(new Memory());
                    x.image_index = 0;
                    break;
                case (8):
                    new_element form10 = new new_element("elements/SAW_DRAM", x);
                    x.elements.Add(new Memory());
                    x.image_index = 0;
                    break;
                case (9):
                    new_element form11 = new new_element("elements/SAW_SRAM", x);
                    x.elements.Add(new Memory());
                    x.image_index = 0;
                    break;
                case (10):
                    new_element form12 = new new_element("elements/BROM", x);
                    x.elements.Add(new Memory());
                    x.image_index = 0;
                    break;
                case (11):
                    new_element form13 = new new_element("elements/BSRAM", x);
                    x.elements.Add(new Memory());
                    x.image_index = 0;
                    break;
                case (12):
                    new_element form14 = new new_element("elements/BMPU", x);
                    x.elements.Add(new Digital_Bipolar());
                    x.image_index = 0;
                    break;
                case (13):
                    new_element form15 = new new_element("elements/SAWMPU", x);
                    x.elements.Add(new Analog_Bipolar_and_MOS());
                    x.image_index = 0;
                    break;
                case (14):
                    new_element form16 = new new_element("elements/CLSI", x);
                    x.elements.Add(new CLSI());
                    x.image_index = 0;
                    break;
                case (15):
                    new_element form17 = new new_element("elements/GaAsMMIC_SC", x);
                    x.elements.Add(new GaAsMMIC_SC());
                    x.image_index = 0;
                    break;
                case (16):
                    new_element form18 = new new_element("elements/SAW", x);
                    x.elements.Add(new SAW());
                    x.image_index = 0;
                    break;
                case (17):
                    new_element form19 = new new_element("elements/LFD", x);
                    x.elements.Add(new LFD());
                    x.image_index = 1;
                    break;
                case (18):
                    new_element form20 = new new_element("elements/HFD", x);
                    x.elements.Add(new HFD());
                    x.image_index = 1;
                    break;
                case (19):
                    new_element form21 = new new_element("elements/LFT", x);
                    x.elements.Add(new LFT());
                    x.image_index = 2;
                    break;
                case (20):
                    new_element form22 = new new_element("elements/LFTSFE", x);
                    x.elements.Add(new LFTSFE());
                    x.image_index = 2;
                    break;
                case (21):
                    new_element form23 = new new_element("elements/SPT", x);
                    x.elements.Add(new SPT());
                    x.image_index = 2;
                    break;
                case (22):
                    new_element form24 = new new_element("elements/LNHFBT", x);
                    x.elements.Add(new LNHFBT());
                    x.image_index = 2;
                    break;
                case (23):
                    new_element form25 = new new_element("elements/PHFBT", x);
                    x.elements.Add(new PHFBT());
                    x.image_index = 2;
                    break;
                case (24):
                    new_element form26 = new new_element("elements/HFGaAsFET", x);
                    x.elements.Add(new HFGaAsFET());
                    x.image_index = 2;
                    break;
                case (25):
                    new_element form27 = new new_element("elements/HFSiFET",x);
                    x.elements.Add(new HFSiFET());
                    x.image_index = 2;
                    break;
                case (26):
                    new_element form28 = new new_element("elements/Thyristor", x);
                    x.elements.Add(new Thyristor());
                    x.image_index = 2;
                    break;
                case (27):
                    new_element form29 = new new_element("elements/DOE", x);
                    x.elements.Add(new DOE());
                    break;
                case (28):
                    new_element form30 = new new_element("elements/LDD", x);
                    x.elements.Add(new LDD());
                    break;
                case (29):
                    new_element form31 = new new_element("elements/LD", x);
                    x.elements.Add(new LD());
                    break;
                case (30):
                    new_element form32 = new new_element("elements/R", x);
                    x.elements.Add(new Resistors());
                    break;
                case (31):
                    new_element form33 = new new_element("elements/C", x);
                    x.elements.Add(new Capacitor());
                    break;
                case (32):
                    new_element form34 = new new_element("elements/Transformer", x);
                    x.elements.Add(new Transformer());
                    break;
                case (33):
                    new_element form35 = new new_element("elements/SP", x);
                    x.elements.Add(new SP());
                    break;
                case (34):
                    new_element form36 = new new_element("elements/Connectors", x);
                    x.elements.Add(new Connectors());
                    break;
                case (35):
                    new_element form37 = new new_element("elements/Relay", x);
                    x.elements.Add(new Relay());
                    break;
                case (36):
                    new_element form38 = new new_element("elements/Relay2", x);
                    x.elements.Add(new Relay2());
                    break;
                case (37):
                    new_element form39 = new new_element("elements/Lamp", x);
                    x.elements.Add(new Lamp());
                    break;
                case (38):
                    new_element form40 = new new_element("elements/QD", x);
                    x.elements.Add(new QD());
                    break;
                case (39):
                    new_element form41 = new new_element("elements/Fuse", x);
                    x.elements.Add(new Fuse());
                    break;
            }
            Close();
        }

    }
}
