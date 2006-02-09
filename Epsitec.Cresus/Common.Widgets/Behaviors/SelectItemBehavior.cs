//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Behaviors
{
	public delegate void SelectItemCallback(string search, bool continued);
	
	/// <summary>
	/// La classe SelectItemBehavior gère la sélection automatique d'éléments
	/// dans une liste lors de frappe de texte (par ex. "A" --> sélectionne le
	/// premier élément commençant par "A", puis "b" --> sélectionne le premier
	/// élément commençant par "Ab", etc.).
	/// </summary>
	public class SelectItemBehavior
	{
		public SelectItemBehavior(SelectItemCallback callback)
		{
			this.callback = callback;
			this.text     = "";
		}
		
		
		public bool ProcessKeyPress(Message message)
		{
			if (message.KeyChar >= 32)
			{
				char key = (char) message.KeyChar;
				long now = Types.Time.Now.Ticks;
				
				long delta = now - this.last_event_ticks;
				
				if ((delta > Types.Time.TicksPerSecond * this.timeout_ms / 1000) ||
					(this.text.Length == 0))
				{
					this.ResetSearch (key.ToString ());
				}
				else
				{
					this.ExpandSearch (key.ToString ());
				}
				
				this.last_event_ticks = now;
				
				return true;
			}
			else
			{
				this.ClearSearch ();
				
				return false;
			}
		}
		
		
		public void ClearSearch()
		{
			this.text = "";
		}
		
		public void ResetSearch(string value)
		{
			this.text = value;
			this.Search (false);
		}
		
		public void ExpandSearch(string value)
		{
			this.text = string.Concat (this.text, value);
			this.Search (true);
		}
		
		
		public void Search(bool continued)
		{
			if (this.callback != null)
			{
				this.callback (this.text, continued);
			}
		}
		
		
		protected int							timeout_ms = 500;
		
		protected SelectItemCallback			callback;
		protected string						text;
		protected long							last_event_ticks;
	}
}
