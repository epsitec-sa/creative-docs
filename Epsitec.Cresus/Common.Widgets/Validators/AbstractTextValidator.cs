namespace Epsitec.Common.Widgets.Validators
{
	using BundleAttribute = Support.BundleAttribute;
	using SuppressBundleSupportAttribute = Support.SuppressBundleSupportAttribute;
	
	/// <summary>
	/// La classe AbstractTextValidator permet de simplifier l'implémentation de
	/// l'interface IValidator dans les cas où le texte sert de source pour la
	/// validation.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public abstract class AbstractTextValidator : AbstractValidator
	{
		public AbstractTextValidator(Widget widget) : base (widget)
		{
		}
		
		
		public override void Validate()
		{
			string text = this.widget.Text;
			
			this.ValidateText (text);
		}
		
		
		protected abstract void ValidateText(string text);
		
		protected override void AttachWidget(Widget widget)
		{
			base.AttachWidget (widget);
			
			widget.TextChanged += new Epsitec.Common.Support.EventHandler (this.HandleWidgetTextChanged);
		}
		
		protected override void DetachWidget(Widget widget)
		{
			base.DetachWidget (widget);
			
			widget.TextChanged -= new Epsitec.Common.Support.EventHandler (this.HandleWidgetTextChanged);
		}
		
		
		private void HandleWidgetTextChanged(object sender)
		{
			this.MakeDirty (false);
		}
	}
}
