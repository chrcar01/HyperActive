using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ConfigEditorUI.Models
{
	public class ComponentSelectorViewModel : INotifyPropertyChanged
	{
		private bool _isTypeSelected;
		public bool IsTypeSelected
		{
			get
			{
				return _isTypeSelected;
			}
			set
			{
				_isTypeSelected = value;
				RaisePropertyChanged("IsTypeSelected");
			}
		}
		private ObservableCollection<Type> _types;
		public ObservableCollection<Type> Types
		{
			get
			{
				if (_types == null)
				{
					_types = new ObservableCollection<Type>();
				}
				return _types;
			}
		}
		private bool _containsTypes;
		public bool ContainsTypes
		{
			get
			{
				return _containsTypes;
			}
			set
			{
				_containsTypes = value;
				RaisePropertyChanged("ContainsTypes");
			}
		}
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged == null) return;
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}

}
