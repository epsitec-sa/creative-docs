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

		protected override void Initialize()
		{
			this.boolValue = false;
		}

		public bool BoolValue
		{
			//	Valeur de la propri�t�.
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
			//	Retourne le nom d'un type donn�.
			if ( type )  return Res.Strings.Property.Close.Yes;
			else         return Res.Strings.Property.Close.No;
		}

		public static string GetIconText(bool type)
		{
			//	Retourne l'ic�ne pour un type donn�.
			if ( type )  return "CloseYes";
			else         return "CloseNo";
		}

		public override bool AlterBoundingBox
		{
			//	Indique si un changement de cette propri�t� modifie la bbox de l'objet.
			get { return ( this.type == Type.PolyClose ); }
		}

		public override void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propri�t�.
			base.CopyTo(property);
			Bool p = property as Bool;
			p.boolValue = this.boolValue;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propri�t�s.
			if ( !base.Compare(property) )  return false;

			Bool p = property as Bool;
			if ( p.boolValue != this.boolValue )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Cr�e le panneau permettant d'�diter la propri�t�.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Bool(document);
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise la propri�t�.
			base.GetObjectData(info, context);

			info.AddValue("BoolValue", this.boolValue);
		}

		protected Bool(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui d�s�rialise la propri�t�.
			this.boolValue = info.GetBoolean("BoolValue");
		}
		#endregion

	
		protected bool			boolValue;
	}
}
