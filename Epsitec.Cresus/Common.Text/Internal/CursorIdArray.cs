//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La structure CursorIdArray stocke les CursorIds associ�s � un
	/// TextChunk.
	/// </summary>
	internal struct CursorIdArray
	{
		public int								ElementCount
		{
			get
			{
				return this.length;
			}
		}
		
		
		public void Add(Internal.CursorId id, int position)
		{
			//	Ins�re un �l�ment pour repr�senter le curseur � la position
			//	donn�e.
			
			Debug.Assert.IsFalse (this.Contains (id));
			Debug.Assert.IsInBounds (position, 0, 1000000-1);
			
			//	Ici, il n'y a aucun moyen de savoir si la position sp�cifi�e est
			//	valide par rapport au TextChunk, car il n'y a pas de lien direct
			//	avec le TextChunk consid�r�.
			
			int offset;
			int index = this.FindElementAtPosition (position, out offset);
			
			Debug.Assert.IsInBounds (index, 0, this.length);
			
			this.InsertElement (index, new Element (id, offset));
		}
		
		public void Move(Internal.CursorId id, int position)
		{
			//	D�place le curseur sp�cifi� � la nouvelle position donn�e.
			
			Debug.Assert.IsTrue (this.Contains (id));
			Debug.Assert.IsInBounds (position, 0, 1000000-1);
			
			int offset;
			int from_index = this.FindElement (id);
			int to_index   = this.FindElementAtPosition (position, out offset);
			
			Debug.Assert.IsTrue (offset >= 0);
			Debug.Assert.IsInBounds (from_index, 0, this.length-1);
			Debug.Assert.IsInBounds (to_index, 0, this.length);
			
			if (from_index == to_index)
			{
				//	Pas besoin de d�placer l'�l�ment dans le tableau, car il va
				//	occuper la m�me place.
				
				int old_offset = this.elements[from_index].offset;
				int new_offset = offset;
				
				this.elements[from_index].offset = new_offset;
				
				if (from_index+1 < this.length)
				{
					this.elements[from_index+1].offset += old_offset - new_offset;
				}
			}
			else
			{
				int index;
				
				this.MoveElement (from_index, to_index, out index);
				
				//	L'�l�ment a �t� d�plac� dans le tableau. Il faut ajuster l'offset
				//	de l'�l�ment courant et de l'�l�ment suivant (s'il y en a un) :
				
				if (offset > 0)
				{
					this.elements[index].offset += offset;
					
					if (index+1 < this.length)
					{
						this.elements[index+1].offset -= offset;
					}
				}
			}
		}
		
		public void Remove(Internal.CursorId id)
		{
			Debug.Assert.IsTrue (this.Contains (id));
			
			this.RemoveElement (this.FindElement (id));
		}
		
		
		public int GetCursorPosition(Internal.CursorId id)
		{
			Debug.Assert.IsTrue (this.Contains (id));
			return this.FindElementPosition (this.FindElement (id));
		}
		
		public int GetCursorElement(Internal.CursorId id)
		{
			Debug.Assert.IsTrue (this.Contains (id));
			return this.FindElementPosition (id);
		}
		
		public int GetCursorElementBeforePosition(int position)
		{
			return this.FindElementBeforePosition (position);
		}
		
		
		public Internal.CursorId GetElementCursorId(int element)
		{
			Debug.Assert.IsInBounds (element, 0, this.elements.Length-1);
			
			return this.elements[element].id;
		}
		
		
		public void ProcessInsertion(int position, int length)
		{
			//	D�cale les curseurs en fonction d'une insertion � la position
			//	sp�cifi�e. Ceci affecte en fait le curseur plac� imm�diatement
			//	apr�s le point d'insertion.
			
			int index = this.FindElementBeforePosition (position) + 1;
			
			if (index < this.length)
			{
				this.elements[index].offset += length;
			}
		}
		
		public void ProcessRemoval(int position, int length)
		{
			//	D�cale les curseurs en fonction d'une suppression depuis la
			//	position indiqu�e.
			
			//	Si des curseurs se trouvent dans la tranche supprim�e, ils
			//	sont d�plac�s au d�but de la tranche.
			
			int index_before = this.FindElementBeforePosition (position);
			int index_after  = this.FindElementBeforePosition (position + length) + 1;
			
			int pos_at   = this.FindElementPosition (index_before);
			int index_at = index_before + 1;
			
			//	S'il y a des curseurs dans la tranche supprim�e, on les d�place
			//	un � un :
			
			while ((index_at < index_after)
				&& (index_at < this.length))
			{
				pos_at += this.elements[index_at].offset;
				
				int coverage = pos_at - position;
				
				Debug.Assert.IsTrue (this.FindElementPosition (index_at) == pos_at);
				Debug.Assert.IsTrue (coverage >= 0);
				
				this.elements[index_at].offset -= coverage;
				
				Debug.Assert.IsTrue (this.FindElementPosition (index_at) == position);
				
				length -= coverage;
				pos_at  = position;
				
				index_at++;
			}
			
			if (index_after < this.length)
			{
				Debug.Assert.IsTrue (this.GetCursorPosition (this.elements[index_after].id) >= position + length);
				Debug.Assert.IsTrue (this.elements[index_after].offset >= length);
					
				this.elements[index_after].offset -= length;
			}
		}
		
		public void ProcessMigration(int origin, ref CursorIdArray destination)
		{
			//	Migre des curseurs situ�s apr�s la position indiqu�e vers
			//	la destination.
			
			int delta;
			int index = this.FindElementAtPosition (origin, out delta);
			int pos   = (index > 0) ? this.FindElementPosition (index-1) : 0;
			int count = index;
			int dst_i = 0;
			
			while (index < this.length)
			{
				Element element = this.elements[index++];
				
				pos           += element.offset;
				element.offset = pos - origin;
				origin         = pos;
				
				destination.InsertElement (dst_i++, element);
			}
			
			//	Supprime encore les curseurs que l'on vient de copier vers la
			//	destination :
			
			this.length = count;
		}
		
		
		private int FindElement(Internal.CursorId id)
		{
			Debug.Assert.IsTrue (id.IsValid);
			
			//	Trouve l'�l�ment qui d�crit le curseur sp�cifi� et retourne
			//	son index.
			
			for (int i = 0; i < this.length; i++)
			{
				if (this.elements[i].id == id)
				{
					return i;
				}
			}
			
			return -1;
		}
		
		private int FindElementAtPosition(int position, out int delta)
		{
			//	Trouve l'�l�ment qui correspond � la position indiqu�e et
			//	retourne son index et la distance (delta) par rapport au
			//	pr�c�dent.
			
			//	L'index de l'�l�ment retourn� correspond en fait au point
			//	d'insertion pour la position sp�cifi�e. S'il y a d�j� des
			//	�l�ments � cette position, on va retourner l'index apr�s
			//	ces �l�ments-l�.
			
			for (int i = 0; i < this.length; i++)
			{
				int distance = this.elements[i].offset;
				
				if (position < distance)
				{
					delta = position;
					return i;
				}
				
				position -= distance;
			}
			
			delta = position;
			return this.length;
		}
		
		private int FindElementBeforePosition(int position)
		{
			//	Trouve l'�l�ment qui pr�c�de la position indiqu�e et
			//	retourne son index. S'il n'y a aucun �l�ment avant la
			//	position en question, retourne -1.
			
			for (int i = 0; i < this.length; i++)
			{
				position -= this.elements[i].offset;
				
				if (position <= 0)
				{
					return i-1;
				}
			}
			
			return this.length-1;
		}
		
		private int FindElementPosition(int index)
		{
			Debug.Assert.IsInBounds (index, 0, this.length-1);
			
			int position = 0;
			
			for (int i = 0; i <= index; i++)
			{
				position += this.elements[i].offset;
			}
			
			return position;
		}
		
		
		private bool Contains(Internal.CursorId id)
		{
			return this.FindElement (id) >= 0;
		}
		
		
		private void InsertElement(int index, Element element)
		{
			//	Ins�re un �l�ment � l'endroit indiqu� dans le tableau.
			//	index = 0 ins�re au d�but; index = n ins�re � la fin.
			//
			//	Ajuste l'offset de l'�l�ment suivant.
			
			if (this.elements == null)
			{
				this.elements = new Element[0];
			}
			
			Debug.Assert.IsInBounds (this.length, 0, this.elements.Length);
			Debug.Assert.IsInBounds (index, 0, this.length);
			
			Element[] old_elements = this.elements;
			Element[] new_elements = new Element[this.length+1];
			
			//	Si l'�l�ment n'est pas ajout� � la fin du tableau, il
			//	faut encore ajuster l'offset de l'�l�ment qui va se
			//	retrouver juste apr�s l'�l�ment ins�r� :
			
			if (index < this.length)
			{
				Debug.Assert.IsTrue (old_elements[index].offset > element.offset);
				old_elements[index].offset -= element.offset;
			}
			
			int n1 = index;
			int n2 = this.length - index;
			
			System.Array.Copy (old_elements, 0, new_elements, 0, n1);
			System.Array.Copy (old_elements, n1, new_elements, n1+1, n2);
			
			new_elements[index] = element;
			
			this.elements = new_elements;
			this.length   = new_elements.Length;
		}
		
		private void RemoveElement(int index)
		{
			//	Supprime un �l�ment du tableau.
			//
			//	Ajuste l'offset de l'�l�ment suivant.
			
			Debug.Assert.IsInBounds (this.length, 1, this.elements.Length);
			Debug.Assert.IsInBounds (index, 0, this.length-1);
			
			int offset = this.elements[index].offset;
			int last   = this.elements.Length-1;
			
			for (int i = index; i < last; i++)
			{
				this.elements[i] = this.elements[i+1];
			}
			
			this.length--;
			
			//	S'il y avait un �l�ment apr�s celui qui vient d'�tre
			//	supprim�, il faut encore ajuster son offset :
			
			if (index < this.length)
			{
				this.elements[index].offset += offset;
			}
		}
		
		private bool MoveElement(int from_index, int to_index, out int index)
		{
			//	D�place un �l�ment au sein du tableau et retourne true
			//	si l'�l�ment a r�ellement chang� de position.
			//
			//	Ajuste l'offset des �l�ments; l'�l�ment d�plac� se voit
			//	affecter temporairement un offset nul (ce qui doit �tre
			//	ajust� par l'appelant); si l'�l�ment n'est pas d�place,
			//	son offset n'est pas modifi� non plus.
			
			//	NB: La position de l'�l�ment d'arriv�e (to_index) est d�finie
			//		par rapport � l'�tat du tableau avant la modification.
			
			//	to_index = i place l'�l�ment � la position 'i' (i = 0 place
			//	au d�but, i = n place apr�s le dernier �l�ment).
			
			Debug.Assert.IsTrue (this.length > 0);
			Debug.Assert.IsTrue (this.length <= this.elements.Length);
			
			Debug.Assert.IsInBounds (from_index, 0, this.length-1);
			Debug.Assert.IsInBounds (to_index, 0, this.length);
			
			if (from_index < to_index)
			{
				index = to_index - 1;
				
				//	Si on d�cale simplement apr�s soi-m�me, il n'y a rien �
				//	faire :
				
				if (from_index+1 == to_index)
				{
					return false;
				}
				
				//	D�place l'�l�ment plus loin dans le tableau :
				
				Element element = this.elements[from_index];
				
				for (int i = from_index; i < to_index-1; i++)
				{
					this.elements[i] = this.elements[i+1];
				}
				
				//	Ajuste l'offset de l'�l�ment qui se trouvait juste apr�s
				//	celui qui vient d'�tre retir� du tableau :
				
				this.elements[from_index].offset += element.offset;
				
				//	L'�l�ment que l'on va ins�rer � nouveau aura un offset
				//	nul par rapport � son pr�d�cesseur, en attendant que
				//	l'appelant lui affecte un offset d�finitif :
				
				element.offset = 0;
				
				//	Termine le d�placement.
				
				this.elements[index] = element;
			}
			else
			{
				index = to_index;
				
				if (from_index == to_index)
				{
					return false;
				}
				
				//	D�place l'�l�ment plus en avant dans le tableau :
				
				Element element = this.elements[from_index];
				
				for (int i = from_index; i > to_index; i--)
				{
					this.elements[i] = this.elements[i-1];
				}
				
				//	Ajuste l'offset de l'�l�ment qui se trouvait juste apr�s
				//	celui qui vient d'�tre retir� du tableau, pour autant qu'il
				//	y en ait eu un :
				
				if (from_index+1 < this.length)
				{
					this.elements[from_index+1].offset += element.offset;
				}
				
				//	L'�l�ment que l'on va ins�rer � nouveau aura un offset
				//	nul par rapport � son pr�d�cesseur, en attendant que
				//	l'appelant lui affecte un offset d�finitif :
				
				element.offset = 0;
				
				//	Termine le d�placement.
				
				this.elements[to_index] = element;
			}
			
			return true;
		}
		
		
		
		private struct Element
		{
			public Element(Internal.CursorId id, int offset)
			{
				this.id     = id;
				this.offset = offset;
			}
			
			
			public	Internal.CursorId			id;
			public	int							offset;
		}
		
		
		private Element[]						elements;
		private int								length;
	}
}
