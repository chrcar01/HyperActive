using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace HyperActive
{
	public static class MyConnectionStrings
	{
		public static string Pantera
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["pantera"].ConnectionString;
			}
		}
		public static string WidgetsRUs
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["WidgetsRUs"].ConnectionString;
			}
		}
		public static string EasementsDev
		{
        	get
            {
				return ConfigurationManager.ConnectionStrings["EasementsDev"].ConnectionString;
            }
        }
		public static string WebsiteBuilder
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["WebsiteBuilder"].ConnectionString;
			}
		}
		public static string Scratch
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["Scratch"].ConnectionString;
			}
		}
		public static string LeroyLsgp
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["LeroyLsgp"].ConnectionString;
			}
		}
		public static string FundMgrChris
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["FundMgrChris"].ConnectionString;
			}
		}
	}
}
