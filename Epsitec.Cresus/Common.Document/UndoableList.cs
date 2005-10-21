using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	public enum UndoableListType
	{
		ObjectsInsideDocument,		// liste d'objets du document, d'une page, d'un calque ou d'un groupe
		ObjectsInsideProperty,		// liste des objets propri�taires d'une propri�t�
		PropertiesInsideDocument,	// liste de propri�t�s ou de styles du document
		PropertiesInsideObject,		// liste des propri�t�s utilis�es par un objet
		StylesInsideAggregate,		// styles utilis�s par un agr�gat
		AggregatesInsideDocument,	// liste des agr�gats du document
		AggregatesInsideObject,		// liste des agr�gats d'un objet
		AggregatesChildrens,		// liste des agr�gats fils
		Guides,						// liste des rep�res
		TextFlows,					// flux de textes
		ObjectsChain,				// cha�ne d'objets pour un flux de textes
		TextStylesInsideDocument,	// styles de texte du document
	}

	/// <summary>
	/// La classe UndoableList impl�mente un ArrayList avec possibilit� de undo/redo.
	/// </summary>
	[System.Serializable()]
	public class UndoableList : System.Collections.IEnumerable, ISerializable
	{
		// Cr�e une nouvelle liste vide.
		public UndoableList(Document document, UndoableListType type)
		{
			this.document = document;
			this.type = type;
			this.arrayList = new System.Collections.ArrayList();
			this.selected = -1;
		}

		protected Document Document
		{
			get { return this.document; }
		}

		protected UndoableListType Type
		{
			get { return this.type; }
		}

		public void Dispose()
		{
			this.arrayList.Clear();
			this.arrayList = null;
		}

		// Vide toute la liste.
		public void Clear()
		{
			if ( this.IsOpletQueueEnable )  // m�morise l'op�ration ?
			{
				int total = this.arrayList.Count;
				for ( int i=0 ; i<total ; i++ )
				{
					this.RemoveAt(0);
				}
			}

			this.arrayList.Clear();
			this.Selected = -1;
		}

		// Nombre d'objets dans la liste.
		public int Count
		{
			get { return this.arrayList.Count; }
		}

		// Indique si la liste contient un objet.
		public bool Contains(object item)
		{
			return this.arrayList.Contains(item);
		}

		// Retourne l'index d'un objet.
		public int IndexOf(object value)
		{
			return this.arrayList.IndexOf(value);
		}

		// Acc�s � un objet quelconque.
		public object this[int index]
		{
			get
			{
				return this.arrayList[index];
			}

			set
			{
				if ( this.IsOpletQueueEnable )  // m�morise l'op�ration ?
				{
					object obj = this.arrayList[index];
					OpletUndoableList operation = new OpletUndoableList(this, OperationType.Change, index, obj);
					this.document.Modifier.OpletQueue.Insert(operation);
					this.document.Notifier.NotifyUndoRedoChanged();
				}

				this.arrayList[index] = value;
			}
		}

		// Ajoute un objet � la fin de la liste.
		public int Add(object value)
		{
			int index = this.arrayList.Count;
			this.Insert(index, value);  // ins�re � la fin
			return index;
		}

		// Ajoute un objet dans la liste.
		public void Insert(int index, object value)
		{
			if ( this.IsOpletQueueEnable )  // m�morise l'op�ration ?
			{
				OpletUndoableList operation = new OpletUndoableList(this, OperationType.Insert, index, value);
				this.document.Modifier.OpletQueue.Insert(operation);
				this.document.Notifier.NotifyUndoRedoChanged();
			}

			this.arrayList.Insert(index, value);
		}

		// Supprime un objet de la liste.
		public void Remove(object value)
		{
			int index = this.arrayList.IndexOf(value);
			System.Diagnostics.Debug.Assert(index != -1);
			this.RemoveAt(index);
		}

		// Supprime un objet de la liste.
		public void RemoveAt(int index)
		{
			if ( this.IsOpletQueueEnable )  // m�morise l'op�ration ?
			{
				object obj = this.arrayList[index];
				OpletUndoableList operation = new OpletUndoableList(this, OperationType.Remove, index, obj);
				this.document.Modifier.OpletQueue.Insert(operation);
				this.document.Notifier.NotifyUndoRedoChanged();
			}

			this.arrayList.RemoveAt(index);
		}

		// Indique l'objet s�lectionn� dans la liste.
		public int Selected
		{
			get
			{
				return this.selected;
			}

			set
			{
				if ( this.selected != value )
				{
					if ( this.IsOpletQueueEnable )  // m�morise l'op�ration ?
					{
						OpletUndoableList operation = new OpletUndoableList(this, OperationType.Selected, this.selected, null);
						this.document.Modifier.OpletQueue.Insert(operation);
						this.document.Notifier.NotifyUndoRedoChanged();
					}

					this.selected = value;
				}
			}
		}

		// Copie toute la liste, avec possibilit� d'annulation.
		public void UndoableCopyTo(UndoableList dst)
		{
			dst.Clear();
			foreach ( object obj in this.arrayList )
			{
				dst.Add(obj);
			}
		}

		// Copie toute la liste, sans possibilit� d'annulation.
		public void CopyTo(UndoableList dst)
		{
			dst.arrayList.Clear();
			foreach ( object obj in this.arrayList )
			{
				dst.arrayList.Add(obj);
			}
		}

		// Indique s'il faut m�moriser l'op�ration.
		protected bool IsOpletQueueEnable
		{
			get
			{
				if ( this.document.Modifier == null )  return false;
				return this.document.Modifier.OpletQueueEnable;
			}
		}

		// Retourne l'�num�rateur, pour pouvoir utiliser foreach.
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.arrayList.GetEnumerator();
		}

		// D�fait une op�ration dans une UndoableList.
		// Une prochaine ex�cution de UndoRedoOperation refera l'op�ration.
		protected static void UndoRedoOperation(OpletUndoableList operation)
		{
			Document document = operation.List.document;
			UndoableListType listType = operation.List.type;
			System.Collections.ArrayList arrayList = operation.List.arrayList;
			int index = operation.Index;
			//?System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", operation.Type.ToString(), operation.Object.ToString()));

			int incSelect = 0;
			if ( operation.Type == OperationType.Insert )
			{
				arrayList.RemoveAt(index);
				operation.Type = OperationType.Remove;
				incSelect = -1;
			}
			else if ( operation.Type == OperationType.Remove )
			{
				arrayList.Insert(index, operation.Object);
				operation.Type = OperationType.Insert;
				incSelect = 1;
			}
			else if ( operation.Type == OperationType.Change )
			{
				object temp = arrayList[index];
				arrayList[index] = operation.Object;
				operation.Object = temp;
			}
			else if ( operation.Type == OperationType.Selected )
			{
				int temp = operation.Index;
				operation.Index = operation.List.selected;
				operation.List.selected = temp;
			}

			if ( listType == UndoableListType.ObjectsInsideDocument &&
				 operation.Object is Objects.Abstract )
			{
				if ( operation.Object is Objects.Page )
				{
					document.Notifier.NotifyArea();
					document.Notifier.NotifyPagesChanged();
					document.Notifier.NotifySelectionChanged();
				}
				else if ( operation.Object is Objects.Layer )
				{
					document.Notifier.NotifyArea();
					document.Notifier.NotifyLayersChanged();
					document.Notifier.NotifySelectionChanged();
				}
				else
				{
					Objects.Abstract obj = operation.Object as Objects.Abstract;
					if ( obj.IsSelected )
					{
						//document.Modifier.DirtyCounters();
						document.Modifier.TotalSelected += incSelect;
					}
					document.Notifier.NotifyArea(obj.BoundingBox);
					document.Notifier.NotifySelectionChanged();
				}
			}

			if ( operation.Object is Properties.Abstract )
			{
				Properties.Abstract prop = operation.Object as Properties.Abstract;
				if ( prop.IsStyle )
				{
					document.Notifier.NotifyStyleChanged();
				}
			}

			if ( listType == UndoableListType.PropertiesInsideObject &&
				 operation.Type == OperationType.Change )
			{
				Properties.Abstract prop1 = operation.Object as Properties.Abstract;
				Properties.Abstract prop2 = arrayList[index] as Properties.Abstract;
				
				if ( prop1.Owners.Count > 0 )
				{
					Objects.Abstract obj = prop1.Owners[prop1.Owners.Count-1] as Objects.Abstract;
					document.Notifier.NotifyArea(obj.BoundingBox);
				}

				if ( prop2.Owners.Count > 0 )
				{
					Objects.Abstract obj = prop2.Owners[prop2.Owners.Count-1] as Objects.Abstract;
					document.Notifier.NotifyArea(obj.BoundingBox);
				}
			}

			if ( listType == UndoableListType.Guides )
			{
				document.Notifier.NotifyGuidesChanged();
			}
		}


		protected enum OperationType
		{
			Insert,		// ajout d'un nouvel objet dans une UndoableList
			Remove,		// suppression d'un objet dans une UndoableList
			Change,		// changement d'un objet dans une UndoableList
			Selected,	// s�lection d'un objet dans une UndoableList
		}

		protected class OpletUndoableList : AbstractOplet
		{
			public OpletUndoableList(UndoableList list, OperationType type, int index, object obj)
			{
				this.list  = list;
				this.type  = type;
				this.index = index;
				this.obj   = obj;
			}

			public UndoableList List
			{
				get { return this.list; }
			}

			public OperationType Type
			{
				get { return this.type; }
				set { this.type = value; }
			}

			public int Index
			{
				get { return this.index; }
				set { this.index = value; }
			}

			public object Object
			{
				get { return this.obj; }
				set { this.obj = value; }
			}

			public override IOplet Undo()
			{
				UndoableList.UndoRedoOperation(this);
				return this;
			}

			public override IOplet Redo()
			{
				UndoableList.UndoRedoOperation(this);
				return this;
			}

			protected UndoableList			list;
			protected OperationType			type;
			protected int					index;
			protected object				obj;
		}


		#region Serialization
		// S�rialise la liste.
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Type", this.type);
			info.AddValue("List", this.arrayList);
			info.AddValue("Selected", this.selected);
		}

		// Constructeur qui d�s�rialise la liste.
		protected UndoableList(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;
			this.type = (UndoableListType) info.GetValue("Type", typeof(UndoableListType));
			this.arrayList = (System.Collections.ArrayList) info.GetValue("List", typeof(System.Collections.ArrayList));
			this.selected = info.GetInt32("Selected");
		}
		#endregion

		
		protected Document						document;
		protected UndoableListType				type;
		protected System.Collections.ArrayList	arrayList;
		protected int							selected;
	}
}
