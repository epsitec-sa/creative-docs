//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 20/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTypeNum définit les paramètres d'une valeur numérique.
	/// </summary>
	public class DbTypeNum : DbType
	{
		public DbTypeNum(DbNumDef num_def) : base (DbSimpleType.Decimal)
		{
			this.num_def = num_def;
		}
		
		public DbTypeNum(DbNumDef num_def, params string[] attributes) : base (DbSimpleType.Decimal, attributes)
		{
			this.num_def = num_def;
		}
		
		public DbTypeNum(System.Xml.XmlElement xml) : base (DbSimpleType.Decimal)
		{
			string arg_digits = xml.GetAttribute ("digits");
			string arg_shift  = xml.GetAttribute ("shift");
			string arg_min    = xml.GetAttribute ("min");
			string arg_max    = xml.GetAttribute ("max");
			string arg_type   = xml.GetAttribute ("type");
			
			if (arg_type != "")
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
		
		
		internal override void SerialiseXmlAttributes(System.Text.StringBuilder buffer)
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
		}
		
		
		public DbNumDef						NumDef
		{
			get { return this.num_def; }
		}
		
		
		public override object Clone()
		{
			DbTypeNum type = base.Clone () as DbTypeNum;
			
			type.num_def = (this.num_def == null) ? null : this.num_def.Clone () as DbNumDef;
			
			return type;
		}
		
		
		protected DbNumDef					num_def;
	}
}
