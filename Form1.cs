using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AIUBResourceManagementSystem.Properties;

namespace AIUBResourceManagementSystem
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();
        }

        private void Btnstart_Click(object sender, EventArgs e)
        {

            RegistrationForm registrationForm = new RegistrationForm();
            registrationForm.Size = this.Size;
            registrationForm.ShowDialog();

        }

        private void Welcome_Load(object sender, EventArgs e)
        {

        }
    }
    }
           
    
    

    


