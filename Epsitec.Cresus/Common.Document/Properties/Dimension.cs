using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	public enum DimensionJustif
	{
		CenterOrLeft  = 0,
		CenterOrRight = 1,
		Left          = 2,
		Right         = 3,
	}

	public enum DimensionForm
	{
		Auto          = 0,
		Inside        = 1,
		Outside       = 2,
	}

	/// <summary>
	/// La classe Dimension représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Dimension : Abstract
	{
		public Dimension(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialise()
		{
			this.dimensionJustif = DimensionJustif.CenterOrRight;
			this.dimensionForm = DimensionForm.Auto;
			this.addLength = 50.0;
			this.outLength = 20.0;
			this.fontOffset = 0.4;
			this.prefix = " ";
			this.postfix = " mm ";
			this.rotateText = false;
		}

		public DimensionJustif DimensionJustif
		{
			get
			{
				return this.dimensionJustif;
			}

			set
			{
				if ( this.dimensionJustif != value )
				{
					this.NotifyBefore();
					this.dimensionJustif = value;
					this.NotifyAfter();
				}
			}
		}

		public DimensionForm DimensionForm
		{
			get
			{
				return this.dimensionForm;
			}

			set
			{
				if ( this.dimensionForm != value )
				{
					this.NotifyBefore();
					this.dimensionForm = value;
					this.NotifyAfter();
				}
			}
		}

		public double AddLength
		{
			get
			{
				return this.addLength;
			}

			set
			{
				if ( this.addLength != value )
				{
					this.NotifyBefore();
					this.addLength = value;
					this.NotifyAfter();
				}
			}
		}

		public double OutLength
		{
			get
			{
				return this.outLength;
			}

			set
			{
				if ( this.outLength != value )
				{
					this.NotifyBefore();
					this.outLength = value;
					this.NotifyAfter();
				}
			}
		}

		public double FontOffset
		{
			get
			{
				return this.fontOffset;
			}

			set
			{
				if ( this.fontOffset != value )
				{
					this.NotifyBefore();
					this.fontOffset = value;
					this.NotifyAfter();
				}
			}
		}

		public string Prefix
		{
			get
			{
				return this.prefix;
			}

			set
			{
				if ( this.prefix != value )
				{
					this.NotifyBefore();
					this.prefix = value;
					this.NotifyAfter();
				}
			}
		}

		public string Postfix
		{
			get
			{
				return this.postfix;
			}

			set
			{
				if ( this.postfix != value )
				{
					this.NotifyBefore();
					this.postfix = value;
					this.NotifyAfter();
				}
			}
		}

		public bool RotateText
		{
			get
			{
				return this.rotateText;
			}

			set
			{
				if ( this.rotateText != value )
				{
					this.NotifyBefore();
					this.rotateText = value;
					this.NotifyAfter();
				}
			}
		}

		// Indique si un changement de cette propriété modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Dimension p = property as Dimension;
			p.dimensionJustif = this.dimensionJustif;
			p.dimensionForm   = this.dimensionForm;
			p.addLength       = this.addLength;
			p.outLength       = this.outLength;
			p.fontOffset      = this.fontOffset;
			p.prefix          = this.prefix;
			p.postfix         = this.postfix;
			p.rotateText      = this.rotateText;
		}

		// Compare deux propriétés.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Dimension p = property as Dimension;
			if ( p.dimensionJustif != this.dimensionJustif )  return false;
			if ( p.dimensionForm   != this.dimensionForm   )  return false;
			if ( p.addLength       != this.addLength       )  return false;
			if ( p.outLength       != this.outLength       )  return false;
			if ( p.fontOffset      != this.fontOffset      )  return false;
			if ( p.prefix          != this.prefix          )  return false;
			if ( p.postfix         != this.postfix         )  return false;
			if ( p.rotateText      != this.rotateText      )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override Panels.Abstract CreatePanel(Document document)
		{
			return new Panels.Dimension(document);
		}


		#region Serialization
		// Sérialise la propriété.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("DimensionJustif", this.dimensionJustif);
			info.AddValue("DimensionForm", this.dimensionForm);
			info.AddValue("AddLength", this.addLength);
			info.AddValue("OutLength", this.outLength);
			info.AddValue("FontOffset", this.fontOffset);
			info.AddValue("Prefix", this.prefix);
			info.AddValue("Postfix", this.postfix);
			info.AddValue("RotateText", this.rotateText);
		}

		// Constructeur qui désérialise la propriété.
		protected Dimension(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dimensionJustif = (DimensionJustif) info.GetValue("DimensionJustif", typeof(DimensionJustif));
			this.dimensionForm = (DimensionForm) info.GetValue("DimensionForm", typeof(DimensionForm));
			this.addLength = info.GetDouble("AddLength");
			this.outLength = info.GetDouble("OutLength");
			this.fontOffset = info.GetDouble("FontOffset");
			this.prefix = info.GetString("Prefix");
			this.postfix = info.GetString("Postfix");
			this.rotateText = info.GetBoolean("RotateText");
		}
		#endregion


		protected DimensionJustif		dimensionJustif;
		protected DimensionForm			dimensionForm;
		protected double				addLength;
		protected double				outLength;
		protected double				fontOffset;
		protected string				prefix;
		protected string				postfix;
		protected bool					rotateText;
	}
}
