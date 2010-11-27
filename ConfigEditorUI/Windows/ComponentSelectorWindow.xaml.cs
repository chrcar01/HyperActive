using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using winforms = System.Windows.Forms;
using ConfigEditorUI.Models;

namespace ConfigEditorUI.Windows
{
	/// <summary>
	/// Interaction logic for ComponentSelectorWindow.xaml
	/// </summary>
	public partial class ComponentSelectorWindow : Window
	{
		private string _typeAndAssemblyName;
		public string TypeAndAssemblyName
		{
			get
			{
				return _typeAndAssemblyName;
			}
		}
        
		private Type _componentType;
		public ComponentSelectorWindow(Type componentType)
		{
			InitializeComponent();
			_componentType = componentType;
			DataContext = new ComponentSelectorViewModel();
		}
		private void BrowseForAssemblyButton_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new winforms.OpenFileDialog();
			dlg.Title = "Select Assembly";
			dlg.DefaultExt = "*.dll";
			dlg.Filter = "Assemblies (*.dll) | *.dll";
			if (winforms.DialogResult.Cancel == dlg.ShowDialog())
			{
				return;
			}


			var assembly = Assembly.LoadFile(dlg.FileName);
			AppDomain.CurrentDomain.AssemblyResolve += CreateResolver(new FileInfo(dlg.FileName).Directory.FullName);

			var query = from type in assembly.GetExportedTypes()
						select type;
			var model = new ComponentSelectorViewModel();
			model.Types.AddRange<Type>(query);
			model.ContainsTypes = query.Count() > 0;
			DataContext = model;
		}
		private ResolveEventHandler CreateResolver(string initialDirectory)
		{
			return (s, e) =>
			{
				var places = new List<string> { initialDirectory, @"C:\Windows\Microsoft.NET\Framework\v2.0.50727" };
				var assemblyName = new AssemblyName(e.Name);
				Assembly result = null;
				foreach (var place in places)
				{
					string filePath = Path.Combine(place, assemblyName.Name + ".dll");
					if (!File.Exists(filePath)) continue;
					result = Assembly.LoadFile(filePath);
				}
				return result;
			};
		}

		private void TypesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			(DataContext as ComponentSelectorViewModel).IsTypeSelected = TypesListBox.SelectedItem != null;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			var type = TypesListBox.SelectedItem as Type;
			_typeAndAssemblyName = String.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
