using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace StatsCompiler
{
	/// <summary>
	/// Descrizione di riepilogo per PathsDialog.
	/// </summary>
	public class PathsDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label3;
        private Label label4;
        private TextBox textBox4;
        private Label label5;
        private TextBox textBox6;
        private Label label6;
        private TextBox textBox5;
		/// <summary>
		/// Variabile di progettazione necessaria.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PathsDialog()
		{
			//
			// Necessario per il supporto di Progettazione Windows Form
			//
			InitializeComponent();

			//
			// TODO: aggiungere il codice del costruttore dopo la chiamata a InitializeComponent.
			//
			this.textBox1.Text=DataPath.OriginalLuasPath;
			this.textBox2.Text=DataPath.ModLuasPath;
			this.textBox3.Text=DataPath.OutputPath;
            textBox4.Text = DataPath.W40kUcsPath;
            textBox5.Text = DataPath.Dxp2UcsPath;
            textBox6.Text = DataPath.ModUcsPath;
		}

		/// <summary>
		/// Pulire le risorse in uso.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Metodo necessario per il supporto della finestra di progettazione. Non modificare
		/// il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent()
		{
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(15, 28);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(481, 20);
            this.textBox1.TabIndex = 0;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(15, 79);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(481, 20);
            this.textBox2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Original Lua\'s";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Mod Lua\'s";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(364, 337);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 32);
            this.button1.TabIndex = 6;
            this.button1.Text = "Apply";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(434, 337);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(64, 32);
            this.button2.TabIndex = 7;
            this.button2.Text = "Cancel";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(15, 287);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(481, 20);
            this.textBox3.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 268);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Output Folder";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "Wh40k.ucs";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(15, 125);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(481, 20);
            this.textBox4.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 199);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 16);
            this.label5.TabIndex = 15;
            this.label5.Text = "Mod.ucs";
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(15, 218);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(481, 20);
            this.textBox6.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 152);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 16);
            this.label6.TabIndex = 13;
            this.label6.Text = "DxP2.ucs";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(15, 171);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(481, 20);
            this.textBox5.TabIndex = 12;
            // 
            // PathsDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(520, 389);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PathsDialog";
            this.ShowInTaskbar = false;
            this.Text = "PathsDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{

			DataPath.OriginalLuasPath = textBox1.Text; 
			DataPath.ModLuasPath=textBox2.Text;
			DataPath.OutputPath=textBox3.Text;
            DataPath.W40kUcsPath = textBox4.Text;
            DataPath.Dxp2UcsPath = textBox5.Text;
            DataPath.ModUcsPath = textBox6.Text;

            try
            {
                StreamWriter writer = new StreamWriter(File.Open("Config/StatsDump.Ini", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write));
                if (writer != null)
                {
                    writer.WriteLine(@"OriginalLuaPath = """ + DataPath.OriginalLuasPath+@"""");
                    writer.WriteLine(@"ModLuaPath = """ + DataPath.ModLuasPath + @"""");
                    writer.WriteLine(@"OutputPath = """ + DataPath.OutputPath + @"""");
                    writer.WriteLine(@"W40kUcsPath = """ + DataPath.W40kUcsPath + @"""");
                    writer.WriteLine(@"Dxp2UcsPath = """ + DataPath.Dxp2UcsPath + @"""");
                    writer.WriteLine(@"ModUcsPath = """ + DataPath.ModUcsPath + @"""");


                    writer.Close();
                }
            }
            catch { }

            Translation.Initialize();
			
			this.Close();
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

	}
}
