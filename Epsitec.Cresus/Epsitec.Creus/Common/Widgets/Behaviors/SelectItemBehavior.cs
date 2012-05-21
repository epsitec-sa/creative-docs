//	Copyright © 2006-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Behaviors
{
	/// <summary>
	/// La classe SelectItemBehavior gère la sélection automatique d'éléments
	/// dans une liste lors de frappe de texte (par ex. "A" --> sélectionne le
	/// premier élément commençant par "A", puis "b" --> sélectionne le premier
	/// élément commençant par "Ab", etc.).
	/// </summary>
	public sealed class SelectItemBehavior
	{
		public SelectItemBehavior(System.Action<string, bool> callback)
		{
			this.TimeoutMilliseconds = 500;
			
			this.callback = callback;
			this.text     = "";
		}


		public int TimeoutMilliseconds
		{
			get;
			set;
		}
		
		
		public bool ProcessKeyPress(Message message)
		{
			if (message.KeyChar >= 32)
			{
				char key = (char) message.KeyChar;
				long now = Types.Time.Now.Ticks;
				
				long delta = now - this.lastEventTicks;
				long timeout = Types.Time.TicksPerSecond * this.TimeoutMilliseconds / 1000;
				
				if ((delta > timeout) ||
					(this.text.Length == 0))
				{
					this.ResetSearch (key.ToString ());
				}
				else
				{
					this.ExpandSearch (key.ToString ());
				}
				
				this.lastEventTicks = now;
				
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
		
		private void ResetSearch(string value)
		{
			this.text = value;
			this.Search (false);
		}
		
		private void ExpandSearch(string value)
		{
			this.text = string.Concat (this.text, value);
			this.Search (true);
		}
		
		private void Search(bool continued)
		{
			if (this.callback != null)
			{
				this.callback (this.text, continued);
			}
		}
		
		
		private readonly System.Action<string, bool> callback;
		private string							text;
		private long							lastEventTicks;
	}
}
