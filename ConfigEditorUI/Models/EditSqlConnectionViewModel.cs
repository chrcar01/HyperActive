using System;
using System.ComponentModel;

namespace ConfigEditorUI.Models
{
	public class EditSqlConnectionViewModel : INotifyPropertyChanged
	{
		private string _database;
		public string Database
		{
			get
			{
				return _database;
			}
			set
			{
				_database = value;
				RaisePropertyChanged("Database");
			}
		}

		private bool _isIntegratedSecurity;
		public bool IsIntegratedSecurity
		{
			get
			{
				return _isIntegratedSecurity;
			}
			set
			{
				_isIntegratedSecurity = value;
				RaisePropertyChanged("IsIntegratedSecurity");
			}
		}
		private bool _isSqlServerSecurity;
		public bool IsSqlServerSecurity
		{
			get
			{
				return _isSqlServerSecurity;
			}
			set
			{
				_isSqlServerSecurity = value;
				RaisePropertyChanged("IsSqlServerSecurity");
			}
		}
		private bool _canEnterUsername;
		public bool CanEnterUsername
		{
			get
			{
				return _canEnterUsername;
			}
			set
			{
				_canEnterUsername = value;
				RaisePropertyChanged("CanEnterUsername");
			}
		}

		private string _server;
		public string Server
		{
			get
			{
				return _server;
			}
			set
			{
				_server = value;
				RaisePropertyChanged("Server");
			}
		}
		private string _username;
		public string Username
		{
			get
			{
				return _username;
			}
			set
			{
				_username = value;
				RaisePropertyChanged("Username");
			}
		}
		private string _password;
		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;
				RaisePropertyChanged("Password");
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged == null) return;
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			Console.WriteLine(propertyName);
		}

	}
}
