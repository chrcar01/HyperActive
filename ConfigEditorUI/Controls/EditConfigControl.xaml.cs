using System;
using System.Windows.Controls;
using ConfigEditorUI.Models;


namespace ConfigEditorUI.Controls
{
	public partial class EditConfigControl : UserControl
	{
		public EditConfigControl(ConfigurationOptionsViewModel options)
		{
			InitializeComponent();
			DataContext = options;
		}
	}
}
