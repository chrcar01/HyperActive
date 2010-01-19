using System;
using System.Xml.XPath;

namespace HyperActive.ConsoleUI
{
	/// <summary>
	/// Extensions to the XPathNavigator.  These are probably overkill, but whatever.
	/// </summary>
	public static class XPathNavigatorExtensions
	{

		/// <summary>
		/// Selects the value if the node was found, returns the defaultValueIfMissing 
		/// if the node is not found. This returns an empty string if the node is not found.
		/// </summary>
		/// <param name="navigator">The navigator instance to query.</param>
		/// <param name="key">The key of the add node we're looking through.</param>
		/// <returns>Value of the node if it's found, otherwise returns defaultValueIfMissing</returns>
		public static string SelectValue(this XPathNavigator navigator, string key)
		{
			string defaultValueIfMissing = "";
			return SelectValue(navigator, key, defaultValueIfMissing);
		}

        /// <summary>
		/// Selects the value if the node was found, returns the defaultValueIfMissing if the node is not found.
		/// </summary>
		/// <param name="navigator">The navigator instance to query.</param>
		/// <param name="key">The key of the add node we're looking through.</param>
		/// <param name="defaultValueIfMissing">The default value if missing.</param>
		/// <returns>Value of the node if it's found, otherwise returns defaultValueIfMissing</returns>
		public static string SelectValue(this XPathNavigator navigator, string key, string defaultValueIfMissing)
		{
			XPathNavigator node = navigator.SelectSingleNode(String.Format("add[@key='{0}']/@value", key));
			if (node == null)
				return defaultValueIfMissing;
			else
				return node.Value;
		}
	}
}
