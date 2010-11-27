using System;
using System.Windows;
using System.Data.SqlClient;
using ConfigEditorUI.Models;

namespace ConfigEditorUI.Windows
{
	public partial class EditSqlConnectionStringWindow : Window
	{
		private string _connectionString;
		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
			set
			{
				_connectionString = value;
			}
		}
		public EditSqlConnectionStringWindow()
			: this(String.Empty)
		{
		}
        public EditSqlConnectionStringWindow(string connectionString)
		{
			InitializeComponent();
			var model = new EditSqlConnectionViewModel();
			if (!String.IsNullOrEmpty(connectionString))
			{
				var builder = new SqlConnectionStringBuilder(connectionString);
				model.Server = builder.DataSource;
				model.Database = builder.InitialCatalog;
				if (!builder.IntegratedSecurity)
				{
					model.IsSqlServerSecurity = true;
					model.Username = builder.UserID;
					model.Password = builder.Password;
				}
			}
			DataContext = model;
		}

		private void SqlConnectionControl_DatabaseSelected(object sender, EventArgs e)
		{
			OKButton.IsEnabled = true;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			var builder = new SqlConnectionStringBuilder();
			var model = DataContext as EditSqlConnectionViewModel;
			builder.InitialCatalog = model.Database;
			builder.DataSource = model.Server;
			if (model.IsSqlServerSecurity)
			{
				builder.UserID = model.Username;
				builder.Password = model.Password;
			}
			ConnectionString = builder.ConnectionString;
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
