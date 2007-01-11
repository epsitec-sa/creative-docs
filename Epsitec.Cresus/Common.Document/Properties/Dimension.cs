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

		protected override void Initialize()
		{
			base.Initialize ();
			this.dimensionJustif = DimensionJustif.CenterOrRight;
			this.dimensionForm = DimensionForm.Auto;

			if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
			{
				this.addLength = 50.0;  // 5mm
				this.outLength = 20.0;  // 2mm
				this.dimensionText = Res.Strings.Property.Dimension.TextMm;
			}
			else
			{
				this.addLength = 50.8;  // 0.2in
				this.outLength = 25.4;  // 0.1in
				this.dimensionText = Res.Strings.Property.Dimension.TextIn;
			}
			
			this.fontOffset = 0.4;
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

		public string DimensionText
		{
			get
			{
				return this.dimensionText;
			}

			set
			{
				if ( this.dimensionText != value )
				{
					this.NotifyBefore();
					this.dimensionText = value;
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

		public override string SampleText
		{
			//	Donne le petit texte pour les échantillons.
			get
			{
				if ( this.dimensionForm == DimensionForm.Inside  )  return "<-->";
				if ( this.dimensionForm == DimensionForm.Outside )  return ">--<";
				return "<*>";
			}
		}

		public override void PutStyleBrief(System.Text.StringBuilder builder)
		{
			//	Construit le texte résumé d'un style pour une propriété.
			this.PutStyleBriefPrefix(builder);

			builder.Append(TextLayout.ConvertToTaggedText(this.SampleText));
			builder.Append(", &quot;");
			builder.Append(TextLayout.ConvertToTaggedText(this.dimensionText));
			builder.Append("&quot;");
			
			this.PutStyleBriefPostfix(builder);
		}

		public static string GetName(DimensionForm type)
		{
			//	Retourne le nom d'un type donné.
			string name = "";
			switch ( type )
			{
				case DimensionForm.Auto:     name = Res.Strings.Property.Dimension.Auto;     break;
				case DimensionForm.Inside:   name = Res.Strings.Property.Dimension.Inside;   break;
				case DimensionForm.Outside:  name = Res.Strings.Property.Dimension.Outside;  break;
			}
			return name;
		}

		public static string GetIconText(DimensionForm type)
		{
			//	Retourne l'icône pour un type donné.
			switch ( type )
			{
				case DimensionForm.Auto:     return "DimensionAuto";
				case DimensionForm.Inside:   return "DimensionInside";
				case DimensionForm.Outside:  return "DimensionOutside";
			}
			return "";
		}

		public static string GetName(DimensionJustif type)
		{
			//	Retourne le nom d'un type donné.
			string name = "";
			switch ( type )
			{
				case DimensionJustif.CenterOrLeft:   name = Res.Strings.Property.Dimension.CenterOrLeft;   break;
				case DimensionJustif.CenterOrRight:  name = Res.Strings.Property.Dimension.CenterOrRight;  break;
				case DimensionJustif.Left:           name = Res.Strings.Property.Dimension.Left;           break;
				case DimensionJustif.Right:          name = Res.Strings.Property.Dimension.Right;          break;
			}
			return name;
		}

		public static string GetIconText(DimensionJustif type)
		{
			//	Retourne l'icône pour un type donné.
			switch ( type )
			{
				case DimensionJustif.CenterOrLeft:   return "DimensionCenterOrLeft";
				case DimensionJustif.CenterOrRight:  return "DimensionCenterOrRight";
				case DimensionJustif.Left:           return "DimensionLeft";
				case DimensionJustif.Right:          return "DimensionRight";
			}
			return "";
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
			Dimension p = property as Dimension;
			p.dimensionJustif = this.dimensionJustif;
			p.dimensionForm   = this.dimensionForm;
			p.addLength       = this.addLength;
			p.outLength       = this.outLength;
			p.fontOffset      = this.fontOffset;
			p.dimensionText   = this.dimensionText;
			p.rotateText      = this.rotateText;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			Dimension p = property as Dimension;
			if ( p.dimensionJustif != this.dimensionJustif )  return false;
			if ( p.dimensionForm   != this.dimensionForm   )  return false;
			if ( p.addLength       != this.addLength       )  return false;
			if ( p.outLength       != this.outLength       )  return false;
			if ( p.fontOffset      != this.fontOffset      )  return false;
			if ( p.dimensionText   != this.dimensionText   )  return false;
			if ( p.rotateText      != this.rotateText      )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Dimension(document);
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise la propriété.
			base.GetObjectData(info, context);

			info.AddValue("DimensionJustif", this.dimensionJustif);
			info.AddValue("DimensionForm", this.dimensionForm);
			info.AddValue("AddLength", this.addLength);
			info.AddValue("OutLength", this.outLength);
			info.AddValue("FontOffset", this.fontOffset);
			info.AddValue("DimensionText", this.dimensionText);
			info.AddValue("RotateText", this.rotateText);
		}

		protected Dimension(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise la propriété.
			this.dimensionJustif = (DimensionJustif) info.GetValue("DimensionJustif", typeof(DimensionJustif));
			this.dimensionForm = (DimensionForm) info.GetValue("DimensionForm", typeof(DimensionForm));
			this.addLength = info.GetDouble("AddLength");
			this.outLength = info.GetDouble("OutLength");
			this.fontOffset = info.GetDouble("FontOffset");
			this.rotateText = info.GetBoolean("RotateText");

			if ( this.document.IsRevisionGreaterOrEqual(1,0,25) )
			{
				this.dimensionText = info.GetString("DimensionText");
			}
			else
			{
				string prefix = info.GetString("Prefix");
				string postfix = info.GetString("Postfix");
				this.dimensionText = prefix + "#" + postfix;
			}
		}
		#endregion


		protected DimensionJustif		dimensionJustif;
		protected DimensionForm			dimensionForm;
		protected double				addLength;
		protected double				outLength;
		protected double				fontOffset;
		protected string				dimensionText;
		protected bool					rotateText;
	}
}
