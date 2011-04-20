namespace Bounce.LevelEditor
{
    partial class RoomSelectDialog
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
            this.room_ListBox = new System.Windows.Forms.ListBox();
            this.room_Label = new System.Windows.Forms.Label();
            this.ok_Button = new System.Windows.Forms.Button();
            this.cancel_Button = new System.Windows.Forms.Button();
            this.exit_Label = new System.Windows.Forms.Label();
            this.doors_ListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // room_ListBox
            // 
            this.room_ListBox.FormattingEnabled = true;
            this.room_ListBox.Location = new System.Drawing.Point(12, 47);
            this.room_ListBox.Name = "room_ListBox";
            this.room_ListBox.Size = new System.Drawing.Size(200, 355);
            this.room_ListBox.TabIndex = 0;
            this.room_ListBox.SelectedIndexChanged += new System.EventHandler(this.room_ListBox_SelectedIndexChanged);
            // 
            // room_Label
            // 
            this.room_Label.AutoSize = true;
            this.room_Label.Location = new System.Drawing.Point(58, 31);
            this.room_Label.Name = "room_Label";
            this.room_Label.Size = new System.Drawing.Size(40, 13);
            this.room_Label.TabIndex = 1;
            this.room_Label.Text = "Rooms";
            // 
            // ok_Button
            // 
            this.ok_Button.Location = new System.Drawing.Point(12, 430);
            this.ok_Button.Name = "ok_Button";
            this.ok_Button.Size = new System.Drawing.Size(112, 31);
            this.ok_Button.TabIndex = 2;
            this.ok_Button.Text = "OK";
            this.ok_Button.UseVisualStyleBackColor = true;
            this.ok_Button.Click += new System.EventHandler(this.ok_Button_Click);
            // 
            // cancel_Button
            // 
            this.cancel_Button.Location = new System.Drawing.Point(130, 430);
            this.cancel_Button.Name = "cancel_Button";
            this.cancel_Button.Size = new System.Drawing.Size(110, 31);
            this.cancel_Button.TabIndex = 3;
            this.cancel_Button.Text = "Cancel";
            this.cancel_Button.UseVisualStyleBackColor = true;
            this.cancel_Button.Click += new System.EventHandler(this.cancel_Button_Click);
            // 
            // exit_Label
            // 
            this.exit_Label.AutoSize = true;
            this.exit_Label.Location = new System.Drawing.Point(275, 31);
            this.exit_Label.Name = "exit_Label";
            this.exit_Label.Size = new System.Drawing.Size(122, 13);
            this.exit_Label.TabIndex = 5;
            this.exit_Label.Text = "Doors in Selected Room";
            // 
            // doors_ListBox
            // 
            this.doors_ListBox.FormattingEnabled = true;
            this.doors_ListBox.Location = new System.Drawing.Point(242, 47);
            this.doors_ListBox.Name = "doors_ListBox";
            this.doors_ListBox.Size = new System.Drawing.Size(200, 355);
            this.doors_ListBox.TabIndex = 6;
            this.doors_ListBox.SelectedIndexChanged += new System.EventHandler(this.doors_ListBox_SelectedIndexChanged);
            // 
            // RoomSelectDialog
            // 
            this.AcceptButton = this.ok_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 473);
            this.Controls.Add(this.doors_ListBox);
            this.Controls.Add(this.exit_Label);
            this.Controls.Add(this.cancel_Button);
            this.Controls.Add(this.ok_Button);
            this.Controls.Add(this.room_Label);
            this.Controls.Add(this.room_ListBox);
            this.Name = "RoomSelectDialog";
            this.Text = "RoomSelect";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox room_ListBox;
        private System.Windows.Forms.Label room_Label;
        private System.Windows.Forms.Button ok_Button;
        private System.Windows.Forms.Button cancel_Button;
        private System.Windows.Forms.Label exit_Label;
        private System.Windows.Forms.ListBox doors_ListBox;
    }
}