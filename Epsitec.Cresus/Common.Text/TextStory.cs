//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextStory représente un texte complet, avec tous ses attributs
	/// typographiques, ses curseurs, sa gestion du undo, etc.
	/// </summary>
	public class TextStory
	{
		public TextStory()
		{
			this.text = new Internal.TextTable ();
			
			this.text_length = 0;
			this.undo_length = 0;
			
			this.temp_cursor_id = this.text.NewCursor ();
			
			this.text.InsertText (this.temp_cursor_id, new ulong[] { 0ul });
			
			this.oplet_queue = new Support.OpletQueue ();
		}
		
		
		public int								TextLength
		{
			get
			{
				return this.text_length;
			}
		}
		
		public int								UndoLength
		{
			get
			{
				return this.undo_length;
			}
		}
		
		public Support.OpletQueue				OpletQueue
		{
			get
			{
				return this.oplet_queue;
			}
		}
		
		
		public int NewCursor()
		{
			return this.text.NewCursor ();
		}
		
		public void MoveCursor(int cursor_id, int distance)
		{
			this.text.MoveCursor (cursor_id, distance);
		}
		
		public int GetCursorPosition(int cursor_id)
		{
			return this.text.GetCursorPosition (cursor_id);
		}
		
		
		public void InsertText(int cursor_id, ulong[] text)
		{
			int position = this.text.GetCursorPosition (cursor_id);
			int length   = text.Length;
			
			this.text.InsertText (cursor_id, text);
			this.text_length += length;
			
			using (this.oplet_queue.BeginAction ())
			{
				this.oplet_queue.Insert (new TextInsertOplet (this, position, length));
				this.oplet_queue.ValidateAction ();
			}
		}
		
		public void DeleteText(int cursor_id, int length)
		{
			int position = this.text.GetCursorPosition (cursor_id);
			
			//	TODO: implémenter
		}
		
		
		public string GetDebugText()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			this.text.SetCursorPosition (this.temp_cursor_id, 0);
			
			for (int i = 0; i < this.text_length; i++)
			{
				ulong code = this.text[this.temp_cursor_id];
				this.text.MoveCursor (this.temp_cursor_id, 1);
				
				buffer.Append ((char) Unicode.Bits.GetCode (code));
			}
			
			return buffer.ToString ();
		}
		
		public string GetDebugUndo()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			this.text.SetCursorPosition (this.temp_cursor_id, this.text_length + 1);
			
			for (int i = 0; i < this.undo_length; i++)
			{
				ulong code = this.text[this.temp_cursor_id];
				this.text.MoveCursor (this.temp_cursor_id, 1);
				
				buffer.Append ((char) Unicode.Bits.GetCode (code));
			}
			
			return buffer.ToString ();
		}
		
		
		protected void InternalInsertText(int position, ulong[] text)
		{
			this.text.SetCursorPosition (this.temp_cursor_id, position);
			this.text.InsertText (this.temp_cursor_id, text);
		}
		
		protected void InternalDeleteText(int position, int length, out CursorInfo[] infos)
		{
			this.text.SetCursorPosition (this.temp_cursor_id, position);
			this.text.DeleteText (this.temp_cursor_id, length, out infos);
		}
		
		protected void InternalMoveText(int from_pos, int to_pos, int length)
		{
			//	Déplace le texte sans gestion du undo/redo ni mise à jour des
			//	longueurs respectives de 'text area' et 'undo area', ni gestion
			//	des curseurs.
			
			//	L'appelant fournit une position de destination qui est valide
			//	seulement après la suppression (temporaire) du texte.
			
			this.text.SetCursorPosition (this.temp_cursor_id, from_pos);
			
			ulong[] data = new ulong[length];
			int     read = this.text.ReadText (this.temp_cursor_id, length, data, 0);
			
			Debug.Assert.IsTrue (read == length);
			
			CursorInfo[] infos;
			
			this.InternalDeleteText (from_pos, length, out infos);
			this.InternalInsertText (to_pos, data);
		}
		
		protected void InternalSaveCursorPositions(int position, int length, out CursorInfo[] infos)
		{
			infos = this.text.FindCursors (position, length);
		}
		
		protected void InternalRestoreCursorPositions(CursorInfo[] infos)
		{
			for (int i = 0; i < infos.Length; i++)
			{
				this.text.SetCursorPosition (infos[i].CursorId, infos[i].Position);
			}
		}
		
		
		protected abstract class BaseOplet : Support.AbstractOplet
		{
			protected BaseOplet(TextStory story)
			{
				this.story = story;
			}
			
			
			protected readonly TextStory		story;
		}
		
		protected class TextInsertOplet : BaseOplet
		{
			public TextInsertOplet(TextStory story, int position, int length) : base (story)
			{
				this.position = position;
				this.length   = length;
			}
			
			
			public override Support.IOplet Undo()
			{
				this.story.InternalSaveCursorPositions (this.position, this.length, out this.cursors);
				
				Debug.Assert.IsNotNull (this.cursors);
				
				int undo_start = this.story.text_length + 1;
				int undo_end   = undo_start + this.story.undo_length;
				
				this.story.InternalMoveText (this.position, undo_end - this.length, this.length);
				
				this.story.text_length -= this.length;
				this.story.undo_length += this.length;
				
				return this;
			}
			
			public override Support.IOplet Redo()
			{
				int undo_start = this.story.text_length + 1;
				int undo_end   = undo_start + this.story.undo_length;
				
				this.story.InternalMoveText (undo_end - this.length, this.position, this.length);
				
				this.story.text_length += this.length;
				this.story.undo_length -= this.length;
				
				this.story.InternalRestoreCursorPositions (this.cursors);
				
				this.cursors = null;
				
				return this;
			}
			
			
			public override void Dispose()
			{
				//	Lorsque l'on supprime une information permettant d'annuler
				//	une insertion, il n'y a rien à faire. Par contre, si l'oplet
				//	est dans l'état "redoable", il faudra supprimer le texte de
				//	la "undo area".
				
				Debug.Assert.IsTrue (this.length > 0);
				
				if (this.cursors != null)
				{
					int undo_start = this.story.text_length + 1;
					int undo_end   = undo_start + this.story.undo_length;
				
					CursorInfo[] infos;
					this.story.InternalDeleteText (undo_end - this.length, this.length, out infos);
					
					//	TODO: gérer la suppression des curseurs...
					
					this.story.undo_length -= this.length;
					this.cursors = null;
				}
				
				this.length   = 0;
				
				base.Dispose ();
			}
			
			
			
			
			
			private int							position;
			private int							length;
			private CursorInfo[]				cursors;
		}
		
		
		private Internal.TextTable				text;
		private int								text_length;
		private int								undo_length;
		private Internal.CursorId				temp_cursor_id;
		private Support.OpletQueue				oplet_queue;
	}
}
