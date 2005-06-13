//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		
		public void Process(Executor method)
		{
			this.Process (method, 100*1000, 10*1000);
		}
		
		public void Process(Executor method, int chunk_size)
		{
			this.Process (method, chunk_size, System.Math.Max (1000, chunk_size / 10));
		}
		
		public void Process(Executor method, int chunk_size, int chunk_size_increment)
		{
			//	Exécute une méthode pour tout le texte, en procédant par tranches
			//	(exécution itérative).
			
			int pos = 0;
			
			Cursors.TempCursor cursor = new Cursors.TempCursor ();
			
			this.story.NewCursor (cursor);
			
			try
			{
				int step = chunk_size;
				
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
						
						step += chunk_size_increment;
					}
					else
					{
						//	Avance le curseur du nombre de caractères consommés par la
						//	méthode de traitement et reprend une taille par défaut :
						
						this.story.MoveCursor (cursor, length);
						
						pos += length;
						step = chunk_size;
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
	}
}
