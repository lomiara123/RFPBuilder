﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RFPBuilder
{
    public partial class MappingForm : Form
    {
        public string RFPName { get; set; }
        DataSet ds;

        public MappingForm(string rfpName) {
            InitializeComponent();

            RFPName = rfpName;
            string moduleMember, responseMember, positionMember;
            
            (ds, moduleMember, responseMember, positionMember) = DBHandler.getMapping(RFPName);

            ModulesMapGrid.DataSource = ds;
            ModulesMapGrid.DataMember = moduleMember;

            ResponsesGrid.DataSource = ds;
            ResponsesGrid.DataMember = responseMember;

            PositionMapGrid.DataSource = ds;
            PositionMapGrid.DataMember = positionMember;
        }

        private void btnClose_Click(object sender, EventArgs e) {
            DBHandler.updateMapping(ds);
            this.Close();
        }

        private void buttonMinimize_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
