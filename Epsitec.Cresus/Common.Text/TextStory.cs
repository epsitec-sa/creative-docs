//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	using OpletEventHandler = Epsitec.Common.Support.OpletEventHandler;
	using OpletEventArgs	= Epsitec.Common.Support.OpletEventArgs;
	
	/// <summary>
	/// La classe TextStory repr�sente un texte complet, avec tous ses attributs
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
				this.InternalAddOplet (new CursorNewRecycleOplet (this, cursor, -1));
			}
		}
		
		public void MoveCursor(ICursor cursor, int distance)
		{
			if (distance == 0)
			{
				return;
			}
			
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
			//	ATTENTION: la suppression d'un curseur doit �tre g�r�e avec
			//	prudence, car les m�canismes de Undo/Redo doivent pouvoir y
			//	faire r�f�rence en tout temps via ICursor.
			
			int pos = this.GetCursorPosition (cursor);
			
			this.text.RecycleCursor (cursor.CursorId);
			
			if (cursor.Attachment != CursorAttachment.Temporary)
			{
				this.InternalAddOplet (new CursorNewRecycleOplet (this, cursor, pos));
			}
		}
		
		
		public int GetCursorPosition(ICursor cursor)
		{
			return this.text.GetCursorPosition (cursor.CursorId);
		}
		
		public void SetCursorPosition(ICursor cursor, int position)
		{
			if (cursor.Attachment == CursorAttachment.Temporary)
			{
				this.text.SetCursorPosition (cursor.CursorId, position);
			}
			else
			{
				int old_pos = this.GetCursorPosition (cursor);
				
				this.text.SetCursorPosition (cursor.CursorId, position);
				
				int new_pos = this.GetCursorPosition (cursor);
				
				if (old_pos != new_pos)
				{
					this.InternalAddOplet (new CursorMoveOplet (this, cursor, old_pos));
				}
			}
		}
		
		
		public void InsertText(ICursor cursor, ulong[] text)
		{
			int position = this.text.GetCursorPosition (cursor.CursorId);
			int length   = text.Length;
			
			//	Passe en revue tous les caract�res et met � jour les compteurs
			//	d'utilisation pour les styles associ�s :
			
			this.IncrementUserCount (text, length);
			
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
		
		
		internal bool ReplaceText(ICursor cursor, int length, ulong[] text)
		{
			//	Remplace le texte sans mettre � jour les informations de undo
			//	et de redo. Retourne true si une modification a eu lieu.
			
			//	Le texte de remplacement n'a pas besoin d'avoir la m�me longueur
			//	que le texte qui est remplac� (contrairement � WriteText qui se
			//	contente d'�craser le texte sur place).
			
			int position = this.text.GetCursorPosition (cursor.CursorId);
			
			bool changed = this.InternalReplaceText (position, length, text);
			
			this.text_length -= length;
			this.text_length += text.Length;
			
			this.UpdateTextBreakInformation (position, length);
			
			return changed;
		}
		
		internal bool ReplaceText(ICursor cursor, int length, string simple_text)
		{
			int position = this.text.GetCursorPosition (cursor.CursorId);
			
			uint[] utf32;
			
			TextConverter.ConvertFromString (simple_text, out utf32);
			
			bool changed = this.InternalReplaceText (position, length, utf32);
			
			this.text_length -= length;
			this.text_length += utf32.Length;
			
			this.UpdateTextBreakInformation (position, length);
			
			return changed;
		}
		
		
		public int ReadText(int position, int length, ulong[] buffer)
		{
			this.text.SetCursorPosition (this.temp_cursor.CursorId, position);
			return this.ReadText (this.temp_cursor, 0, length, buffer);
		}
		
		public int ReadText(ICursor cursor, int length, ulong[] buffer)
		{
			return this.ReadText (cursor, 0, length, buffer);
		}
		
		public int ReadText(ICursor cursor, int offset, int length, ulong[] buffer)
		{
			return this.text.ReadText (cursor.CursorId, offset, length, buffer);
		}
		
		
		public int WriteText(ICursor cursor, ulong[] new_text)
		{
			return this.WriteText (cursor, 0, new_text);
		}
		
		public int WriteText(ICursor cursor, int offset, ulong[] new_text)
		{
			//	Remplace du texte existant par un nouveau texte; cette op�ration
			//	�crase l'ancien texte, lequel est remplac� caract�re par caract�re.
			
			//	Comparer avec ReplaceText qui peut travailler avec des longueurs
			//	diff�rentes.
			
			int length  = new_text.Length;
			int changes = 0;
			
			Internal.StyleTable styles   = this.StyleList.InternalStyleTable;
			ulong[]             old_text = new ulong[length];
			
			this.text.ReadText (cursor.CursorId, offset, length, old_text);
			
			//	Y a-t-il vraiment des modifications ?
			
			for (int i = 0; i < length; i++)
			{
				if (old_text[i] != new_text[i])
				{
					changes++;
				}
			}
			
			//	S'il n'y a aucune modification dans le texte, il n'y pas non plus
			//	lieu de faire quoi que ce soit :
			
			if (changes == 0)
			{
				return length;
			}
			
			
			this.IncrementUserCount (new_text, length);
			
			int position = this.text.GetCursorPosition (cursor.CursorId) + offset;
			int written  = this.text.WriteText (cursor.CursorId, offset, length, new_text);
			
			this.InternalAddOplet (new TextChangeOplet (this, position, length, old_text));
			
			this.UpdateTextBreakInformation (position, length);
			
			return written;
		}
		
		
		public int ChangeAllMarkers(ulong marker, bool set)
		{
			//	Cette op�ration n'est pas annulable.
			
			this.text.SetCursorPosition (this.temp_cursor.CursorId, 0);
			
			return this.ChangeMarkers (this.temp_cursor, this.TextLength, marker, set);
		}
		
		public int ChangeMarkers(int position, int length, ulong marker, bool set)
		{
			this.text.SetCursorPosition (this.temp_cursor.CursorId, position);
			return this.ChangeMarkers (this.temp_cursor, length, marker, set);
		}
		
		public int ChangeMarkers(ICursor cursor, int length, ulong marker, bool set)
		{
			//	Modifie les marqueurs associ�s � une tranche de texte; voir aussi
			//	Text.Context.Marker pour des marqueurs "standard".
			//
			//	Retourne le nombre de caract�res modifi�s.
			//
			//	Cette op�ration n'est pas annulable.
			
			return this.text.ChangeMarkers (cursor.CursorId, length, marker, set);
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
				foreach (Property property in style)
				{
					if (n > 0) buffer.Append (", ");
					buffer.Append ("S=");
					buffer.Append (property.WellKnownType.ToString ());
					n++;
				}
			}
			
			if (local != null)
			{
				foreach (Property property in local)
				{
					if (n > 0) buffer.Append (", ");
					buffer.Append ("L=");
					buffer.Append (property.WellKnownType.ToString ());
					n++;
				}
			}
			
			if (extra != null)
			{
				foreach (Property property in extra)
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
				foreach (Property property in style)
				{
					switch (property.WellKnownType)
					{
						case Properties.WellKnownType.Styles:
						case Properties.WellKnownType.Properties:
							buffer.Append (n == 0 ? "\n    " : ", ");
							Property.SerializeToText (buffer, property);
							n++;
							break;
					}
				}
			}
			
			buffer.Append ("\n");
		}
		
		
		public void ConvertToStyledText(string simple_text, TextStyle text_style, out ulong[] styled_text)
		{
			//	G�n�re un texte en lui appliquant les propri�t�s qui d�finissent
			//	le style et les r�glages associ�s.
			
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
			this.ConvertToStyledText (simple_text, this.FlattenStylesAndProperties (text_styles, properties), out styled_text);
		}
		
		public void ConvertToStyledText(string simple_text, System.Collections.ICollection properties, out ulong[] styled_text)
		{
			uint[] utf32;
			
			TextConverter.ConvertFromString (simple_text, out utf32);
			
			this.ConvertToStyledText (utf32, properties, out styled_text);
		}
		
		public void ConvertToStyledText(uint[] utf32, System.Collections.ICollection properties, out ulong[] styled_text)
		{
			ulong style;
			int   length = utf32.Length;
			
			this.ConvertToStyledText (properties, out style);
			
			styled_text = new ulong[length];
			
			for (int i = 0; i < length; i++)
			{
				styled_text[i] = utf32[i] | style;
			}
		}
		
		public void ConvertToStyledText(System.Collections.ICollection properties, out ulong style)
		{
			//	Trie les propri�t�s selon leur type; certaines vont servir �
			//	d�finir le style, d'autres � d�finir les r�glages locaux et
			//	sp�ciaux :
			
			int length = properties == null ? 0 : properties.Count;
			
			Property[] prop_mixed = new Property[length];
			
			Styles.SimpleStyle   search_style = new Styles.SimpleStyle ();
			Styles.LocalSettings search_local = new Styles.LocalSettings ();
			Styles.ExtraSettings search_extra = new Styles.ExtraSettings ();
			
			Styles.PropertyContainer.Accumulator style_acc = search_style.StartAccumulation ();
			Styles.PropertyContainer.Accumulator local_acc = search_local.StartAccumulation ();
			Styles.PropertyContainer.Accumulator extra_acc = search_extra.StartAccumulation ();
			
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
			
			//	G�n�re le style et les r�glages en fonction des propri�t�s :
			
			style_acc.Done ();
			local_acc.Done ();
			extra_acc.Done ();
			
			style = 0;
			
			//	Attache le style et les r�glages; r�utilise de mani�re interne
			//	un style existant, si possible :
			
			this.StyleList.InternalStyleTable.Attach (ref style, search_style, search_local, search_extra);
			
			if ((style_acc.RequiresSpecialCodeProcessing) ||
				(local_acc.RequiresSpecialCodeProcessing) ||
				(extra_acc.RequiresSpecialCodeProcessing))
			{
				Unicode.Bits.SetSpecialCodeFlag (ref style, true);
			}
		}
		
		
		public System.Collections.ArrayList FlattenStylesAndProperties(System.Collections.ICollection text_styles, System.Collections.ICollection properties)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			if ((text_styles != null) &&
				(text_styles.Count > 0))
			{
				//	Les diverses propri�t�s des styles pass�s en entr�e sont
				//	d'abord extraites et ajout�es dans la liste compl�te des
				//	propri�t�s :
				
				foreach (TextStyle style in text_styles)
				{
					//	Passe en revue toutes les propri�t�s d�finies par le style
					//	en cours d'analyse et ajoute celles-ci s�quentiellement dans
					//	la liste des propri�t�s :
					
					int n = style.CountProperties;
					
					for (int i = 0; i < n; i++)
					{
						Property property = style[i];
						
						if (property.WellKnownType != Properties.WellKnownType.Styles)
						{
							list.Add (property);
						}
						else
						{
							//	TODO: g�rer les styles cascad�s en faisant une descente r�cursive
							//	dans les propri�t�s correspondantes.
						}
					}
				}
				
				//	Cr�e une propri�t� StylesProperty qui r�sume les styles dont
				//	les propri�t�s viennent d'�tre mises � plat ci-dessus :
				
				list.Add (new Properties.StylesProperty (text_styles));
			}
			
			if ((properties != null) &&
				(properties.Count > 0))
			{
				//	Les propri�t�s "manuelles" viennent s'ajouter � la fin de la
				//	liste, apr�s les propri�t�s du/des styles.
				
				list.AddRange (properties);
				
				//	Prend note (sous une forme s�rialis�e) de toutes ces propri�t�s
				//	additionnelles. Sans PropertiesProperty, il serait impossible de
				//	reconstituer les propri�t�s "manuelles" dans certains cas.
				
				list.Add (new Properties.PropertiesProperty (properties));
			}
			
			return list;
		}
		
		
		private void IncrementUserCount(ulong[] text, int length)
		{
			Internal.StyleTable styles = this.StyleList.InternalStyleTable;
			
			for (int i = 0; i < length; i++)
			{
				ulong code = text[i];
				
				Styles.SimpleStyle   style = styles.GetStyle (code);
				Styles.LocalSettings local = styles.GetLocalSettings (code);
				Styles.ExtraSettings extra = styles.GetExtraSettings (code);
				
				if (style != null) style.IncrementUserCount ();
				if (local != null) local.IncrementUserCount ();
				if (extra != null) extra.IncrementUserCount ();
			}
		}
		
		private void DecrementUserCount(ulong[] text, int length)
		{
			Internal.StyleTable styles = this.StyleList.InternalStyleTable;
			
			for (int i = 0; i < length; i++)
			{
				ulong code = text[i];
				
				Styles.SimpleStyle   style = styles.GetStyle (code);
				Styles.LocalSettings local = styles.GetLocalSettings (code);
				Styles.ExtraSettings extra = styles.GetExtraSettings (code);
				
				if (style != null) style.DecrementUserCount ();
				if (local != null) local.DecrementUserCount ();
				if (extra != null) extra.DecrementUserCount ();
			}
		}
		
		
		private void InternalAddOplet(Common.Support.IOplet oplet)
		{
			if ((this.debug_disable_oplet == false) &&
				(this.oplet_queue != null))
			{
				//	TODO: g�rer la fusion d'oplets identiques
				
				using (this.oplet_queue.BeginAction ())
				{
					this.oplet_queue.Insert (oplet);
					this.oplet_queue.ValidateAction ();
				}
			}
			else
			{
				oplet.Dispose ();
			}
		}
		
		
		protected void UpdateTextBreakInformation(int position, int length)
		{
			//	Met � jour l'information relative � la coupure des lignes autour
			//	du passage modifi�.
			
			this.text_change_version += 1;
			
			if (position < this.text_change_mark)
			{
				this.text_change_mark = position;
			}
			
			System.Diagnostics.Debug.Assert (position <= this.TextLength);
			
			int area_begin = System.Math.Max (0, position - 20);
			int area_end   = System.Math.Min (position + length + 20, this.text_length);
			
			if (area_end > area_begin)
			{
				ulong[] text = new ulong[area_end - area_begin];
				
				this.text.SetCursorPosition (this.temp_cursor.CursorId, area_begin);
				this.text.ReadText (this.temp_cursor.CursorId, area_end - area_begin, text);
				
				//	S'il y a des sauts de lignes "forc�s" dans le texte avant et
				//	apr�s le passage modifi�, on recadre la fen�tre :
				
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
				
				//	Cherche les fronti�res de mots les plus proches, avant/apr�s le
				//	passage consid�r� :
				
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
				
				System.Diagnostics.Debug.Assert (from_pos <= position);
				System.Diagnostics.Debug.Assert (to_pos >= position + length);
				
				System.Diagnostics.Debug.Assert (word_begin >= from_pos);
				System.Diagnostics.Debug.Assert (word_end <= to_pos);
				
				//	Demande une analyse du passage consid�r� et recopie les
				//	informations dans le texte lui-m�me :
				
				int text_offset = word_begin - area_begin;
				int text_length = word_end - word_begin;
				
				Unicode.BreakInfo[] breaks = new Unicode.BreakInfo[text_length];
				Unicode.DefaultBreakAnalyzer.GenerateBreaks (text, text_offset, text_length, breaks);
				LanguageEngine.GenerateHyphens (this.context, text, text_offset, text_length, breaks);
				Unicode.Bits.SetBreakInfo (text, text_offset, breaks);
				
				Internal.CharMarker.SetMarkers (this.context.Markers.RequiresSpellChecking, text, text_offset, text_length);
				
				this.text.WriteText (this.temp_cursor.CursorId, area_end - area_begin, text);
			}
		}
		
		protected void InternalInsertText(int position, ulong[] text, bool book_keeping)
		{
			if (book_keeping)
			{
				//	Passe en revue tous les caract�res et met � jour les compteurs
				//	d'utilisation pour les styles associ�s :
				
				this.IncrementUserCount (text, text.Length);
			}
			
			this.text.SetCursorPosition (this.temp_cursor.CursorId, position);
			this.text.InsertText (this.temp_cursor.CursorId, text);
		}
		
		protected void InternalDeleteText(int position, int length, out CursorInfo[] infos, bool book_keeping)
		{
			if (book_keeping)
			{
				//	Passe en revue tous les caract�res et met � jour les compteurs
				//	d'utilisation pour les styles associ�s :
				
				ulong[] text = new ulong[length];
				
				this.text.SetCursorPosition (this.temp_cursor.CursorId, position);
				this.text.ReadText (this.temp_cursor.CursorId, length, text);
				
				this.DecrementUserCount (text, length);
			}
			
			this.text.SetCursorPosition (this.temp_cursor.CursorId, position);
			this.text.DeleteText (this.temp_cursor.CursorId, length, out infos);
		}
		
		protected bool InternalReplaceText(int position, int length, ulong[] text)
		{
			//	Remplace du texte sans gestion du undo/redo ni mise � jour des
			//	longueurs respectives de 'text area' et 'undo area', ni gestion
			//	des curseurs.
			
			this.text.SetCursorPosition (this.temp_cursor.CursorId, position);
			
			ulong[] data = new ulong[length];
			int     read = this.text.ReadText (this.temp_cursor.CursorId, length, data);
			
			System.Diagnostics.Debug.Assert (read == length);
			
			CursorInfo[] infos;
			
			this.InternalDeleteText (position, length, out infos, false);
			this.InternalInsertText (position, text, false);
			
			if ((infos != null) &&
				(infos.Length > 0))
			{
				this.InternalRestoreCursorPositions (infos, 0);
			}
			
			if (data.Length != text.Length)
			{
				return true;
			}
			
			for (int i = 0; i < data.Length; i++)
			{
				ulong a = data[i] & ~Unicode.Bits.InfoMask;
				ulong b = text[i] & ~Unicode.Bits.InfoMask;
				
				if (a != b)
				{
					return true;
				}
			}
			
			return false;
		}
		
		protected bool InternalReplaceText(int position, int length, uint[] utf32)
		{
			//	Remplace du texte sans gestion du undo/redo ni mise � jour des
			//	longueurs respectives de 'text area' et 'undo area', ni gestion
			//	des curseurs.
			
			this.text.SetCursorPosition (this.temp_cursor.CursorId, position);
			
			ulong[] text = new ulong[utf32.Length];
			ulong[] data = new ulong[length];
			int     read = this.text.ReadText (this.temp_cursor.CursorId, length, data);
			
			System.Diagnostics.Debug.Assert (read == length);
			
			ulong code = (~ Unicode.Bits.FullCodeMask) & data[0];
			
			for (int i = 0; i < text.Length; i++)
			{
				text[i] = code | utf32[i];
			}
			
			CursorInfo[] infos;
			
			this.InternalDeleteText (position, length, out infos, false);
			this.InternalInsertText (position, text, false);
			
			if ((infos != null) &&
				(infos.Length > 0))
			{
				this.InternalRestoreCursorPositions (infos, 0);
			}
			
			if (data.Length != text.Length)
			{
				return true;
			}
			
			for (int i = 0; i < data.Length; i++)
			{
				ulong a = data[i] & ~Unicode.Bits.InfoMask;
				ulong b = text[i] & ~Unicode.Bits.InfoMask;
				
				if (a != b)
				{
					return true;
				}
			}
			
			return false;
		}
		
		protected void InternalMoveText(int from_pos, int to_pos, int length)
		{
			//	D�place le texte sans gestion du undo/redo ni mise � jour des
			//	longueurs respectives de 'text area' et 'undo area', ni gestion
			//	des curseurs.
			
			//	L'appelant fournit une position de destination qui est valide
			//	seulement apr�s la suppression (temporaire) du texte.
			
			this.text.SetCursorPosition (this.temp_cursor.CursorId, from_pos);
			
			ulong[] data = new ulong[length];
			int     read = this.text.ReadText (this.temp_cursor.CursorId, length, data);
			
			System.Diagnostics.Debug.Assert (read == length);
			
			CursorInfo[] infos;
			
			this.InternalDeleteText (from_pos, length, out infos, false);
			this.InternalInsertText (to_pos, data, false);
			
			if ((infos != null) &&
				(infos.Length > 0))
			{
				//	La liste des curseurs affect�s contient peut-�tre des curseurs
				//	temporaires; on commence par les filtrer, puis on d�place tous
				//	les curseurs restants � la nouvelle position :
				
				infos = this.text.FilterCursors (infos, new CursorInfo.Filter (this.FilterSaveCursors));
				
				this.InternalRestoreCursorPositions (infos, to_pos - from_pos);
			}
		}
		
		protected void InternalReadText(int position, ulong[] buffer)
		{
			//	Lit le texte � la position donn�e.
			
			this.text.SetCursorPosition (this.temp_cursor.CursorId, position);
			
			int length = buffer.Length;
			int read   = this.text.ReadText (this.temp_cursor.CursorId, length, buffer);
			
			System.Diagnostics.Debug.Assert (read == length);
		}
		
		protected void InternalWriteText(int position, ulong[] text)
		{
			//	Ecrit le texte � la position donn�e.
			
			this.text.SetCursorPosition (this.temp_cursor.CursorId, position);
			
			int length  = text.Length;
			int written = this.text.WriteText (this.temp_cursor.CursorId, length, text);
			
			System.Diagnostics.Debug.Assert (written == length);
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
		
		
		private void NotifyUndoExecuted(BaseOplet oplet)
		{
			this.OnOpletExecuted (new OpletEventArgs (oplet, Common.Support.OpletEvent.UndoExecuted));
		}
		
		private void NotifyRedoExecuted(BaseOplet oplet)
		{
			this.OnOpletExecuted (new OpletEventArgs (oplet, Common.Support.OpletEvent.RedoExecuted));
		}
		
		
		protected virtual void OnOpletExecuted(OpletEventArgs e)
		{
			if (this.OpletExecuted != null)
			{
				this.OpletExecuted (this, e);
			}
		}
		
		
		#region Abstract BaseOplet Class
		internal abstract class BaseOplet : Common.Support.AbstractOplet
		{
			protected BaseOplet(TextStory story)
			{
				this.story = story;
			}
			
			
			
			protected void NotifyUndoExecuted()
			{
				this.story.NotifyUndoExecuted (this);
			}
			
			protected void NotifyRedoExecuted()
			{
				this.story.NotifyRedoExecuted (this);
			}
			
			
			protected readonly TextStory		story;
		}
		#endregion
		
		#region Abstract BaseCursorOplet Class
		internal abstract class BaseCursorOplet : BaseOplet
		{
			public BaseCursorOplet(TextStory story, ICursor cursor) : base (story)
			{
				this.cursor = cursor;
			}
			
			
			public ICursor						Cursor
			{
				get
				{
					return this.cursor;
				}
			}
			
			
			protected readonly ICursor			cursor;
		}
		#endregion
		
		#region TextInsertOplet Class
		internal class TextInsertOplet : BaseOplet
		{
			public TextInsertOplet(TextStory story, int position, int length) : base (story)
			{
				this.position = position;
				this.length   = length;
			}
			
			
			public override Common.Support.IOplet Undo()
			{
				this.story.InternalSaveCursorPositions (this.position, this.length, out this.cursors);
				
				System.Diagnostics.Debug.Assert (this.cursors != null);
				
				int undo_start = this.story.text_length + 1;
				int undo_end   = undo_start + this.story.undo_length;
				
				this.story.InternalMoveText (this.position, undo_end - this.length, this.length);
				
				this.story.text_length -= this.length;
				this.story.undo_length += this.length;
				
				this.story.UpdateTextBreakInformation (this.position, 0);
				this.NotifyUndoExecuted ();
				
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
				this.NotifyRedoExecuted ();
				
				return this;
			}
			
			
			public override void Dispose()
			{
				//	Lorsque l'on supprime une information permettant d'annuler
				//	une insertion, il n'y a rien � faire. Par contre, si l'oplet
				//	est dans l'�tat "redoable", il faudra supprimer le texte de
				//	la "undo area".
				
				System.Diagnostics.Debug.Assert (this.length > 0);
				
				if (this.cursors != null)
				{
					int undo_start = this.story.text_length + 1;
					int undo_end   = undo_start + this.story.undo_length;
					
					CursorInfo[] infos;
					this.story.InternalDeleteText (undo_end - this.length, this.length, out infos, true);
					
					//	TODO: g�rer la suppression des curseurs...
					//	TODO: g�rer la suppression des styles...
					
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
		internal class TextDeleteOplet : BaseOplet
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
				this.NotifyUndoExecuted ();
				
				return this;
			}
			
			public override Common.Support.IOplet Redo()
			{
				this.story.InternalSaveCursorPositions (this.position, this.length, out this.cursors);
				
				System.Diagnostics.Debug.Assert (this.cursors != null);
				
				int undo_start = this.story.text_length + 1;
				int undo_end   = undo_start + this.story.undo_length;
				
				this.story.InternalMoveText (this.position, undo_end - this.length, this.length);
				
				this.story.text_length -= this.length;
				this.story.undo_length += this.length;
				
				this.story.UpdateTextBreakInformation (this.position, 0);
				this.NotifyRedoExecuted ();
				
				return this;
			}
			
			
			public override void Dispose()
			{
				//	Lorsque l'on supprime une information permettant de refaire
				//	une destruction, il n'y a rien � faire. Par contre, si l'oplet
				//	est dans l'�tat "undoable", il faudra supprimer le texte de
				//	la "undo area".
				
				System.Diagnostics.Debug.Assert (this.length > 0);
				
				if (this.cursors != null)
				{
					int undo_start = this.story.text_length + 1;
					int undo_end   = undo_start + this.story.undo_length;
					
					CursorInfo[] infos;
					this.story.InternalDeleteText (undo_end - this.length, this.length, out infos, true);
					
					//	TODO: g�rer la suppression des curseurs...
					//	TODO: g�rer la suppression des styles...
					
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
		
		#region TextChangeOplet Class
		internal class TextChangeOplet : BaseOplet
		{
			public TextChangeOplet(TextStory story, int position, int length, ulong[] old_text) : base (story)
			{
				this.position = position;
				this.length   = length;
				this.text     = old_text;
			}
			
			
			public override Common.Support.IOplet Undo()
			{
				ulong[] old_text = new ulong[this.length];
				ulong[] new_text = this.text;
				
				this.story.InternalReadText (this.position, old_text);
				this.story.InternalWriteText (this.position, new_text);
				
				this.text = old_text;
				
				this.story.UpdateTextBreakInformation (this.position, this.length);
				this.NotifyUndoExecuted ();
				
				return this;
			}
			
			public override Common.Support.IOplet Redo()
			{
				ulong[] old_text = new ulong[this.length];
				ulong[] new_text = this.text;
				
				this.story.InternalReadText (this.position, old_text);
				this.story.InternalWriteText (this.position, new_text);
				
				this.text = old_text;
				
				this.story.UpdateTextBreakInformation (this.position, 0);
				this.NotifyRedoExecuted ();
				
				return this;
			}
			
			
			public override void Dispose()
			{
				//	Lorsque l'on supprime une information permettant de refaire
				//	ou d�faire une modification, il suffit de mettre � jour les
				//	compteurs d'utilisation des styles.
				
				System.Diagnostics.Debug.Assert (this.length > 0);
				
				this.story.DecrementUserCount (this.text, this.length);
				
				this.text   = null;
				this.length = 0;
				
				base.Dispose ();
			}
			
			
			private int							position;
			private int							length;
			private ulong[]						text;
		}
		#endregion
		
		#region CursorMoveOplet Class
		internal class CursorMoveOplet : BaseCursorOplet
		{
			public CursorMoveOplet(TextStory story, ICursor cursor, int position) : base (story, cursor)
			{
				this.position = position;
			}
			
			
			public override Common.Support.IOplet Undo()
			{
				Common.Support.IOplet oplet = this.Swap ();
				this.NotifyUndoExecuted ();
				return oplet;
			}
			
			public override Common.Support.IOplet Redo()
			{
				Common.Support.IOplet oplet = this.Swap ();
				this.NotifyRedoExecuted ();
				return oplet;
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
		}
		#endregion
		
		#region CursorNewRecycleOplet Class
		internal class CursorNewRecycleOplet : BaseCursorOplet
		{
			public CursorNewRecycleOplet(TextStory story, ICursor cursor, int position) : base (story, cursor)
			{
				this.position = position;
			}
			
			
			public override Common.Support.IOplet Undo()
			{
				Common.Support.IOplet oplet = this.Swap ();
				this.NotifyUndoExecuted ();
				return oplet;
			}
			
			public override Common.Support.IOplet Redo()
			{
				Common.Support.IOplet oplet = this.Swap ();
				this.NotifyRedoExecuted ();
				return oplet;
			}
			
			
			private Common.Support.IOplet Swap()
			{
				if (this.position == -1)
				{
					this.position = this.story.text.GetCursorPosition (this.cursor.CursorId);
					this.story.text.RecycleCursor (this.cursor.CursorId);
				}
				else
				{
					this.story.text.NewCursor (this.cursor);
					this.story.text.MoveCursor (this.cursor.CursorId, this.position);
					this.position = -1;
				}
				
				return this;
			}
			
			
			private int							position;
		}
		#endregion
		
		public delegate bool CodeCallback(ulong code);
		
		
		public event OpletEventHandler			OpletExecuted;
		
		
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
