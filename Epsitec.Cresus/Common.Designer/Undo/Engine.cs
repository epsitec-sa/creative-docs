using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Undo
{
	/// <summary>
	/// Moteur général simple pour un undo/redo basé sur des photographies complètes
	/// des états auxquels on désire revenir.
	/// </summary>
	public class Engine
	{
		public Engine()
		{
			this.snapshots = new List<Shapshot>();
			this.count = 0;
			this.index = 0;
		}


		public void Flush()
		{
			//	Les commandes annuler/refaire ne seront plus possibles.
			this.snapshots.Clear();
			this.count = 0;
			this.index = 0;
		}

		public Shapshot Undo(Shapshot current)
		{
			//	Retourne la dernière photographie à annuler.
			System.Diagnostics.Debug.Assert(this.IsUndoEnable);

			if (this.snapshots.Count == this.index)
			{
				this.snapshots.Add(current);  // ajoute l'état actuel à la fin de la liste
			}

			return this.snapshots[--this.index];
		}

		public Shapshot Redo()
		{
			//	Retourne la photographie à refaire.
			System.Diagnostics.Debug.Assert(this.IsRedoEnable);

			return this.snapshots[++this.index];
		}

		public Shapshot Goto(int index, Shapshot current)
		{
			//	Retourne la photographie à laquelle revenir, selon le menu.
			if (this.snapshots.Count == this.index)
			{
				this.snapshots.Add(current);  // ajoute l'état actuel à la fin de la liste
			}

			if (index >= this.index)
			{
				index++;
			}

			this.index = index;
			return this.snapshots[index];
		}


		public bool IsUndoEnable
		{
			//	Retourne true si la commande "Undo" doit être active.
			get
			{
				return this.index > 0;
			}
		}

		public bool IsRedoEnable
		{
			//	Retourne true si la commande "Redo" doit être active.
			get
			{
				return this.index < this.count;
			}
		}

		public bool IsUndoRedoListEnable
		{
			//	Retourne true si la commande "UndoRedoList" pour le menu doit être active.
			get
			{
				return this.count > 0;
			}
		}

		public bool IsSameLastShapshot(string snapshotName)
		{
			//	Indique si la dernière photographie mémorisée était du même type.
			return (this.count > 0 && this.index > 0 && this.snapshots[this.index-1].Name == snapshotName);
		}

		public void Memorize(Shapshot snapshot, bool merge)
		{
			//	Mémorise l'état actuel, avant d'effectuer une modification.
			//	Si merge = true et que la dernière photographie avait le même nom, on conserve le dernier
			//	état mémorisé.
			if (merge && this.IsSameLastShapshot(snapshot.Name))
			{
				// Conserve le dernier état mémorisé.
			}
			else
			{
				this.Memorize(snapshot);
			}
		}

		public void Memorize(Shapshot snapshot)
		{
			//	Mémorise un état donné, avant d'effectuer une modification.
			while (this.snapshots.Count > this.index)
			{
				this.snapshots.RemoveAt(this.snapshots.Count-1);  // supprime la dernière photographie
			}

			this.snapshots.Add(snapshot);
			this.index = this.snapshots.Count;
			this.count = this.index;
		}


		public VMenu CreateMenu(Support.EventHandler<MessageEventArgs> message)
		{
			//	Crée le menu undo/redo. Même si le nombre de photographies mémorisées est grand, le menu
			//	présente toujours un nombre raisonnable de lignes. La première photographie (undo) et la
			//	dernière (redo) sont toujours présentes.
			int undoLength = this.index;
			int redoLength = this.count-this.index;
			int all = this.count;
			int total = System.Math.Min(all, 20);
			int start = this.index;
			start -= total/2;
			if (start < 0)  start = 0;
			start += total-1;
			if (start > all-1)  start = all-1;

			List<MenuItem> list = new List<MenuItem>();

			//	Met éventuellement la dernière photographie à refaire.
			if (start < all-1)
			{
				string snapshot = this.snapshots[all-1].Name;
				snapshot = Misc.Italic(snapshot);
				list.Add(this.CreateItem(message, 0, all, snapshot, all-1));

				if (start < all-2)
				{
					list.Add(new MenuSeparator());
				}
			}

			//	Met les photographies à refaire puis à celles à annuler.
			for (int i=start; i>start-total; i--)
			{
				if (i >= undoLength)  // redo ?
				{
					string snapshot = this.snapshots[i].Name;
					snapshot = Misc.Italic(snapshot);
					list.Add(this.CreateItem(message, 0, i+1, snapshot, i));

					if (i == undoLength && undoLength != 0)
					{
						list.Add(new MenuSeparator());
					}
				}
				else	// undo ?
				{
					string snapshot = this.snapshots[i].Name;
					int active = 1;
					if (i == undoLength-1)
					{
						active = 2;
						snapshot = Misc.Bold(snapshot);
					}
					list.Add(this.CreateItem(message, active, i+1, snapshot, i));
				}
			}

			//	Met éventuellement la dernière photographie à annuler.
			if (start-total >= 0)
			{
				if (start-total > 0)
				{
					list.Add(new MenuSeparator());
				}

				string snapshot = this.snapshots[0].Name;
				list.Add(this.CreateItem(message, 1, 1, snapshot, 0));
			}

			//	Génère le menu à l'envers, c'est-à-dire la première photographie au
			//	début du menu (en haut).
			VMenu menu = new VMenu();
			menu.Items.AddRange (list);
			menu.AdjustSize();
			return menu;
		}

		protected MenuItem CreateItem(Support.EventHandler<MessageEventArgs> message, int active, int rank, string snapshot, int todo)
		{
			//	Crée une case du menu des photographies à refaire/annuler.
			string icon = "";
			if (active == 1)  icon = Misc.Icon("ActiveNo");
			if (active == 2)  icon = Misc.Icon("ActiveCurrent");

			string name = string.Format("{0}: {1}", rank.ToString(), snapshot);
			string cmd = "UndoRedoListDo";
			Misc.CreateStructuredCommandWithName(cmd);

			MenuItem item = new MenuItem(cmd, icon, name, "", todo.ToString());

			if (message != null)
			{
				item.Pressed += message;
			}

			return item;
		}


		protected List<Shapshot>			snapshots;
		protected int						count;
		protected int						index;
	}
}
