using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Tension représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Tension : Abstract
	{
		public Tension(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialize()
		{
			base.Initialize ();
			this.tensionValue = 0.6;  // tension de 60% par défaut
		}

		public double TensionValue
		{
			get
			{
				return this.tensionValue;
			}
			
			set
			{
				if ( this.tensionValue != value )
				{
					this.NotifyBefore();
					this.tensionValue = value;
					this.NotifyAfter();
				}
			}
		}

		public override string SampleText
		{
			//	Donne le petit texte pour les échantillons.
			get
			{
				return string.Concat(Res.Strings.Property.Tension.Short.Value, this.tensionValue.ToString());
			}
		}

		public override void PutStyleBrief(System.Text.StringBuilder builder)
		{
			//	Construit le texte résumé d'un style pour une propriété.
			this.PutStyleBriefPrefix(builder);
			builder.Append(this.SampleText);
			this.PutStyleBriefPostfix(builder);
		}

		public override bool AlterBoundingBox
		{
			//	Indique si un changement de cette propriété modifie la bbox de l'objet.
			get { return true; }
		}


		public override void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			Tension p = property as Tension;
			p.tensionValue = this.tensionValue;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			Tension p = property as Tension;
			if ( p.tensionValue != this.tensionValue )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Tension(document);
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise la propriété.
			base.GetObjectData(info, context);

			info.AddValue("TensionValue", this.tensionValue);
		}

		protected Tension(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise la propriété.
			this.tensionValue = info.GetDouble("TensionValue");
		}
		#endregion

	
		protected double				tensionValue;
	}
}
