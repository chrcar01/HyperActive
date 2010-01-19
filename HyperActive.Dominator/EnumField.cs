using System;
using System.CodeDom;
using System.Collections.Generic;

namespace HyperActive.Dominator
{
	/// <summary>
	/// A field used in an enum.
	/// </summary>
	public class EnumField : ICodeDom<CodeTypeMember>
	{
		private string _name;
		/// <summary>
		/// Gets or sets the name of the enum field.
		/// </summary>
		/// <value>The name of the enum field.</value>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		private int? _value;
		/// <summary>
		/// Gets or sets the value to set for the enum field, if left null,
		/// the value is will not be initialized.
		/// </summary>
		/// <value>The value of the field.</value>
		public int? Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumField"/> class.
		/// </summary>
		public EnumField()
			: this(String.Empty)
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumField"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public EnumField(string name)
			: this(name, null)
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumField"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public EnumField(string name, int? value)
		{
			_name = name;
			_value = value;
		}
		/// <summary>
		/// When called on implementing class, a CodeObject representing the class is returned.
		/// </summary>
		/// <returns></returns>
		public CodeTypeMember ToCodeDom()
		{
			CodeMemberField result = new CodeMemberField(typeof(int), this.Name);
			if (this.Value.HasValue)
			{
				result.InitExpression = new CodeSnippetExpression(this.Value.Value.ToString());
			}
			if (_comments != null && _comments.Count > 0)
			{
				result.Comments.Add(new CodeCommentStatement("<summary>", true));
				foreach (string comment in _comments)
				{
					result.Comments.Add(new CodeCommentStatement(comment, true));
				}
				result.Comments.Add(new CodeCommentStatement("</summary>", true));
			}
			return result;
		}
		
		private List<string> _comments;
		/// <summary>
		/// Adds the comment.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public EnumField AddComment(string format, params object[] args)
		{
			if (_comments == null) _comments = new List<string>();
			_comments.Add(String.Format(format, args));
			return this;
		}
	}
}
