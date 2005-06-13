//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextProcessor encapsule les op�rations qui s'appliquent � un
	/// texte (et rend l'ex�cution en t�che de fond possible).
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
			//	Ex�cute une m�thode pour tout le texte, en proc�dant par tranches
			//	(ex�cution it�rative).
			
			int pos = 0;
			
			Cursors.TempCursor cursor = new Cursors.TempCursor ();
			
			this.story.NewCursor (cursor);
			
			try
			{
				int step = chunk_size;
				
				for (;;)
				{
					//	TODO: lock et d�tection d'alt�rations du texte (et de la liste des
					//	ITextFrame li�s � ce TextFitter).
					
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
						//	La m�thode de traitement n'a rien pu faire avec le texte pass�
						//	en entr�e, vraisemblablement parce que le paragraphe mesure plus
						//	de 'step' caract�res. Il faut donc augmenter cette limite avant
						//	de tenter un nouvel essai :
						
						step += chunk_size_increment;
					}
					else
					{
						//	Avance le curseur du nombre de caract�res consomm�s par la
						//	m�thode de traitement et reprend une taille par d�faut :
						
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
