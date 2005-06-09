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
			this.SetupOpletQueue (new Common.Support.OpletQueue ());
			this.SetupContext (new Context ());
		}
		
		public TextStory(Common.Support.OpletQueue oplet_queue)
		{
			this.SetupTextStory ();
			this.SetupOpletQueue (oplet_queue);
			this.SetupContext (new Context ());
		}
		
		public TextStory(Context context)
		{
			this.SetupTextStory ();
			this.SetupOpletQueue (new Common.Support.OpletQueue ());
			this.SetupContext (context);
		}
		
		public TextStory(Common.Support.OpletQueue oplet_queue, Context context)
		{
			this.SetupTextStory ();
			this.SetupOpletQueue (oplet_queue);
			this.SetupContext (context);
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
		
		public Common.Support.OpletQueue		OpletQueue
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
				return this.context.StyleList;
			}
		}
		
		public Text.Context						TextContext
		{
			get
			{
				return this.context;
			}
		}
		
		
		public int								TextChangeMark
		{
			get
			{
				return this.text_change_mark;
			}
		}
		
		public long								TextChangeVersion
		{
			get
			{
				return this.text_change_version;
			}
		}
		
		
		internal Internal.TextTable				TextTable
		{
			get
			{
				return this.text;
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
			
			if (cursor.Attachment != CursorAttachment.Temporary)
			{
				this.InternalAddOplet (new CursorMoveOplet (this, cursor, 0));
			}
		}
		
		public void MoveCursor(ICursor cursor, int distance)
		{
			if (cursor.Attachment == CursorAttachment.Temporary)
			{
				this.text.MoveCursor (cursor.CursorId, distance);
			}
			else
			{
				int old_pos = this.GetCursorPosition (cursor);
				
				this.text.MoveCursor (cursor.CursorId, distance);
				
				int new_pos = this.GetCursorPosition (cursor);
				
				if (old_pos != new_pos)
				{
					this.InternalAddOplet (new CursorMoveOplet (this, cursor, old_pos));
				}
			}
		}
		
		public void RecycleCursor(ICursor cursor)
		{
			//	ATTENTION: la suppression d'un curseur doit être gérée avec
			//	prudence, car les mécanismes de Undo/Redo doivent pouvoir y
			//	faire référence en tout temps via ICursor.
			
			this.text.RecycleCursor (cursor.CursorId);
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
			
			//	Passe en revue tous les caractères et met à jour les compteurs
			//	d'utilisation pour les styles associés :
			
			Internal.StyleTable styles = this.StyleList.InternalStyleTable;
			
			for (int i = 0; i < length; i++)
			{
				ulong code = text[i];
				
				Styles.SimpleStyle   style = styles.GetStyle (code);
				Styles.LocalSettings local = styles.GetLocalSettings (code);
				Styles.ExtraSettings extra = styles.GetExtraSettings (code);
				
				if (style != null)
				{
					style.IncrementUserCount ();
				}
				
				if (local != null)
				{
					local.IncrementUserCount ();
				}
				
				if (extra != null)
				{
					extra.IncrementUserCount ();
				}
			}
			
			this.text.InsertText (cursor.CursorId, text);
			this.text_length += length;
			
			this.InternalAddOplet (new TextInsertOplet (this, position, length));
			
			this.UpdateTextBreakInformation (position, length);
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
			
			this.UpdateTextBreakInformation (position, 0);
		}
		
		public int ReadText(ICursor cursor, int length, ulong[] buffer)
		{
			return this.ReadText (cursor, length, buffer, 0);
		}
		
		public int ReadText(ICursor cursor, int length, ulong[] buffer, int offset)
		{
			return this.text.ReadText (cursor.CursorId, length, buffer, offset);
		}
		
		public void ChangeMarkers(ICursor cursor, int length, ulong marker, bool set)
		{
			this.text.ChangeMarkers (cursor.CursorId, length, marker, set);
		}
		
		
		public void ResetTextChangeMarkPosition()
		{
			this.text_change_mark     = this.TextLength;
			this.text_change_version += 1;
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
		
		public string GetDebugStyledText(ulong[] text)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			ulong prev = Internal.CharMarker.ExtractStyleAndSettings (text[0]);
			
			int start = 0;
			int count = 1;
			
			for (int i = 1; i < text.Length; i++)
			{
				ulong code = Internal.CharMarker.ExtractStyleAndSettings (text[i]);
				
				if (code == prev)
				{
					count++;
					continue;
				}
				
				this.GenerateDebugStyledTextForRun (text, prev, start, count, buffer);
				
				prev  = code;
				start = i;
				count = 1;
			}
			
			this.GenerateDebugStyledTextForRun (text, prev, start, count, buffer);
			
			return buffer.ToString ();
		}
		
		private void GenerateDebugStyledTextForRun(ulong[] text, ulong code, int offset, int length, System.Text.StringBuilder buffer)
		{
			if (length == 0)
			{
				return;
			}
			
			buffer.Append ("[");
			
			for (int i = 0; i < length; i++)
			{
				buffer.Append ((char) Unicode.Bits.GetCode (text[offset+i]));
			}
			
			buffer.Append ("]\n    ");
			
			Internal.StyleTable styles = this.StyleList.InternalStyleTable;
			
			Styles.SimpleStyle   style = styles.GetStyle (code);
			Styles.LocalSettings local = styles.GetLocalSettings (code);
			Styles.ExtraSettings extra = styles.GetExtraSettings (code);
			
			int n = 0;
			
			if (style != null)
			{
				foreach (Properties.BaseProperty property in style)
				{
					if (n > 0) buffer.Append (", ");
					buffer.Append ("S=");
					buffer.Append (property.WellKnownType.ToString ());
					n++;
				}
			}
			
			if (local != null)
			{
				foreach (Properties.BaseProperty property in local)
				{
					if (n > 0) buffer.Append (", ");
					buffer.Append ("L=");
					buffer.Append (property.WellKnownType.ToString ());
					n++;
				}
			}
			
			if (extra != null)
			{
				foreach (Properties.BaseProperty property in extra)
				{
					if (n > 0) buffer.Append (", ");
					buffer.Append ("E=");
					buffer.Append (property.WellKnownType.ToString ());
					n++;
				}
			}
			
			n = 0;
			
			if (style != null)
			{
				foreach (Properties.BaseProperty property in style)
				{
					switch (property.WellKnownType)
					{
						case Properties.WellKnownType.Styles:
						case Properties.WellKnownType.Properties:
							buffer.Append (n == 0 ? "\n    " : ", ");
							Properties.BaseProperty.SerializeToText (buffer, property);
							n++;
							break;
					}
				}
			}
			
			buffer.Append ("\n");
		}
		
		
		public void ConvertToStyledText(string simple_text, TextStyle text_style, out ulong[] styled_text)
		{
			//	Génère un texte en lui appliquant les propriétés qui définissent
			//	le style et les réglages associés.
			
			TextStyle[] text_styles = { text_style };
			
			this.ConvertToStyledText (simple_text, text_styles, null, out styled_text);
		}
		
		public void ConvertToStyledText(string simple_text, TextStyle text_style, System.Collections.ICollection properties, out ulong[] styled_text)
		{
			TextStyle[] text_styles = (text_style == null) ? new TextStyle[0] : new TextStyle[] { text_style };
			
			this.ConvertToStyledText (simple_text, text_styles, properties, out styled_text);
		}
		
		public void ConvertToStyledText(string simple_text, System.Collections.ICollection text_styles, System.Collections.ICollection properties, out ulong[] styled_text)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			if ((text_styles != null) &&
				(text_styles.Count > 0))
			{
				//	Les diverses propriétés des styles passés en entrée sont
				//	d'abord extraites et ajoutées dans la liste complète des
				//	propriétés :
				
				foreach (TextStyle style in text_styles)
				{
					//	Passe en revue toutes les propriétés définies par le style
					//	en cours d'analyse et ajoute celles-ci séquentiellement dans
					//	la liste des propriétés :
					
					int n = style.CountProperties;
					
					for (int i = 0; i < n; i++)
					{
						Properties.BaseProperty property = style[i];
						
						if (property.WellKnownType != Properties.WellKnownType.Styles)
						{
							list.Add (property);
						}
						else
						{
							//	TODO: gérer les styles cascadés en faisant une descente récursive
							//	dans les propriétés correspondantes.
						}
					}
				}
				
				//	Crée une propriété StylesProperty qui résume les styles dont
				//	les propriétés viennent d'être mises à plat ci-dessus :
				
				list.Add (new Properties.StylesProperty (text_styles));
			}
			
			if ((properties != null) &&
				(properties.Count > 0))
			{
				//	Les propriétés "manuelles" viennent s'ajouter à la fin de la
				//	liste, après les propriétés du/des styles.
				
				list.AddRange (properties);
				
				//	Prend note (sous une forme sérialisée) de toutes ces propriétés
				//	additionnelles. Sans PropertiesProperty, il serait impossible de
				//	reconstituer les propriétés "manuelles" dans certains cas.
				
				list.Add (new Properties.PropertiesProperty (properties));
			}
			
			this.ConvertToStyledText (simple_text, list, out styled_text);
		}
		
		public void ConvertToStyledText(string simple_text, System.Collections.ICollection properties, out ulong[] styled_text)
		{
			uint[] utf32;
			
			TextConverter.ConvertFromString (simple_text, out utf32);
			
			//	Trie les propriétés selon leur type; certaines vont servir à
			//	définir le style, d'autres à définir les réglages locaux et
			//	spéciaux :
			
			int length = properties == null ? 0 : properties.Count;
			
			Properties.BaseProperty[] prop_mixed = new Properties.BaseProperty[length];
			
			Styles.SimpleStyle   search_style = new Styles.SimpleStyle ();
			Styles.LocalSettings search_local = new Styles.LocalSettings ();
			Styles.ExtraSettings search_extra = new Styles.ExtraSettings ();
			
			Styles.BasePropertyContainer.Accumulator style_acc = search_style.StartAccumulation ();
			Styles.BasePropertyContainer.Accumulator local_acc = search_local.StartAccumulation ();
			Styles.BasePropertyContainer.Accumulator extra_acc = search_extra.StartAccumulation ();
			
			if (length > 0)
			{
				properties.CopyTo (prop_mixed, 0);
			}
			
			for (int i = 0; i < length; i++)
			{
				switch (prop_mixed[i].PropertyType)
				{
					case Properties.PropertyType.Style:			style_acc.Accumulate (prop_mixed[i]); break;
					case Properties.PropertyType.LocalSetting:	local_acc.Accumulate (prop_mixed[i]); break;
					case Properties.PropertyType.ExtraSetting:	extra_acc.Accumulate (prop_mixed[i]); break;
					
					default:
						throw new System.ArgumentException ("Invalid property type", "properties");
				}
			}
			
			//	Génère le style et les réglages en fonction des propriétés :
			
			style_acc.Done ();
			local_acc.Done ();
			extra_acc.Done ();
			
			ulong style = 0;
			
			//	Attache le style et les réglages; réutilise de manière interne
			//	un style existant, si possible :
			
			this.StyleList.InternalStyleTable.Attach (ref style, search_style, search_local, search_extra);
			
			if ((style_acc.RequiresSpecialCodeProcessing) ||
				(local_acc.RequiresSpecialCodeProcessing) ||
				(extra_acc.RequiresSpecialCodeProcessing))
			{
				Unicode.Bits.SetSpecialCodeFlag (ref style, true);
			}
			
			length      = utf32.Length;
			styled_text = new ulong[length];
			
			for (int i = 0; i < length; i++)
			{
				styled_text[i] = utf32[i] | style;
			}
		}
		
		
		
		private void InternalAddOplet(Common.Support.IOplet oplet)
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
		
		
		protected void UpdateTextBreakInformation(int position, int length)
		{
			//	Met à jour l'information relative à la coupure des lignes autour
			//	du passage modifié.
			
			this.text_change_version += 1;
			
			if (position < this.text_change_mark)
			{
				this.text_change_mark = position;
			}
			
			Debug.Assert.IsTrue (position <= this.TextLength);
			
			int area_begin = System.Math.Max (0, position - 20);
			int area_end   = System.Math.Min (position + length + 20, this.text_length);
			
			if (area_end > area_begin)
			{
				ulong[] text = new ulong[area_end - area_begin];
				
				this.text.SetCursorPosition (this.temp_cursor.CursorId, area_begin);
				this.text.ReadText (this.temp_cursor.CursorId, area_end - area_begin, text);
				
				//	S'il y a des sauts de lignes "forcés" dans le texte avant et
				//	après le passage modifié, on recadre la fenêtre :
				
				int from_pos = area_begin;
				int to_pos   = area_end;
				
				for (int i = position - 1; i >= area_begin; i--)
				{
					if (Unicode.Bits.GetBreakInfo (text[i - area_begin]) == Unicode.BreakInfo.Yes)
					{
						from_pos = i + 1;
						break;
					}
				}
				
				for (int i = position + length; i < area_end; i++)
				{
					if (Unicode.Bits.GetBreakInfo (text[i - area_begin]) == Unicode.BreakInfo.Yes)
					{
						to_pos = i;
						break;
					}
				}
				
				//	Cherche les frontières de mots les plus proches, avant/après le
				//	passage considéré :
				
				int word_begin = from_pos;
				int word_end   = to_pos;
				
				for (int i = position - 2; i >= from_pos; i--)
				{
					if (Unicode.Bits.GetBreakInfo (text[i - area_begin]) == Unicode.BreakInfo.Optional)
					{
						word_begin = i + 1;
						break;
					}
				}
				
				for (int i = position + length; i < to_pos; i++)
				{
					if (Unicode.Bits.GetBreakInfo (text[i - area_begin]) == Unicode.BreakInfo.Optional)
					{
						word_end = i;
						break;
					}
				}
				
				Debug.Assert.IsTrue (from_pos <= position);
				Debug.Assert.IsTrue (to_pos >= position + length);
				
				Debug.Assert.IsTrue (word_begin >= from_pos);
				Debug.Assert.IsTrue (word_end <= to_pos);
				
				//	Demande une analyse du passage considéré et recopie les
				//	informations dans le texte lui-même :
				
				int text_offset = word_begin - area_begin;
				int text_length = word_end - word_begin;
				
				Unicode.BreakInfo[] breaks = new Unicode.BreakInfo[text_length];
				Unicode.DefaultBreakAnalyzer.GenerateBreaks (text, text_offset, text_length, breaks);
				LanguageEngine.GenerateHyphens (this.context, text, text_offset, text_length, breaks);
				Unicode.Bits.SetBreakInfo (text, text_offset, breaks);
				
				Internal.CharMarker.SetMarkers (this.context.Marker.RequiresSpellChecking, text, text_offset, text_length);
				
				this.text.WriteText (this.temp_cursor.CursorId, area_end - area_begin, text);
			}
		}
		
		protected void InternalInsertText(int position, ulong[] text)
		{
			this.text.SetCursorPosition (this.temp_cursor.CursorId, position);
			this.text.InsertText (this.temp_cursor.CursorId, text);
		}
		
		protected void InternalDeleteText(int position, int length, out CursorInfo[] infos, bool book_keeping)
		{
			if (book_keeping)
			{
				//	Passe en revue tous les caractères et met à jour les compteurs
				//	d'utilisation pour les styles associés :
				
				ulong[] text = new ulong[length];
				
				this.text.SetCursorPosition (this.temp_cursor.CursorId, position);
				this.text.ReadText (this.temp_cursor.CursorId, length, text);
				
				Internal.StyleTable styles = this.StyleList.InternalStyleTable;
			
				for (int i = 0; i < length; i++)
				{
					ulong code = text[i];
					
					Styles.SimpleStyle   style = styles.GetStyle (code);
					Styles.LocalSettings local = styles.GetLocalSettings (code);
					Styles.ExtraSettings extra = styles.GetExtraSettings (code);
					
					if (style != null)
					{
						style.DecrementUserCount ();
					}
					
					if (local != null)
					{
						local.DecrementUserCount ();
					}
					
					if (extra != null)
					{
						extra.DecrementUserCount ();
					}
				}
			}
			
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
			
			this.InternalDeleteText (from_pos, length, out infos, false);
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
		
		private void SetupOpletQueue(Common.Support.OpletQueue oplet_queue)
		{
			this.oplet_queue = oplet_queue;
		}
		
		private void SetupContext(Context context)
		{
			this.context = context;
		}
		
		
		#region Abstract BaseOplet Class
		protected abstract class BaseOplet : Common.Support.AbstractOplet
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
			
			
			public override Common.Support.IOplet Undo()
			{
				this.story.InternalSaveCursorPositions (this.position, this.length, out this.cursors);
				
				Debug.Assert.IsNotNull (this.cursors);
				
				int undo_start = this.story.text_length + 1;
				int undo_end   = undo_start + this.story.undo_length;
				
				this.story.InternalMoveText (this.position, undo_end - this.length, this.length);
				
				this.story.text_length -= this.length;
				this.story.undo_length += this.length;
				
				this.story.UpdateTextBreakInformation (this.position, 0);
				
				return this;
			}
			
			public override Common.Support.IOplet Redo()
			{
				int undo_start = this.story.text_length + 1;
				int undo_end   = undo_start + this.story.undo_length;
				
				this.story.InternalMoveText (undo_end - this.length, this.position, this.length);
				
				this.story.text_length += this.length;
				this.story.undo_length -= this.length;
				
				this.story.UpdateTextBreakInformation (this.position, this.length);
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
					this.story.InternalDeleteText (undo_end - this.length, this.length, out infos, true);
					
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
			
			
			public override Common.Support.IOplet Undo()
			{
				int undo_start = this.story.text_length + 1;
				int undo_end   = undo_start + this.story.undo_length;
				
				this.story.InternalMoveText (undo_end - this.length, this.position, this.length);
				
				this.story.text_length += this.length;
				this.story.undo_length -= this.length;
				
				this.story.UpdateTextBreakInformation (this.position, this.length);
				this.story.InternalRestoreCursorPositions (this.cursors, 0);
				
				this.cursors = null;
				
				return this;
			}
			
			public override Common.Support.IOplet Redo()
			{
				this.story.InternalSaveCursorPositions (this.position, this.length, out this.cursors);
				
				Debug.Assert.IsNotNull (this.cursors);
				
				int undo_start = this.story.text_length + 1;
				int undo_end   = undo_start + this.story.undo_length;
				
				this.story.InternalMoveText (this.position, undo_end - this.length, this.length);
				
				this.story.text_length -= this.length;
				this.story.undo_length += this.length;
				
				this.story.UpdateTextBreakInformation (this.position, 0);
				
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
					this.story.InternalDeleteText (undo_end - this.length, this.length, out infos, true);
					
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
			
			
			public override Common.Support.IOplet Undo()
			{
				return this.Swap ();
			}
			
			public override Common.Support.IOplet Redo()
			{
				return this.Swap ();
			}
			
			
			private Common.Support.IOplet Swap()
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
		
		private Common.Support.OpletQueue		oplet_queue;
		private Context							context;
		
		private bool							debug_disable_oplet;
		
		private int								text_change_mark;
		private long							text_change_version;
	}
}
