namespace Epsitec.Common.Widgets
{
	public enum RealUnitType
	{
		None                = 0,
		Scalar              = 1,		// valeur scalaire
		Percent             = 2,		// pourcent				

		DimensionMicrometer = 100,
		DimensionMillimeter,
		DimensionCentimeter,
		DimensionMeter,
		DimensionInch,

		AngleDeg            = 200,
		AngleRad,
	}

	/// <summary>
	/// La classe TextFieldReal implémente la ligne éditable numérique
	/// avec slider et unité réelle.
	/// </summary>
	public class TextFieldReal : TextFieldSlider
	{
		public TextFieldReal() : base()
		{
			if ( Support.ObjectBundler.IsBooting )
			{
				//	N'initialise rien, car cela prend passablement de temps... et de toute
				//	manière, on n'a pas besoin de toutes ces informations pour pouvoir
				//	utiliser IBundleSupport.
				
				return;
			}
			
			this.realUnitType = RealUnitType.None;
			this.realScale = 1.0M;
			this.factorMinRange = -1.0M;
			this.factorMaxRange = 1.0M;
			this.factorStep = 1.0M;
		}
		
		public TextFieldReal(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public RealUnitType						UnitType
		{
			get
			{
				return this.realUnitType;
			}

			set
			{
				this.realUnitType = value;
			}
		}

		public bool								IsDimension
		{
			get
			{
				return ( this.realUnitType == RealUnitType.DimensionMicrometer ||
						 this.realUnitType == RealUnitType.DimensionMillimeter ||
						 this.realUnitType == RealUnitType.DimensionCentimeter ||
						 this.realUnitType == RealUnitType.DimensionMeter      ||
						 this.realUnitType == RealUnitType.DimensionInch       );
			}
		}

		public bool								IsAngle
		{
			get
			{
				return ( this.realUnitType == RealUnitType.AngleDeg ||
						 this.realUnitType == RealUnitType.AngleRad );
			}
		}

		public decimal							Scale
		{
			get
			{
				return this.realScale;
			}

			set
			{
				this.realScale = value;
			}
		}

		public decimal							InternalValue
		{
			get
			{
				return this.RealToInternal(this.Value);
			}

			set
			{
				this.Value = this.InternalToReal(value);
			}
		}

		public decimal							InternalMinValue
		{
			get
			{
				return this.RealToInternal(this.MinValue);
			}

			set
			{
				this.MinValue = this.InternalToReal(value);
			}
		}
		
		public decimal							InternalMaxValue
		{
			get
			{
				return this.RealToInternal(this.MaxValue);
			}

			set
			{
				this.MaxValue = this.InternalToReal(value);
			}
		}


		public decimal							FactorMinRange
		{
			get
			{
				return this.factorMinRange;
			}

			set
			{
				this.factorMinRange = value;
			}
		}

		public decimal							FactorMaxRange
		{
			get
			{
				return this.factorMaxRange;
			}

			set
			{
				this.factorMaxRange = value;
			}
		}

		public decimal							FactorStep
		{
			get
			{
				return this.factorStep;
			}

			set
			{
				this.factorStep = value;
			}
		}


		public decimal InternalToReal(decimal value)
		{
			if ( this.realUnitType == RealUnitType.None )
			{
				return value;
			}

			if ( this.realUnitType == RealUnitType.Percent )
			{
				return value*100.0M;
			}

			return value/this.realScale;
		}

		public decimal RealToInternal(decimal value)
		{
			if ( this.realUnitType == RealUnitType.None )
			{
				return value;
			}

			if ( this.realUnitType == RealUnitType.Percent )
			{
				return value/100.0M;
			}

			return value*this.realScale;
		}


		protected RealUnitType			realUnitType;
		protected decimal				realScale;
		protected decimal				factorMinRange;
		protected decimal				factorMaxRange;
		protected decimal				factorStep;
	}
}
