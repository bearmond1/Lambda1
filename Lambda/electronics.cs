using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Reflection;
using System.Security;
using System.Xml;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Globalization;


namespace Lambda
{
    public class Electronics : ICloneable
    {
        public Dictionary<string, string> parametrs = new Dictionary<string, string>();
        public Dictionary<string, string> input = new Dictionary<string, string>();
        public Dictionary<string, double> calc_input = new Dictionary<string, double>();

        protected Dictionary<int, double> Pe_1_17 =  new Dictionary<int, double>() { {0,0.5 }, { 1,2}, { 2,4}, { 3,4}, { 5,6}, { 6, 4 }, { 7, 5 },{ 8, 5 },{ 9, 8 },{ 10, 8 },{ 11, 0.5 }, { 12, 5 },{ 13, 12 },{ 14, 220 } };
        protected Dictionary<int, double> Pe_18_24 = new Dictionary<int, double>() { { 0, 1 }, { 1, 6 }, { 2, 9 }, { 3, 9 }, { 5, 19 }, { 6, 13 }, { 7, 29 }, { 8, 20 }, { 9, 43 }, { 10, 24 }, { 11, 0.5 }, { 12, 14 }, { 13, 32 }, { 14, 320 } };

        public string type, text_type, LN;
        virtual public void calc(Dictionary<string, double> calc_input) { }

        public void round()
        {
            Dictionary<string, string> new_parametrs = new Dictionary<string, string>();

            foreach(KeyValuePair<string, string> par in parametrs)
            {
                double val;
                Double.TryParse(par.Value, out val);
                double m = Math.Pow(10, 3 - Math.Ceiling(Math.Log10(val)));
                val = ((Math.Round(val * m)) / m);
                new_parametrs.Add(par.Key,val.ToString());
            }
            parametrs = new_parametrs;
        }

        public object Clone()
        {
            Electronics x = (Electronics)Activator.CreateInstance(this.GetType());
            x.input = this.input;
            x.parametrs = this.parametrs;
            x.calc_input = this.calc_input;
            x.type = this.type;
            x.text_type = this.text_type;
            x.LN = this.LN;
            return x;
        }

        public XmlElement ToXML()
        {
            XmlDocument xDoc = new XmlDocument();
            //int index = Convert.ToInt32(node.Tag);

            XmlElement device = xDoc.CreateElement("device");

            XmlAttribute name = xDoc.CreateAttribute("name");
            name.Value = this.input["Название"];
            device.Attributes.Append(name);

            XmlAttribute type = xDoc.CreateAttribute("type");
            type.AppendChild(xDoc.CreateTextNode(this.type));
            device.Attributes.Append(type);

            XmlAttribute txt_type = xDoc.CreateAttribute("txt_type");
            txt_type.AppendChild(xDoc.CreateTextNode(this.text_type));
            device.Attributes.Append(txt_type);

            XmlAttribute LN = xDoc.CreateAttribute("LN");
            LN.AppendChild(xDoc.CreateTextNode(this.LN));
            device.Attributes.Append(LN);

            XmlElement parametrs = xDoc.CreateElement("parametrs");
            XmlElement input = xDoc.CreateElement("input");
            XmlElement calc_input = xDoc.CreateElement("calc_input");

            foreach (KeyValuePair<string, string> par in this.parametrs)
            {
                XmlAttribute param_type = xDoc.CreateAttribute("type");
                XmlAttribute name_value = xDoc.CreateAttribute("name");

                param_type.AppendChild(xDoc.CreateTextNode(par.Key));
                name_value.AppendChild(xDoc.CreateTextNode(par.Value));

                XmlElement param = xDoc.CreateElement("parametr");
                param.Attributes.Append(param_type);
                param.Attributes.Append(name_value);

                parametrs.AppendChild(param);
            }
            device.AppendChild(parametrs);

            foreach (KeyValuePair<string, string> par in this.input)
            {
                XmlAttribute param_type = xDoc.CreateAttribute("type");
                XmlAttribute name_value = xDoc.CreateAttribute("name");

                param_type.AppendChild(xDoc.CreateTextNode(par.Key));
                name_value.AppendChild(xDoc.CreateTextNode(par.Value));

                XmlElement param = xDoc.CreateElement("input");
                param.Attributes.Append(param_type);
                param.Attributes.Append(name_value);

                input.AppendChild(param);
            }
            device.AppendChild(input);

            foreach (KeyValuePair<string, double> par in this.calc_input)
            {
                XmlAttribute param_type = xDoc.CreateAttribute("type");
                XmlAttribute name_value = xDoc.CreateAttribute("name");

                param_type.AppendChild(xDoc.CreateTextNode(par.Key));
                name_value.AppendChild(xDoc.CreateTextNode(par.Value.ToString()));

                XmlElement param = xDoc.CreateElement("calc_input");
                param.Attributes.Append(param_type);
                param.Attributes.Append(name_value);

                calc_input.AppendChild(param);
            }
            device.AppendChild(calc_input);
            return device;
        }

        static public Electronics getElectronics(XmlNode node)
        {
            Dictionary<string, string> parametrs = new Dictionary<string, string>();
            Dictionary<string, string> input = new Dictionary<string, string>();
            Dictionary<string, double> calc_input = new Dictionary<string, double>();

            foreach (XmlNode parameter in node.FirstChild.ChildNodes)
            {
                parametrs.Add(parameter.Attributes["type"].Value, parameter.Attributes["name"].Value);
            }

            foreach (XmlNode parameter in node.ChildNodes[1].ChildNodes)
            {
                input.Add(parameter.Attributes["type"].Value, parameter.Attributes["name"].Value);
            }

            foreach (XmlNode parameter in node.LastChild.ChildNodes)
            {
                double y;
                string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                if (double.TryParse(parameter.Attributes["name"].Value.Replace(','.ToString(), sep), out y))
                    calc_input.Add(parameter.Attributes["type"].Value, y);
                else
                {
                    if (double.TryParse(parameter.Attributes["name"].Value.Replace('.'.ToString(), sep), out y))
                        calc_input.Add(parameter.Attributes["type"].Value, y);
                    else
                    {
                        MessageBox.Show("Неверный формат числа в XML");
                        break;
                    }
                }
            }

            var x = (Electronics)Activator.CreateInstance(Main_window.get_type(node.Attributes["type"].Value));
            x.input = input;
            x.calc_input = calc_input;
            x.parametrs = parametrs;
            x.type = node.Attributes["type"].Value;
            x.text_type = node.Attributes["txt_type"].Value;
            x.LN = node.Attributes["LN"].Value;
            return x;
        }
    }

    class Digital_Bipolar : Electronics
    {
        public double Ea = 0.4;
        public double Nt = 298;
        public override void calc(Dictionary<string, double> calc_input)
        {
            double C1 = calc_input["C1"];
            double Pq = calc_input["Pq"];
            double C2 = calc_input["C2"];
            double N = calc_input["N"];
            double T = calc_input["T"];
            double Pe = calc_input["Pe"];

            double Pt = 0.1 * Math.Exp((-this.Ea / 8.617E-5) * ((1.0 / (T + 273)) - 1.0 / Nt));
            switch (C2)
            {
                case (1):
                    C2 = 2.8E-4 * Math.Pow(N, 1.08);
                    break;
                case (2):
                    C2 = 9E-5 * Math.Pow(N, 1.51);
                    break;
                case (3):
                    C2 = 3E-5 * Math.Pow(N, 1.82);
                    break;
                case (4):
                    C2 = 3E-5 * Math.Pow(N, 2.01);
                    break;
                case (5):
                    C2 = 3.6E-4 * Math.Pow(N, 1.08);
                    break;
            }

            double lambda = (C1 * Pt + C2 * Pe) * Pq / 1E6;

            //input = _input;
            //calc_input = par;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("C1", C1.ToString());
            parametrs.Add("C2", C2.ToString());
            parametrs.Add("Pt", Pt.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }

    }
    class Digital_MOS : Electronics
    {
        public new double Ea = 0.35;
        public double Nt = 298;
        public override void calc(Dictionary<string, double> calc_input)
        {
            double C1 = calc_input["C1"];
            double Pq = calc_input["Pq"];
            double C2 = calc_input["C2"];
            double N = calc_input["N"];
            double T = calc_input["T"];
            double Pe = calc_input["Pe"];

            double Pt = 0.1 * Math.Exp((-this.Ea / 8.617E-5) * ((1.0 / (T + 273)) - 1.0 / Nt));
            switch (C2)
            {
                case (1):
                    C2 = 2.8E-4 * Math.Pow(N, 1.08);
                    break;
                case (2):
                    C2 = 9E-5 * Math.Pow(N, 1.51);
                    break;
                case (3):
                    C2 = 3E-5 * Math.Pow(N, 1.82);
                    break;
                case (4):
                    C2 = 3E-5 * Math.Pow(N, 2.01);
                    break;
                case (5):
                    C2 = 3.6E-4 * Math.Pow(N, 1.08);
                    break;
            }

            double lambda = (C1 * Pt + C2 * Pe) * Pq / 1E6;

            //input = _input;
            //calc_input = par;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("C1", C1.ToString());
            parametrs.Add("C2", C2.ToString());
            parametrs.Add("Pt", Pt.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }

    class Analog_Bipolar_and_MOS: Electronics
    {
        public new double Ea = 0.65;
        public double Nt = 298;
        public override void calc(Dictionary<string, double> calc_input)
        {
            double C1 = calc_input["C1"];
            double Pq = calc_input["Pq"];
            double C2 = calc_input["C2"];
            double N = calc_input["N"];
            double T = calc_input["T"];
            double Pe = calc_input["Pe"];

            double Pt = 0.1 * Math.Exp((-this.Ea / 8.617E-5) * ((1.0 / (T + 273)) - 1.0 / Nt));
            switch (C2)
            {
                case (1):
                    C2 = 2.8E-4 * Math.Pow(N, 1.08);
                    break;
                case (2):
                    C2 = 9E-5 * Math.Pow(N, 1.51);
                    break;
                case (3):
                    C2 = 3E-5 * Math.Pow(N, 1.82);
                    break;
                case (4):
                    C2 = 3E-5 * Math.Pow(N, 2.01);
                    break;
                case (5):
                    C2 = 3.6E-4 * Math.Pow(N, 1.08);
                    break;
            }

            double lambda = (C1 * Pt + C2 * Pe) * Pq / 1E6;

            //input = _input;
            //calc_input = par;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("C1", C1.ToString());
            parametrs.Add("C2", C2.ToString());
            parametrs.Add("Pt", Pt.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }
    class Memory : Electronics
    {
        public new double Ea = 0.6;
        public double Nt = 298;
        public override void calc(Dictionary<string, double> calc_input)
        {
            double C1 = calc_input["C1"];
            double Pq = calc_input["Pq"];
            double C2 = calc_input["C2"];
            double N = calc_input["N"];
            double T = calc_input["T"];
            double Pe = calc_input["Pe"];

            double Pt = 0.1 * Math.Exp((-this.Ea / 8.617E-5) * ((1.0 / (T + 273)) - 1.0 / Nt));
            switch (C2)
            {
                case (1):
                    C2 = 2.8E-4 * Math.Pow(N, 1.08);
                    break;
                case (2):
                    C2 = 9E-5 * Math.Pow(N, 1.51);
                    break;
                case (3):
                    C2 = 3E-5 * Math.Pow(N, 1.82);
                    break;
                case (4):
                    C2 = 3E-5 * Math.Pow(N, 2.01);
                    break;
                case (5):
                    C2 = 3.6E-4 * Math.Pow(N, 1.08);
                    break;
            }

            double lambda = (C1 * Pt + C2 * Pe) * Pq / 1E6;

            //input = _input;
            //calc_input = par;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("C1", C1.ToString());
            parametrs.Add("C2", C2.ToString());
            parametrs.Add("Pt", Pt.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }
    //
    class GaAsMMIC_SC:Electronics
    {
        public GaAsMMIC_SC() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double C1 = calc_input["C1"];
            double Pa = calc_input["Pa"];
            double Pq = calc_input["Pq"];
            double C2 = calc_input["C2"];
            double N = calc_input["N"];
            double T = calc_input["T"];
            double Pe = calc_input["Pe"];

            double Ea = 1.5;
            if(C1 > 8) { Ea = 1.4; }

            switch (C2)
            {
                case (1):
                    C2 = 2.8e-4 * Math.Pow(N, 1.08);
                    break;
                case (2):
                    C2 = 9e-5 * Math.Pow(N, 1.51);
                    break;
                case (3):
                    C2 = 3e-5 * Math.Pow(N, 1.82);
                    break;
                case (4):
                    C2 = 3e-5 * Math.Pow(N, 2.01);
                    break;
                case (5):
                    C2 = 3.6e-4 * Math.Pow(N, 1.08);
                    break;
            }
            T = (0.1 * Math.Exp((-Ea / 8.617E-5) * ((1.0 / (T + 273)) - 1.0 / 423)));
            double lambda = (Pa * C1 * T + C2 * Pe) * Pq / 1E6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("C1", C1.ToString());
            parametrs.Add("C2", C2.ToString());
            parametrs.Add("Pt", T.ToString());
            parametrs.Add("Pa", Pa.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }

    }
    //
    class SAW:Electronics
    {
        public SAW() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];
            double lambda = 2.1*Pq*Pe*1E-6;
            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }
    //
    class LFD:Electronics  
    {
        public LFD() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Lb = calc_input["Lb"];
            double Pt = calc_input["T"];
            double Kel = calc_input["Kel"];
            double Pc = calc_input["Pc"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];
            double Ps;
            if (Lb == 0.0034 || Lb == 0.002)
                Pt = Math.Exp(-1925*((1.0 / (Pt + 273)) - (1.0 / 298)));
            else Pt = Math.Exp(-3091 * ((1.0 / (Pt + 273)) - (1.0 / 298)));

            if (Lb == 0.0034 || Lb == 0.002 || Lb == 0.0013)
                Ps = 1;
            else if (Kel <= 0.3) Ps = 0.054;
            else Ps = Math.Pow(Kel, 2.43);
            double lambda = Lb * Pt * Ps * Pc * Pq *Pe / 1E6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Ps", Ps.ToString());
            parametrs.Add("Pc", Pc.ToString());
            parametrs.Add("Pt", Pt.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());

            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }
    // 
    class CLSI :Electronics 
    {
        public CLSI() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Lbd = calc_input["Lbd"];
            double Pmfg = calc_input["Pmfg"];
            double Ppt = calc_input["Ppt"];
            double Pt = calc_input["T"];
            double A = calc_input["A"];
            double Xs = calc_input["Xs"];
            double N = calc_input["N"];
            double V = calc_input["V"];
            double Pq = calc_input["Pq"];
            double type = calc_input["type"];
            double Pe = calc_input["Pe"];

            double Pcd = 0.36 + (0.64 * (A / 0.21) * (2 / Xs) * (2 / Xs));
            double Lbp = 0.0022 + (1.72E-5) * N;
            double Leos = (-Math.Log(1 - 0.00057 * Math.Exp(-0.0002 * V))) / 0.00876;
            
            switch (type)
            {
                case 1:
                    Pt = 0.1 * Math.Exp((-0.4 / 8.617E-5) * ((1 / (Pt + 273)) - 1.0 / 298));
                    break;
                case 2:
                    Pt = 0.1 * Math.Exp((-0.35 / 8.617E-5) * ((1 / (Pt + 273)) - 1.0 / 298));
                    break;
                case 3:
                    Pt = 0.1 * Math.Exp((-0.65 / 8.617E-5) * ((1 / (Pt + 273)) - 1.0 / 298));
                    break;
                case 4:
                    Pt = 0.1 * Math.Exp((-0.6 / 8.617E-5) * ((1 / (Pt + 273)) - 1.0 / 298));
                    break;
                case 5:
                    Pt = 0.1 * Math.Exp((-1.5 / 8.617E-5) * ((1 / (Pt + 273)) - 1.0 / 423));
                    break;
                case 6:
                    Pt = 0.1 * Math.Exp((-1.4 / 8.617E-5) * ((1 / (Pt + 273)) - 1.0 / 423));
                    break;
            }

            double lambda = Lbd * Pmfg * Ppt * Pcd + Lbp * Pq * Pt * Pe + Leos;
            lambda /= 1E6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lbd", Lbd.ToString());
            parametrs.Add("Pcd", Pcd.ToString());
            parametrs.Add("Pt", Pt.ToString());
            parametrs.Add("Leos", Leos.ToString());
            parametrs.Add("Lbp", Lbp.ToString());
            parametrs.Add("Pmfg", Pmfg.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }

    }

    class HFD :Electronics  
    {
        public double lambda, Pr;
        public HFD() { }

        public override void calc(Dictionary<string, double> calc_input)
        {
            double Lb = calc_input["Lb"];
            double T = calc_input["T"];
            double Pa = calc_input["Pa"];
            double Pm = calc_input["Pm"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];

            if (Lb == 0.22)
                T = Math.Exp(-5260 * ((1.0 / (T + 273)) - (1.0 / 298)));
            else
                T = Math.Exp(-2100 * ((1.0 / (T + 273)) - (1.0 / 298)));

            if (Lb == 0.0081)
                Pr = 0.362 * Math.Log(Pm) - 0.25;
            else Pr = 1;

            if(Lb == 0.027)
            {
                    if (Pq == 5) Pq = 1.8;
                    if (Pq == 25) Pq = 2.5;
            }
            lambda = Lb * T * Pa * Pq * Pr *Pe / 1E6; 

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pm", Pm.ToString());
            parametrs.Add("Pt", T.ToString());
            parametrs.Add("Pa", Pa.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
        }

    }
    // 
    class LFT :Electronics 
    {
        public double lambda;
        public LFT() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Pa = calc_input["Pa"];
            double T = calc_input["T"];
            double Pm = calc_input["Pm"];
            double Kel = calc_input["Kel"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];
            double Pr, Ps;
            T = Math.Exp(-2114 * ((1.0 / (T + 273)) - 1.0 / 298));
            if (Pm > 0.1) Pr = Math.Pow(Pm, 0.37);
            else Pr = 0.43;

            Ps = 0.045 * Math.Exp(3.1 * Kel);

            lambda = 0.00074 * T * Pa * Pr * Ps * Pq *Pe / 1E6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", "0,00074");
            parametrs.Add("Pm", Pm.ToString());
            parametrs.Add("Pt", T.ToString());
            parametrs.Add("Pa", Pa.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pr", Pr.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }

    }
    //
    class LFTSFE:Electronics
    {
        public double lambda;
        public LFTSFE() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Lb = calc_input["Lb"];
            double T = calc_input["T"];
            double Pa = calc_input["Pa"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];
            T = Math.Exp(-1925 *( (1.0 / (T + 273)) - 1.0 / 298));
            
            lambda = Lb * T * Pa * Pq *Pe / 1E6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pt", T.ToString());
            parametrs.Add("Pa", Pa.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }

    }
    //
    class SPT:Electronics
    {
        public double lambda;

        public SPT() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double T = calc_input["T"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];
            T = Math.Exp(-2483 *( (1.0 / (T + 273)) - 1.0 / 298));

            lambda = 0.0038 * T * Pq *Pe / 1E6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", "0,0038");
            parametrs.Add("Pt", T.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }

    }
    //
    class LNHFBT:Electronics
    {
        public double lambda;
        public LNHFBT() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double T = calc_input["T"];
            double Pm = calc_input["Pm"];
            double Kel = calc_input["Kel"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];

            double Pr, Ps = 0.045 * Math.Exp(3.1*Kel); ;
            if (Pm > 0.1) Pr = Math.Pow(Pm, 0.37);
            else Pr = 0.43;

            T = Math.Exp(-2114 *( (1.0 / (T + 273)) - 1.0 / 298));
            
            lambda = 0.18 * T * Pr * Ps  * Pq * Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", "0,18");
            parametrs.Add("Pt", T.ToString());
            parametrs.Add("Pr", Pr.ToString());
            parametrs.Add("Ps", Ps.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }
    //
    class PHFBT:Electronics
    {
        public double lambda;
        public PHFBT() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double F = calc_input["F"];
            double Pb = calc_input["Pb"];
            double Kel = calc_input["Kel"];
            double T = calc_input["T"];
            double Pt = calc_input["Pt"];
            double Pa = calc_input["Pa"];
            double S = calc_input["S"];
            double Pq = calc_input["Pq"];
            double Pm = calc_input["Pm"];
            double Pe = calc_input["Pe"];

            double Lb = 0.032 * Math.Exp(0.354 * F + 0.00558 * Pb);

            if (Kel <= 0.4)
                if (Pt == 1)
                    Pt = 0.1 * Math.Exp(-2903 * ((1.0 / (T + 273)) - 1.0 / 373));
                else
                    Pt = 0.38 * Math.Exp(-5794 * ((1.0 / (T + 273)) - 1.0 / 373));
            else if (Pt == 1)
                Pt = 2 * (Kel-0.35) * Math.Exp(-2903 * ((1.0 / (T + 273)) - 1.0 / 373));
            else
                Pt = 7.55 * (Kel - 0.35) * Math.Exp(-5794 * ((1.0 / (T + 273)) - 1.0 / 373));

            if (Pa == 1) Pa = 7.6;
            if (Pa == 2) Pa = 0.06 * S + 0.4;
            
            lambda = Lb * Pt * Pa * Pm * Pq *Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pt", Pt.ToString());
            parametrs.Add("Pa", Pa.ToString());
            parametrs.Add("Pm", Pm.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }
    //
    class HFGaAsFET:Electronics
    {
        public double lambda;
        public HFGaAsFET() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double F = calc_input["F"];
            double Pb = calc_input["Pb"];
            double T = calc_input["T"];
            double Pm = calc_input["Pm"];
            double Pa = calc_input["Pa"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];

            double Lb =0;
            if (1 <= F & F <= 10 & Pb <= 0.1) Lb = 0.052;
            else Lb = 0.0093 * Math.Exp(0.429 * F + 0.486 * Pb);
            //(4 <= F & F <= 10 & 0.1 <= Pb & Pb <= 6)

            double Pt = Math.Exp(-4485 * ((1.0 / (T + 273)) - 1.0 / 298));    
            lambda = Lb * Pt * Pa * Pm * Pq * Pe* 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pt", Pt.ToString());
            parametrs.Add("Pa", Pa.ToString());
            parametrs.Add("Pm", Pm.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }
    //
    class HFSiFET:Electronics
    {
        public double lambda;
        public HFSiFET() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Lb = calc_input["Lb"];
            double T = calc_input["T"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];
            double Pt = Math.Exp(-1925 * ((1.0 / (T + 273)) - 1.0 / 298));
            lambda = Lb * Pt * Pq *Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pt", Pt.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }
    //
    class Thyristor:Electronics
    {
        public double lambda;
        public Thyristor() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double T = calc_input["T"];
            double Ims = calc_input["Ims"];
            double Kel = calc_input["Kel"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];

            double Ps;
            if (0.3 <= Kel) Ps = Math.Pow(Kel, 1.9);
            else Ps = 0.1;
            T = Math.Exp(-3082 * ((1.0 / (T + 273)) - 1.0 / 298));
            lambda = 0.0022 * T * Math.Pow(Ims, 0.4) * Ps * Pq *Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", "0,0022");
            parametrs.Add("Pt", T.ToString());
            parametrs.Add("Ims", Ims.ToString());
            parametrs.Add("Ps", Ps.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }
    // Detectors, opt., emitters
    class DOE : Electronics
    {
        public double lambda;
        public DOE() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Lb = calc_input["Lb"];
            double T = calc_input["T"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];
            T = Math.Exp(-2790 * ((1.0 / (T + 273)) - 1.0 / 298));
            lambda = Lb * T * Pq *Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pt", T.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }

    class LDD : Electronics
    {
        public double lambda;
        public LDD() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Lb = calc_input["Lb"];
            double C = calc_input["C"];
            double Lk = calc_input["Lk"];
            double T = calc_input["T"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];

            if (Lb == 0.000043) Lb = Lb * C + Lk;
            if (Lb == 0.00017) Lb = 0.00017 * C + 0.00009 + Lk;
            T = Math.Exp(2790 * ((1.0 / (T + 273)) - 1.0 / 298));
            lambda = Lb * T * Pq * Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pt", T.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }

    class LD : Electronics
    {
        public double lambda;
        public LD() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Lb = calc_input["Lb"];
            double T = calc_input["T"];
            double Pq = calc_input["Pq"];
            double Pi = calc_input["Pi"];
            double Pa = calc_input["Pa"];
            double Ps = calc_input["Ps"];
            double Pp = calc_input["Pp"];
            double Pe = calc_input["Pe"];

            T = Math.Exp(-4635 * ((1.0 / (T + 273)) - 1.0 / 298));
            Pi = Math.Pow(Pi, 0.68);
            if (Pa == 1) Pa = Math.Pow(Ps, 0.5);
            if (Pp > 0.95) Pp = 0.95;
            if (Pp < 0) Pp = 0.01;
            Pp = 1 / (2 * (1 - Pp));
            lambda = Lb * T * Pi * Pa * Pp* Pq* Pe *1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pt", T.ToString());
            parametrs.Add("Pi", Pi.ToString());
            parametrs.Add("Pa", Pa.ToString());
            parametrs.Add("Pp", Pp.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }

    class Resistors : Electronics
    {
        public double lambda;
        public Resistors() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Lb = calc_input["Lb"];
            double T = calc_input["T"];
            double Pf = calc_input["Pf"];
            double Pm = calc_input["Pm"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];

            double Ea =0, A=0, B=0;
            switch (Lb) 
            { 
                case 0:
                    Lb = 0.0017;
                    Ea = 0.2;
                    A = 0.54;
                    B = 2.04;
                break;
                case 1:
                    Lb = 0.0037;
                    Ea = 0.08;
                    A = 0.71;
                    B = 1.1;
                    break;
                case 2:
                    Lb = 0.0037;
                    Ea = 0;
                    A = 0.54;
                    B = 2.04;
                    break;
                case 3:
                    Lb = 0.0019;
                    Ea = 0.2;
                    A = 1;
                    B = 0;
                    break;
                case 4:
                    Lb = 0.0024;
                    Ea = 0.08;
                    A = 0.71;
                    B = 1.1;
                    break;
                case 5:
                    Lb = 0.0024;
                    Ea = 0.08;
                    A = 0.54;
                    B = 2.04;
                    break;
                case 6:
                    Lb = 0.0019;
                    Ea = 0;
                    A = 1;
                    B = 0;
                    break;
                case 7:
                    Lb = 0.0024;
                    Ea = 0.08;
                    A = 0.71;
                    B = 1.1;
                    break;
                case 8:
                    Lb = 0.0024;
                    Ea = 0.2;
                    A = 0.71;
                    B = 1.1;
                    break;
                case 9:
                    Lb = 0.0024;
                    Ea = 0.08;
                    A = 0.71;
                    B = 1.1;
                    break;
                case 10:
                    Lb = 0.0037;
                    Ea = 0.08;
                    A = 0.71;
                    B = 1.1;
                    break;
                case 11:
                    Lb = 0.0037;
                    Ea = 0.2;
                    A = 0.71;
                    B = 1.1;
                    break;
            }

            T = Math.Pow(Math.E, -Ea / 8.617E-5 * ((1.0 / (T + 273)) - 1.0 / 298));

            double Pp = Math.Pow(Pf, 0.39);
            double Ps = A * Math.Exp( B * (Pf / Pm));
            lambda = Lb * T * Pp * Ps * Pq * Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pt", T.ToString());
            parametrs.Add("Pp", Pp.ToString());
            parametrs.Add("Ps", Ps.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }

    class Capacitor : Electronics
    {
        public double lambda;
        public Capacitor() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Lb = calc_input["Lb"];
            double T = calc_input["T"];
            double Pc = calc_input["Pc"];
            double Kel = calc_input["Kel"];
            double Psr = calc_input["Psr"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];

            double Ea = 0, A = 0, B = 0, C = 0;
            switch (Lb)
            {
                case 0:
                case 1:
                case 2:
                    Lb = 0.00037;
                    Ea = 0.15;
                    C = 0.09;
                    A = 5;
                    B = 0.6;
                    Psr = 1;
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                    Lb = 0.00051;
                    Ea = 0.15;
                    C = 0.09;
                    A = 5;
                    B = 0.6;
                    Psr = 1;
                    break;
                case 7:
                case 8:
                case 9:
                    Lb = 0.00076;
                    Ea = 0.35;
                    C = 0.09;
                    A = 10;
                    B = 0.6;
                    Psr = 1;
                    break;
                case 10:
                case 11:
                    Lb = 0.00099;
                    Ea = 0.35;
                    C = 0.09;
                    A = 3;
                    B = 0.6;
                    Psr = 1;
                    break;
                case 12:
                    Lb = 0.002;
                    Ea = 0.35;
                    C = 0.09;
                    A = 3;
                    B = 0.6;
                    Psr = 1;
                    break;
                case 13:
                    Lb = 0.0004;
                    Ea = 0.15;
                    C = 0.23;
                    A = 17;
                    B = 0.6;
                    break;
                case 14:
                    Lb = 0.00005;
                    Ea = 0.15;
                    C = 0.23;
                    A = 17;
                    B = 0.6;
                    break;
                case 15:
                    Lb = 0.0004;
                    Ea = 0.15;
                    C = 0.23;
                    A = 17;
                    B = 0.6;
                    Psr = 1;
                    break;
                case 16:
                case 17:
                    Lb = 0.00012;
                    Ea = 0.35;
                    C = 0.23;
                    A = 5;
                    B = 0.6;
                    Psr = 1;
                    break;
                case 18:
                    Lb = 0.0079;
                    Ea = 0.15;
                    C = 0.09;
                    A = 3;
                    B = 0.5;
                    Psr = 1;
                    break;
                case 19:
                    Lb = 0.006;
                    Ea = 0.35;
                    C = 0.09;
                    A = 3;
                    B = 0.5;
                    Psr = 1;
                    break;
                case 20:
                    Lb = 0.00037;
                    Ea = 0.35;
                    C = 0.09;
                    A = 3;
                    B = 0.5;
                    Psr = 1;
                    break;
                case 21:
                    Lb = 0.00037;
                    Ea = 0.15;
                    C = 0.09;
                    A = 3;
                    B = 0.5;
                    Psr = 1;
                    break;
            }

            T = Math.Pow(Math.E, -Ea / 8.617E-5 * ((1.0 / (T + 273)) - 1.0 / 298));
            Pc = Math.Pow(Pc, C);
            double Pv = Math.Pow((Kel / B), A)+1;
            lambda = Lb * T * Pc*Pv*Psr * Pq * Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pt", T.ToString());
            parametrs.Add("Pc", Pc.ToString());
            parametrs.Add("Pv", Pv.ToString());
            parametrs.Add("Psr", Psr.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }

    class Transformer : Electronics
    {
        public Transformer() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Lb = calc_input["Lb"];
            double T = calc_input["T"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];

            double Pt = Math.Exp((-0.11/8.617E-5) * ((1.0 / (T + 273)) - 1.0 / 298));
            double lambda = Lb * Pt * Pq * Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pt", Pt.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }
    class SP : Electronics
    {
        public SP() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Lb = calc_input["Lb"];
            double Pi = calc_input["Pi"];
            double Kel = calc_input["Kel"];
            double Pq = calc_input["Pq"];
            double N = calc_input["N"];
            double Pc = calc_input["Pc"];
            double Pe = calc_input["Pe"];
            Pi = Math.Pow(Math.E, (Math.Pow(Kel / Pi, 2)));
            if (Pc == 2) Pc = Math.Pow(N, 0.33);
            double lambda = Lb * Pi * Pc * Pq * Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Pi", Pi.ToString());
            parametrs.Add("Pc", Pc.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }

    class Connectors : Electronics
    {
        public Connectors() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Lb = calc_input["Lb"];
            double T = calc_input["T"];
            double I = calc_input["I"];
            double A = calc_input["A"];
            double Pk = calc_input["Pk"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];
            double dT = A * Math.Pow(I, 1.85);
            if (Lb == 0.00041) dT = 5;
            if (Lb == 0.00042) dT = 50;
            double Pt = Math.Pow(Math.E, ((-0.14/8.617E-5)*((1.0 / ((T+dT)+T+273))-1.0/298)));

            double lambda = Lb * Pt * Pk * Pq * Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pt", Pt.ToString());
            parametrs.Add("Pk", Pk.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }

    class Relay : Electronics
    {
        public Relay() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Lb = calc_input["Lb"];
            double T = calc_input["T"];
            double Pi = calc_input["Pi"];
            double Kel = calc_input["Kel"];
            double Pq = calc_input["Pq"];
            double Pc = calc_input["Pc"];
            double N = calc_input["N"];
            double Pf = calc_input["Pf"];
            double Pe = calc_input["Pe"];

            Lb = Math.Pow(Math.E, (Lb / 8.617E-5 * ((1.0 / (T + 273)) - 1.0 / 298)));
            Pi = Math.Exp(Kel * Kel / Pi / Pi);
            double Pcyc = 0;
            if (Pq == 2.9)
            {
                if (N > 1000) Pcyc = N * N / 100 / 100;
                if (N > 10 && N <= 1000) Pcyc = N / 10;
                if (N < 10) Pcyc = 1;
            }
            else if (N < 1) Pcyc = 0.1;
            else Pcyc =N / 10;

            double lambda = Lb * Pi * Pc * Pcyc * Pf * Pq * Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pi", Pi.ToString());
            parametrs.Add("Pc", Pc.ToString());
            parametrs.Add("Pcyc", Pcyc.ToString());
            parametrs.Add("Pf", Pf.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }

    class Relay2 : Electronics
    {
        public Relay2() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];

            double lambda = 0.029 * Pq * Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", "0,029");
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }

    class Lamp : Electronics
    {
        public Lamp() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double Vr = calc_input["Vr"];
            double Pu = calc_input["Pu"];
            double Pa = calc_input["Pa"];
            double Pe = calc_input["Pe"];
            double Lb = 0.074 * Math.Pow(Vr, 1.29);

            double lambda = Lb * Pu * Pa * Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pu", Pu.ToString());
            parametrs.Add("Pa", Pa.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();

        }
    }

    class QD : Electronics
    {
        public QD() { }
        public override void calc(Dictionary<string, double> calc_input)
        {
            double F = calc_input["F"];
            double Pq = calc_input["Pq"];
            double Pe = calc_input["Pe"];
            double Lb = 0.013 * Math.Pow(F, 0.23);

            double lambda = Lb * Pq * Pe * 1E-6;

            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Lb", Lb.ToString());
            parametrs.Add("Pq", Pq.ToString());
            parametrs.Add("Pe", Pe.ToString());
            round();
            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }

    class Fuse : Electronics
    {
        public Fuse() { }
        public override void calc(Dictionary<string, double> calc_input)
        { 
            double lambda = 0.01 * calc_input["Pe"];
            parametrs.Add("L", lambda.ToString());
            parametrs.Add("Pe", calc_input["Pe"].ToString());
            round();

            this.LN = (lambda * Convert.ToDouble(this.input["Количество"])).ToString();
        }
    }
}
