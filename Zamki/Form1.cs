using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;

namespace Zamki
{
    public partial class Form1 : Form
    {

        List<Wymiatacz> WymiataczList = new List<Wymiatacz>();
        ZamekETH zamETH = new ZamekETH("01F025F9070000FD", "192.168.10.186"); 

        //string dataType;


        public Form1()
        {
            InitializeComponent();
            textBox2.Text = zamETH.Ip;
            showAdres();
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            zamETH.Ip = textBox2.Text;
            label21.Text = zamETH.GetAdress().ToString();
            showAdres();

        }

        private void showAdres()
        {

            adres.Text = "";

            if (radioButton1.Checked) adres.Text = BitConverter.ToString(zamETH.Adres).Replace("-", string.Empty);
            if (radioButton2.Checked)
            {
                for (int i = 0; i < 8; i++)
                {
                    adres.Text += zamETH.Adres[i];
                    if (i != 7) adres.Text += ",";
                }

            }
            if (radioButton3.Checked) adres.Text = System.Text.Encoding.ASCII.GetString(zamETH.Adres);


        }



        private void button3_Click(object sender, EventArgs e)
        {
            zamETH.Ip = textBox2.Text;
            label21.Text = zamETH.Setings((ushort)numericUpDown1.Value, (ushort)numericUpDown2.Value).ToString();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            showAdres();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            showAdres();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

       
        }

        private void button4_Click(object sender, EventArgs e)
        {
            zamETH.Ip = textBox2.Text;
            label21.Text = zamETH.Status().ToString();
            if (zamETH.Hall) label14.Text = "Open"; else label14.Text = "Close";
            if (zamETH.Tamper) label13.Text = "Open"; else label13.Text = "Close";
            label16.Text = Convert.ToString(zamETH.OpenTime);
            label15.Text = Convert.ToString(zamETH.SustainTime);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            zamETH.Ip = textBox2.Text;
            label21.Text = zamETH.Condition().ToString();
            label19.Text = Convert.ToString(zamETH.Temerature/3) + " C"; ;
            label18.Text = Convert.ToString(zamETH.Current)+" mA";
            label17.Text = Convert.ToString((float)zamETH.Tensity/100)+ " V";

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

            showAdres();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            zamETH.Ip = textBox2.Text;
            int wynik = zamETH.Open();
            label21.Text = wynik.ToString();
            switch (wynik)
            {
                case 1:
                    label5.Text = "Wykonano";
                    break;
                case 2:
                    label5.Text = "Otwarty";
                    break;
                case 3:
                    label5.Text = "Awaria";
                    break;
            }

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button6_Click_1(object sender, EventArgs e)
        {

            int counter = 0;
            string line;
            string hex;
            byte[] adres = new byte[8];


            OpenFileDialog okienko = new OpenFileDialog();
            okienko.Filter = "Pliki textowe (txt)|*.txt";
            if (okienko.ShowDialog() == DialogResult.OK)
            {



                // Read the file and display it line by line.
                System.IO.StreamReader file =
                new System.IO.StreamReader(okienko.FileName);
                while ((line = file.ReadLine()) != null)
                {
                    //System.Console.WriteLine(line);
                    
                    hex = line.Replace(" ", ""); 

                    if (hex.Length == 16)
                        for (int i = 0; i < hex.Length; i += 2)
                            adres[i / 2] = Convert.ToByte(hex.Substring(i, 2),16);

                    textBox1.Text += hex +"\r\n";
                    WymiataczList.Add(new Wymiatacz(counter, adres, false));
                    counter++;
                }

                file.Close();
                
                System.Console.WriteLine("There were {0} lines.", counter);
                // Suspend the screen.
                System.Console.ReadLine();

            }



        }

        private void button7_Click(object sender, EventArgs e)
        {
            int wynik;
            int ilosc = 0;
            zamETH.Ip = textBox2.Text;

            foreach (Wymiatacz oWymiatacz in WymiataczList)
            {
                zamETH.Adres = oWymiatacz.Adres;
                wynik = zamETH.Status();
                textBox1.Text += BitConverter.ToString(zamETH.Adres).Replace("-", string.Empty) +" "+wynik+ "\r\n";
                textBox1.Refresh();
                Thread.Sleep(1000); //min 300

            }


        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            WymiataczList.Clear();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            
            zamETH.Open().ToString(); 
        }


        private void GetCRC(byte[] message, ref byte[] CRC)
        {
            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);
                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);
                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            int wynik;
            zamETH.Ip = textBox2.Text;

            foreach (Wymiatacz oWymiatacz in WymiataczList)
            {
                zamETH.Adres = oWymiatacz.Adres;
                wynik = zamETH.Open();
                textBox1.Text += BitConverter.ToString(zamETH.Adres).Replace("-", string.Empty) + " " + wynik +"\r\n";
                textBox1.Refresh();
                Thread.Sleep(1000); //min 300

            }
            

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
