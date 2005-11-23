//	Copyright � 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Validators
{
	using BundleAttribute = Support.BundleAttribute;
	
	/// <summary>
	/// La classe RegexValidator permet de valider un widget selon une expression
	/// r�guli�re.
	/// </summary>
	public class RegexValidator : AbstractTextValidator
	{
		public RegexValidator() : this (null, "", true)
		{
		}
		
		public RegexValidator(Widget widget, string regex) : this (widget, regex, true)
		{
		}
		
		public RegexValidator(Widget widget, string regex, bool accept_empty) : base (widget)
		{
			this.SetRegex (regex);
			this.accept_empty = accept_empty;
		}
		
		public RegexValidator(Widget widget, System.Text.RegularExpressions.Regex regex) : this (widget, regex, true)
		{
		}
		
		public RegexValidator(Widget widget, System.Text.RegularExpressions.Regex regex, bool accept_empty) : base (widget)
		{
			this.SetRegex (regex);
			this.accept_empty = accept_empty;
		}
		
		
		[Bundle] public bool							AcceptEmptyText
		{
			get
			{
				return this.accept_empty;
			}
			set
			{
				this.accept_empty = value;
			}
		}
		
		[Bundle] public string							Regex
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
			if (((this.accept_empty) && (text.Length == 0)) ||
				((text.Length > 0)) && (this.regex.IsMatch (text)))
			{
				this.state = ValidationState.Ok;
			}
			else
			{
				this.state = ValidationState.Error;
			}
		}
		
		
		protected System.Text.RegularExpressions.Regex	regex;
		protected bool									accept_empty;
	}
}
