using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ConfigEditorUI.Models
{
	public class EditConfigsViewModel : INotifyPropertyChanged
	{
		private ObservableCollection<ConfigurationOptionsViewModel> _configs;
		public ObservableCollection<ConfigurationOptionsViewModel> Configs
		{
			get
			{
				if (_configs == null)
				{
					_configs = new ObservableCollection<ConfigurationOptionsViewModel>();
				}
				return _configs;
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
		protected virtual void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged == null) return;
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
