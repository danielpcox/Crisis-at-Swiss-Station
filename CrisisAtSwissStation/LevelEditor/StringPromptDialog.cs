using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//using Bounce.Common;

namespace CrisisAtSwissStation.LevelEditor
{
    public partial class StringPromptDialog : Form
    {
        public string UserInput
        {
            get;
            set;
        }

        public StringPromptDialog(string title)
        {
            InitializeComponent();
            this.Text = "";
            label.Text = title;
        }


        //Gets the 
        private void ok_Button_Click(object sender, EventArgs e)
        {
            UserInput = name_TextBox.Text;

            this.DialogResult = DialogResult.OK;
            Close();
        }

        //Closes the current dialog with a result of cancel.
        private void cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
