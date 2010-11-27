using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ConfigEditorUI.Models
{
	public static class ObservableCollectionExtensions
	{
		public static void AddRange<T>(this ObservableCollection<T> @this, IEnumerable<T> items)
		{
			if (items == null || items.Count() == 0) return;
			foreach (var item in items)
			{
				@this.Add(item);
			}
		}
	}
}
