using System;

namespace HyperActive.SchemaProber
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TFirst">The type of the first.</typeparam>
	public class Tuple<TFirst>
	{
		private TFirst _first = default(TFirst);
		public TFirst First
		{
			get
			{
				return _first;
			}
			set
			{
				_first = value;
			}
		}
		public Tuple()
		{
		}
		public Tuple(TFirst first)
		{
			_first = first;
		}
	}
}
