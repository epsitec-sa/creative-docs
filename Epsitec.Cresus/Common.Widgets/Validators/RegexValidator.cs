namespace Epsitec.Common.Widgets.Validators
{
	/// <summary>
	/// La classe RegexValidator permet de valider un widget selon une expression
	/// régulière.
	/// </summary>
	public class RegexValidator : AbstractTextValidator
	{
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
			if (((this.accept_empty) || (text.Length > 0)) &&
				(this.regex.IsMatch (text)))
			{
				this.state = Support.ValidationState.Ok;
			}
			else
			{
				this.state = Support.ValidationState.Error;
			}
		}
		
		
		protected System.Text.RegularExpressions.Regex	regex;
		protected bool									accept_empty;
	}
}
