//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public class UndoGroup
	{
		public UndoGroup()
		{
			this.items = new List<UndoItem> ();
		}


		public string							Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		public IViewState						BeforeViewState
		{
			get
			{
				return this.beforeViewState;
			}
			set
			{
				this.beforeViewState = value;
			}
		}

		public IViewState						AfterViewState
		{
			get
			{
				return this.afterViewState;
			}
			set
			{
				this.afterViewState = value;
			}
		}


		public int								Size
		{
			get
			{
				return this.items.Count;
			}
		}

		public void Push(UndoItem item)
		{
			this.items.Add (item);
		}

		public void Undo()
		{
			for (int i=this.items.Count-1; i>=0; i--)
			{
				this.Swap (i);
			}
		}

		public void Redo()
		{
			for (int i=0; i<this.items.Count; i++)
			{
				this.Swap (i);
			}
		}

		private void Swap(int index)
		{
			//	Annule une action et remplace les informations qui ont permis de la
			//	faire par les informations pour la défaire.
			var item = this.items[index];
			this.items[index] = item.undoOperation (item.undoData);
		}


		private readonly List<UndoItem>			items;

		private string							description;
		private IViewState						beforeViewState;
		private IViewState						afterViewState;
	}
}
