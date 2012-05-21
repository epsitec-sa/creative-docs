using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Aggregate représente une collection de styles.
	/// </summary>
	[System.Serializable()]
	public class Aggregate : ISerializable
	{
		public Aggregate(Document document)
		{
			this.document = document;
			this.styles = new UndoableList(this.document, UndoableListType.StylesInsideAggregate);
			this.children = new UndoableList (this.document, UndoableListType.AggregatesChildren);
		}


		public string AggregateName
		{
			//	Nom de l'agrégat.
			get
			{
				return this.aggregateName;
			}

			set
			{
				if ( this.aggregateName != value )
				{
					this.InsertOpletAggregate();
					this.aggregateName = value;
				}
			}
		}

		public UndoableList Styles
		{
			//	Liste des styles de l'agrégat.
			get
			{
				return this.styles;
			}
		}

		public UndoableList Children
		{
			//	Liste des fils de l'agrégat.
			get
			{
				return this.children;
			}
		}

		public Properties.Abstract Property(Properties.Type type, bool deep)
		{
			//	Donne une propriété de l'agrégat.
			if ( deep )  return this.PropertyDeep(type);
			else         return this.Property(type);
		}

		public Properties.Abstract Property(Properties.Type type)
		{
			//	Donne une propriété de l'agrégat.
			foreach ( Properties.Abstract property in this.styles )
			{
				if ( property.Type == type )  return property;
			}
			return null;
		}

		public Properties.Abstract PropertyDeep(Properties.Type type)
		{
			//	Donne une propriété de l'agrégat ou de l'un des agrégats enfants.
			return this.PropertyDeep(type, 0);
		}
		
		protected Properties.Abstract PropertyDeep(Properties.Type type, int deep)
		{
			if ( deep > 10 )  return null;

			Properties.Abstract property = this.Property(type);
			if ( property != null )  return property;

			if ( this.children.Count != 0 )
			{
				//	Cherche depuis la fin, pour obtenir le même ordre que les styles de texte.
				for ( int i=this.children.Count-1 ; i>=0 ; i-- )
				{
					Properties.Aggregate children = this.children[i] as Properties.Aggregate;
					property = children.PropertyDeep(type, deep+1);
					if ( property != null )  return property;
				}
			}
			return null;
		}

		public bool Contains(Properties.Abstract property)
		{
			//	Indique si l'agrégat contient une propriété.
			return this.styles.Contains(property);
		}

		public bool IsUsedByObject(Objects.Abstract obj)
		{
			//	Vérifie si un objet utilise cet agrégat.
			return this.IsUsedByObject(obj, 0);
		}
		
		protected bool IsUsedByObject(Objects.Abstract obj, int deep)
		{
			if ( deep > 10 )  return false;

			UndoableList list = obj.Aggregates;
			for ( int i=0 ; i<list.Count ; i++ )
			{
				Properties.Aggregate agg = list[i] as Properties.Aggregate;

				if ( agg == this )  return true;

				if ( this.children.Count != 0 )
				{
					foreach ( Properties.Aggregate children in this.children )
					{
						if ( children.IsUsedByObject(obj, deep+1) )  return true;
					}
				}
			}
			return false;
		}


		public string GetStyleBrief()
		{
			//	Donne un texte résumé sur un style graphique.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append("<font size=\"80%\">");

			foreach ( Properties.Abstract property in this.styles )
			{
				property.PutStyleBrief(builder);
			}

			builder.Append("</font>");
			return builder.ToString();
		}


		public void CopyTo(Aggregate dst)
		{
			//	Copie tout l'agrégat.
			dst.aggregateName = this.aggregateName;
			this.styles.CopyTo(dst.styles);
			this.children.CopyTo(dst.children);
		}

		public void DuplicateTo(Aggregate dst)
		{
			//	Duplique tout l'agrégat.
			dst.aggregateName = this.aggregateName;
			this.children.CopyTo(dst.children);

			foreach ( Properties.Abstract srcProp in this.styles )
			{
				Abstract newProp = Abstract.NewProperty(this.document, srcProp.Type);
				srcProp.CopyTo(newProp);
				dst.styles.Add(newProp);
			}
		}


		public static int UniqueId(UndoableList aggregates, Properties.Abstract property)
		{
			//	Donne un identificateur unique pour une propriété.
			int id = (int) property.Type;

			if ( property.IsStyle )
			{
				id = 100;
				foreach ( Properties.Aggregate agg in aggregates )
				{
					int index = agg.Styles.IndexOf(property);
					if ( index == -1 )
					{
						id += agg.Styles.Count;
					}
					else
					{
						id += index;
						break;
					}
				}
			}

			return id;
		}


		#region OpletAggregate
		protected void InsertOpletAggregate()
		{
			//	Ajoute un oplet pour mémoriser l'agrégat.
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletAggregate oplet = new OpletAggregate(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	Mémorise l'agrégat.
		protected class OpletAggregate : AbstractOplet
		{
			public OpletAggregate(Aggregate host)
			{
				this.host = host;
				this.name = host.aggregateName;
			}

			protected void Swap()
			{
				string temp = this.host.aggregateName;
				this.host.aggregateName = this.name;
				this.name = temp;

				this.host.document.Notifier.NotifyStyleChanged();
			}

			public override IOplet Undo()
			{
				this.Swap();
				return this;
			}

			public override IOplet Redo()
			{
				this.Swap();
				return this;
			}

			protected Aggregate				host;
			protected string				name;
		}
		#endregion

		
		#region Serialization
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'agrégat.
			info.AddValue("AggregateName", this.aggregateName);
			info.AddValue("Styles", this.styles);
			info.AddValue("Children", this.children);
		}

		protected Aggregate(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui désérialise l'agrégat.
			this.document = Document.ReadDocument;
			this.aggregateName = info.GetString("AggregateName");
			this.styles = (UndoableList) info.GetValue("Styles", typeof(UndoableList));

			if (this.document.IsRevisionGreaterOrEqual (2, 0, 0))
			{
				this.children = (UndoableList) info.GetValue ("Children", typeof (UndoableList));
			}
			else if (this.document.IsRevisionGreaterOrEqual (1, 0, 27))
			{
				this.children = (UndoableList) info.GetValue ("Childrens", typeof (UndoableList));
			}
			else
			{
				this.children = new UndoableList (this.document, UndoableListType.AggregatesChildren);
			}
		}
		#endregion


		protected Document						document;
		protected string						aggregateName = "";
		protected UndoableList					styles;
		protected UndoableList					children;
	}
}
