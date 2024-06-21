using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Serialization;

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

    public class BasicUndoableList : AbstractUndoableList<object>
    {
        public BasicUndoableList(Document document, UndoableListType type)
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
            this.arrayList = new();
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

        public void Clear()
        {
            //	Vide toute la liste.
            if (this.IsOpletQueueEnable) // mémorise l'opération ?
            {
                int total = this.arrayList.Count;
                for (int i = 0; i < total; i++)
                {
                    this.RemoveAt(0);
                }
            }

            this.arrayList.Clear();
            this.Selected = -1;
        }

        public int Count
        {
            //	Nombre d'objets dans la liste.
            get { return this.arrayList.Count; }
        }

        public bool Contains(T item)
        {
            //	Indique si la liste contient un objet.
            return this.arrayList.Contains(item);
        }

        public int IndexOf(T value)
        {
            //	Retourne l'index d'un objet.
            return this.arrayList.IndexOf(value);
        }

        public T this[int index]
        {
            //	Accès à un objet quelconque.
            get { return this.arrayList[index]; }
            set
            {
                if (this.IsOpletQueueEnable) // mémorise l'opération ?
                {
                    T obj = this.arrayList[index];
                    OpletUndoableList operation = new OpletUndoableList(
                        this,
                        OperationType.Change,
                        index,
                        obj
                    );
                    this.document.Modifier.OpletQueue.Insert(operation);
                    this.document.Notifier.NotifyUndoRedoChanged();
                }

                this.arrayList[index] = value;
            }
        }

        public int Add(T value)
        {
            //	Ajoute un objet à la fin de la liste.
            int index = this.arrayList.Count;
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

            this.arrayList.Insert(index, value);
        }

        public void Remove(T value)
        {
            //	Supprime un objet de la liste.
            int index = this.arrayList.IndexOf(value);
            System.Diagnostics.Debug.Assert(index != -1);
            this.RemoveAt(index);
        }

        public void RemoveAt(int index)
        {
            //	Supprime un objet de la liste.
            if (this.IsOpletQueueEnable) // mémorise l'opération ?
            {
                T obj = this.arrayList[index];
                OpletUndoableList operation = new OpletUndoableList(
                    this,
                    OperationType.Remove,
                    index,
                    obj
                );
                this.document.Modifier.OpletQueue.Insert(operation);
                this.document.Notifier.NotifyUndoRedoChanged();
            }

            this.arrayList.RemoveAt(index);
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
                if (predicate(this.arrayList[i]))
                {
                    return this.arrayList[i];
                }
            }

            return null;
        }

        public object FindLast(System.Predicate<object> predicate)
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (predicate(this.arrayList[i]))
                {
                    return this.arrayList[i];
                }
            }

            return null;
        }

        public int FindIndex(System.Predicate<object> predicate)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (predicate(this.arrayList[i]))
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
                if (predicate(this.arrayList[i]))
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
            foreach (T obj in this.arrayList)
            {
                dst.Add(obj);
            }
        }

        public void CopyTo(AbstractUndoableList<T> dst)
        {
            //	Copie toute la liste, sans possibilité d'annulation.
            dst.arrayList.Clear();
            foreach (T obj in this.arrayList)
            {
                dst.arrayList.Add(obj);
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
            return this.arrayList.GetEnumerator();
        }

        protected static void UndoRedoOperation(OpletUndoableList operation)
        {
            //	Défait une opération dans une NewUndoableList.
            //	Une prochaine exécution de UndoRedoOperation refera l'opération.
            Document document = operation.List.document;
            UndoableListType listType = operation.List.type;
            List<T> objectList = operation.List.arrayList;
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

        public Document document;
        public UndoableListType type;
        public List<T> arrayList;
        public int selected;
    }

    public class NewUndoableList
        : AbstractUndoableList<IXMLWritable>,
            IXMLSerializable<NewUndoableList>
    {
        public NewUndoableList(Document document, UndoableListType type)
            : base(document, type) { }

        public static NewUndoableList FromOld(UndoableList oldList)
        {
            if (oldList == null)
            {
                return null;
            }
            return new NewUndoableList(oldList);
        }

        private NewUndoableList(UndoableList oldList)
            : base(oldList.document, oldList.type)
        {
            this.selected = oldList.selected;
            this.arrayList = oldList.arrayList.Cast<IXMLWritable>().ToList();
        }

        public new IXMLWritable this[int index]
        {
            //	Accès à un objet quelconque.
            get { return base[index]; }
            set
            {
                if (value == null)
                {
                    throw new System.ArgumentException("null object is not IXMLWritable");
                }
                base[index] = value;
            }
        }

        public new int Add(IXMLWritable value)
        {
            if (value == null)
            {
                throw new System.ArgumentException("null object is not IXMLWritable");
            }
            return base.Add(value);
        }

        public new void Insert(int index, IXMLWritable value)
        {
            if (value == null)
            {
                throw new System.ArgumentException("null object is not IXMLWritable");
            }
            base.Insert(index, value);
        }

        #region Serialization
        public bool HasEquivalentData(IXMLWritable other)
        {
            NewUndoableList otherList = (NewUndoableList)other;
            if (this.type != otherList.type || this.selected != otherList.selected)
            {
                return false;
            }
            if (this.arrayList.Count != otherList.arrayList.Count)
            {
                return false;
            }
            return this.arrayList.HasEquivalentData(otherList.arrayList);
        }

        public XElement ToXML()
        {
            return new XElement(
                "BasicUndoableList",
                new XAttribute("Type", this.type),
                new XAttribute("Selected", this.selected),
                new XElement(
                    "List",
                    this.arrayList.ToArray().Where(o => o != null).Select(o => o.ToXML())
                )
            );
        }

        public static NewUndoableList FromXML(XElement xml)
        {
            return new NewUndoableList(xml);
        }

        private NewUndoableList(XElement xml)
        {
            var root = xml.Element("BasicUndoableList");
            UndoableListType.TryParse(root.Attribute("Type").Value, out this.type);
            var itemLoader = GetListItemLoader(this.type);
            this.selected = int.Parse(root.Attribute("Selected").Value);
            this.arrayList = root.Element("List").Elements().Select(itemLoader).ToList();
        }

        private static System.Func<XElement, IXMLWritable> GetListItemLoader(
            UndoableListType listType
        )
        {
            switch (listType)
            {
                case UndoableListType.ObjectsInsideProperty:
                case UndoableListType.ObjectsInsideDocument:
                    return LoadObjectFromXML;
                case UndoableListType.PropertiesInsideObject:
                case UndoableListType.PropertiesInsideDocument:
                case UndoableListType.StylesInsideAggregate:
                    return LoadPropertyFromXML;
                case UndoableListType.AggregatesInsideDocument:
                case UndoableListType.AggregatesInsideObject:
                    return Properties.Aggregate.FromXML;
                case UndoableListType.Guides:
                    return Settings.Guide.FromXML;
                case UndoableListType.TextFlows:
                    return TextFlow.FromXML;
                default:
                    throw new System.ArgumentException(
                        $"No item loader for list type '{listType}'"
                    );
            }
        }

        private static IXMLWritable LoadObjectFromXML(XElement xml)
        {
            switch (xml.Name.LocalName)
            {
                case "Bezier":
                    return Objects.Bezier.FromXML(xml);
                case "Circle":
                    return Objects.Circle.FromXML(xml);
                case "Dimension":
                    return Objects.Dimension.FromXML(xml);
                case "Ellipse":
                    return Objects.Ellipse.FromXML(xml);
                case "Free":
                    return Objects.Free.FromXML(xml);
                case "Group":
                    return Objects.Group.FromXML(xml);
                case "Image":
                    return Objects.Image.FromXML(xml);
                case "Layer":
                    return Objects.Layer.FromXML(xml);
                case "Line":
                    return Objects.Line.FromXML(xml);
                case "Memory":
                    return Objects.Memory.FromXML(xml);
                case "Page":
                    return Objects.Page.FromXML(xml);
                case "Poly":
                    return Objects.Poly.FromXML(xml);
                case "Rectangle":
                    return Objects.Rectangle.FromXML(xml);
                case "Regular":
                    return Objects.Regular.FromXML(xml);
                case "Surface":
                    return Objects.Surface.FromXML(xml);
                case "TextBox":
                    return Objects.TextBox.FromXML(xml);
                case "TextBox2":
                    return Objects.TextBox2.FromXML(xml);
                case "TextLine":
                    return Objects.TextLine.FromXML(xml);
                case "TextLine2":
                    return Objects.TextLine2.FromXML(xml);
                case "Volume":
                    return Objects.Volume.FromXML(xml);
                default:
                    throw new System.ArgumentException(
                        $"Unknown Object type '{xml.Name.LocalName}'"
                    );
            }
        }

        private static IXMLWritable LoadPropertyFromXML(XElement xml)
        {
            switch (xml.Name.LocalName)
            {
                case "Arc":
                    return Properties.Arc.FromXML(xml);
                case "Arrow":
                    return Properties.Arrow.FromXML(xml);
                case "Bool":
                    return Properties.Bool.FromXML(xml);
                case "Color":
                    return Properties.Color.FromXML(xml);
                case "Corner":
                    return Properties.Corner.FromXML(xml);
                case "Dimension":
                    return Properties.Dimension.FromXML(xml);
                case "Font":
                    return Properties.Font.FromXML(xml);
                case "Frame":
                    return Properties.Frame.FromXML(xml);
                case "Gradient":
                    return Properties.Gradient.FromXML(xml);
                case "Image":
                    return Properties.Image.FromXML(xml);
                case "Justif":
                    return Properties.Justif.FromXML(xml);
                case "Line":
                    return Properties.Line.FromXML(xml);
                case "ModColor":
                    return Properties.ModColor.FromXML(xml);
                case "Name":
                    return Properties.Name.FromXML(xml);
                case "Regular":
                    return Properties.Regular.FromXML(xml);
                case "Shadow":
                    return Properties.Shadow.FromXML(xml);
                case "Surface":
                    return Properties.Surface.FromXML(xml);
                case "Tension":
                    return Properties.Tension.FromXML(xml);
                case "TextLine":
                    return Properties.TextLine.FromXML(xml);
                case "Volume":
                    return Properties.Volume.FromXML(xml);
                default:
                    throw new System.ArgumentException(
                        $"Unknown Property type '{xml.Name.LocalName}'"
                    );
            }
        }
        #endregion
    }

    /// <summary>
    /// La classe UndoableList implémente un ArrayList avec possibilité de undo/redo.
    /// </summary>
    [System.Serializable()]
    public class UndoableList : System.Collections.IEnumerable, ISerializable
    {
        public UndoableList(Document document, UndoableListType type)
        {
            //	Crée une nouvelle liste vide.
            this.document = document;
            this.type = type;
            this.arrayList = new System.Collections.ArrayList();
            this.selected = -1;
        }

        public UndoableList(NewUndoableList newList)
            : this(newList.document, newList.type)
        {
            this.selected = newList.selected;
            this.arrayList = new(newList.arrayList);
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

        public void Clear()
        {
            //	Vide toute la liste.
            if (this.IsOpletQueueEnable) // mémorise l'opération ?
            {
                int total = this.arrayList.Count;
                for (int i = 0; i < total; i++)
                {
                    this.RemoveAt(0);
                }
            }

            this.arrayList.Clear();
            this.Selected = -1;
        }

        public int Count
        {
            //	Nombre d'objets dans la liste.
            get { return this.arrayList.Count; }
        }

        public bool Contains(object item)
        {
            //	Indique si la liste contient un objet.
            return this.arrayList.Contains(item);
        }

        public int IndexOf(object value)
        {
            //	Retourne l'index d'un objet.
            return this.arrayList.IndexOf(value);
        }

        public object this[int index]
        {
            //	Accès à un objet quelconque.
            get { return this.arrayList[index]; }
            set
            {
                if (this.IsOpletQueueEnable) // mémorise l'opération ?
                {
                    object obj = this.arrayList[index];
                    OpletUndoableList operation = new OpletUndoableList(
                        this,
                        OperationType.Change,
                        index,
                        obj
                    );
                    this.document.Modifier.OpletQueue.Insert(operation);
                    this.document.Notifier.NotifyUndoRedoChanged();
                }

                this.arrayList[index] = value;
            }
        }

        public int Add(object value)
        {
            //	Ajoute un objet à la fin de la liste.
            int index = this.arrayList.Count;
            this.Insert(index, value); // insère à la fin
            return index;
        }

        public void Insert(int index, object value)
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

            this.arrayList.Insert(index, value);
        }

        public void Remove(object value)
        {
            //	Supprime un objet de la liste.
            int index = this.arrayList.IndexOf(value);
            System.Diagnostics.Debug.Assert(index != -1);
            this.RemoveAt(index);
        }

        public void RemoveAt(int index)
        {
            //	Supprime un objet de la liste.
            if (this.IsOpletQueueEnable) // mémorise l'opération ?
            {
                object obj = this.arrayList[index];
                OpletUndoableList operation = new OpletUndoableList(
                    this,
                    OperationType.Remove,
                    index,
                    obj
                );
                this.document.Modifier.OpletQueue.Insert(operation);
                this.document.Notifier.NotifyUndoRedoChanged();
            }

            this.arrayList.RemoveAt(index);
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
                if (predicate(this.arrayList[i]))
                {
                    return this.arrayList[i];
                }
            }

            return null;
        }

        public object FindLast(System.Predicate<object> predicate)
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (predicate(this.arrayList[i]))
                {
                    return this.arrayList[i];
                }
            }

            return null;
        }

        public int FindIndex(System.Predicate<object> predicate)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (predicate(this.arrayList[i]))
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
                if (predicate(this.arrayList[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public void UndoableCopyTo(UndoableList dst)
        {
            //	Copie toute la liste, avec possibilité d'annulation.
            dst.Clear();
            foreach (object obj in this.arrayList)
            {
                dst.Add(obj);
            }
        }

        public void CopyTo(UndoableList dst)
        {
            //	Copie toute la liste, sans possibilité d'annulation.
            dst.arrayList.Clear();
            foreach (object obj in this.arrayList)
            {
                dst.arrayList.Add(obj);
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
            return this.arrayList.GetEnumerator();
        }

        protected static void UndoRedoOperation(OpletUndoableList operation)
        {
            //	Défait une opération dans une NewUndoableList.
            //	Une prochaine exécution de UndoRedoOperation refera l'opération.
            Document document = operation.List.document;
            UndoableListType listType = operation.List.type;
            System.Collections.ArrayList arrayList = operation.List.arrayList;
            int index = operation.Index;
            //?System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", operation.Type.ToString(), operation.Object.ToString()));

            int incSelect = 0;
            if (operation.Type == OperationType.Insert)
            {
                arrayList.RemoveAt(index);
                operation.Type = OperationType.Remove;
                incSelect = -1;
            }
            else if (operation.Type == OperationType.Remove)
            {
                arrayList.Insert(index, operation.Object);
                operation.Type = OperationType.Insert;
                incSelect = 1;
            }
            else if (operation.Type == OperationType.Change)
            {
                object temp = arrayList[index];
                arrayList[index] = operation.Object;
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
                Properties.Abstract prop2 = arrayList[index] as Properties.Abstract;

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
            Insert, // ajout d'un nouvel objet dans une NewUndoableList
            Remove, // suppression d'un objet dans une NewUndoableList
            Change, // changement d'un objet dans une NewUndoableList
            Selected, // sélection d'un objet dans une NewUndoableList
        }

        protected class OpletUndoableList : AbstractOplet
        {
            public OpletUndoableList(UndoableList list, OperationType type, int index, object obj)
            {
                this.list = list;
                this.type = type;
                this.index = index;
                this.obj = obj;
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

            protected UndoableList list;
            protected OperationType type;
            protected int index;
            protected object obj;
        }

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //	Sérialise la liste.
            info.AddValue("Type", this.type);
            info.AddValue("List", this.arrayList);
            info.AddValue("Selected", this.selected);
        }

        protected UndoableList(SerializationInfo info, StreamingContext context)
        {
            //	Constructeur qui désérialise la liste.
            this.document = Document.ReadDocument;
            this.type = (UndoableListType)info.GetValue("Type", typeof(UndoableListType));
            this.arrayList = (System.Collections.ArrayList)
                info.GetValue("List", typeof(System.Collections.ArrayList));
            this.selected = info.GetInt32("Selected");
        }
        #endregion


        public Document document;
        public UndoableListType type;
        public System.Collections.ArrayList arrayList;
        public int selected;
    }
}
