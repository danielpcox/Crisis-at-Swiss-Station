namespace CrisisAtSwissStation.LevelEditor
{
    partial class Editor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
            this.panel_Main = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel_EditOptions = new System.Windows.Forms.Panel();
            this.tabctrl_TexProp = new System.Windows.Forms.TabControl();
            this.tab_Objects = new System.Windows.Forms.TabPage();
            this.gb_ObjTypes = new System.Windows.Forms.GroupBox();
            this.rb_Vanish_Walls = new System.Windows.Forms.RadioButton();
            this.rb_SavePoint = new System.Windows.Forms.RadioButton();
            this.rb_VictoryTest = new System.Windows.Forms.RadioButton();
            this.rb_Doors = new System.Windows.Forms.RadioButton();
            this.rb_Handlebars = new System.Windows.Forms.RadioButton();
            this.rb_Survivors = new System.Windows.Forms.RadioButton();
            this.rb_Parts = new System.Windows.Forms.RadioButton();
            this.rb_HazardDynamic = new System.Windows.Forms.RadioButton();
            this.rb_HazardStatic = new System.Windows.Forms.RadioButton();
            this.rb_SensorObjects = new System.Windows.Forms.RadioButton();
            this.rb_BoxObjects = new System.Windows.Forms.RadioButton();
            this.rb_AnimationObjects = new System.Windows.Forms.RadioButton();
            this.tab_objProps = new System.Windows.Forms.TabPage();
            this.b_Front = new System.Windows.Forms.Button();
            this.tb_Scale = new System.Windows.Forms.TextBox();
            this.lbl_SLevel = new System.Windows.Forms.Label();
            this.tb_Script = new System.Windows.Forms.TextBox();
            this.cbox_Scripted = new System.Windows.Forms.CheckBox();
            this.lbl_DamageWarning = new System.Windows.Forms.Label();
            this.lbl_RotationWarning = new System.Windows.Forms.Label();
            this.b_ApplyProperties = new System.Windows.Forms.Button();
            this.tb_Damage = new System.Windows.Forms.TextBox();
            this.tb_Rotation = new System.Windows.Forms.TextBox();
            this.lbl_Damage = new System.Windows.Forms.Label();
            this.lbl_Rotation = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createNewWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mi_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mi_Load_World = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tool_Insertion = new System.Windows.Forms.ToolStripButton();
            this.tool_Selection = new System.Windows.Forms.ToolStripButton();
            this.panel_TextureList = new System.Windows.Forms.Panel();
            this.lb_TextureList = new System.Windows.Forms.ListBox();
            this.pb_Level = new System.Windows.Forms.PictureBox();
            this.panel_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel_EditOptions.SuspendLayout();
            this.tabctrl_TexProp.SuspendLayout();
            this.tab_Objects.SuspendLayout();
            this.gb_ObjTypes.SuspendLayout();
            this.tab_objProps.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel_TextureList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Level)).BeginInit();
            this.SuspendLayout();
            // 
            // panel_Main
            // 
            this.panel_Main.AutoScroll = true;
            this.panel_Main.Controls.Add(this.splitContainer1);
            this.panel_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Main.Location = new System.Drawing.Point(0, 0);
            this.panel_Main.Name = "panel_Main";
            this.panel_Main.Size = new System.Drawing.Size(1282, 772);
            this.panel_Main.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.pb_Level);
            this.splitContainer1.Size = new System.Drawing.Size(1282, 772);
            this.splitContainer1.SplitterDistance = 250;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panel_EditOptions);
            this.splitContainer2.Panel1.Controls.Add(this.menuStrip1);
            this.splitContainer2.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel_TextureList);
            this.splitContainer2.Size = new System.Drawing.Size(250, 772);
            this.splitContainer2.SplitterDistance = 495;
            this.splitContainer2.TabIndex = 0;
            // 
            // panel_EditOptions
            // 
            this.panel_EditOptions.Controls.Add(this.tabctrl_TexProp);
            this.panel_EditOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_EditOptions.Location = new System.Drawing.Point(0, 49);
            this.panel_EditOptions.Name = "panel_EditOptions";
            this.panel_EditOptions.Size = new System.Drawing.Size(246, 442);
            this.panel_EditOptions.TabIndex = 12;
            // 
            // tabctrl_TexProp
            // 
            this.tabctrl_TexProp.Controls.Add(this.tab_Objects);
            this.tabctrl_TexProp.Controls.Add(this.tab_objProps);
            this.tabctrl_TexProp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabctrl_TexProp.Location = new System.Drawing.Point(0, 0);
            this.tabctrl_TexProp.Name = "tabctrl_TexProp";
            this.tabctrl_TexProp.SelectedIndex = 0;
            this.tabctrl_TexProp.Size = new System.Drawing.Size(246, 442);
            this.tabctrl_TexProp.TabIndex = 1;
            // 
            // tab_Objects
            // 
            this.tab_Objects.Controls.Add(this.gb_ObjTypes);
            this.tab_Objects.Location = new System.Drawing.Point(4, 22);
            this.tab_Objects.Name = "tab_Objects";
            this.tab_Objects.Padding = new System.Windows.Forms.Padding(3);
            this.tab_Objects.Size = new System.Drawing.Size(238, 416);
            this.tab_Objects.TabIndex = 0;
            this.tab_Objects.Text = "Objects";
            this.tab_Objects.UseVisualStyleBackColor = true;
            // 
            // gb_ObjTypes
            // 
            this.gb_ObjTypes.Controls.Add(this.rb_Vanish_Walls);
            this.gb_ObjTypes.Controls.Add(this.rb_SavePoint);
            this.gb_ObjTypes.Controls.Add(this.rb_VictoryTest);
            this.gb_ObjTypes.Controls.Add(this.rb_Doors);
            this.gb_ObjTypes.Controls.Add(this.rb_Handlebars);
            this.gb_ObjTypes.Controls.Add(this.rb_Survivors);
            this.gb_ObjTypes.Controls.Add(this.rb_Parts);
            this.gb_ObjTypes.Controls.Add(this.rb_HazardDynamic);
            this.gb_ObjTypes.Controls.Add(this.rb_HazardStatic);
            this.gb_ObjTypes.Controls.Add(this.rb_SensorObjects);
            this.gb_ObjTypes.Controls.Add(this.rb_BoxObjects);
            this.gb_ObjTypes.Controls.Add(this.rb_AnimationObjects);
            this.gb_ObjTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_ObjTypes.Location = new System.Drawing.Point(3, 3);
            this.gb_ObjTypes.Name = "gb_ObjTypes";
            this.gb_ObjTypes.Size = new System.Drawing.Size(232, 410);
            this.gb_ObjTypes.TabIndex = 0;
            this.gb_ObjTypes.TabStop = false;
            this.gb_ObjTypes.Text = "Object Types";
            // 
            // rb_Vanish_Walls
            // 
            this.rb_Vanish_Walls.AutoSize = true;
            this.rb_Vanish_Walls.Location = new System.Drawing.Point(20, 278);
            this.rb_Vanish_Walls.Name = "rb_Vanish_Walls";
            this.rb_Vanish_Walls.Size = new System.Drawing.Size(86, 17);
            this.rb_Vanish_Walls.TabIndex = 11;
            this.rb_Vanish_Walls.TabStop = true;
            this.rb_Vanish_Walls.Text = "Vanish Walls";
            this.rb_Vanish_Walls.UseVisualStyleBackColor = true;
            this.rb_Vanish_Walls.Visible = false;
            this.rb_Vanish_Walls.CheckedChanged += new System.EventHandler(this.rb_Vanish_Walls_CheckedChanged);
            // 
            // rb_SavePoint
            // 
            this.rb_SavePoint.AutoSize = true;
            this.rb_SavePoint.Location = new System.Drawing.Point(20, 255);
            this.rb_SavePoint.Name = "rb_SavePoint";
            this.rb_SavePoint.Size = new System.Drawing.Size(77, 17);
            this.rb_SavePoint.TabIndex = 10;
            this.rb_SavePoint.TabStop = true;
            this.rb_SavePoint.Text = "Save Point";
            this.rb_SavePoint.UseVisualStyleBackColor = true;
            this.rb_SavePoint.Visible = false;
            this.rb_SavePoint.CheckedChanged += new System.EventHandler(this.rb_SavePoint_CheckedChanged);
            // 
            // rb_VictoryTest
            // 
            this.rb_VictoryTest.AutoSize = true;
            this.rb_VictoryTest.Location = new System.Drawing.Point(20, 232);
            this.rb_VictoryTest.Name = "rb_VictoryTest";
            this.rb_VictoryTest.Size = new System.Drawing.Size(81, 17);
            this.rb_VictoryTest.TabIndex = 9;
            this.rb_VictoryTest.TabStop = true;
            this.rb_VictoryTest.Text = "Victory Test";
            this.rb_VictoryTest.UseVisualStyleBackColor = true;
            this.rb_VictoryTest.Visible = false;
            this.rb_VictoryTest.CheckedChanged += new System.EventHandler(this.rb_VictoryTest_CheckedChanged);
            // 
            // rb_Doors
            // 
            this.rb_Doors.AutoSize = true;
            this.rb_Doors.Location = new System.Drawing.Point(20, 209);
            this.rb_Doors.Name = "rb_Doors";
            this.rb_Doors.Size = new System.Drawing.Size(53, 17);
            this.rb_Doors.TabIndex = 8;
            this.rb_Doors.TabStop = true;
            this.rb_Doors.Text = "Doors";
            this.rb_Doors.UseVisualStyleBackColor = true;
            this.rb_Doors.Visible = false;
            this.rb_Doors.CheckedChanged += new System.EventHandler(this.rb_Doors_CheckedChanged);
            // 
            // rb_Handlebars
            // 
            this.rb_Handlebars.AutoSize = true;
            this.rb_Handlebars.Location = new System.Drawing.Point(20, 186);
            this.rb_Handlebars.Name = "rb_Handlebars";
            this.rb_Handlebars.Size = new System.Drawing.Size(79, 17);
            this.rb_Handlebars.TabIndex = 7;
            this.rb_Handlebars.TabStop = true;
            this.rb_Handlebars.Text = "Handlebars";
            this.rb_Handlebars.UseVisualStyleBackColor = true;
            this.rb_Handlebars.Visible = false;
            this.rb_Handlebars.CheckedChanged += new System.EventHandler(this.rb_Handlebars_CheckedChanged_1);
            // 
            // rb_Survivors
            // 
            this.rb_Survivors.AutoSize = true;
            this.rb_Survivors.Location = new System.Drawing.Point(20, 162);
            this.rb_Survivors.Name = "rb_Survivors";
            this.rb_Survivors.Size = new System.Drawing.Size(69, 17);
            this.rb_Survivors.TabIndex = 6;
            this.rb_Survivors.TabStop = true;
            this.rb_Survivors.Text = "Survivors";
            this.rb_Survivors.UseVisualStyleBackColor = true;
            this.rb_Survivors.Visible = false;
            this.rb_Survivors.CheckedChanged += new System.EventHandler(this.rb_Survivors_CheckedChanged);
            // 
            // rb_Parts
            // 
            this.rb_Parts.AutoSize = true;
            this.rb_Parts.Location = new System.Drawing.Point(20, 138);
            this.rb_Parts.Name = "rb_Parts";
            this.rb_Parts.Size = new System.Drawing.Size(49, 17);
            this.rb_Parts.TabIndex = 5;
            this.rb_Parts.TabStop = true;
            this.rb_Parts.Text = "Parts";
            this.rb_Parts.UseVisualStyleBackColor = true;
            this.rb_Parts.Visible = false;
            this.rb_Parts.CheckedChanged += new System.EventHandler(this.rb_Parts_CheckedChanged_1);
            // 
            // rb_HazardDynamic
            // 
            this.rb_HazardDynamic.AutoSize = true;
            this.rb_HazardDynamic.Location = new System.Drawing.Point(20, 114);
            this.rb_HazardDynamic.Name = "rb_HazardDynamic";
            this.rb_HazardDynamic.Size = new System.Drawing.Size(109, 17);
            this.rb_HazardDynamic.TabIndex = 4;
            this.rb_HazardDynamic.TabStop = true;
            this.rb_HazardDynamic.Text = "Hazard - Dynamic";
            this.rb_HazardDynamic.UseVisualStyleBackColor = true;
            this.rb_HazardDynamic.Visible = false;
            this.rb_HazardDynamic.CheckedChanged += new System.EventHandler(this.rb_HazardDynamic_CheckedChanged_1);
            // 
            // rb_HazardStatic
            // 
            this.rb_HazardStatic.AutoSize = true;
            this.rb_HazardStatic.Location = new System.Drawing.Point(20, 90);
            this.rb_HazardStatic.Name = "rb_HazardStatic";
            this.rb_HazardStatic.Size = new System.Drawing.Size(95, 17);
            this.rb_HazardStatic.TabIndex = 3;
            this.rb_HazardStatic.TabStop = true;
            this.rb_HazardStatic.Text = "Hazard - Static";
            this.rb_HazardStatic.UseVisualStyleBackColor = true;
            this.rb_HazardStatic.Visible = false;
            this.rb_HazardStatic.CheckedChanged += new System.EventHandler(this.rb_HazardStatic_CheckedChanged_1);
            // 
            // rb_SensorObjects
            // 
            this.rb_SensorObjects.AutoSize = true;
            this.rb_SensorObjects.Location = new System.Drawing.Point(20, 66);
            this.rb_SensorObjects.Name = "rb_SensorObjects";
            this.rb_SensorObjects.Size = new System.Drawing.Size(94, 17);
            this.rb_SensorObjects.TabIndex = 2;
            this.rb_SensorObjects.TabStop = true;
            this.rb_SensorObjects.Text = "SensorObjects";
            this.rb_SensorObjects.UseVisualStyleBackColor = true;
            this.rb_SensorObjects.CheckedChanged += new System.EventHandler(this.rb_SensorObjects_CheckedChanged);
            // 
            // rb_BoxObjects
            // 
            this.rb_BoxObjects.AutoSize = true;
            this.rb_BoxObjects.Location = new System.Drawing.Point(20, 42);
            this.rb_BoxObjects.Name = "rb_BoxObjects";
            this.rb_BoxObjects.Size = new System.Drawing.Size(79, 17);
            this.rb_BoxObjects.TabIndex = 1;
            this.rb_BoxObjects.TabStop = true;
            this.rb_BoxObjects.Text = "BoxObjects";
            this.rb_BoxObjects.UseVisualStyleBackColor = true;
            this.rb_BoxObjects.CheckedChanged += new System.EventHandler(this.rb_BoxObjects_CheckedChanged);
            // 
            // rb_AnimationObjects
            // 
            this.rb_AnimationObjects.AutoSize = true;
            this.rb_AnimationObjects.Location = new System.Drawing.Point(20, 19);
            this.rb_AnimationObjects.Name = "rb_AnimationObjects";
            this.rb_AnimationObjects.Size = new System.Drawing.Size(107, 17);
            this.rb_AnimationObjects.TabIndex = 0;
            this.rb_AnimationObjects.TabStop = true;
            this.rb_AnimationObjects.Text = "AnimationObjects";
            this.rb_AnimationObjects.UseVisualStyleBackColor = true;
            this.rb_AnimationObjects.CheckedChanged += new System.EventHandler(this.rb_AnimationObjects_CheckedChanged);
            // 
            // tab_objProps
            // 
            this.tab_objProps.Controls.Add(this.b_Front);
            this.tab_objProps.Controls.Add(this.tb_Scale);
            this.tab_objProps.Controls.Add(this.lbl_SLevel);
            this.tab_objProps.Controls.Add(this.tb_Script);
            this.tab_objProps.Controls.Add(this.cbox_Scripted);
            this.tab_objProps.Controls.Add(this.lbl_DamageWarning);
            this.tab_objProps.Controls.Add(this.lbl_RotationWarning);
            this.tab_objProps.Controls.Add(this.b_ApplyProperties);
            this.tab_objProps.Controls.Add(this.tb_Damage);
            this.tab_objProps.Controls.Add(this.tb_Rotation);
            this.tab_objProps.Controls.Add(this.lbl_Damage);
            this.tab_objProps.Controls.Add(this.lbl_Rotation);
            this.tab_objProps.Location = new System.Drawing.Point(4, 22);
            this.tab_objProps.Name = "tab_objProps";
            this.tab_objProps.Padding = new System.Windows.Forms.Padding(3);
            this.tab_objProps.Size = new System.Drawing.Size(238, 416);
            this.tab_objProps.TabIndex = 1;
            this.tab_objProps.Text = "Object Properties";
            this.tab_objProps.UseVisualStyleBackColor = true;
            // 
            // b_Front
            // 
            this.b_Front.Location = new System.Drawing.Point(125, 98);
            this.b_Front.Margin = new System.Windows.Forms.Padding(2);
            this.b_Front.Name = "b_Front";
            this.b_Front.Size = new System.Drawing.Size(82, 28);
            this.b_Front.TabIndex = 15;
            this.b_Front.Text = "Move to Front";
            this.b_Front.UseVisualStyleBackColor = true;
            this.b_Front.Click += new System.EventHandler(this.b_Front_Click);
            // 
            // tb_Scale
            // 
            this.tb_Scale.Location = new System.Drawing.Point(108, 70);
            this.tb_Scale.Name = "tb_Scale";
            this.tb_Scale.Size = new System.Drawing.Size(100, 20);
            this.tb_Scale.TabIndex = 14;
            this.tb_Scale.Text = "0";
            // 
            // lbl_SLevel
            // 
            this.lbl_SLevel.AutoSize = true;
            this.lbl_SLevel.Location = new System.Drawing.Point(3, 70);
            this.lbl_SLevel.Name = "lbl_SLevel";
            this.lbl_SLevel.Size = new System.Drawing.Size(34, 13);
            this.lbl_SLevel.TabIndex = 13;
            this.lbl_SLevel.Text = "Scale";
            // 
            // tb_Script
            // 
            this.tb_Script.Enabled = false;
            this.tb_Script.Location = new System.Drawing.Point(18, 176);
            this.tb_Script.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Script.Multiline = true;
            this.tb_Script.Name = "tb_Script";
            this.tb_Script.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_Script.Size = new System.Drawing.Size(190, 100);
            this.tb_Script.TabIndex = 12;
            // 
            // cbox_Scripted
            // 
            this.cbox_Scripted.AutoSize = true;
            this.cbox_Scripted.Enabled = false;
            this.cbox_Scripted.Location = new System.Drawing.Point(18, 144);
            this.cbox_Scripted.Margin = new System.Windows.Forms.Padding(2);
            this.cbox_Scripted.Name = "cbox_Scripted";
            this.cbox_Scripted.Size = new System.Drawing.Size(71, 17);
            this.cbox_Scripted.TabIndex = 11;
            this.cbox_Scripted.Text = "Scripted?";
            this.cbox_Scripted.UseVisualStyleBackColor = true;
            this.cbox_Scripted.CheckedChanged += new System.EventHandler(this.cbox_Scripted_CheckedChanged);
            // 
            // lbl_DamageWarning
            // 
            this.lbl_DamageWarning.AutoSize = true;
            this.lbl_DamageWarning.Location = new System.Drawing.Point(3, 116);
            this.lbl_DamageWarning.Name = "lbl_DamageWarning";
            this.lbl_DamageWarning.Size = new System.Drawing.Size(104, 13);
            this.lbl_DamageWarning.TabIndex = 10;
            this.lbl_DamageWarning.Text = "Damage must be int!";
            this.lbl_DamageWarning.Visible = false;
            // 
            // lbl_RotationWarning
            // 
            this.lbl_RotationWarning.AutoSize = true;
            this.lbl_RotationWarning.Location = new System.Drawing.Point(3, 103);
            this.lbl_RotationWarning.Name = "lbl_RotationWarning";
            this.lbl_RotationWarning.Size = new System.Drawing.Size(113, 13);
            this.lbl_RotationWarning.TabIndex = 9;
            this.lbl_RotationWarning.Text = "Rotation must be float!";
            this.lbl_RotationWarning.Visible = false;
            // 
            // b_ApplyProperties
            // 
            this.b_ApplyProperties.Location = new System.Drawing.Point(125, 132);
            this.b_ApplyProperties.Name = "b_ApplyProperties";
            this.b_ApplyProperties.Size = new System.Drawing.Size(82, 39);
            this.b_ApplyProperties.TabIndex = 8;
            this.b_ApplyProperties.Text = "Apply";
            this.b_ApplyProperties.UseVisualStyleBackColor = true;
            this.b_ApplyProperties.Click += new System.EventHandler(this.b_ApplyProperties_Click);
            // 
            // tb_Damage
            // 
            this.tb_Damage.Location = new System.Drawing.Point(108, 40);
            this.tb_Damage.Name = "tb_Damage";
            this.tb_Damage.Size = new System.Drawing.Size(100, 20);
            this.tb_Damage.TabIndex = 7;
            this.tb_Damage.Leave += new System.EventHandler(this.tb_Damage_Leave);
            // 
            // tb_Rotation
            // 
            this.tb_Rotation.Location = new System.Drawing.Point(108, 13);
            this.tb_Rotation.Name = "tb_Rotation";
            this.tb_Rotation.Size = new System.Drawing.Size(100, 20);
            this.tb_Rotation.TabIndex = 4;
            this.tb_Rotation.Text = "0.0";
            this.tb_Rotation.Leave += new System.EventHandler(this.tb_Rotation_Leave);
            // 
            // lbl_Damage
            // 
            this.lbl_Damage.AutoSize = true;
            this.lbl_Damage.Location = new System.Drawing.Point(3, 40);
            this.lbl_Damage.Name = "lbl_Damage";
            this.lbl_Damage.Size = new System.Drawing.Size(47, 13);
            this.lbl_Damage.TabIndex = 6;
            this.lbl_Damage.Text = "Damage";
            // 
            // lbl_Rotation
            // 
            this.lbl_Rotation.AutoSize = true;
            this.lbl_Rotation.Location = new System.Drawing.Point(3, 16);
            this.lbl_Rotation.Name = "lbl_Rotation";
            this.lbl_Rotation.Size = new System.Drawing.Size(94, 13);
            this.lbl_Rotation.TabIndex = 5;
            this.lbl_Rotation.Text = "Rotation (degrees)";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 25);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(246, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menu_Main";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewWorld,
            this.toolStripSeparator1,
            this.mi_Save,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator3,
            this.mi_Load_World,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.fileToolStripMenuItem.Text = "World";
            // 
            // createNewWorld
            // 
            this.createNewWorld.Name = "createNewWorld";
            this.createNewWorld.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.createNewWorld.Size = new System.Drawing.Size(215, 22);
            this.createNewWorld.Text = "Create New World";
            this.createNewWorld.Click += new System.EventHandler(this.mi_New_World_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(212, 6);
            // 
            // mi_Save
            // 
            this.mi_Save.Name = "mi_Save";
            this.mi_Save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mi_Save.Size = new System.Drawing.Size(215, 22);
            this.mi_Save.Text = "Save";
            this.mi_Save.Click += new System.EventHandler(this.mi_Save_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.mi_Save_As_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(212, 6);
            // 
            // mi_Load_World
            // 
            this.mi_Load_World.Name = "mi_Load_World";
            this.mi_Load_World.Size = new System.Drawing.Size(215, 22);
            this.mi_Load_World.Text = "Load World...";
            this.mi_Load_World.Click += new System.EventHandler(this.mi_Load_World_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(212, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tool_Insertion,
            this.tool_Selection});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(246, 25);
            this.toolStrip1.TabIndex = 11;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tool_Insertion
            // 
            this.tool_Insertion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tool_Insertion.Image = ((System.Drawing.Image)(resources.GetObject("tool_Insertion.Image")));
            this.tool_Insertion.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_Insertion.Name = "tool_Insertion";
            this.tool_Insertion.Size = new System.Drawing.Size(40, 22);
            this.tool_Insertion.Text = "Insert";
            this.tool_Insertion.Click += new System.EventHandler(this.tool_Insertion_Click);
            // 
            // tool_Selection
            // 
            this.tool_Selection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tool_Selection.Image = ((System.Drawing.Image)(resources.GetObject("tool_Selection.Image")));
            this.tool_Selection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_Selection.Name = "tool_Selection";
            this.tool_Selection.Size = new System.Drawing.Size(40, 22);
            this.tool_Selection.Text = "Select";
            this.tool_Selection.Click += new System.EventHandler(this.tool_Selection_Click);
            // 
            // panel_TextureList
            // 
            this.panel_TextureList.Controls.Add(this.lb_TextureList);
            this.panel_TextureList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_TextureList.Location = new System.Drawing.Point(0, 0);
            this.panel_TextureList.Name = "panel_TextureList";
            this.panel_TextureList.Size = new System.Drawing.Size(246, 269);
            this.panel_TextureList.TabIndex = 0;
            // 
            // lb_TextureList
            // 
            this.lb_TextureList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_TextureList.FormattingEnabled = true;
            this.lb_TextureList.Location = new System.Drawing.Point(0, 0);
            this.lb_TextureList.Name = "lb_TextureList";
            this.lb_TextureList.Size = new System.Drawing.Size(246, 269);
            this.lb_TextureList.TabIndex = 0;
            // 
            // pb_Level
            // 
            this.pb_Level.Location = new System.Drawing.Point(0, 0);
            this.pb_Level.Name = "pb_Level";
            this.pb_Level.Size = new System.Drawing.Size(1024, 768);
            this.pb_Level.TabIndex = 0;
            this.pb_Level.TabStop = false;
            this.pb_Level.Paint += new System.Windows.Forms.PaintEventHandler(this.pb_Level_Paint);
            this.pb_Level.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pb_Level_MouseClick);
            this.pb_Level.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pb_Level_MouseDown);
            this.pb_Level.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pb_Level_MouseMove);
            this.pb_Level.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pb_Level_MouseUp);
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1282, 772);
            this.Controls.Add(this.panel_Main);
            this.Name = "Editor";
            this.Text = "Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Editor_FormClosing);
            this.panel_Main.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel_EditOptions.ResumeLayout(false);
            this.tabctrl_TexProp.ResumeLayout(false);
            this.tab_Objects.ResumeLayout(false);
            this.gb_ObjTypes.ResumeLayout(false);
            this.gb_ObjTypes.PerformLayout();
            this.tab_objProps.ResumeLayout(false);
            this.tab_objProps.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel_TextureList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pb_Level)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_Main;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel_TextureList;
        private System.Windows.Forms.ListBox lb_TextureList;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mi_Save;
        private System.Windows.Forms.ToolStripMenuItem mi_Load_World;
        private System.Windows.Forms.ToolStripMenuItem createNewWorld;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tool_Insertion;
        private System.Windows.Forms.ToolStripButton tool_Selection;
        private System.Windows.Forms.Panel panel_EditOptions;
        private System.Windows.Forms.TabControl tabctrl_TexProp;
        private System.Windows.Forms.TabPage tab_Objects;
        private System.Windows.Forms.GroupBox gb_ObjTypes;
        private System.Windows.Forms.RadioButton rb_Doors;
        private System.Windows.Forms.RadioButton rb_Handlebars;
        private System.Windows.Forms.RadioButton rb_Survivors;
        private System.Windows.Forms.RadioButton rb_Parts;
        private System.Windows.Forms.RadioButton rb_HazardDynamic;
        private System.Windows.Forms.RadioButton rb_HazardStatic;
        private System.Windows.Forms.RadioButton rb_SensorObjects;
        private System.Windows.Forms.RadioButton rb_BoxObjects;
        private System.Windows.Forms.RadioButton rb_AnimationObjects;
        private System.Windows.Forms.TabPage tab_objProps;
        private System.Windows.Forms.Label lbl_DamageWarning;
        private System.Windows.Forms.Label lbl_RotationWarning;
        private System.Windows.Forms.Button b_ApplyProperties;
        private System.Windows.Forms.TextBox tb_Damage;
        private System.Windows.Forms.Label lbl_Damage;
        private System.Windows.Forms.Label lbl_Rotation;
        private System.Windows.Forms.CheckBox cbox_Scripted;
        private System.Windows.Forms.TextBox tb_Script;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.RadioButton rb_VictoryTest;
        private System.Windows.Forms.RadioButton rb_SavePoint;
        private System.Windows.Forms.TextBox tb_Scale;
        private System.Windows.Forms.Label lbl_SLevel;
        private System.Windows.Forms.TextBox tb_Rotation;
        private System.Windows.Forms.Button b_Front;
        private System.Windows.Forms.RadioButton rb_Vanish_Walls;
        private System.Windows.Forms.PictureBox pb_Level;



    }
}