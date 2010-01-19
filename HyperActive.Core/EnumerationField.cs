using System;
using System.Collections.Generic;

namespace HyperActive.Core
{
	public class EnumerationField : IEquatable<EnumerationField>
	{
		private int _value;
		public int Value
		{
			get
			{
				return _value;
			}
		}
		private string _description;
		public string Description
		{
			get
			{
				return _description;
			}
		}
		public EnumerationField(int value, string description)
		{
			_value = value;
			_description = description;
		}
		public static implicit operator int(EnumerationField field)
		{
			return field.Value;
		}
		public static implicit operator string(EnumerationField field)
		{
			return field.ToString();
		}
		public override string ToString()
		{
			return this.Description;
		}


		public bool Equals(EnumerationField other)
		{
			if (other == null)
				return false;
			if (!EqualityComparer<int>.Default.Equals(_value, other.Value))
				return false;
			if (!EqualityComparer<string>.Default.Equals(_description, other.Description))
				return false;
			return true;
		}

		public override bool Equals(object obj)
		{
			var field = obj as EnumerationField;
			if (obj != null)
				return Equals(field);
			return false;
		}

		public override int GetHashCode()
		{
			int hash = 0;
			hash ^= EqualityComparer<int>.Default.GetHashCode(_value);
			hash ^= EqualityComparer<string>.Default.GetHashCode(_description);
			return hash;
		}
	}
}
