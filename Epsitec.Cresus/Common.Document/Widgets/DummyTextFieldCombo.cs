using Epsitec.Common.Support;
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
			EventHandler handler = (EventHandler) this.GetUserEventHandler("ComboOpenPressed");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		public event EventHandler			ComboOpenPressed
		{
			add
			{
				this.AddUserEventHandler("ComboOpenPressed", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ComboOpenPressed", value);
			}
		}
	}
}
