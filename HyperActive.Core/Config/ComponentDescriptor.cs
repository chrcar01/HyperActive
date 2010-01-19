using System;
using System.Collections.Generic;

namespace HyperActive.Core.Config
{
	public class ComponentDescriptor
	{
		private string _serviceTypeName;
		public string ServiceTypeName
		{
			get { return _serviceTypeName; }
			set
			{
				_serviceTypeName = value;
			}
		}
		private string _serviceImplementationTypeName;
		public string ServiceImplementationTypeName
		{
			get { return _serviceImplementationTypeName; }
			set
			{
				_serviceImplementationTypeName = value;
			}
		}
		private List<ConstructorParameterDescriptor> _parameters;
		public List<ConstructorParameterDescriptor> Parameters
		{
			get
			{
				if (_parameters == null)
				{
					_parameters = new List<ConstructorParameterDescriptor>();
				}
				return _parameters;
			}
		}


	}
}
