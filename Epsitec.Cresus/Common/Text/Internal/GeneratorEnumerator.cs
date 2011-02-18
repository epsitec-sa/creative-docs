//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe GeneratorEnumerator permet de trouver rapidement tous les
	/// textes correspondant à un générateur donné.
	/// </summary>
	public class GeneratorEnumerator : System.IDisposable, System.Collections.IEnumerator, System.Collections.IEnumerable
	{
		public GeneratorEnumerator(TextStory story, Properties.ManagedParagraphProperty property, string generator)
		{
			this.story      = story;
			this.property   = property;
			this.generator  = generator;
			this.styleList = this.story.StyleList;
			
			this.failureCache = new System.Collections.Hashtable ();
			this.cursor        = new Cursors.TempCursor ();
			
			this.story.NewCursor (this.cursor);
			
			this.atStartOfText = true;
			this.atEndOfText   = false;
			
			this.state = State.None;
		}
		
		public GeneratorEnumerator(TextStory story, Properties.GeneratorProperty generator) : this (story, null, generator.Generator)
		{
		}
		
		
		public Cursors.TempCursor				Cursor
		{
			get
			{
				return this.cursor;
			}
		}
		
		public bool								RestartGenerator
		{
			get
			{
				return this.state == State.Restarted;
			}
		}
		
		
		public bool MoveNext()
		{
			//	Recherche l'occurrence suivante dans le texte; il faut appeler
			//	cette méthode au moins une fois avant de pouvoir lire la position
			//	du curseur.
			
			if (this.atEndOfText)
			{
				return false;
			}
			
			int step   = 10000;
			int length = 0;
			
			for (;;)
			{
				ulong code;
				ulong next;

				int pos = this.story.GetCursorPosition (this.cursor);

				length = System.Math.Min (step, this.story.TextLength - pos);
				length = this.story.TextTable.GetRunLength (this.cursor.CursorId, length, out code, out next);
				
				if (length == 0)
				{
					this.atEndOfText = true;
					return false;
				}
				
				if (this.atStartOfText)
				{
					//	Au début du texte, on ne va pas avancer, mais simplement
					//	analyser le code lié à la première tranche :
					
					this.atStartOfText = false;
				}
				else
				{
					//	Dans le texte, on commence par sauter l'ancienne tranche
					//	déjà analysée et on va s'intéresser à la suite :
					
					this.story.MoveCursor (this.cursor, length);
					code = next;
				}
				
				if (this.failureCache.Contains (code))
				{
					if (this.property != null)
					{
						this.Process (this.failureCache[code] as Properties.ManagedParagraphProperty);
					}
					
					continue;
				}
				
				//	Le code n'a pas encore été classé dans la catégorie de ceux
				//	qui ne correspondent pas à notre générateur, c'est donc un
				//	candidat sérieux :
				
				Properties.GeneratorProperty generator = this.GetGeneratorProperty (code);
				
				if (generator != null)
				{
					if ((pos > 0) &&
						(generator.UniqueId == this.generatorUniqueId))
					{
						continue;
					}

					this.generatorUniqueId = generator.UniqueId;
					
					switch (this.state)
					{
						case State.RestartPending:	this.state = State.Restarted;	break;
						case State.Restarted:		this.state = State.Continue;	break;
						default:					this.state = State.Continue;	break;
					}
					
					return true;
				}
				
				if (this.property != null)
				{
					Properties.ManagedParagraphProperty managed = this.GetManagedParagraphProperty (code);
					
					this.Process (managed);
					this.failureCache[code] = managed;
				}
				else
				{
					this.failureCache[code] = null;
				}
			}
		}
		
		private void Process(Properties.ManagedParagraphProperty property)
		{
			if ((this.state == State.Continue) ||
				(this.state == State.Restarted))
			{
				if (property == null)
				{
					this.state = State.RestartPending;
				}
				else
				{
					//	Evolue-t-on actuellement dans un paragraphe géré par le
					//	même paragraph manager que celui qui nous intéresse ?
						
					if (Property.CompareEqualContents (this.property, property))
					{
						//	OK.
					}
					else
					{
						this.state = State.RestartPending;
					}
				}
			}
		}
		
		
		public Properties.ManagedParagraphProperty GetManagedParagraphProperty(ulong code)
		{
			Styles.CoreSettings core = this.styleList[code];
			
			return core[Properties.WellKnownType.ManagedParagraph] as Properties.ManagedParagraphProperty;
		}
		
		public Properties.GeneratorProperty GetGeneratorProperty(ulong code)
		{
			Styles.CoreSettings  core  = this.styleList[code];
			Styles.ExtraSettings extra = core.GetExtraSettings (code);
				
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
			
			this.state = State.None;
			
			this.atStartOfText = true;
			this.atEndOfText   = false;
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
			
			using (GeneratorEnumerator ge = new GeneratorEnumerator (story, null, generator))
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
				this.failureCache = null;
			}
		}
		
		private enum State
		{
			None,
			Continue,							//	continue la séquence normalement
			RestartPending,						//	il faudra reprendre au début
			Restarted							//	on vient de reprendre au début
		}
		
		private TextStory						story;
		private StyleList						styleList;
		Properties.ManagedParagraphProperty		property;
		private string							generator;
		private System.Collections.Hashtable	failureCache;
		private Cursors.TempCursor				cursor;
		private long							generatorUniqueId;
		private bool							atStartOfText;
		private bool							atEndOfText;
		private State							state;
	}
}
