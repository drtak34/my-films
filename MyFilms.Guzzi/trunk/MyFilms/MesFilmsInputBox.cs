﻿using System;
using System.Windows.Forms;

namespace MesFilms.MyFilms
{
    public partial class MesFilmsInputBox : Form
    {
        public MesFilmsInputBox()
        {
            InitializeComponent();
        }

        public string UserName
        {
            get { return textBoxNewName.Text; }
            set { textBoxNewName.Text = value; }
        }


        private void textBoxNewName_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

        }
    }
}
