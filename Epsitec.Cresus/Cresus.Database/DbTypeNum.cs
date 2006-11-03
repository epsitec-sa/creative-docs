//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTypeNum définit les paramètres d'une valeur numérique.
	/// </summary>
	public class DbTypeNum : DbType, Common.Types.INumericType
	{
		internal DbTypeNum() : base (DbSimpleType.Decimal)
		{
		}
		
		
		public DbTypeNum(DbNumDef num_def) : base (DbSimpleType.Decimal)
		{
			this.num_def = num_def;
		}
		
		public DbTypeNum(DbNumDef num_def, params string[] attributes) : base (DbSimpleType.Decimal, attributes)
		{
			this.num_def = num_def;
		}
		
		
		public DbNumDef							NumDef
		{
			get { return this.num_def; }
		}
		
		public System.Type						SystemType
		{
			get
			{
				return typeof (decimal);
			}
		}

		public string							DefaultController
		{
			get
			{
				return "Numeric";
			}
		}
		
		public string							DefaultControllerParameter
		{
			get
			{
				return null;
			}
		}

		public Common.Types.DecimalRange		Range
		{
			get
			{
				return this.num_def.ToDecimalRange ();
			}
		}

		public Common.Types.DecimalRange		PreferredRange
		{
			get
			{
				return Common.Types.DecimalRange.Empty;
			}
		}

		public decimal							SmallStep
		{
			get
			{
				return 0;
			}
		}

		public decimal							LargeStep
		{
			get
			{
				return 0;
			}
		}
		
		
		internal override void SerializeXmlAttributes(System.Text.StringBuilder buffer, bool full)
		{
			if (this.num_def.InternalRawType != DbRawType.Unsupported)
			{
				buffer.Append (@" type=""");
				buffer.Append (this.num_def.InternalRawType.ToString ());
				buffer.Append (@"""");
			}
			else
			{
				buffer.Append (@" digits=""");
				buffer.Append (this.num_def.DigitPrecision.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append (@""" shift=""");
				buffer.Append (this.num_def.DigitShift.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append (@""" min=""");
				buffer.Append (this.num_def.MinValue.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append (@""" max=""");
				buffer.Append (this.num_def.MaxValue.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append (@"""");
			}
			
			base.SerializeXmlAttributes (buffer, full);
		}
		
		internal override void DeserializeXmlAttributes(System.Xml.XmlElement xml)
		{
			base.DeserializeXmlAttributes (xml);
			
			string arg_digits = xml.GetAttribute ("digits");
			string arg_shift  = xml.GetAttribute ("shift");
			string arg_min    = xml.GetAttribute ("min");
			string arg_max    = xml.GetAttribute ("max");
			string arg_type   = xml.GetAttribute ("type");
			
			if (arg_type.Length > 0)
			{
				//	Le type numérique correspond à un type brut natif.
				
				DbRawType raw_type = (DbRawType) System.Enum.Parse (typeof (DbRawType), arg_type, true);
				
				this.num_def = DbNumDef.FromRawType (raw_type);
			}
			else
			{
				//	Si ce n'est pas un type prédéfini, on utilise les paramètres stockés
				//	indépendamment.
				
				int digits = System.Int32.Parse (arg_digits, System.Globalization.CultureInfo.InvariantCulture);
				int shift  = System.Int32.Parse (arg_shift, System.Globalization.CultureInfo.InvariantCulture);
				
				decimal min = System.Decimal.Parse (arg_min, System.Globalization.CultureInfo.InvariantCulture);
				decimal max = System.Decimal.Parse (arg_max, System.Globalization.CultureInfo.InvariantCulture);
				
				this.num_def = new DbNumDef (digits, shift, min, max);
			}
		}
		
		
		protected override object CloneNewObject()
		{
			return new DbTypeNum ();
		}
		
		protected override object CloneCopyToNewObject(object o)
		{
			DbTypeNum that = o as DbTypeNum;
			
			base.CloneCopyToNewObject (that);
			
			that.num_def = (this.num_def == null) ? null : this.num_def.Clone () as DbNumDef;
			
			return that;
		}
		
		
		private DbNumDef						num_def;

		bool Epsitec.Common.Types.INumericType.UseCompactStorage
		{
			get
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}
		}
	}
}
