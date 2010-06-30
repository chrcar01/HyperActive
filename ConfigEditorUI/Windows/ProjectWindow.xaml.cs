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
		private static readonly string AUTHENTICATION_WINDOWS = "Windows Authentication";
		private static readonly string AUTHENTICATION_SQLSERVER = "Sql Server Authentication";
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
			AuthenticationComboBox.Items.Add(AUTHENTICATION_WINDOWS);
			AuthenticationComboBox.Items.Add(AUTHENTICATION_SQLSERVER);
			AuthenticationComboBox.SelectedValue = AUTHENTICATION_WINDOWS;
			Model.Server = "(local)";
			Loaded += ProjectWindow_Loaded;
		}
		public void TryLoadDatabases()
		{
			try
			{
				var builder = new SqlConnectionStringBuilder();
				builder.DataSource = Model.Server;
				builder.IntegratedSecurity = !Model.IsSqlServerSecurity;
				if (Model.IsSqlServerSecurity)
				{
					builder.UserID = Model.Username;
					builder.Password = Model.Password;
				}
				builder.InitialCatalog = "master";
				var helper = new DbHelper(builder.ConnectionString);
				var databases = helper.ExecuteArray<string>("select [name] from sys.databases where name not in ('master', 'tempdb','model','msdb') order by [name]");
				DatabasesComboBox.ItemsSource = databases;
			}
			catch (Exception ex)
			{
				MessageBox.Show("The following error occurred while trying to populate the list of databases:\t\t" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		void ProjectWindow_Loaded(object sender, RoutedEventArgs e)
		{
			ServerTextBox.SelectAll();
			ServerTextBox.Focus();
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
			Model.CanEnterUsername = false;
			StartButton.Content = "Open";
			ConfigFileTextBox.SelectAll();
			ConfigFileTextBox.Focus();
		}

		private void NewProjectRadioButton_Click(object sender, RoutedEventArgs e)
		{
			StartButton.Content = "Create";
			Model.CanEnterUsername = Model.IsSqlServerSecurity;
			ServerTextBox.SelectAll();
			ServerTextBox.Focus();
		}

		private void AuthenticationComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			Model.IsSqlServerSecurity = (AuthenticationComboBox.SelectedItem.ToString() == AUTHENTICATION_SQLSERVER);
			Model.CanEnterUsername = Model.IsSqlServerSecurity && Model.IsNewProject;
			if (Model.IsSqlServerSecurity)
			{
				UsernameTextBox.SelectAll();
				UsernameTextBox.Focus();
			}
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

		private void DatabasesComboBox_DropDownOpened(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(Model.Server) || 
				(Model.IsSqlServerSecurity && (String.IsNullOrEmpty(Model.Username) || String.IsNullOrEmpty(Model.Password))))
				return;

			TryLoadDatabases();
		}

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void DatabasesComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			Model.StartButtonEnabled = true;
		}
	}
}
