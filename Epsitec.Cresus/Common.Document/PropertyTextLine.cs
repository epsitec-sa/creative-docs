using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe PropertyTextLine repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class PropertyTextLine : AbstractProperty
	{
		public PropertyTextLine(Document document) : base(document)
		{
			this.horizontal = JustifHorizontal.Left;
			this.offset     = 0.0;
			this.add        = 0.0;
		}

		public JustifHorizontal Horizontal
		{
			get
			{
				return this.horizontal;
			}
			
			set
			{
				if ( this.horizontal != value )
				{
					this.NotifyBefore();
					this.horizontal = value;
					this.NotifyAfter();
				}
			}
		}

		public double Offset
		{
			get
			{
				return this.offset;
			}
			
			set
			{
				if ( this.offset != value )
				{
					this.NotifyBefore();
					this.offset = value;
					this.NotifyAfter();
				}
			}
		}

		public double Add
		{
			get
			{
				return this.add;
			}
			
			set
			{
				if ( this.add != value )
				{
					this.NotifyBefore();
					this.add = value;
					this.NotifyAfter();
				}
			}
		}

		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}

		// Effectue une copie de la propri�t�.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyTextLine p = property as PropertyTextLine;
			p.horizontal = this.horizontal;
			p.offset     = this.offset;
			p.add        = this.add;
		}

		// Compare deux propri�t�s.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyTextLine p = property as PropertyTextLine;
			if ( p.horizontal != this.horizontal )  return false;
			if ( p.offset     != this.offset     )  return false;
			if ( p.add        != this.add        )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override AbstractPanel CreatePanel(Document document)
		{
			return new PanelTextLine(document);
		}


		#region Serialization
		// S�rialise la propri�t�.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Horizontal", this.horizontal);
			info.AddValue("Offset", this.offset);
			info.AddValue("Add", this.add);
		}

		// Constructeur qui d�s�rialise la propri�t�.
		protected PropertyTextLine(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.horizontal = (JustifHorizontal) info.GetValue("Horizontal", typeof(JustifHorizontal));
			this.offset = info.GetDouble("Offset");
			this.add = info.GetDouble("Add");
		}
		#endregion

	
		protected JustifHorizontal		horizontal;
		protected double				offset;
		protected double				add;
	}
}
