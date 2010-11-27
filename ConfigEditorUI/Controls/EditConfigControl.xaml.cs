using ConfigEditorUI.Models;
using ConfigEditorUI.Windows;
using HyperActive.Core.Generators;
using System;
using System.Windows;
using System.Windows.Controls;


namespace ConfigEditorUI.Controls
{
	public partial class EditConfigControl : UserControl
	{
		private ConfigurationOptionsViewModel Model
		{
			get
			{
				return DataContext as ConfigurationOptionsViewModel;
			}
			set
			{
				DataContext = value;
			}
		}
		public EditConfigControl()
		{
			
			InitializeComponent();
		}
		public EditConfigControl(ConfigurationOptionsViewModel options)
		{
			InitializeComponent();
			Model = options;
		}

		private void SelectGeneratorButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var win = new ComponentSelectorWindow(typeof(ActiveRecordGenerator));
			win.Owner = Window.GetWindow(this);
			win.ShowDialog();
			if (String.IsNullOrEmpty(win.TypeAndAssemblyName)) return;
			Model.GeneratorTypeName = win.TypeAndAssemblyName;
		}

		private void EditConnectionStringButton_Click(object sender, RoutedEventArgs e)
		{
			
			var win = new EditSqlConnectionStringWindow(Model.ConnectionString);
			win.Owner = Window.GetWindow(this);
			win.ShowDialog();
			if (String.IsNullOrEmpty(win.ConnectionString)) return;
			Model.ConnectionString = win.ConnectionString;
		}

		private void FindOutputPathButton_Click(object sender, RoutedEventArgs e)
		{
			var fb = new System.Windows.Forms.FolderBrowserDialog();
			fb.Description = "Select Output Path";
			fb.ShowNewFolderButton = true;
			
			if (System.Windows.Forms.DialogResult.OK == fb.ShowDialog())
			{
				Model.OutputPath = fb.SelectedPath;
			}
		}
	}
}
