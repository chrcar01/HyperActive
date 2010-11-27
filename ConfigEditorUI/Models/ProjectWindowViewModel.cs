using System;
using System.ComponentModel;

namespace ConfigEditorUI.Models
{
	public class ProjectWindowViewModel : INotifyPropertyChanged
	{

		private EditSqlConnectionViewModel _sqlConnection;
		public EditSqlConnectionViewModel SqlConnection
		{
			get
			{
				if (_sqlConnection == null)
				{
					_sqlConnection = new EditSqlConnectionViewModel();
				}
				return _sqlConnection;
			}
			set
			{
				_sqlConnection = value;
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
