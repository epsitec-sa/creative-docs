namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe RadioButton réalise un bouton radio.
	/// </summary>
	public class RadioButton : AbstractButton
	{
		public RadioButton()
		{
		}
		
		public string					Group
		{
			get { return this.group; }
			set { this.group = value; }
		}
		
		
		protected override void OnActiveStateChanged(System.EventArgs e)
		{
			base.OnActiveStateChanged (e);
			
			if (this.ActiveState != WidgetState.ActiveNo)
			{
				RadioButton.TurnOffRadio (this.Parent, this.Group, this);
			}
		}

		
		static public void TurnOffRadio(Widget parent, string group, RadioButton keep)
		{
			//	TODO: passe en revue tous les widgets, prend en compte ceux
			//	qui sont de type 'RadioButton' (widget is RadioButton) et
			//	dont le groupe correspond.
			//
			//	Ne touche pas à l'état du bouton 'keep'.
			//
			//	Utiliser FindRadioNotOff...
		}
		
		static public RadioButton[] FindRadioNotOff(Widget parent, string group)
		{
			//	TODO: trouve tous les boutons radio dont l'état n'est pas
			//	WidgetState.ActiveNo...
			
			return new RadioButton[0];
		}
		
		
		protected string				group;
	}
}
