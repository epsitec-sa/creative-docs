using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe UndoList implémente un ArrayList avec possibilité de undo/redo.
	/// </summary>
	public class UndoList : System.Collections.ArrayList
	{
		public override int Add(object value)
		{
			//	Ajoute un objet à la fin de la liste, en mémorisant éventuellement
			//	l'opération dans UndoList.Operations.
			return this.Add(value, false);
		}

		public int Add(object value, bool selectAfterCreate)
		{
			int index = this.Count;
			this.Insert(index, value, selectAfterCreate);  // insère à la fin
			return index;
		}

		public override void Insert(int index, object value)
		{
			//	Ajoute un objet dans la liste, en mémorisant éventuellement
			//	l'opération dans UndoList.Operations.
			this.Insert(index, value, false);
		}

		public void Insert(int index, object value, bool selectAfterCreate)
		{
			if ( UndoList.Operations != null )  // mémorise l'opération ?
			{
				System.Diagnostics.Debug.Assert(UndoList.Beginning);
				OpletUndoList operation = new OpletUndoList(this, OperationType.Insert, selectAfterCreate, index, value);
				UndoList.Operations.Add(operation);
			}

			base.Insert(index, value);
		}

		public override void RemoveAt(int index)
		{
			//	Supprime un objet de la liste, en mémorisant éventuellement
			//	l'opération dans UndoList.Operations.
			if ( UndoList.Operations != null )  // mémorise l'opération ?
			{
				System.Diagnostics.Debug.Assert(UndoList.Beginning);
				object obj = this[index];
				OpletUndoList operation = new OpletUndoList(this, OperationType.Remove, false, index, obj);
				UndoList.Operations.Add(operation);
			}

			base.RemoveAt(index);
		}

		public void WillBeChanged(object value)
		{
			//	Indique qu'un objet de la liste sera changé, en mémorisant éventuellement
			//	un copie de l'objet actuel dans UndoList.Operations.
			int index = this.IndexOf(value);
			this.WillBeChanged(index);
		}

		public void WillBeChanged(int index)
		{
			//	Indique qu'un objet de la liste sera changé, en mémorisant éventuellement
			//	un copie de l'objet actuel dans UndoList.Operations.
			if ( UndoList.Operations != null )  // mémorise l'opération ?
			{
				System.Diagnostics.Debug.Assert(UndoList.Beginning);

				AbstractObject obj = this[index] as AbstractObject;
				if ( obj != null )
				{
					AbstractObject newObject = null;
					obj.DuplicateObject(ref newObject);  // effectue une copie complète de l'objet

					OpletUndoList operation = new OpletUndoList(this, OperationType.Modify, false, index, newObject);
					UndoList.Operations.Add(operation);
				}

				AbstractProperty prop = this[index] as AbstractProperty;
				if ( prop != null )
				{
					AbstractProperty newProp = AbstractProperty.NewProperty(prop.Type);
					prop.CopyTo(newProp);  // effectue une copie complète de la proptiété

					OpletUndoList operation = new OpletUndoList(this, OperationType.Modify, false, index, newProp);
					UndoList.Operations.Add(operation);
				}
			}
		}

		public static void UndoRedoOperation(OpletUndoList operation)
		{
			//	Défait une opération dans une UndoList.
			//	Une prochaine exécution de UndoRedoOperation refera l'opération.
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
				if ( undoObj != null )
				{
					undoObj.UndoStamp = true;

					if ( operation.Created )
					{
						undoObj.Select();
						UndoList.SelectAfterCreate = true;
					}
				}
			}
			else if ( operation.Type == OperationType.Modify )
			{
				if ( objects[index] is AbstractObject )
				{
					AbstractObject currObj = objects[index] as AbstractObject;
					AbstractObject undoObj = operation.Object as AbstractObject;

					AbstractObject tempObj = null;
					currObj.DuplicateObject(ref tempObj);

					currObj.CloneObject(undoObj);  // reprend les caractéristiques de l'objet dans la liste
					undoObj.CloneObject(tempObj);

					currObj.UndoStamp = true;
				}

				if ( objects[index] is AbstractProperty )
				{
					AbstractProperty currProp = objects[index] as AbstractProperty;
					AbstractProperty undoProp = operation.Object as AbstractProperty;

					AbstractProperty tempProp = AbstractProperty.NewProperty(currProp.Type);
					currProp.CopyTo(tempProp);

					undoProp.CopyTo(currProp);
					tempProp.CopyTo(undoProp);
				}
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


		//	Si cette liste est définie, toutes les opérations dans toutes les
		//	UndoList y sont mémorisées (instances de la classe OpletUndoList).
		public static System.Collections.ArrayList	Operations = null;
		public static bool							Beginning = false;
		public static bool							SelectAfterCreate = false;
	}


	//	Oplet mémorisant divers paramètres.
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


	//	Oplet créé toujours au début.
	public class OpletBeginning : AbstractOplet
	{
		public OpletBeginning(Widgets.Drawer drawer, int styleID, string operation)
		{
			this.host = drawer;
			this.styleID = styleID;
			this.operation = operation;

			if ( this.styleID != 0 && this.operation != "StyleChanged" )
			{
				AbstractProperty style = this.host.IconObjects.StylesCollection.GetProperty(this.styleID-1);
				this.property = AbstractProperty.NewProperty(style.Type);
				style.CopyTo(this.property);
			}
		}

		public void SwapStyle()
		{
			if ( this.styleID != 0 && this.operation != "StyleChanged" )
			{
				AbstractProperty style = this.host.IconObjects.StylesCollection.GetProperty(this.styleID-1);
				AbstractProperty temp = AbstractProperty.NewProperty(style.Type);
				style.CopyTo(temp);
				this.property.CopyTo(style);
				temp.CopyTo(this.property);
			}
		}

		public int StyleID
		{
			get { return this.styleID; }
		}

		public override IOplet Undo()
		{
			this.SwapStyle();
			this.host.UndoFinalWork(this.styleID);  // lorsque tout est fini
			return this;
		}

		public override IOplet Redo()
		{
			this.host.UndoInitialWork();  // au tout début
			return this;
		}

		protected Widgets.Drawer		host;
		protected int					styleID;
		protected AbstractProperty		property;
		protected string				operation;
	}


	//	Oplet créé toujours à la fin.
	public class OpletEnding : AbstractOplet
	{
		public OpletEnding(Widgets.Drawer drawer, OpletBeginning beginning)
		{
			this.host = drawer;
			this.beginning = beginning;
		}

		public override IOplet Undo()
		{
			this.host.UndoInitialWork();  // au tout début
			return this;
		}

		public override IOplet Redo()
		{
			this.beginning.SwapStyle();
			this.host.UndoFinalWork(this.beginning.StyleID);  // lorsque tout est fini
			return this;
		}

		protected Widgets.Drawer		host;
		protected OpletBeginning		beginning;
	}
}
