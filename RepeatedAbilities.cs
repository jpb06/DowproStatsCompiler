using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace StatsCompiler
{
    public partial class RepeatedAbilities : Form
    {
        public RepeatedAbilities()
        {
            InitializeComponent();
            Fill();
        }
        public void Fill()
        {
            foreach (string s in DataDumper.RepeatedAbilities.Keys)
            {
                string[] transRow = (string[])DataDumper.RepeatedAbilities[s];
                string trans = transRow[0];
                string owner = transRow[1];

                DataGridViewRowCollection rows = this.dataGridView1.Rows;
                rows.Add(new object[] { s, trans, owner });
            }
           
        }
        public static void WriteAbilities()
        {
            StreamWriter writer = new StreamWriter("Config/RepeatedAbilities.def");
            ArrayList list = new ArrayList(DataDumper.RepeatedAbilities.Keys);
            list.Sort();
            foreach (string key in list)
            {
                string[] ab = (string[])DataDumper.RepeatedAbilities[key];
                string output = key + @" - """ + ab[0] + @""""+ @" - """+ ab[1]+@"""" ;
               
                writer.WriteLine(output);
            }
            writer.Close();
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Accept_Button_Click(object sender, EventArgs e)
        {
            DataDumper.RepeatedAbilities.Clear();
            DataGridViewRowCollection rows = this.dataGridView1.Rows;
            foreach (DataGridViewRow row in rows)
                if (row.Cells[0].Value != null && !DataDumper.RepeatedAbilities.ContainsKey(row.Cells[0].Value))
                    DataDumper.RepeatedAbilities.Add(row.Cells[0].Value,new string[]{(string)row.Cells[1].Value,(string)row.Cells[2].Value});
            WriteAbilities(); 
            this.Close();
        }
    }
}