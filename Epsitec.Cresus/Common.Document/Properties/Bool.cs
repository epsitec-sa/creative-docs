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

		protected override void Initialise()
		{
			this.boolValue = false;
		}

		// Valeur de la propriété.
		public bool BoolValue
		{
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

		// Retourne le nom d'un type donné.
		public static string GetName(bool type)
		{
			if ( type )  return Res.Strings.Property.Close.Yes;
			else         return Res.Strings.Property.Close.No;
		}

		// Retourne l'icône pour un type donné.
		public static string GetIconText(bool type)
		{
			if ( type )  return "CloseYes";
			else         return "CloseNo";
		}

		// Indique si un changement de cette propriété modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return ( this.type == Type.PolyClose ); }
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Bool p = property as Bool;
			p.boolValue = this.boolValue;
		}

		// Compare deux propriétés.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Bool p = property as Bool;
			if ( p.boolValue != this.boolValue )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override Panels.Abstract CreatePanel(Document document)
		{
			Panels.Abstract.StaticDocument = document;
			return new Panels.Bool(document);
		}


		#region Serialization
		// Sérialise la propriété.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("BoolValue", this.boolValue);
		}

		// Constructeur qui désérialise la propriété.
		protected Bool(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.boolValue = info.GetBoolean("BoolValue");
		}
		#endregion

	
		protected bool			boolValue;
	}
}
