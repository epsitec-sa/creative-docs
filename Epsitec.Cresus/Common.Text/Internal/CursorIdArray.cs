//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La structure CursorIdArray stocke les CursorIds associés à un
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
			//	Insère un élément pour représenter le curseur à la position
			//	donnée.
			
			Debug.Assert.IsFalse (this.Contains (id));
			Debug.Assert.IsInBounds (position, 0, 1000000-1);
			
			//	Ici, il n'y a aucun moyen de savoir si la position spécifiée est
			//	valide par rapport au TextChunk, car il n'y a pas de lien direct
			//	avec le TextChunk considéré.
			
			int offset;
			int index = this.FindElementAtPosition (position, out offset);
			
			Debug.Assert.IsInBounds (index, 0, this.length);
			
			this.InsertElement (index, new Element (id, offset));
		}
		
		public void Move(Internal.CursorId id, int position)
		{
			//	Déplace le curseur spécifié à la nouvelle position donnée.
			
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
				//	Pas besoin de déplacer l'élément dans le tableau, car il va
				//	occuper la même place.
				
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
				
				//	L'élément a été déplacé dans le tableau. Il faut ajuster l'offset
				//	de l'élément courant et de l'élément suivant (s'il y en a un) :
				
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
			//	Décale les curseurs en fonction d'une insertion à la position
			//	spécifiée. Ceci affecte en fait le curseur placé immédiatement
			//	après le point d'insertion.
			
			int index = this.FindElementBeforePosition (position) + 1;
			
			if (index < this.length)
			{
				this.elements[index].offset += length;
			}
		}
		
		public void ProcessRemoval(int position, int length)
		{
			//	Décale les curseurs en fonction d'une suppression depuis la
			//	position indiquée.
			
			//	Si des curseurs se trouvent dans la tranche supprimée, ils
			//	sont déplacés au début de la tranche.
			
			int index_before = this.FindElementBeforePosition (position);
			int index_after  = this.FindElementBeforePosition (position + length) + 1;
			
			int pos_at   = this.FindElementPosition (index_before);
			int index_at = index_before + 1;
			
			//	S'il y a des curseurs dans la tranche supprimée, on les déplace
			//	un à un :
			
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
			//	Migre des curseurs situés après la position indiquée vers
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
			
			//	Trouve l'élément qui décrit le curseur spécifié et retourne
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
			//	Trouve l'élément qui correspond à la position indiquée et
			//	retourne son index et la distance (delta) par rapport au
			//	précédent.
			
			//	L'index de l'élément retourné correspond en fait au point
			//	d'insertion pour la position spécifiée. S'il y a déjà des
			//	éléments à cette position, on va retourner l'index après
			//	ces éléments-là.
			
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
			//	Trouve l'élément qui précède la position indiquée et
			//	retourne son index. S'il n'y a aucun élément avant la
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
			//	Insère un élément à l'endroit indiqué dans le tableau.
			//	index = 0 insère au début; index = n insère à la fin.
			//
			//	Ajuste l'offset de l'élément suivant.
			
			if (this.elements == null)
			{
				this.elements = new Element[0];
			}
			
			Debug.Assert.IsInBounds (this.length, 0, this.elements.Length);
			Debug.Assert.IsInBounds (index, 0, this.length);
			
			Element[] old_elements = this.elements;
			Element[] new_elements = new Element[this.length+1];
			
			//	Si l'élément n'est pas ajouté à la fin du tableau, il
			//	faut encore ajuster l'offset de l'élément qui va se
			//	retrouver juste après l'élément inséré :
			
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
			//	Supprime un élément du tableau.
			//
			//	Ajuste l'offset de l'élément suivant.
			
			Debug.Assert.IsInBounds (this.length, 1, this.elements.Length);
			Debug.Assert.IsInBounds (index, 0, this.length-1);
			
			int offset = this.elements[index].offset;
			int last   = this.elements.Length-1;
			
			for (int i = index; i < last; i++)
			{
				this.elements[i] = this.elements[i+1];
			}
			
			this.length--;
			
			//	S'il y avait un élément après celui qui vient d'être
			//	supprimé, il faut encore ajuster son offset :
			
			if (index < this.length)
			{
				this.elements[index].offset += offset;
			}
		}
		
		private bool MoveElement(int from_index, int to_index, out int index)
		{
			//	Déplace un élément au sein du tableau et retourne true
			//	si l'élément a réellement changé de position.
			//
			//	Ajuste l'offset des éléments; l'élément déplacé se voit
			//	affecter temporairement un offset nul (ce qui doit être
			//	ajusté par l'appelant); si l'élément n'est pas déplace,
			//	son offset n'est pas modifié non plus.
			
			//	NB: La position de l'élément d'arrivée (to_index) est définie
			//		par rapport à l'état du tableau avant la modification.
			
			//	to_index = i place l'élément à la position 'i' (i = 0 place
			//	au début, i = n place après le dernier élément).
			
			Debug.Assert.IsTrue (this.length > 0);
			Debug.Assert.IsTrue (this.length <= this.elements.Length);
			
			Debug.Assert.IsInBounds (from_index, 0, this.length-1);
			Debug.Assert.IsInBounds (to_index, 0, this.length);
			
			if (from_index < to_index)
			{
				index = to_index - 1;
				
				//	Si on décale simplement après soi-même, il n'y a rien à
				//	faire :
				
				if (from_index+1 == to_index)
				{
					return false;
				}
				
				//	Déplace l'élément plus loin dans le tableau :
				
				Element element = this.elements[from_index];
				
				for (int i = from_index; i < to_index-1; i++)
				{
					this.elements[i] = this.elements[i+1];
				}
				
				//	Ajuste l'offset de l'élément qui se trouvait juste après
				//	celui qui vient d'être retiré du tableau :
				
				this.elements[from_index].offset += element.offset;
				
				//	L'élément que l'on va insérer à nouveau aura un offset
				//	nul par rapport à son prédécesseur, en attendant que
				//	l'appelant lui affecte un offset définitif :
				
				element.offset = 0;
				
				//	Termine le déplacement.
				
				this.elements[index] = element;
			}
			else
			{
				index = to_index;
				
				if (from_index == to_index)
				{
					return false;
				}
				
				//	Déplace l'élément plus en avant dans le tableau :
				
				Element element = this.elements[from_index];
				
				for (int i = from_index; i > to_index; i--)
				{
					this.elements[i] = this.elements[i-1];
				}
				
				//	Ajuste l'offset de l'élément qui se trouvait juste après
				//	celui qui vient d'être retiré du tableau, pour autant qu'il
				//	y en ait eu un :
				
				if (from_index+1 < this.length)
				{
					this.elements[from_index+1].offset += element.offset;
				}
				
				//	L'élément que l'on va insérer à nouveau aura un offset
				//	nul par rapport à son prédécesseur, en attendant que
				//	l'appelant lui affecte un offset définitif :
				
				element.offset = 0;
				
				//	Termine le déplacement.
				
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
