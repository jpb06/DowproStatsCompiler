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
    public partial class UselessAbilitiesForm : Form
    {
        public UselessAbilitiesForm()
        {
            InitializeComponent();
            Fill();
        }
        public void Fill()
        {
            foreach (string s in DataDumper.UnnecessaryAbilities)
            {
                DataGridViewRowCollection rows = this.dataGridView1.Rows;
                rows.Add(new object[] { s });
            }
            
        }
        public static void WriteAbilities()
        {
            StreamWriter writer = new StreamWriter("Config/UnnecessaryAbilities.def");
                        
            foreach (string s in DataDumper.UnnecessaryAbilities)             
                writer.WriteLine(s);            
            writer.Close();
        }

        private void Accept_Button_Click(object sender, EventArgs e)
        {
            DataDumper.UnnecessaryAbilities.Clear();
            DataGridViewRowCollection rows = this.dataGridView1.Rows;
            foreach (DataGridViewRow row in rows)
                if (row.Cells[0].Value != null)
                    DataDumper.UnnecessaryAbilities.Add(row.Cells[0].Value);
            WriteAbilities();
            this.Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}