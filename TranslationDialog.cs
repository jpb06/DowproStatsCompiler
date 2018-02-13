using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace StatsCompiler
{
    public partial class TranslationDialog : Form
    {
        public TranslationDialog()
        {
            InitializeComponent();
            Fill();
        }
        public void Fill()
        {
            foreach (string s in Translation.TranslationTable.Keys)
            {
                string[] transRow = (string[])Translation.TranslationTable[s];
                string trans = transRow[0];
                string owner = transRow[1];

                DataGridViewRowCollection rows = this.dataGridView1.Rows;
                rows.Add(new object[] { s, trans, owner });
            }
            dataGridView1.Sort(LuaCol, ListSortDirection.Ascending);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            Translation.TranslationTable.Clear();
            DataGridViewRowCollection rows = this.dataGridView1.Rows;
            foreach (DataGridViewRow row in rows)
            {
                if (row.Cells[0].Value != null && !Translation.TranslationTable.Contains(row.Cells[0]))
                {
                    Translation.TranslationTable.Add(row.Cells[0].Value, new string[] { (string)row.Cells[1].Value, (string)row.Cells[2].Value });     
                }
            }
            Translation.WriteTable();
            this.Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public int LastSearchedRow=-1;
        public string PreviousSearchCriteria="";

        public void Search()
        {
            if (SearchBox.Text != PreviousSearchCriteria)
            {
                LastSearchedRow = -1;
                PreviousSearchCriteria = SearchBox.Text;
            }
            bool found = false;
            
            DataGridViewRowCollection rows = this.dataGridView1.Rows;
            
            for (int i = (LastSearchedRow+1); i < rows.Count; i++ )
            {
                DataGridViewRow row = rows[i];
                
                for (int j = 0; j < 3; j++)
                {                    
                    if (row.Cells[j].Value != null)
                    {
                        string s = row.Cells[j].Value as string;
                        if (s.Contains(SearchBox.Text))
                        {
                            dataGridView1.CurrentCell = row.Cells[0];
                            row.Selected = true;
                            LastSearchedRow = i;   
                            return;
                        }
                    }
                }
            }
            if (!found)
                LastSearchedRow = -1;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (SearchBox.Text != "")
            {                
                Search();
            }
        }

        private void SearchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                Search();

        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.F3)
                Search();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.F3)
                Search();
        }
    }
}