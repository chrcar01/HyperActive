using System;

namespace HyperActive.Core.Config
{
	public class ConstructorParameterDescriptor
	{
		private string _name;
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
			}
		}
		private string _type;
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public string Type
		{
			get { return _type; }
			set
			{
				_type = value;
			}
		}
		private string _value;
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public string Value
		{
			get { return _value; }
			set
			{
				_value = value;
			}
		}

	}
}
