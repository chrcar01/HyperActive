using System;
using System.Linq;
using System.Windows;
using winforms = System.Windows.Forms;
using HyperActive.Core.Config;
using System.Windows.Controls;
using System.Collections.Generic;
using System.ComponentModel;

namespace HyperActive.ConfigEditor
{
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		private void FindConfigFileButton_Click(object sender, RoutedEventArgs e)
		{
			//using (var dlg = new winforms.OpenFileDialog())
			//{
			//	dlg.Title = "Select HyperActive Config File";
			//	dlg.Filter = "HyperActive Config (*.config) | *.config";
			//	if (dlg.ShowDialog() == winforms.DialogResult.OK)
			//	{
			//		ConfigFileTextBox.Text = dlg.FileName;
			//		var configParser = new DefaultConfigParser();
			//		var configs = configParser.ParseFile(dlg.FileName);
			//		
			//		for (int i = 0;i<configs.Count();i++)
			//		{
			//			var config = configs.ElementAt(i);
			//			ListBoxItem item = new ListBoxItem();
			//			item.Content = "Config" + i;
			//			item.DataContext = config;
			//			ConfigsListBox.Items.Add(item);
			//		}
			//	}
			//}
		}

		private void FindConfigFileButton_Click_1(object sender, RoutedEventArgs e)
		{

		}
		
		
	}
}
