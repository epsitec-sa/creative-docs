namespace Epsitec.Common.Widgets.Validators
{
	/// <summary>
	/// La classe RegexValidator permet de valider un widget selon une expression
	/// régulière.
	/// </summary>
	public class RegexValidator : AbstractTextValidator
	{
		public RegexValidator(Widget widget, string regex) : base (widget)
		{
			this.SetRegex (regex);
		}
		
		
		public void SetRegex(string regex)
		{
			System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.Compiled;
			
			this.regex = new System.Text.RegularExpressions.Regex (regex, options);
		}
		
		
		protected override void ValidateText(string text)
		{
			if (this.regex.IsMatch (text))
			{
				this.state = Support.ValidationState.Ok;
			}
			else
			{
				this.state = Support.ValidationState.Error;
			}
		}
		
		
		protected System.Text.RegularExpressions.Regex	regex;
	}
}
