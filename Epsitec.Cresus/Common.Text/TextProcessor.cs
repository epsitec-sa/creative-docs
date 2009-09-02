//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextProcessor encapsule les opérations qui s'appliquent à un
	/// texte (et rend l'exécution en tâche de fond possible).
	/// </summary>
	public class TextProcessor
	{
		public TextProcessor(TextStory story)
		{
			this.story = story;
		}
		
		
		public void DefineStartFence(int pos)
		{
			this.startPos = pos;
		}
		
		public void DefineEndFence(int pos)
		{
			this.endPos = pos;
		}
		
		
		public void Process(Executor method)
		{
			this.Process (method, 100*1000, 10*1000);
		}
		
		public void Process(Executor method, int chunkSize)
		{
			this.Process (method, chunkSize, System.Math.Max (1000, chunkSize / 10));
		}
		
		public void Process(Executor method, int chunkSize, int chunkSizeIncrement)
		{
			//	Exécute une méthode pour tout le texte, en procédant par tranches
			//	(exécution itérative).
			
			int pos = this.startPos;
			
			Cursors.TempCursor cursor = new Cursors.TempCursor ();
			
			this.story.NewCursor (cursor);
			this.story.SetCursorPosition (cursor, pos);
			
			try
			{
				int step = chunkSize;
				
				for (;;)
				{
					//	TODO: lock et détection d'altérations du texte (et de la liste des
					//	ITextFrame liés à ce TextFitter).
					
					int    length = System.Math.Min (this.story.TextLength - pos, step);
					Status status;
					
					if (length <= 0)
					{
						break;
					}
					if ((this.startPos <= this.endPos) &&
						(pos >= this.endPos))
					{
						break;
					}
					
					method (cursor, pos, ref length, out status);
					
					switch (status)
					{
						case Status.Abort:
							return;
						
						case Status.Continue:
							break;
						
						default:
							throw new System.InvalidOperationException ();
					}
					
					if (length == 0)
					{
						//	La méthode de traitement n'a rien pu faire avec le texte passé
						//	en entrée, vraisemblablement parce que le paragraphe mesure plus
						//	de 'step' caractères. Il faut donc augmenter cette limite avant
						//	de tenter un nouvel essai :
						
						step += chunkSizeIncrement;
					}
					else
					{
						//	Avance le curseur du nombre de caractères consommés par la
						//	méthode de traitement et reprend une taille par défaut :
						
						this.story.MoveCursor (cursor, length);
						
						pos += length;
						step = chunkSize;
					}
				}
			}
			finally
			{
				this.story.RecycleCursor (cursor);
			}
		}
		
		
		public void Process(Iterator method)
		{
			Status status;
			
			for (;;)
			{
				method (out status);
				
				switch (status)
				{
					case Status.Abort:
						return;
					
					case Status.Continue:
						break;
					
					default:
						throw new System.InvalidOperationException ();
				}
			}
		}
		
		
		public delegate void Executor(Cursors.TempCursor cursor, int pos, ref int length, out Status status);
		public delegate void Iterator(out Status status);
		
		public enum Status
		{
			Continue,
			Abort,
		}
		
		private TextStory						story;
		
		private int								startPos	= 0;
		private int								endPos		= -1;
	}
}
