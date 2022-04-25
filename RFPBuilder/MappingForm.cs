using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RFPBuilder
{
    public partial class MappingForm : Form
    {
        public string RFPName {private get; set; }

        public MappingForm()
        {
            InitializeComponent();
            // this.moduleMapBindingSource.Filter = 'RFPName = SOMEVALUE'
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
