using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Threading;

namespace StatsCompiler
{
    public enum Bars
    {
        Html,
        Data,
    }
    /// <summary>
	/// Descrizione di riepilogo per MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
    {
        private MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.TextBox LogWindow;
		private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private ProgressBar Bar_Html;
        private Button button3;
        private IContainer components;
        private ProgressBar Bar_Data;
        private Label label1;
        private Label label2;
        private MenuItem menuItem3;
        private MenuItem menuItem4;
        private MenuItem menuItem5;
        private MenuItem menuItem6;
        private MenuItem menuItem7;
        private MenuItem menuItem11;
        private Thread m_DumpThread;
		public MainForm()
		{
			//
			// Necessario per il supporto di Progettazione Windows Form
			//
			InitializeComponent();

			//
			// TODO: aggiungere il codice del costruttore dopo la chiamata a InitializeComponent.
			//
			DataPath.LoadPaths();
			Translation.Initialize();
			DataDumper.LoadFilesList();
            DataDumper.LoadAbilitiesConfig();
		}

		/// <summary>
		/// Pulire le risorse in uso.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.LogWindow = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Bar_Html = new System.Windows.Forms.ProgressBar();
            this.button3 = new System.Windows.Forms.Button();
            this.Bar_Data = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem3});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2});
            this.menuItem1.Text = "File";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 0;
            this.menuItem2.Text = "Set Paths";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem4,
            this.menuItem5,
            this.menuItem11});
            this.menuItem3.Text = "Edit";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 0;
            this.menuItem4.Text = "Translations";
            this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 1;
            this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem6,
            this.menuItem7});
            this.menuItem5.Text = "Abilities";
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 0;
            this.menuItem6.Text = "Discarded";
            this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 1;
            this.menuItem7.Text = "Repeated";
            this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 2;
            this.menuItem11.Text = "TechTrees";
            // 
            // LogWindow
            // 
            this.LogWindow.Location = new System.Drawing.Point(164, 100);
            this.LogWindow.MaxLength = 0;
            this.LogWindow.Multiline = true;
            this.LogWindow.Name = "LogWindow";
            this.LogWindow.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.LogWindow.Size = new System.Drawing.Size(396, 172);
            this.LogWindow.TabIndex = 0;
            this.LogWindow.WordWrap = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(44, 113);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 32);
            this.button1.TabIndex = 1;
            this.button1.Text = "Dump";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(44, 52);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 32);
            this.button2.TabIndex = 2;
            this.button2.Text = "File list";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Bar_Html
            // 
            this.Bar_Html.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Bar_Html.ForeColor = System.Drawing.SystemColors.Desktop;
            this.Bar_Html.Location = new System.Drawing.Point(242, 68);
            this.Bar_Html.Name = "Bar_Html";
            this.Bar_Html.Size = new System.Drawing.Size(268, 10);
            this.Bar_Html.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.Bar_Html.TabIndex = 4;
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(44, 171);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(72, 32);
            this.button3.TabIndex = 5;
            this.button3.Text = "Abort";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Bar_Data
            // 
            this.Bar_Data.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Bar_Data.ForeColor = System.Drawing.SystemColors.Desktop;
            this.Bar_Data.Location = new System.Drawing.Point(242, 41);
            this.Bar_Data.Name = "Bar_Data";
            this.Bar_Data.Size = new System.Drawing.Size(268, 10);
            this.Bar_Data.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.Bar_Data.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(198, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Data";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(198, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Html";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(576, 291);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Bar_Data);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.Bar_Html);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LogWindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Menu = this.mainMenu1;
            this.Name = "MainForm";
            this.Text = "Dark Crusade Stats Dumper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public static MainForm mForm;

		/// <summary>
		/// Il punto di ingresso principale dell'applicazione.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			mForm = new MainForm();
			Application.Run(mForm);            
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			new PathsDialog().ShowDialog();
		}

		public static void Log(string message)
		{
			mForm.LogWindow.AppendText(message+Environment.NewLine);
        }
        
        #region PROGRESS BARS
        public static void BarInc(Bars bar)
        {
            switch (bar)
            {
                case Bars.Data:
                    if (mForm.Bar_Data.Value < mForm.Bar_Data.Maximum)
                        mForm.Bar_Data.Value++;
                    break;
                case Bars.Html:
                    if (mForm.Bar_Html.Value < mForm.Bar_Html.Maximum)
                        mForm.Bar_Html.Value++;
                    break;
            }
            
        }

        public static void BarSetMax(Bars bar, int Max)
        {
            switch (bar)
            {
                case Bars.Data:
                    mForm.Bar_Data.Value = Math.Min(Max, mForm.Bar_Data.Value);
                    mForm.Bar_Data.Maximum = Max;    
                    break;
                case Bars.Html:
                    mForm.Bar_Html.Value = Math.Min(Max, mForm.Bar_Html.Value);
                    mForm.Bar_Html.Maximum = Max; 
                    break;
            }
        }

        public static void BarReset(Bars bar)
        {
            switch (bar)
            {
                case Bars.Data:
                    mForm.Bar_Data.Value = 0; 
                    break;
                case Bars.Html:
                    mForm.Bar_Html.Value = 0;
                    break;
            }
        }
        #endregion
        
        public static void LogFile(string message)
		{
			StreamWriter writer = new StreamWriter("log.log",true);
			writer.WriteLine(message);
			writer.Close();
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			
			if (DataDumper.FilesTable==null)
			{
				Log("Files index missing");
				return;
			}

            m_DumpThread = new Thread(new ThreadStart(Dump));
            m_DumpThread.Start();	
		}
        public static void ToggleLock()
        {
            MainForm.mForm.button1.Enabled = !(MainForm.mForm.button1.Enabled ^ false) ;
            MainForm.mForm.button2.Enabled = !(MainForm.mForm.button2.Enabled ^ false) ;
            MainForm.mForm.button3.Enabled = !(MainForm.mForm.button3.Enabled ^ false);
        }
       
        public static void Dump()
        {
            MainForm.ToggleLock();
            BarReset(Bars.Data);
            BarReset(Bars.Html);
            DataDumper.Dump();
            HtmlCompiler.CompileHtml();            
            MainForm.ToggleLock();
        }

		private void button2_Click(object sender, System.EventArgs e)
		{
			new LuaFilesListDialog().ShowDialog();			
		}

        private void button3_Click(object sender, EventArgs e)
        {
            if (m_DumpThread != null && m_DumpThread.IsAlive)
                m_DumpThread.Abort();
            MainForm.Log("Aborted");
            ToggleLock();
            BarReset(Bars.Data);
            BarReset(Bars.Html);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_DumpThread != null && m_DumpThread.IsAlive)
                m_DumpThread.Abort();
        }

        private void menuItem4_Click(object sender, EventArgs e)
        {
            new TranslationDialog().ShowDialog(); 
        }

        private void menuItem6_Click(object sender, EventArgs e)
        {
            new UselessAbilitiesForm().ShowDialog();
        }

        private void menuItem7_Click(object sender, EventArgs e)
        {
            new RepeatedAbilities().ShowDialog();
        }
       
	}
}
