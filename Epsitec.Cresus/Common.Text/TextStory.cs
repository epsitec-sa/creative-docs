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
			this.SetupTextStory ();
			this.SetupOpletQueue (new Support.OpletQueue ());
			this.SetupStyleList (new StyleList ());
		}
		
		public TextStory(Support.OpletQueue oplet_queue)
		{
			this.SetupTextStory ();
			this.SetupOpletQueue (oplet_queue);
			this.SetupStyleList (new StyleList ());
		}
		
		public TextStory(StyleList style_list)
		{
			this.SetupTextStory ();
			this.SetupOpletQueue (new Support.OpletQueue ());
			this.SetupStyleList (style_list);
		}
		
		public TextStory(Support.OpletQueue oplet_queue, StyleList style_list)
		{
			this.SetupTextStory ();
			this.SetupOpletQueue (oplet_queue);
			this.SetupStyleList (style_list);
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
		
		public StyleList						StyleList
		{
			get
			{
				return this.style_list;
			}
		}
		
		
		internal bool							DebugDisableOpletQueue
		{
			get
			{
				return this.debug_disable_oplet;
			}
			set
			{
				this.debug_disable_oplet = value;
			}
		}
		
		
		public void NewCursor(ICursor cursor)
		{
			this.text.NewCursor (cursor);
			
			this.InternalAddOplet (new CursorMoveOplet (this, cursor, 0));
		}
		
		public void MoveCursor(ICursor cursor, int distance)
		{
			int old_pos = this.GetCursorPosition (cursor);
			
			this.text.MoveCursor (cursor.CursorId, distance);
			
			int new_pos = this.GetCursorPosition (cursor);
			
			if (old_pos != new_pos)
			{
				this.InternalAddOplet (new CursorMoveOplet (this, cursor, old_pos));
			}
		}
		
		
		public int GetCursorPosition(ICursor cursor)
		{
			return this.text.GetCursorPosition (cursor.CursorId);
		}
		
		public void SetCursorPosition(ICursor cursor, int position)
		{
			int old_pos = this.GetCursorPosition (cursor);
			
			this.text.SetCursorPosition (cursor.CursorId, position);
			
			int new_pos = this.GetCursorPosition (cursor);
			
			if (old_pos != new_pos)
			{
				this.InternalAddOplet (new CursorMoveOplet (this, cursor, old_pos));
			}
		}
		
		
		public void InsertText(ICursor cursor, ulong[] text)
		{
			int position = this.text.GetCursorPosition (cursor.CursorId);
			int length   = text.Length;
			
			this.text.InsertText (cursor.CursorId, text);
			this.text_length += length;
			
			this.InternalAddOplet (new TextInsertOplet (this, position, length));
		}
		
		public void DeleteText(ICursor cursor, int length)
		{
			int position = this.text.GetCursorPosition (cursor.CursorId);
			
			CursorInfo[] cursors;
			
			this.InternalSaveCursorPositions (position, length, out cursors);
			
			int undo_start = this.text_length + 1;
			int undo_end   = undo_start + this.undo_length;
				
			this.InternalMoveText (position, undo_end - length, length);
				
			this.text_length -= length;
			this.undo_length += length;
			
			this.InternalAddOplet (new TextDeleteOplet (this, position, length, cursors));
		}
		
		
		public string GetDebugText()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			this.text.SetCursorPosition (this.temp_cursor.CursorId, 0);
			
			for (int i = 0; i < this.text_length; i++)
			{
				ulong code = this.text[this.temp_cursor.CursorId];
				this.text.MoveCursor (this.temp_cursor.CursorId, 1);
				
				buffer.Append ((char) Unicode.Bits.GetCode (code));
			}
			
			return buffer.ToString ();
		}
		
		public string GetDebugUndo()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			this.text.SetCursorPosition (this.temp_cursor.CursorId, this.text_length + 1);
			
			for (int i = 0; i < this.undo_length; i++)
			{
				ulong code = this.text[this.temp_cursor.CursorId];
				this.text.MoveCursor (this.temp_cursor.CursorId, 1);
				
				buffer.Append ((char) Unicode.Bits.GetCode (code));
			}
			
			return buffer.ToString ();
		}
		
		
		private void InternalAddOplet(Support.IOplet oplet)
		{
			if ((this.debug_disable_oplet == false) &&
				(this.oplet_queue != null))
			{
				//	TODO: gérer la fusion d'oplets identiques
				
				using (this.oplet_queue.BeginAction ())
				{
					this.oplet_queue.Insert (oplet);
					this.oplet_queue.ValidateAction ();
				}
			}
		}
		
		
		protected void InternalInsertText(int position, ulong[] text)
		{
			this.text.SetCursorPosition (this.temp_cursor.CursorId, position);
			this.text.InsertText (this.temp_cursor.CursorId, text);
		}
		
		protected void InternalDeleteText(int position, int length, out CursorInfo[] infos)
		{
			this.text.SetCursorPosition (this.temp_cursor.CursorId, position);
			this.text.DeleteText (this.temp_cursor.CursorId, length, out infos);
		}
		
		protected void InternalMoveText(int from_pos, int to_pos, int length)
		{
			//	Déplace le texte sans gestion du undo/redo ni mise à jour des
			//	longueurs respectives de 'text area' et 'undo area', ni gestion
			//	des curseurs.
			
			//	L'appelant fournit une position de destination qui est valide
			//	seulement après la suppression (temporaire) du texte.
			
			this.text.SetCursorPosition (this.temp_cursor.CursorId, from_pos);
			
			ulong[] data = new ulong[length];
			int     read = this.text.ReadText (this.temp_cursor.CursorId, length, data, 0);
			
			Debug.Assert.IsTrue (read == length);
			
			CursorInfo[] infos;
			
			this.InternalDeleteText (from_pos, length, out infos);
			this.InternalInsertText (to_pos, data);
			
			if ((infos != null) &&
				(infos.Length > 0))
			{
				//	La liste des curseurs affectés contient peut-être des curseurs
				//	temporaires; on commence par les filtrer, puis on déplace tous
				//	les curseurs restants à la nouvelle position :
				
				infos = this.text.FilterCursors (infos, new CursorInfo.Filter (this.FilterSaveCursors));
				
				this.InternalRestoreCursorPositions (infos, to_pos - from_pos);
			}
		}
		
		protected void InternalSaveCursorPositions(int position, int length, out CursorInfo[] infos)
		{
			infos = this.text.FindCursors (position, length, new CursorInfo.Filter (this.FilterSaveCursors));
		}
		
		protected void InternalRestoreCursorPositions(CursorInfo[] infos, int offset)
		{
			if ((infos != null) &&
				(infos.Length > 0))
			{
				for (int i = 0; i < infos.Length; i++)
				{
					this.text.SetCursorPosition (infos[i].CursorId, infos[i].Position + offset);
				}
			}
		}
		
		
		protected bool FilterSaveCursors(ICursor cursor, int position)
		{
			return (cursor != null) && (cursor.Attachment != CursorAttachment.Temporary);
		}
		
		
		private void SetupTextStory()
		{
			this.text = new Internal.TextTable ();
			
			this.text_length = 0;
			this.undo_length = 0;
			
			this.temp_cursor = new Cursors.TempCursor ();
			
			this.text.NewCursor (this.temp_cursor);
			this.text.InsertText (this.temp_cursor.CursorId, new ulong[] { 0ul });
		}
		
		private void SetupOpletQueue(Support.OpletQueue oplet_queue)
		{
			this.oplet_queue = oplet_queue;
		}
		
		private void SetupStyleList(StyleList style_list)
		{
			this.style_list = style_list;
		}
		
		
		#region Abstract BaseOplet Class
		protected abstract class BaseOplet : Support.AbstractOplet
		{
			protected BaseOplet(TextStory story)
			{
				this.story = story;
			}
			
			
			protected readonly TextStory		story;
		}
		#endregion
		
		#region TextInsertOplet Class
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
				
				this.story.InternalRestoreCursorPositions (this.cursors, 0);
				
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
					//	TODO: gérer la suppression des styles...
					
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
		#endregion
		
		#region TextDeleteOplet Class
		protected class TextDeleteOplet : BaseOplet
		{
			public TextDeleteOplet(TextStory story, int position, int length, CursorInfo[] cursors) : base (story)
			{
				this.position = position;
				this.length   = length;
				this.cursors  = cursors;
			}
			
			
			public override Support.IOplet Undo()
			{
				int undo_start = this.story.text_length + 1;
				int undo_end   = undo_start + this.story.undo_length;
				
				this.story.InternalMoveText (undo_end - this.length, this.position, this.length);
				
				this.story.text_length += this.length;
				this.story.undo_length -= this.length;
				
				this.story.InternalRestoreCursorPositions (this.cursors, 0);
				
				this.cursors = null;
				
				return this;
			}
			
			public override Support.IOplet Redo()
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
			
			
			public override void Dispose()
			{
				//	Lorsque l'on supprime une information permettant de refaire
				//	une destruction, il n'y a rien à faire. Par contre, si l'oplet
				//	est dans l'état "undoable", il faudra supprimer le texte de
				//	la "undo area".
				
				Debug.Assert.IsTrue (this.length > 0);
				
				if (this.cursors != null)
				{
					int undo_start = this.story.text_length + 1;
					int undo_end   = undo_start + this.story.undo_length;
					
					CursorInfo[] infos;
					this.story.InternalDeleteText (undo_end - this.length, this.length, out infos);
					
					//	TODO: gérer la suppression des curseurs...
					//	TODO: gérer la suppression des styles...
					
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
		#endregion
		
		#region CursorMoveOplet Class
		protected class CursorMoveOplet : BaseOplet
		{
			public CursorMoveOplet(TextStory story, ICursor cursor, int position) : base (story)
			{
				this.cursor   = cursor;
				this.position = position;
			}
			
			
			public override Support.IOplet Undo()
			{
				return this.Swap ();
			}
			
			public override Support.IOplet Redo()
			{
				return this.Swap ();
			}
			
			
			private Support.IOplet Swap()
			{
				int old_pos = this.position;
				int new_pos = this.story.text.GetCursorPosition (this.cursor.CursorId);
				
				this.story.text.SetCursorPosition (this.cursor.CursorId, old_pos);
				
				this.position = new_pos;
				
				return this;
			}

			
			private int							position;
			private ICursor						cursor;
		}
		#endregion
		
		
		private Internal.TextTable				text;
		private int								text_length;		//	texte dans la zone texte
		private int								undo_length;		//	texte dans la zone undo
		private ICursor							temp_cursor;
		
		private Support.OpletQueue				oplet_queue;
		private StyleList						style_list;
		
		private bool							debug_disable_oplet;
	}
}
