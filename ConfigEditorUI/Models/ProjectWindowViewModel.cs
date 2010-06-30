using System;
using System.ComponentModel;

namespace ConfigEditorUI.Models
{
	public class ProjectWindowViewModel : INotifyPropertyChanged
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

		private bool _startButtonEnabled;
		public bool StartButtonEnabled
		{
			get
			{
				return _startButtonEnabled;
			}
			set
			{
				_startButtonEnabled = value;
				RaisePropertyChanged("StartButtonEnabled");
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
        
		private bool _isNewProject = true;
		public bool IsNewProject
		{
			get
			{
				return _isNewProject;
			}
			set
			{
				_isNewProject = value;
				_isExistingProject = !_isNewProject;
				RaisePropertyChanged("IsNewProject");
				RaisePropertyChanged("IsExistingProject");
			}
		}
		private bool _isExistingProject = false;
		public bool IsExistingProject
		{
			get
			{
				return _isExistingProject;
			}
			set
			{
				_isExistingProject = value;
				_isNewProject = !_isExistingProject;
				RaisePropertyChanged("IsNewProject");
				RaisePropertyChanged("IsExistingProject");
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
		private string _configFilePath;
		public string ConfigFilePath
		{
			get
			{
				return _configFilePath;
			}
			set
			{
				_configFilePath = value;
				RaisePropertyChanged("ConfigFilePath");
			}
		}
                
        
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		private void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged == null) return;
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
