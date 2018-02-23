using System;
using System.Windows.Forms;
using DokuFlex.WinForms.Common;
using AJMigratorApp.Services;

namespace AJMigratorApp
{
    public partial class MainForm : Form, IMainForm
    {
        private readonly AJImportController aJImportController;
        private readonly ILoginService loginService;

        public MainForm()
        {
            InitializeComponent();
            loginService = new LoginService();
            aJImportController = new AJImportController(loginService, this);
            StopProgress();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new SettingsForm())
            {
                form.ShowDialog();
            }
        }

        private async void ImportButton_Click(object sender, EventArgs e)
        {
            ImportButton.Enabled = false;

            try
            {
                await aJImportController.StartImportAsync();
            }
            finally
            {
                ImportButton.Enabled = true;
            }
        }

        public void StartProgress()
        {
            progressPane.Visible = true;
            progressLabel.Visible = true;
            progressBar.Visible = true;
        }

        public void StopProgress()
        {
            progressPane.Visible = false;
            progressLabel.Visible = false;
            progressBar.Visible = false;
        }

        public void SetProgressInfo(string text)
        {
            progressLabel.Text = text;
        }
    }
}
