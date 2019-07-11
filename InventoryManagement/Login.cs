using InventoryManagement.Entities;
using InventoryManagement.Enums;
using InventoryManagement.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryManagement
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUserId.Text.Trim()) && !string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {
                User currentUserInfo = UserManager.Instance.AuthenticateUser(txtUserId.Text.Trim(), txtPassword.Text.Trim());

                if (currentUserInfo != null)
                {
                    if (currentUserInfo.roleType.Equals(RoleType.Administrator))
                    {
                        // login to the admin page
                        this.Hide();
                        Administration administrationWindow = new Administration();
                        administrationWindow.Show(this);
                    }
                    else
                    {
                        // login to the user page
                    }
                }
                else
                {
                    MessageBox.Show("Invalid User Id or Password!");
                }
            }
            else
            {
                MessageBox.Show("Invalid User Id or Password!");
            }
        }

        private void Login_VisibleChanged(object sender, EventArgs e)
        {
            txtPassword.Text = string.Empty;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
