using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Bool repr�sente une propri�t� d'un objet graphique.
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

		// Valeur de la propri�t�.
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

		// Retourne le nom d'un type donn�.
		public static string GetName(bool type)
		{
			if ( type )  return Res.Strings.Property.Close.Yes;
			else         return Res.Strings.Property.Close.No;
		}

		// Retourne l'ic�ne pour un type donn�.
		public static string GetIconText(bool type)
		{
			if ( type )  return "CloseYes";
			else         return "CloseNo";
		}

		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return ( this.type == Type.PolyClose ); }
		}

		// Effectue une copie de la propri�t�.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Bool p = property as Bool;
			p.boolValue = this.boolValue;
		}

		// Compare deux propri�t�s.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Bool p = property as Bool;
			if ( p.boolValue != this.boolValue )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override Panels.Abstract CreatePanel(Document document)
		{
			Panels.Abstract.StaticDocument = document;
			return new Panels.Bool(document);
		}


		#region Serialization
		// S�rialise la propri�t�.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("BoolValue", this.boolValue);
		}

		// Constructeur qui d�s�rialise la propri�t�.
		protected Bool(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.boolValue = info.GetBoolean("BoolValue");
		}
		#endregion

	
		protected bool			boolValue;
	}
}
