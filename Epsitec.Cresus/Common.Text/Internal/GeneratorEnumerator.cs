//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe GeneratorEnumerator permet de trouver rapidement tous les
	/// textes correspondant à un générateur donné.
	/// </summary>
	public class GeneratorEnumerator : System.IDisposable, System.Collections.IEnumerator, System.Collections.IEnumerable
	{
		public GeneratorEnumerator(TextStory story, string generator)
		{
			this.story      = story;
			this.generator  = generator;
			this.style_list = this.story.StyleList;
			
			this.failure_cache = new System.Collections.Hashtable ();
			this.cursor        = new Cursors.TempCursor ();
			
			this.story.NewCursor (this.cursor);
			
			this.at_start_of_text = true;
			this.at_end_of_text   = false;
		}
		
		public GeneratorEnumerator(TextStory story, Properties.GeneratorProperty generator) : this (story, generator.Generator)
		{
		}
		
		
		public Cursors.TempCursor				Cursor
		{
			get
			{
				return this.cursor;
			}
		}
		
		
		public bool MoveNext()
		{
			//	Recherche l'occurrence suivante dans le texte; il faut appeler
			//	cette méthode au moins une fois avant de pouvoir lire la position
			//	du curseur.
			
			if (this.at_end_of_text)
			{
				return false;
			}
			
			int step   = 10000;
			int length = 0;
			
			for (;;)
			{
				ulong code;
				ulong next;
				
				length = System.Math.Min (step, this.story.TextLength - this.story.GetCursorPosition (cursor));
				length = this.story.TextTable.GetRunLength (this.cursor.CursorId, length, out code, out next);
				
				if (length == 0)
				{
					this.at_end_of_text = true;
					return false;
				}
				
				if (this.at_start_of_text)
				{
					//	Au début du texte, on ne va pas avancer, mais simplement
					//	analyser le code lié à la première tranche :
					
					this.at_start_of_text = false;
				}
				else
				{
					//	Dans le texte, on commence par sauter l'ancienne tranche
					//	déjà analysée et on va s'intéresser à la suite :
					
					this.story.MoveCursor (this.cursor, length);
					code = next;
				}
				
				if (this.failure_cache.Contains (code))
				{
					continue;
				}
				
				Properties.GeneratorProperty generator = this.GetGeneratorProperty (code);
				
				if (generator != null)
				{
					return true;
				}
				
				this.failure_cache[code] = this.generator;
			}
		}
		
		
		public Properties.GeneratorProperty GetGeneratorProperty(ulong code)
		{
			Styles.SimpleStyle   style = this.style_list[code];
			Styles.ExtraSettings extra = style.GetExtraSettings (code);
				
			if (extra != null)
			{
				Property[] properties = extra.FindProperties (Properties.WellKnownType.Generator);
					
				foreach (Properties.GeneratorProperty generator in properties)
				{
					//	On vient de trouver un générateur qui correspond à ce
					//	que nous rechercons :
						
					if (generator.Generator == this.generator)
					{
						return generator;
					}
				}
			}
			
			return null;
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
		}
		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this;
		}
		#endregion
		
		#region IEnumerator Members
		object									System.Collections.IEnumerator.Current
		{
			get
			{
				return this.Cursor;
			}
		}
		
		void System.Collections.IEnumerator.Reset()
		{
			this.story.SetCursorPosition (this.cursor, 0);
			
			this.at_start_of_text = true;
			this.at_end_of_text   = false;
		}
		
		bool System.Collections.IEnumerator.MoveNext()
		{
			return this.MoveNext ();
		}
		#endregion
		
		public static Cursors.GeneratorCursor[] CreateCursors(TextStory story, string generator)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			Internal.TextTable           text = story.TextTable;
			
			using (GeneratorEnumerator ge = new GeneratorEnumerator (story, generator))
			{
				foreach (ICursor model in ge)
				{
					Cursors.GeneratorCursor mark = new Cursors.GeneratorCursor ();
					
					text.NewCursor (mark, model.CursorId);
					list.Add (mark);
				}
			}
			
			Cursors.GeneratorCursor[] cursors = new Cursors.GeneratorCursor[list.Count];
			list.CopyTo (cursors);
			
			return cursors;
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if ((this.story != null) &&
					(this.cursor != null))
				{
					this.story.RecycleCursor (this.cursor);
				}
				
				this.story         = null;
				this.cursor        = null;
				this.failure_cache = null;
			}
		}
		
		
		private TextStory						story;
		private StyleList						style_list;
		private string							generator;
		private System.Collections.Hashtable	failure_cache;
		private Cursors.TempCursor				cursor;
		private bool							at_start_of_text;
		private bool							at_end_of_text;
	}
}
