using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Bool représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Bool : Abstract
	{
		public Bool(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialize()
		{
			base.Initialize ();
			this.boolValue = false;
		}

		public bool BoolValue
		{
			//	Valeur de la propriété.
			get
			{
				return this.boolValue;
			}
			
			set
			{
				if ( this.boolValue != value )
				{
					this.NotifyBefore();
					this.boolValue = value;
					this.NotifyAfter();
				}
			}
		}

		public static string GetName(bool type)
		{
			//	Retourne le nom d'un type donné.
			if ( type )  return Res.Strings.Property.Close.Yes;
			else         return Res.Strings.Property.Close.No;
		}

		public static string GetIconText(bool type)
		{
			//	Retourne l'icône pour un type donné.
			if ( type )  return "CloseYes";
			else         return "CloseNo";
		}

		public override bool AlterBoundingBox
		{
			//	Indique si un changement de cette propriété modifie la bbox de l'objet.
			get { return ( this.type == Type.PolyClose ); }
		}

		public override void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			Bool p = property as Bool;
			p.boolValue = this.boolValue;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			Bool p = property as Bool;
			if ( p.boolValue != this.boolValue )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Bool(document);
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise la propriété.
			base.GetObjectData(info, context);

			info.AddValue("BoolValue", this.boolValue);
		}

		protected Bool(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise la propriété.
			this.boolValue = info.GetBoolean("BoolValue");
		}
		#endregion

	
		protected bool			boolValue;
	}
}
