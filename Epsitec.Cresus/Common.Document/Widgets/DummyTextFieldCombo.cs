using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe DummyTextFieldCombo est un TextFieldCombo qui ne d�roule aucun menu.
	/// Il faut utiliser DummyTextFieldCombo.Button.Clicked pour d�rouler qq chose � choix.
	/// </summary>
	public class DummyTextFieldCombo : TextFieldCombo
	{
		public DummyTextFieldCombo()
		{
		}

		public DummyTextFieldCombo(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		protected override void OpenCombo()
		{
			this.OnComboOpenPressed ();
		}

		protected virtual void OnComboOpenPressed()
		{
			if (this.ComboOpenPressed != null)
			{
				this.ComboOpenPressed (this);
			}
		}
		
		public event Support.EventHandler		ComboOpenPressed;
	}
}
