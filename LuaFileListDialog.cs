using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace StatsCompiler
{
	/// <summary>
	/// Descrizione di riepilogo per Form1.
	/// </summary>
	public class LuaFilesListDialog : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button button1;
        private Label available_label;
        private Label added_Label;
        private Button button3;
        private Button button4;
        private TreeView treeView1;
        private TreeView treeView2;
        private Button button2;
		/// <summary>
		/// Variabile di progettazione necessaria.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public LuaFilesListDialog()
		{
			//
			// Necessario per il supporto di Progettazione Windows Form
			//
			InitializeComponent();

			//
			// TODO: aggiungere il codice del costruttore dopo la chiamata a InitializeComponent.
			//

            treeView1.Nodes.Add("Buildings");            
            treeView1.Nodes.Add("Squads");
            
            for (int i = 0; i < 2; i++)
                treeView1.Nodes[i].Name = treeView1.Nodes[i].Text;

            treeView2.Nodes.Add("Buildings"); 
            treeView2.Nodes.Add("Squads");
            
            for (int i = 0; i < 2; i++)
                treeView2.Nodes[i].Name = treeView2.Nodes[i].Text;
            
            foreach (TreeNode node in treeView1.Nodes)
            {    
                    node.Nodes.Add("chaos");
                    node.Nodes.Add("eldar");
                    node.Nodes.Add("guard");
                    node.Nodes.Add("orks");
                    node.Nodes.Add("necrons");
                    node.Nodes.Add("space_marines");
                    node.Nodes.Add("tau");
                    node.Nodes.Add("dark_eldar");
                    node.Nodes.Add("sisters");
                    for (int i = 0; i < 9; i++)
                        node.Nodes[i].Name = node.Nodes[i].Text;
            }
            foreach (TreeNode node in treeView2.Nodes)
            {

                node.Nodes.Add("chaos");
                node.Nodes.Add("eldar");
                node.Nodes.Add("guard");
                node.Nodes.Add("orks");
                node.Nodes.Add("necrons");
                node.Nodes.Add("space_marines");
                node.Nodes.Add("tau");
                node.Nodes.Add("dark_eldar");
                node.Nodes.Add("sisters");

                for (int i = 0; i < 9; i++)
                    node.Nodes[i].Name = node.Nodes[i].Text;

            }
            FillAvailable();
            FillAdded();
            RefreshAll();
            treeView1.Sort();
            treeView2.Sort();
		}

        public void FillAvailable()
        {
            // Contains all SubPaths that program must scan
            string[] SquadsToLoad = new string[]
				{
					@"\sbps\races\chaos",
					@"\sbps\races\eldar",
					@"\sbps\races\guard",
					@"\sbps\races\necrons",
					@"\sbps\races\orks",
					@"\sbps\races\space_marines",
					@"\sbps\races\tau",
                    // Soulstorm Squads
                    @"\sbps\races\dark_eldar",
                    @"\sbps\races\sisters"
				};
            string[] BuildingsToLoad = new string[]
				{
					@"\ebps\races\chaos\structures",
					@"\ebps\races\eldar\structures",
					@"\ebps\races\guard\structures",
					@"\ebps\races\necrons\structures",
					@"\ebps\races\orks\structures",
					@"\ebps\races\space_marines\structures",
					@"\ebps\races\tau\structures",
                    // Soulstorm Structures
                    @"\ebps\races\dark_eldar\structures",
                    @"\ebps\races\sisters\structures"
				};

            string[] Races = new string[]
			{
				"chaos",
				"eldar",
				"guard",
				"necrons",
				"orks",
				"space_marines",
				"tau",
                // Soulstorm races
                "dark_eldar",
                "sisters"
			};

            string W40KPath = DataPath.OriginalLuasPath + "\\W40K\\Attrib";
            string DXP2Path = DataPath.OriginalLuasPath + "\\DXP2\\Attrib";
            string ModPath = DataPath.ModLuasPath + "\\Attrib";
            
            int racesCount = 9;
            
            // Load Buildings
            for (int i = 0; i < racesCount; i++)
            {
                if (Directory.Exists(W40KPath + SquadsToLoad[i]))
                    LoadFiles(W40KPath + BuildingsToLoad[i], "Building", Races[i]);
                if (Directory.Exists(DXP2Path + SquadsToLoad[i]))
                    LoadFiles(DXP2Path + BuildingsToLoad[i], "Building", Races[i]);
                if (Directory.Exists(ModPath + SquadsToLoad[i]))
                    LoadFiles(ModPath + BuildingsToLoad[i], "Building", Races[i]);
            }

            // Load Squads
            for (int i = 0; i < racesCount; i++)
            {
                if (Directory.Exists(W40KPath + SquadsToLoad[i]))
                    LoadFiles(W40KPath + SquadsToLoad[i], "Unit", Races[i]);
                if (Directory.Exists(DXP2Path + SquadsToLoad[i]))
                    LoadFiles(DXP2Path + SquadsToLoad[i], "Unit", Races[i]);
                if (Directory.Exists(ModPath + SquadsToLoad[i]))
                    LoadFiles(ModPath + SquadsToLoad[i], "Unit", Races[i]);
            }
            
        }
        public void FillAdded()
        {
            foreach(string key in DataDumper.FilesTable.Keys)
            {
                LuaInfo lua = DataDumper.FilesTable[key] as LuaInfo;

                string Type = "Buildings";
                switch (lua.Type)
                {
                    case InfoTypes.Unit:
                        Type = "Squads";
                        break;
                    case InfoTypes.Building:
                        Type = "Buildings";
                        break;
                }

                TreeNode node = treeView2.Nodes[treeView2.Nodes.IndexOfKey(Type)];                
               
                    TreeNode subNode = node.Nodes[node.Nodes.IndexOfKey(lua.Race)];
                    TreeNode newNode = new TreeNode(key);
                    newNode.Name = key;
                    subNode.Nodes.Add(newNode);
                
            }
        }
        public void RefreshAll()
        {
            for (int i = 0; i < 2; i++)
            {
                TreeNode tn1 = treeView1.Nodes[i];
                TreeNode tn2 = treeView2.Nodes[i];
                
                
                
                for (int j = 0; j < 9; j++)
                    {
                        TreeNode tsn1 = tn1.Nodes[j];
                        TreeNode tsn2 = tn2.Nodes[j];
                        RefreshFolder(tsn1, tsn2);                    
                    }                
            }
            
        }

        public void RefreshFolder(TreeNode tn1, TreeNode tn2)
        {
            foreach (TreeNode tn in tn1.Nodes)
            {
                Refresh(tn,tn2);
            }
        }

        public void Refresh(TreeNode tn, TreeNode tn2)
        {
            if (!tn2.Nodes.ContainsKey(tn.Name))
                tn.ForeColor = Color.Red;
            else tn.ForeColor = Color.Black;
        }

        public void LoadFiles(string folder, string type)
        {
            LoadFiles(folder, type, null);
        }
        public void LoadFiles(string folder, string type, string race)
        {

            string[] files = Directory.GetFiles(folder);
            foreach (string f in files)
            {
                string Race = race;
                if (f.EndsWith(".lua") || f.EndsWith(".nil"))
                {
                    string fileName = f.Remove(0, folder.Length + 1);

                    if (Race == null || Race == "")
                        Race = DataDumper.GetRace(fileName);

                    if (Race == "none")
                        continue;

                    TreeNodeCollection t1 = this.treeView1.Nodes;
                    string subs = "";

                    switch (type)
                    {
                        case "Unit":
                            subs = "Squads";
                            break;
                        case "Building":
                            subs = "Buildings";
                            break;
                    }
                    TreeNode t1_sub = t1.Find(subs, false)[0];


                    foreach (TreeNode node in t1_sub.Nodes)
                    {
                        if (node.Text == Race)
                        {
                            TreeNodeCollection t1_sub_race = node.Nodes;
                            TreeNode newNode = new TreeNode(fileName);
                            newNode.Name = fileName;
                            if (!t1_sub_race.ContainsKey(newNode.Name))
                                t1_sub_race.Add(newNode);
                            break;
                        }
                    }

                }
            }

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
            this.button1 = new System.Windows.Forms.Button();
            this.available_label = new System.Windows.Forms.Label();
            this.added_Label = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(371, 382);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(59, 32);
            this.button1.TabIndex = 1;
            this.button1.Text = "Accept";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // available_label
            // 
            this.available_label.AutoSize = true;
            this.available_label.Location = new System.Drawing.Point(12, 27);
            this.available_label.Name = "available_label";
            this.available_label.Size = new System.Drawing.Size(50, 13);
            this.available_label.TabIndex = 6;
            this.available_label.Text = "Available";
            // 
            // added_Label
            // 
            this.added_Label.AutoSize = true;
            this.added_Label.Location = new System.Drawing.Point(444, 27);
            this.added_Label.Name = "added_Label";
            this.added_Label.Size = new System.Drawing.Size(38, 13);
            this.added_Label.TabIndex = 7;
            this.added_Label.Text = "Added";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(371, 69);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(59, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "Add";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(371, 144);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(59, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "Remove";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(12, 46);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(342, 329);
            this.treeView1.TabIndex = 10;
            // 
            // treeView2
            // 
            this.treeView2.Location = new System.Drawing.Point(447, 46);
            this.treeView2.Name = "treeView2";
            this.treeView2.Size = new System.Drawing.Size(351, 329);
            this.treeView2.TabIndex = 11;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(371, 98);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(59, 40);
            this.button2.TabIndex = 12;
            this.button2.Text = "Add children";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // LuaFilesListDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(828, 429);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.treeView2);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.added_Label);
            this.Controls.Add(this.available_label);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LuaFilesListDialog";
            this.Text = "Lua files List";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{
            CompileFilesTable();

            DataDumper.WriteFilesList();
            
            this.Close();
		}

        private void CompileFilesTable()
        {
            DataDumper.FilesTable.Clear();
            for (int i = 0; i < 2; i++)
            {
                TreeNode tn = treeView2.Nodes[i];
                string path = "";

                for (int j = 0; j < 9; j++)
                {
                    TreeNode tn_sub = tn.Nodes[j];
                    foreach (TreeNode tn_sub_child in tn_sub.Nodes)
                    {
                        if (tn.Text == "Squads")
                             path = DataPath.GetPath(DataPath.GetCategoryPath(tn_sub_child.Text, DataDumper.GetInfoType(tn.Text), tn_sub.Text));                                 
                        else  
                        if (tn.Text == "Buildings")
                            path = DataPath.GetPath(DataPath.GetCategoryPath(tn_sub_child.Text, DataDumper.GetInfoType(tn.Text), tn_sub.Text));
                        DataDumper.FilesTable.Add(tn_sub_child.Text, new LuaInfo(path, DataDumper.GetInfoType(tn.Text), tn_sub.Text));
                    }
                }
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
		{
			DataDumper.CreateFilesList();
			DataDumper.WriteFilesList();
			MainForm.Log("File list Builded");
			this.Close();
		}

        

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 2; i++)
            {
                TreeNode tn1 = treeView1.Nodes[i];
                TreeNode tn2 = treeView2.Nodes[i];

                
                for (int j = 0; j < 9; j++)
                    {
                        TreeNode tsn1 = tn1.Nodes[j];
                        TreeNode tsn2 = tn2.Nodes[j];
                        AddFromFolder(tsn1, tsn2);
                    }
            }
            treeView1.Sort();
            treeView2.Sort();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 2; i++)
            {
                TreeNode tn1 = treeView1.Nodes[i];
                TreeNode tn2 = treeView2.Nodes[i];


                for (int j = 0; j < 9; j++)
                {
                    TreeNode tsn1 = tn1.Nodes[j];
                    TreeNode tsn2 = tn2.Nodes[j];
                    RemoveFromFolder(tsn1, tsn2);
                }
            }
            treeView1.Sort();
            treeView2.Sort();
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            TreeNode selected = treeView1.SelectedNode;

            if (selected != null && selected.Nodes.Count > 0)
            {
                AddFolder(selected, treeView2.Nodes);
                RefreshAll();
                treeView1.Sort();
                treeView2.Sort();
            }
            
        }
        
        private void AddFolder(TreeNode selected, TreeNodeCollection tcoll)
        {
            TreeNode[] matches = tcoll.Find(selected.Name, true);
            if (matches.Length > 0)
            {
                for (int i = 0; i < matches.Length; i++)
                {
                    if (matches[i].Parent == null || matches[i].Parent.Name == selected.Parent.Name)
                    {
                        TreeNode match = matches[i];

                        foreach (TreeNode subnode in selected.Nodes)
                        {
                            if (subnode.Nodes.Count == 0)
                            {
                                if (!match.Nodes.ContainsKey(subnode.Name))
                                {
                                    TreeNode newNode = new TreeNode(subnode.Text);
                                    newNode.Name = subnode.Text;
                                    match.Nodes.Add(newNode);
                                }
                            }
                            else AddFolder(subnode, match.Nodes);
                        }
                    }
                }
            }            
        }
        private void RemoveFromFolder(TreeNode tn1,TreeNode tn2)
        {
            foreach (TreeNode tn in tn2.Nodes)
            {
                if (tn.IsSelected)
                {
                    if (tn2.Nodes.ContainsKey(tn.Name))
                    {
                        TreeNode tn3 = tn1.Nodes[tn1.Nodes.IndexOfKey(tn.Name)];
                        tn2.Nodes.RemoveByKey(tn.Name);
                        
                        Refresh(tn3, tn2);
                        break;
                    }
                }
            }
        }

        private void AddFromFolder(TreeNode tn1, TreeNode tn2)
        {
            foreach (TreeNode tn in tn1.Nodes)
            {
                if (tn.IsSelected)
                {
                    if (!tn2.Nodes.ContainsKey(tn.Name))
                    {
                        TreeNode newNode = new TreeNode(tn.Text);
                        newNode.Name = tn.Text;
                        tn2.Nodes.Add(newNode);
                        Refresh(tn, tn2);
                        break;
                    }
                }
            }
        }

        
	}
}
