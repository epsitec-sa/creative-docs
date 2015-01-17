//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			this.Add (id, position, CursorAttachment.Floating);
		}
		
		public void Add(Internal.CursorId id, int position, CursorAttachment attachment)
		{
			//	Ins�re un �l�ment pour repr�senter le curseur � la position
			//	donn�e.
			
			Debug.Assert.IsFalse (this.Contains (id));
			Debug.Assert.IsInBounds (position, 0, CursorIdArray.MaxPosition);
			
			//	Ici, il n'y a aucun moyen de savoir si la position sp�cifi�e est
			//	valide par rapport au TextChunk, car il n'y a pas de lien direct
			//	avec le TextChunk consid�r�.
			
			int offset;
			int index = this.FindElementAtPosition (position, out offset);
			
			Debug.Assert.IsInBounds (index, 0, this.length);
			
			this.InsertElement (index, new Element (id, offset, attachment));
		}
		
		public void Move(Internal.CursorId id, int position)
		{
			//	D�place le curseur sp�cifi� � la nouvelle position donn�e.
			
			Debug.Assert.IsTrue (this.Contains (id));
			Debug.Assert.IsInBounds (position, 0, CursorIdArray.MaxPosition);
			
			int offset;
			int fromIndex = this.FindElement (id);
			int toIndex   = this.FindElementAtPosition (position, out offset);
			
			Debug.Assert.IsTrue (offset >= 0);
			Debug.Assert.IsInBounds (fromIndex, 0, this.length-1);
			Debug.Assert.IsInBounds (toIndex, 0, this.length);
			
			if (fromIndex == toIndex)
			{
				//	Pas besoin de d�placer l'�l�ment dans le tableau, car il va
				//	occuper la m�me place.
				
				int oldOffset = this.elements[fromIndex].offset;
				int newOffset = offset;
				
				this.elements[fromIndex].offset = newOffset;
				
				if (fromIndex+1 < this.length)
				{
					this.elements[fromIndex+1].offset += oldOffset - newOffset;
				}
			}
			else
			{
				int index;
				
				this.MoveElement (fromIndex, toIndex, out index);
				
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
		
		public int GetElementCursorOffset(int element)
		{
			Debug.Assert.IsInBounds (element, 0, this.elements.Length-1);
			
			return this.elements[element].offset;
		}
		
		public CursorAttachment GetElementCursorAttachment(int element)
		{
			Debug.Assert.IsInBounds (element, 0, this.elements.Length-1);
			
			return this.elements[element].attachment;
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
		
		public void ProcessRemoval(int position, int length, int absOrigin, bool removalContinuation, out CursorInfo[] removed)
		{
			//	D�cale les curseurs en fonction d'une suppression depuis la
			//	position indiqu�e et g�n�re la table des curseurs attach�s
			//	ainsi d�plac�s (la table contient la position absolue des
			//	curseurs avant leur d�placement; 'absOrigin' sp�cifie le
			//	d�but absolu du morceau de texte dans lequel on travaille).
			
			//	Si des curseurs se trouvent dans la tranche supprim�e, ils
			//	sont d�plac�s au d�but de la tranche.
			
			System.Collections.ArrayList list = null;
			
			int indexBefore = this.FindElementBeforePosition (position);
			int indexAfter  = this.FindElementBeforePosition (position + length + 1) + 1;
			
			int posAt   = indexBefore < 0 ? 0 : this.FindElementPosition (indexBefore);
			int posAbs  = absOrigin + posAt;
			int indexAt = indexBefore + 1;
			
			//	S'il y a des curseurs dans la tranche supprim�e, on les d�place
			//	un � un :
			
			while ((indexAt < indexAfter)
				&& (indexAt < this.length))
			{
				posAt  += this.elements[indexAt].offset;
				posAbs += this.elements[indexAt].offset;
				
				Debug.Assert.IsTrue (this.FindElementPosition (indexAt) == posAt);
				
				int coverage = posAt - position;
				
				length -= coverage;
				posAt  = position;
				
				if (coverage > 0)
				{
					removalContinuation = true;
				}
				
				Debug.Assert.IsTrue (coverage >= 0);
				Debug.Assert.IsTrue (length >= 0);
				
				//	Si le curseur est enti�rement compris dans la zone � consid�rer
				//	il est directement affect� par la destruction :
				
				CursorAttachment attachment = this.elements[indexAt].attachment;
				
				if (((length >= 1) && (attachment == CursorAttachment.ToNext)) ||
					((removalContinuation) && (attachment == CursorAttachment.ToPrevious)))
				{
					if (list == null)
					{
						list = new System.Collections.ArrayList ();
					}
					
					//	Prend note du curseur et de sa position avant le d�placement
					//	ce qui permet � l'appelant de g�n�rer les informations pour
					//	l'annulation :
					
					list.Add (new CursorInfo (this.elements[indexAt].id, posAbs, 0));
				}
				
				//	D�place le curseur au d�but de la tranche supprim�e :
				
				this.elements[indexAt].offset -= coverage;
				
				Debug.Assert.IsTrue (this.FindElementPosition (indexAt) == position);
				
				indexAt++;
			}
			
			if (indexAfter < this.length)
			{
				Debug.Assert.IsTrue (this.GetCursorPosition (this.elements[indexAfter].id) >= position + length);
				Debug.Assert.IsTrue (this.elements[indexAfter].offset >= length);
				
				this.elements[indexAfter].offset -= length;
			}
			
			if ((list != null) &&
				(list.Count > 0))
			{
				//	Lors du d�placement des curseurs, on a trouv� des curseurs
				//	attach�s au texte. Retourne � l'appelant la description de
				//	ceux-ci (il faudra encore appeler ProcessRemovalCleanup).
				
				removed = new CursorInfo[list.Count];
				list.CopyTo (removed);
			}
			else
			{
				removed = null;
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
			int dstI  = 0;
			
			while (index < this.length)
			{
				Element element = this.elements[index++];
				
				pos           += element.offset;
				element.offset = pos - origin;
				origin         = pos;
				
				destination.InsertElement (dstI++, element);
			}
			
			//	Supprime encore les curseurs que l'on vient de copier vers la
			//	destination :
			
			this.length = count;
		}
		
		
		private int FindElement(Internal.CursorId id)
		{
			Debug.Assert.IsTrue (id.IsValid);

			if (this.cachedCursorId == id)
			{
				if ((this.cachedIndex < this.length) &&
					(this.elements[this.cachedIndex].id == id))
				{
					return this.cachedIndex;
				}
			}
			
			//	Trouve l'�l�ment qui d�crit le curseur sp�cifi� et retourne
			//	son index.
			
			for (int i = 0; i < this.length; i++)
			{
				if (this.elements[i].id == id)
				{
					this.cachedCursorId = id;
					this.cachedIndex = i;
					
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
					//	NB: s'il n'y a aucun �l�ment ou que le premier �l�ment est
					//	d�j� plac� apr�s la position � consid�rer, i = 0 et cela
					//	implique que l'on va retourner -1 :
					
					return i-1;
				}
			}
			
			//	Tous les �l�ments d�crivent des curseurs plac�s avant la position
			//	� consid�rer; on va donc retourner le dernier �l�ment (c'est le
			//	plus proche de la position) :
			
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

			if (this.elements.Length == this.length)
			{
				Element[] oldElements = this.elements;
				Element[] newElements = new Element[this.length+1];

				//	Si l'�l�ment n'est pas ajout� � la fin du tableau, il
				//	faut encore ajuster l'offset de l'�l�ment qui va se
				//	retrouver juste apr�s l'�l�ment ins�r� :

				if (index < this.length)
				{
					Debug.Assert.IsTrue (oldElements[index].offset > element.offset);
					oldElements[index].offset -= element.offset;
				}

				int n1 = index;
				int n2 = this.length - index;

				System.Array.Copy (oldElements, 0, newElements, 0, n1);
				System.Array.Copy (oldElements, n1, newElements, n1+1, n2);

				newElements[index] = element;

				this.elements = newElements;
				this.length   = newElements.Length;
			}
			else
			{
				//	Si l'�l�ment n'est pas ajout� � la fin du tableau, il
				//	faut encore ajuster l'offset de l'�l�ment qui va se
				//	retrouver juste apr�s l'�l�ment ins�r� :

				if (index < this.length)
				{
					Debug.Assert.IsTrue (this.elements[index].offset > element.offset);
					this.elements[index].offset -= element.offset;
				}

				int n1 = index;
				int n2 = this.length - index;

				if (n2 > 0)
				{
					System.Array.Copy (this.elements, n1, this.elements, n1+1, n2);
				}

				this.elements[index] = element;
				this.length++;
			}
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
		
		private bool MoveElement(int fromIndex, int toIndex, out int index)
		{
			//	D�place un �l�ment au sein du tableau et retourne true
			//	si l'�l�ment a r�ellement chang� de position.
			//
			//	Ajuste l'offset des �l�ments; l'�l�ment d�plac� se voit
			//	affecter temporairement un offset nul (ce qui doit �tre
			//	ajust� par l'appelant); si l'�l�ment n'est pas d�place,
			//	son offset n'est pas modifi� non plus.
			
			//	NB: La position de l'�l�ment d'arriv�e (toIndex) est d�finie
			//		par rapport � l'�tat du tableau avant la modification.
			
			//	toIndex = i place l'�l�ment � la position 'i' (i = 0 place
			//	au d�but, i = n place apr�s le dernier �l�ment).
			
			Debug.Assert.IsTrue (this.length > 0);
			Debug.Assert.IsTrue (this.length <= this.elements.Length);
			
			Debug.Assert.IsInBounds (fromIndex, 0, this.length-1);
			Debug.Assert.IsInBounds (toIndex, 0, this.length);
			
			if (fromIndex < toIndex)
			{
				index = toIndex - 1;
				
				//	Si on d�cale simplement apr�s soi-m�me, il n'y a rien �
				//	faire :
				
				if (fromIndex+1 == toIndex)
				{
					return false;
				}
				
				//	D�place l'�l�ment plus loin dans le tableau :
				
				Element element = this.elements[fromIndex];
				
				for (int i = fromIndex; i < toIndex-1; i++)
				{
					this.elements[i] = this.elements[i+1];
				}
				
				//	Ajuste l'offset de l'�l�ment qui se trouvait juste apr�s
				//	celui qui vient d'�tre retir� du tableau :
				
				this.elements[fromIndex].offset += element.offset;
				
				//	L'�l�ment que l'on va ins�rer � nouveau aura un offset
				//	nul par rapport � son pr�d�cesseur, en attendant que
				//	l'appelant lui affecte un offset d�finitif :
				
				element.offset = 0;
				
				//	Termine le d�placement.
				
				this.elements[index] = element;
			}
			else
			{
				index = toIndex;
				
				if (fromIndex == toIndex)
				{
					return false;
				}
				
				//	D�place l'�l�ment plus en avant dans le tableau :
				
				Element element = this.elements[fromIndex];
				
				for (int i = fromIndex; i > toIndex; i--)
				{
					this.elements[i] = this.elements[i-1];
				}
				
				//	Ajuste l'offset de l'�l�ment qui se trouvait juste apr�s
				//	celui qui vient d'�tre retir� du tableau, pour autant qu'il
				//	y en ait eu un :
				
				if (fromIndex+1 < this.length)
				{
					this.elements[fromIndex+1].offset += element.offset;
				}
				
				//	L'�l�ment que l'on va ins�rer � nouveau aura un offset
				//	nul par rapport � son pr�d�cesseur, en attendant que
				//	l'appelant lui affecte un offset d�finitif :
				
				element.offset = 0;
				
				//	Termine le d�placement.
				
				this.elements[toIndex] = element;
			}
			
			return true;
		}
		
		
		
		private struct Element
		{
			public Element(Internal.CursorId id, int offset, CursorAttachment attachment)
			{
				this.id         = id;
				this.offset     = offset;
				this.attachment = attachment;
			}
			
			
			public Internal.CursorId			id;
			public int							offset;
			public CursorAttachment				attachment;
		}
		
		
		public const int						MaxPosition = 10*1000*1000 - 1;
		
		private Element[]						elements;
		private int								length;
		private int								cachedIndex;
		private int								cachedCursorId;
	}
}
