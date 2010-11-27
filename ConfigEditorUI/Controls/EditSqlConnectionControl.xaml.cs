using System;
using System.Windows.Controls;
using ConfigEditorUI.Models;
using System.Data.SqlClient;
using HyperActive.SchemaProber;
using System.Windows;

namespace ConfigEditorUI.Controls
{
	public partial class EditSqlConnectionControl : UserControl
	{
		public event EventHandler DatabaseSelected;
		protected void RaiseDatabaseSelected()
		{
			if (DatabaseSelected == null) return;
			DatabaseSelected(this, new EventArgs());
		}
		private static readonly string AUTHENTICATION_WINDOWS = "Windows Authentication";
		private static readonly string AUTHENTICATION_SQLSERVER = "Sql Server Authentication";
		public EditSqlConnectionViewModel Model
		{
			get
			{
				return DataContext as EditSqlConnectionViewModel;
			}
			set
			{
				DataContext = value;
			}
		}
		public EditSqlConnectionControl()
		{
			InitializeComponent();
			Loaded += EditSqlConnectionControl_Loaded;
			
		}

		void EditSqlConnectionControl_Loaded(object sender, RoutedEventArgs e)
		{
			AuthenticationComboBox.Items.Add(AUTHENTICATION_WINDOWS);
			AuthenticationComboBox.Items.Add(AUTHENTICATION_SQLSERVER);
			if (Model != null) Model.Server = "(local)";

		}

		private void DatabasesComboBox_DropDownOpened(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(Model.Server) ||
				(Model.IsSqlServerSecurity && (String.IsNullOrEmpty(Model.Username) || String.IsNullOrEmpty(Model.Password))))
				return;

			TryLoadDatabases();
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
		protected void AuthenticationComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			Model.IsSqlServerSecurity = (AuthenticationComboBox.SelectedItem.ToString() == AUTHENTICATION_SQLSERVER);
			Model.CanEnterUsername = Model.IsSqlServerSecurity;
			if (Model.IsSqlServerSecurity)
			{
				UsernameTextBox.SelectAll();
				UsernameTextBox.Focus();
			}
		}

		private void DatabasesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RaiseDatabaseSelected();
		}
	}
}
