using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Common.Document
{
    public enum UndoableListType
    {
        ObjectsInsideDocument, // liste d'objets du document, d'une page, d'un calque ou d'un groupe
        ObjectsInsideProperty, // liste des objets propriétaires d'une propriété
        PropertiesInsideDocument, // liste de propriétés ou de styles du document
        PropertiesInsideObject, // liste des propriétés utilisées par un objet
        StylesInsideAggregate, // styles utilisés par un agrégat
        AggregatesInsideDocument, // liste des agrégats du document
        AggregatesInsideObject, // liste des agrégats d'un objet
        AggregatesChildren, // liste des agrégats fils
        Guides, // liste des repères
        TextFlows, // flux de textes
        ObjectsChain, // chaîne d'objets pour un flux de textes
        TextStylesInsideDocument, // styles de texte du document
        SelectedSegments, // segments sélectionnés pour le modeleur
    }

    public class UndoableList : AbstractUndoableList<object>
    {
        public UndoableList(Document document, UndoableListType type)
            : base(document, type) { }
    }

    public abstract class AbstractUndoableList<T> : System.Collections.IEnumerable
        where T : class?
    {
        public AbstractUndoableList(Document document, UndoableListType type)
        {
            //	Crée une nouvelle liste vide.
            this.document = document;
            this.type = type;
            this.objectList = new();
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
            this.objectList.Clear();
            this.objectList = null;
        }

        public void Clear()
        {
            //	Vide toute la liste.
            if (this.IsOpletQueueEnable) // mémorise l'opération ?
            {
                int total = this.objectList.Count;
                for (int i = 0; i < total; i++)
                {
                    this.RemoveAt(0);
                }
            }

            this.objectList.Clear();
            this.Selected = -1;
        }

        public int Count
        {
            //	Nombre d'objets dans la liste.
            get { return this.objectList.Count; }
        }

        public bool Contains(T item)
        {
            //	Indique si la liste contient un objet.
            return this.objectList.Contains(item);
        }

        public int IndexOf(T value)
        {
            //	Retourne l'index d'un objet.
            return this.objectList.IndexOf(value);
        }

        public T this[int index]
        {
            //	Accès à un objet quelconque.
            get { return this.objectList[index]; }
            set
            {
                if (this.IsOpletQueueEnable) // mémorise l'opération ?
                {
                    T obj = this.objectList[index];
                    OpletUndoableList operation = new OpletUndoableList(
                        this,
                        OperationType.Change,
                        index,
                        obj
                    );
                    this.document.Modifier.OpletQueue.Insert(operation);
                    this.document.Notifier.NotifyUndoRedoChanged();
                }

                this.objectList[index] = value;
            }
        }

        public int Add(T value)
        {
            //	Ajoute un objet à la fin de la liste.
            int index = this.objectList.Count;
            this.Insert(index, value); // insère à la fin
            return index;
        }

        public void Insert(int index, T value)
        {
            //	Ajoute un objet dans la liste.
            if (this.IsOpletQueueEnable) // mémorise l'opération ?
            {
                OpletUndoableList operation = new OpletUndoableList(
                    this,
                    OperationType.Insert,
                    index,
                    value
                );
                this.document.Modifier.OpletQueue.Insert(operation);
                this.document.Notifier.NotifyUndoRedoChanged();
            }

            this.objectList.Insert(index, value);
        }

        public void Remove(T value)
        {
            //	Supprime un objet de la liste.
            int index = this.objectList.IndexOf(value);
            System.Diagnostics.Debug.Assert(index != -1);
            this.RemoveAt(index);
        }

        public void RemoveAt(int index)
        {
            //	Supprime un objet de la liste.
            if (this.IsOpletQueueEnable) // mémorise l'opération ?
            {
                T obj = this.objectList[index];
                OpletUndoableList operation = new OpletUndoableList(
                    this,
                    OperationType.Remove,
                    index,
                    obj
                );
                this.document.Modifier.OpletQueue.Insert(operation);
                this.document.Notifier.NotifyUndoRedoChanged();
            }

            this.objectList.RemoveAt(index);
        }

        public int Selected
        {
            //	Indique l'objet sélectionné dans la liste.
            get { return this.selected; }
            set
            {
                if (this.selected != value)
                {
                    if (this.IsOpletQueueEnable) // mémorise l'opération ?
                    {
                        OpletUndoableList operation = new OpletUndoableList(
                            this,
                            OperationType.Selected,
                            this.selected,
                            null
                        );
                        this.document.Modifier.OpletQueue.Insert(operation);
                        this.document.Notifier.NotifyUndoRedoChanged();
                    }

                    this.selected = value;
                }
            }
        }

        public object Find(System.Predicate<object> predicate)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (predicate(this.objectList[i]))
                {
                    return this.objectList[i];
                }
            }

            return null;
        }

        public object FindLast(System.Predicate<object> predicate)
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (predicate(this.objectList[i]))
                {
                    return this.objectList[i];
                }
            }

            return null;
        }

        public int FindIndex(System.Predicate<object> predicate)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (predicate(this.objectList[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public int FindLastIndex(System.Predicate<object> predicate)
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (predicate(this.objectList[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public void UndoableCopyTo(AbstractUndoableList<T> dst)
        {
            //	Copie toute la liste, avec possibilité d'annulation.
            dst.Clear();
            foreach (T obj in this.objectList)
            {
                dst.Add(obj);
            }
        }

        public void CopyTo(AbstractUndoableList<T> dst)
        {
            //	Copie toute la liste, sans possibilité d'annulation.
            dst.objectList.Clear();
            foreach (T obj in this.objectList)
            {
                dst.objectList.Add(obj);
            }
        }

        protected bool IsOpletQueueEnable
        {
            //	Indique s'il faut mémoriser l'opération.
            get
            {
                if (this.document.Modifier == null)
                    return false;
                return this.document.Modifier.OpletQueueEnable;
            }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            //	Retourne l'énumérateur, pour pouvoir utiliser foreach.
            return this.objectList.GetEnumerator();
        }

        protected static void UndoRedoOperation(OpletUndoableList operation)
        {
            //	Défait une opération dans une SerializableUndoableList.
            //	Une prochaine exécution de UndoRedoOperation refera l'opération.
            Document document = operation.List.document;
            UndoableListType listType = operation.List.type;
            List<T> objectList = operation.List.objectList;
            int index = operation.Index;
            //?System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", operation.Type.ToString(), operation.Object.ToString()));

            int incSelect = 0;
            if (operation.Type == OperationType.Insert)
            {
                objectList.RemoveAt(index);
                operation.Type = OperationType.Remove;
                incSelect = -1;
            }
            else if (operation.Type == OperationType.Remove)
            {
                objectList.Insert(index, operation.Object);
                operation.Type = OperationType.Insert;
                incSelect = 1;
            }
            else if (operation.Type == OperationType.Change)
            {
                T temp = objectList[index];
                objectList[index] = operation.Object;
                operation.Object = temp;
            }
            else if (operation.Type == OperationType.Selected)
            {
                int temp = operation.Index;
                operation.Index = operation.List.selected;
                operation.List.selected = temp;
            }

            if (
                listType == UndoableListType.ObjectsInsideDocument
                && operation.Object is Objects.Abstract
            )
            {
                if (operation.Object is Objects.Page)
                {
                    document.Notifier.NotifyArea();
                    document.Notifier.NotifyPagesChanged();
                    document.Notifier.NotifySelectionChanged();
                }
                else if (operation.Object is Objects.Layer)
                {
                    document.Notifier.NotifyArea();
                    document.Notifier.NotifyLayersChanged();
                    document.Notifier.NotifySelectionChanged();
                }
                else
                {
                    Objects.Abstract obj = operation.Object as Objects.Abstract;
                    if (obj.IsSelected)
                    {
                        if (!document.Modifier.IsDirtyCounters)
                        {
                            document.Modifier.TotalSelected += incSelect;
                        }
                    }
                    document.Notifier.NotifyArea(obj.BoundingBox);
                    document.Notifier.NotifySelectionChanged();
                }
            }

            if (operation.Object is Properties.Abstract)
            {
                Properties.Abstract prop = operation.Object as Properties.Abstract;
                if (prop.IsStyle)
                {
                    document.Notifier.NotifyStyleChanged();
                }
            }

            if (
                listType == UndoableListType.PropertiesInsideObject
                && operation.Type == OperationType.Change
            )
            {
                Properties.Abstract prop1 = operation.Object as Properties.Abstract;
                Properties.Abstract prop2 = objectList[index] as Properties.Abstract;

                if (prop1.Owners.Count > 0)
                {
                    Objects.Abstract obj = prop1.Owners[prop1.Owners.Count - 1] as Objects.Abstract;
                    document.Notifier.NotifyArea(obj.BoundingBox);
                }

                if (prop2.Owners.Count > 0)
                {
                    Objects.Abstract obj = prop2.Owners[prop2.Owners.Count - 1] as Objects.Abstract;
                    document.Notifier.NotifyArea(obj.BoundingBox);
                }
            }

            if (listType == UndoableListType.Guides)
            {
                document.Notifier.NotifyGuidesChanged();
            }
        }

        protected enum OperationType
        {
            Insert,
            Remove,
            Change,
            Selected,
        }

        protected class OpletUndoableList : AbstractOplet
        {
            public OpletUndoableList(
                AbstractUndoableList<T> list,
                OperationType type,
                int index,
                T obj
            )
            {
                this.list = list;
                this.type = type;
                this.index = index;
                this.obj = obj;
            }

            public AbstractUndoableList<T> List
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

            public T Object
            {
                get { return this.obj; }
                set { this.obj = value; }
            }

            public override IOplet Undo()
            {
                AbstractUndoableList<T>.UndoRedoOperation(this);
                return this;
            }

            public override IOplet Redo()
            {
                AbstractUndoableList<T>.UndoRedoOperation(this);
                return this;
            }

            protected AbstractUndoableList<T> list;
            protected OperationType type;
            protected int index;
            protected T obj;
        }

        protected AbstractUndoableList() { }

        protected Document document;
        protected UndoableListType type;
        protected List<T> objectList;
        protected int selected;
    }

    [System.Serializable()]
    public class SerializableUndoableList
        : AbstractUndoableList<IXMLWritable>,
            ISerializable,
            IXMLSerializable<SerializableUndoableList>
    {
        public SerializableUndoableList(Document document, UndoableListType type)
            : base(document, type) { }

        #region Serialization
        public XElement ToXML()
        {
            return new XElement(
                "UndoableList",
                new XAttribute("Type", this.type),
                new XAttribute("Selected", this.selected),
                new XElement("List", this.objectList.ToArray().Select(o => o.ToXML()))
            );
        }

        public static SerializableUndoableList FromXML(XElement xml)
        {
            return new SerializableUndoableList(xml);
        }

        private SerializableUndoableList(XElement xml)
        {
            var root = xml.Element("UndoableList");
            UndoableListType.TryParse(root.Attribute("Type").Value, out this.type);
            this.selected = int.Parse(root.Attribute("Selected").Value);
            //this.objectList = root.Elements().Select(el => )
        }

        //private System.Type GetClass(string className)
        //{
        //    switch (className)
        //    {
        //        case "Abstract"
        //    }
        //}

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //	Sérialise la liste.
            info.AddValue("Type", this.type);
            info.AddValue("Selected", this.selected);
        }

        protected SerializableUndoableList(SerializationInfo info, StreamingContext context)
        {
            //	Constructeur qui désérialise la liste.
            this.document = Document.ReadDocument;
            this.type = (UndoableListType)info.GetValue("Type", typeof(UndoableListType));
            this.objectList = (List<IXMLWritable>)info.GetValue("List", typeof(List<IXMLWritable>));
            this.selected = info.GetInt32("Selected");
        }
        #endregion
    }
}
