using System;
using System.Windows;
using ConfigEditorUI.Services;
using ConfigEditorUI.Controls;
using HyperActive.Core.Config;
using ConfigEditorUI.Models;
using System.Diagnostics;
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
				}
			}
		}

		private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			EditConfig(ConfigsListBox.SelectedItem as ConfigurationOptionsViewModel);
		}

		private void EditConfig(ConfigurationOptionsViewModel config)
		{
			ControlContainer.Children.Clear();
			var ctl = new EditConfigControl(config);
			ctl.Height = ControlContainer.Height;
			ctl.Width = ControlContainer.Width;
			ControlContainer.Children.Add(ctl);
		}
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			
			
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
			DataContext = service.EditConfigs(configFilePath);
			ControlContainer.Children.Clear();
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
			EditConfig(model.Configs[0]);
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
				dlg.Filter = "HyperActive Config Files | *.config";
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
		private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			bool anyControlKeyPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
			if (anyControlKeyPressed && Keyboard.IsKeyDown(Key.S))
				FileSaveConfigMenuItem_Click(this, e);

			if (anyControlKeyPressed && Keyboard.IsKeyDown(Key.O))
				FileOpenConfigMenuItem_Click(this, e);

			if (anyControlKeyPressed && Keyboard.IsKeyDown(Key.N))
				FileCreateConfigMenuItem_Click(this, e);
			e.Handled = true;
		}

	
	}
}
