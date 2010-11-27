using System;
using System.Windows;
using ConfigEditorUI.Models;
using System.IO;
using winforms = System.Windows.Forms;
using System.Data.SqlClient;
using HyperActive.SchemaProber;

namespace ConfigEditorUI.Windows
{
	public partial class ProjectWindow : Window
	{
		
		public ProjectWindowViewModel Model
		{
			get
			{
				return DataContext as ProjectWindowViewModel;
			}
			private set
            {
            	DataContext = value;

            }
		}
		public ProjectWindow()
		{
			InitializeComponent();
			Model = new ProjectWindowViewModel();
			
		}
		
		private void ConfigFileTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			string configFilePath = ConfigFileTextBox.Text;
			if (String.IsNullOrEmpty(configFilePath))
			{
				Model.StartButtonEnabled = false;
			}
			if (File.Exists(configFilePath))
			{
				Model.StartButtonEnabled = true;
			}
			else
			{
				Model.StartButtonEnabled = false;
			}
		}

		
		private void ExistingProjectRadioButton_Click(object sender, RoutedEventArgs e)
		{
			Model.SqlConnection.CanEnterUsername = false;
			StartButton.Content = "Open";
			ConfigFileTextBox.SelectAll();
			ConfigFileTextBox.Focus();
		}

		private void NewProjectRadioButton_Click(object sender, RoutedEventArgs e)
		{
			StartButton.Content = "Create";
			Model.SqlConnection.CanEnterUsername = Model.SqlConnection.IsSqlServerSecurity;			
		}

		

		private void BrowseForConfigFileButton_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new winforms.OpenFileDialog();
			dlg.Title = "Select HyperActive Configuration File";
			dlg.DefaultExt = "*.config";
			dlg.Filter = "Config files (*.config) | *.config";
			if (winforms.DialogResult.OK == dlg.ShowDialog())
			{
				Model.ConfigFilePath = dlg.FileName;
			}
		}


		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void SqlConnectionControl_DatabaseSelected(object sender, EventArgs e)
		{
			Model.StartButtonEnabled = true;
		}
	}
}
