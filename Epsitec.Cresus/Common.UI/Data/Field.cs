//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 26/04/2004

namespace Epsitec.Common.UI.Data
{
	/// <summary>
	/// La classe Field décrit un champ d'un Record, utilisé pour échanger des
	/// données entre une application et son interface via mapper/binder/...
	/// </summary>
	public class Field : Types.IDataValue
	{
		public Field()
		{
		}
		
		public Field(string name)
		{
			this.name        = name;
			this.caption     = null;
			this.description = null;
		}
		
		public Field(string name, object value) : this (name)
		{
			this.DefineValue (value);
		}
		
		public Field(string name, object value, Types.INamedType type) : this (name)
		{
			this.DefineValue (value, type);
		}
		
		public Field(string name, object value, Types.INamedType type, Types.IDataConstraint constraint) : this (name, value, type)
		{
			this.DefineConstraint (constraint);
		}
		
		
		public void DefineValue(object value)
		{
			if (value is string)
			{
				this.value = value;
				this.type  = new Types.StringType ();
				
				return;
			}
			if (value is decimal)
			{
				decimal large = 1000000000000M;
				decimal small = 1M / large;
				decimal max   = large - small;
				
				this.value = value;
				this.type  = new Types.DecimalType (-max, max, small);
				
				return;
			}
			if (value is int)
			{
				this.value = value;
				this.type  = new Types.DecimalType ();
				
				return;
			}
			
			throw new System.ArgumentException (string.Format ("The specified value's type is not supported ({0}).", value.GetType ()));
		}
		
		public void DefineValue(object value, Types.INamedType type)
		{
			this.value = value;
			this.type  = type;
		}
		
		public void DefineConstraint(Types.IDataConstraint constraint)
		{
			this.constraint = constraint;
		}
		
		
		#region IDataValue Members
		public Types.INamedType					DataType
		{
			get
			{
				return this.type;
			}
		}
		
		public Types.IDataConstraint			DataConstraint
		{
			get
			{
				return this.constraint;
			}
		}
		
		public object ReadValue()
		{
			return this.value;
		}

		public void WriteValue(object value)
		{
			this.value = value;
		}
		#endregion

		#region IDataItem Members
		public Types.DataItemClasses			Classes
		{
			get
			{
				return Types.DataItemClasses.Value;
			}
		}
		#endregion

		#region INameCaption Members
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		public string							Caption
		{
			get
			{
				return this.caption;
			}
		}
		
		public string							Description
		{
			get
			{
				return this.description;
			}
		}
		#endregion
		
		private string							name;
		private string							caption;
		private string							description;
		
		private Types.INamedType				type;
		private Types.IDataConstraint			constraint;
		
		private object							value;
	}
}
