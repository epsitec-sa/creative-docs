using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe UndoList impl�mente un ArrayList avec possibilit� de undo/redo.
	/// </summary>
	public class UndoList : System.Collections.ArrayList
	{
		// Ajoute un objet � la fin de la liste, en m�morisant �ventuellement
		// l'op�ration dans UndoList.Operations.
		public override int Add(object value)
		{
			return this.Add(value, false);
		}

		public int Add(object value, bool selectAfterCreate)
		{
			int index = this.Count;
			this.Insert(index, value, selectAfterCreate);  // ins�re � la fin
			return index;
		}

		// Ajoute un objet dans la liste, en m�morisant �ventuellement
		// l'op�ration dans UndoList.Operations.
		public override void Insert(int index, object value)
		{
			this.Insert(index, value, false);
		}

		public void Insert(int index, object value, bool selectAfterCreate)
		{
			if ( UndoList.Operations != null )  // m�morise l'op�ration ?
			{
				System.Diagnostics.Debug.Assert(UndoList.Beginning);
				OpletUndoList operation = new OpletUndoList(this, OperationType.Insert, selectAfterCreate, index, value);
				UndoList.Operations.Add(operation);
			}

			base.Insert(index, value);
		}

		// Supprime un objet de la liste, en m�morisant �ventuellement
		// l'op�ration dans UndoList.Operations.
		public override void RemoveAt(int index)
		{
			if ( UndoList.Operations != null )  // m�morise l'op�ration ?
			{
				System.Diagnostics.Debug.Assert(UndoList.Beginning);
				object obj = this[index];
				OpletUndoList operation = new OpletUndoList(this, OperationType.Remove, false, index, obj);
				UndoList.Operations.Add(operation);
			}

			base.RemoveAt(index);
		}

		// Indique qu'un objet de la liste sera chang�, en m�morisant �ventuellement
		// un copie de l'objet actuel dans UndoList.Operations.
		public void WillBeChanged(object value)
		{
			int index = this.IndexOf(value);
			this.WillBeChanged(index);
		}

		// Indique qu'un objet de la liste sera chang�, en m�morisant �ventuellement
		// un copie de l'objet actuel dans UndoList.Operations.
		public void WillBeChanged(int index)
		{
			if ( UndoList.Operations != null )  // m�morise l'op�ration ?
			{
				System.Diagnostics.Debug.Assert(UndoList.Beginning);
				AbstractObject obj = this[index] as AbstractObject;

				AbstractObject newObject = null;
				obj.DuplicateObject(ref newObject);  // effectue une copie compl�te de l'objet

				OpletUndoList operation = new OpletUndoList(this, OperationType.Modify, false, index, newObject);
				UndoList.Operations.Add(operation);
			}
		}

		// D�fait une op�ration dans une UndoList.
		// Une prochaine ex�cution de UndoRedoOperation refera l'op�ration.
		public static void UndoRedoOperation(OpletUndoList operation)
		{
			System.Collections.ArrayList objects = operation.List;
			int index = operation.Index;

			if ( operation.Type == OperationType.Insert )
			{
				objects.RemoveAt(index);
				operation.Type = OperationType.Remove;
			}
			else if ( operation.Type == OperationType.Remove )
			{
				objects.Insert(index, operation.Object);
				operation.Type = OperationType.Insert;

				AbstractObject undoObj = operation.Object as AbstractObject;
				undoObj.UndoStamp = true;

				if ( operation.Created )
				{
					undoObj.Select();
					UndoList.SelectAfterCreate = true;
				}
			}
			else if ( operation.Type == OperationType.Modify )
			{
				AbstractObject currObj = objects[index] as AbstractObject;
				AbstractObject undoObj = operation.Object as AbstractObject;

				AbstractObject tempObj = null;
				currObj.DuplicateObject(ref tempObj);

				currObj.CloneObject(undoObj);  // reprend les caract�ristiques de l'objet dans la liste
				undoObj.CloneObject(tempObj);

				currObj.UndoStamp = true;
			}
		}


		public enum OperationType
		{
			Insert,		// ajout d'un nouvel objet dans une UndoList
			Remove,		// suppression d'un objet dans une UndoList
			Modify,		// modification d'un objet dans une UndoList
		}

		public class OpletUndoList : AbstractOplet
		{
			public OpletUndoList(UndoList list, OperationType type, bool created, int index, object obj)
			{
				this.list    = list;
				this.type    = type;
				this.created = created;
				this.index   = index;
				this.obj     = obj;
			}

			public UndoList List
			{
				get { return this.list; }
			}

			public OperationType Type
			{
				get { return this.type; }
				set { this.type = value; }
			}

			public bool Created
			{
				get { return this.created; }
			}

			public int Index
			{
				get { return this.index; }
			}

			public object Object
			{
				get { return this.obj; }
			}

			public override IOplet Undo()
			{
				UndoList.UndoRedoOperation(this);
				return this;
			}

			public override IOplet Redo()
			{
				UndoList.UndoRedoOperation(this);
				return this;
			}

			protected UndoList				list;
			protected OperationType			type;
			protected bool					created;
			protected int					index;
			protected object				obj;
		}


		// Si cette liste est d�finie, toutes les op�rations dans toutes les
		// UndoList y sont m�moris�es (instances de la classe OpletUndoList).
		public static System.Collections.ArrayList	Operations = null;
		public static bool							Beginning = false;
		public static bool							SelectAfterCreate = false;
	}


	// Oplet m�morisant divers param�tres.
	public class OpletMisc : AbstractOplet
	{
		public OpletMisc(Widgets.Drawer drawer)
		{
			this.host = drawer;

			this.selectedTool = this.host.SelectedTool;

			this.globalModifier = new GlobalModifierData();
			this.host.GlobalModifier.Data.CopyTo(this.globalModifier);

			this.pattern = this.host.IconObjects.CurrentPattern;
			this.page    = this.host.IconObjects.CurrentPage;
			this.layer   = this.host.IconObjects.CurrentLayer;

			this.roots = new System.Collections.ArrayList();
			OpletMisc.ArrayListCopy(this.host.IconObjects.Roots, this.roots);

			this.editObject = this.host.EditObject;
		}

		protected void Do()
		{
			this.host.SwapTool(ref this.selectedTool);

			GlobalModifierData tempMod = new GlobalModifierData();
			this.host.GlobalModifier.Data.CopyTo(tempMod);
			this.globalModifier.CopyTo(this.host.GlobalModifier.Data);
			tempMod.CopyTo(this.globalModifier);

			this.host.IconObjects.SwapPatternPageLayer(ref this.pattern, ref this.page, ref this.layer);

			System.Collections.ArrayList tempRoots = new System.Collections.ArrayList();
			OpletMisc.ArrayListCopy(this.host.IconObjects.Roots, tempRoots);
			OpletMisc.ArrayListCopy(this.roots, this.host.IconObjects.Roots);
			OpletMisc.ArrayListCopy(tempRoots, this.roots);

			this.host.SwapEditObject(ref this.editObject);
		}

		public override IOplet Undo()
		{
			this.Do();
			return this;
		}

		public override IOplet Redo()
		{
			this.Do();
			return this;
		}

		static protected void ArrayListCopy(System.Collections.ArrayList src,
											System.Collections.ArrayList dst)
		{
			dst.Clear();
			foreach( object obj in src )
			{
				dst.Add(obj);
			}
		}

		protected Widgets.Drawer				host;
		protected string						selectedTool;
		protected int							pattern;
		protected int							page;
		protected int							layer;
		protected GlobalModifierData			globalModifier;
		protected System.Collections.ArrayList	roots;
		protected AbstractObject				editObject;
	}


	// Oplet cr�� toujours au d�but.
	public class OpletBeginning : AbstractOplet
	{
		public OpletBeginning(Widgets.Drawer drawer)
		{
			this.host = drawer;
		}

		public override IOplet Undo()
		{
			this.host.UndoFinalWork();  // lorsque tout est fini
			return this;
		}

		public override IOplet Redo()
		{
			this.host.UndoInitialWork();  // au tout d�but
			return this;
		}

		protected Widgets.Drawer		host;
	}


	// Oplet cr�� toujours � la fin.
	public class OpletEnding : AbstractOplet
	{
		public OpletEnding(Widgets.Drawer drawer)
		{
			this.host = drawer;
		}

		public override IOplet Undo()
		{
			this.host.UndoInitialWork();  // au tout d�but
			return this;
		}

		public override IOplet Redo()
		{
			this.host.UndoFinalWork();  // lorsque tout est fini
			return this;
		}

		protected Widgets.Drawer		host;
	}
}
