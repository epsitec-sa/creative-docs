//	Copyright © 2004-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Validators
{
	/// <summary>
	/// La classe RegexValidator permet de valider un widget selon une expression
	/// régulière.
	/// </summary>
	public class RegexValidator : AbstractTextValidator
	{
		public RegexValidator() : this (null, "", true)
		{
		}
		
		public RegexValidator(Widget widget, string regex) : this (widget, regex, true)
		{
		}
		
		public RegexValidator(Widget widget, string regex, bool acceptEmpty) : base (widget)
		{
			this.SetRegex (regex);
			this.acceptEmpty = acceptEmpty;
		}
		
		public RegexValidator(Widget widget, System.Text.RegularExpressions.Regex regex) : this (widget, regex, true)
		{
		}
		
		public RegexValidator(Widget widget, System.Text.RegularExpressions.Regex regex, bool acceptEmpty) : base (widget)
		{
			this.SetRegex (regex);
			this.acceptEmpty = acceptEmpty;
		}
		
		
		public bool										AcceptEmptyText
		{
			get
			{
				return this.acceptEmpty;
			}
			set
			{
				this.acceptEmpty = value;
			}
		}
		
		public string									Regex
		{
			get
			{
				return this.regex.ToString ();
			}
			set
			{
				if (this.Regex != value)
				{
					this.SetRegex (value);
				}
			}
		}
		
		
		public void SetRegex(string regex)
		{
			System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.Compiled;
			
			this.regex = new System.Text.RegularExpressions.Regex (regex, options);
		}
		
		public void SetRegex(System.Text.RegularExpressions.Regex regex)
		{
			this.regex = regex;
		}
		
		
		protected override void ValidateText(string text)
		{
			if (((this.acceptEmpty) && (text.Length == 0)) ||
				((text.Length > 0)) && (this.regex.IsMatch (text)))
			{
				this.SetState (ValidationState.Ok);
			}
			else
			{
				this.SetState (ValidationState.Error);
			}
		}
		
		
		protected System.Text.RegularExpressions.Regex	regex;
		protected bool									acceptEmpty;
	}
}
