using ConfigEditorUI.Controls;
using ConfigEditorUI.Models;
using ConfigEditorUI.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace ConfigEditorUI.Windows
{
	public partial class AppStartWindow : Window
	{
		public AppStartWindow()
		{
			InitializeComponent();
			Loaded += AppStartWindow_Loaded;
		}

		void AppStartWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if (DataContext == null)
			{
				var projectWin = new ProjectWindow();
				projectWin.Owner = this;
				projectWin.ShowDialog();
				if (projectWin.Model != null)
				{
					var service = new ConfigService();
					EditConfigsViewModel model = service.EditConfigs(projectWin.Model);
					DataContext = model;
					ConfigsListBox.SelectedIndex = 0;
				}
			}
		}
	
		private void FileOpenConfigMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new System.Windows.Forms.OpenFileDialog();
			dlg.CheckFileExists = true;
			dlg.DefaultExt = ".config";
			dlg.Filter = "HyperActive Confg Files | *.config";
			dlg.Title = "Select HyperActive Configuration File";

			if (System.Windows.Forms.DialogResult.OK != dlg.ShowDialog()) return;
			var service = new ConfigService();
			string configFilePath = dlg.FileName;
			var model = service.EditConfigs(configFilePath);
			DataContext = model;
			//EditConfig(model.Configs[0]);
		}

		private void FileExitMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// TODO: Add a check to see if anything needs saving
			Close();
		}

		private void FileCreateConfigMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var service = new ConfigService();
			var model = service.CreateConfig();
			DataContext = model;
			//EditConfig(model.Configs[0]);
		}

		private void FileSaveConfigMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var service = new ConfigService();
			EditConfigsViewModel model = DataContext as EditConfigsViewModel;
			if (String.IsNullOrEmpty(model.ConfigFilePath))
			{
				var dlg = new System.Windows.Forms.SaveFileDialog();
				dlg.FileName = "hyperactive.config";
				dlg.CheckFileExists = false;
				dlg.Title = "Select HyperActive Configuration File";
				dlg.DefaultExt = ".config";
				dlg.Filter = "Config Files | *.config";
				if (System.Windows.Forms.DialogResult.OK == dlg.ShowDialog())
				{
					model.ConfigFilePath = dlg.FileName;
				}
			}
			service.Save(model);
			Status("Saved " + model.ConfigFilePath + " at " + DateTime.Now.ToString("MMM d, yyyy hh:mm:ss"));
		}
		private void Status(string message)
		{
			StatusLabel.Content = message;
		}
		

		private void FileSaveAsConfigMenuitem_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new System.Windows.Forms.OpenFileDialog();
			dlg.Filter = "Config Files(*.config) | *.config";
			dlg.FileName = "hyperactive.config";
			dlg.DefaultExt = "*.config";
			dlg.CheckFileExists = false;
			dlg.Title = "Select HyperActive Config File";
			var model = DataContext as EditConfigsViewModel;
			if (System.Windows.Forms.DialogResult.OK == dlg.ShowDialog())
			{
				model.ConfigFilePath = dlg.FileName;
			}
			var service = new ConfigService();
			service.Save(model);
			Status("Saved " + model.ConfigFilePath + " at " + DateTime.Now.ToString("MMM d, yyyy hh:mm:ss"));
		}

		private void AddButton_Click(object sender, RoutedEventArgs e)
		{
			var model = DataContext as EditConfigsViewModel;
			var newConfig = new ConfigurationOptionsViewModel();
			model.Configs.Add(newConfig);
			
		}

	
	}
}
