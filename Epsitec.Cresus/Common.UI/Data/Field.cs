//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.UI.Data
{
	/// <summary>
	/// La classe Field d�crit un champ d'un Record, utilis� pour �changer des
	/// donn�es entre une application et son interface via mapper/binder/...
	/// </summary>
	public class Field : Types.IDataValue, Support.Data.IChangedSource
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
			if (type == null)
			{
				this.DefineValue (value);
			}
			else
			{
				this.DefineValue (value, type);
			}
			
			if (type is Types.IDataConstraint)
			{
				this.DefineConstraint (type as Types.IDataConstraint);
			}
		}
		
		public Field(string name, object value, Types.INamedType type, Types.IDataConstraint constraint) : this (name, value, type)
		{
			this.DefineConstraint (constraint);
		}
		
		
		public void DefineValue(object value)
		{
			if (value is string)
			{
				this.value          = value;
				this.is_value_valid = true;
				this.type           = new Types.StringType ();
				
				return;
			}
			if (value is decimal)
			{
				decimal large = 1000000000000M;
				decimal small = 1M / large;
				decimal max   = large - small;
				
				this.value          = value;
				this.is_value_valid = true;
				this.type           = new Types.DecimalType (-max, max, small);
				
				return;
			}
			if (value is bool)
			{
				this.value          = value;
				this.is_value_valid = true;
				this.type           = new Types.BooleanType ();
				
				return;
			}
			if (value is int)
			{
				this.value          = value;
				this.is_value_valid = true;
				this.type           = new Types.IntegerType ();
				
				return;
			}
			if (value is System.Enum)
			{
				this.value          = value;
				this.is_value_valid = true;
				this.type           = new Types.EnumType (value.GetType ());
				
				return;
			}
			
			throw new System.ArgumentException (string.Format ("The specified value's type is not supported ({0}).", value.GetType ()));
		}
		
		public void DefineValue(object value, Types.INamedType type)
		{
			this.value          = value;
			this.is_value_valid = true;
			this.type           = type;
		}
		
		public void DefineConstraint(Types.IDataConstraint constraint)
		{
			this.constraint = constraint;
		}
		
		public void DefineCaption(string caption)
		{
			this.caption = caption;
		}
		
		public void DefineDescription(string description)
		{
			this.description = description;
		}
		
		
		public object							Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if ((this.is_value_valid == false) ||
					(Common.Types.Comparer.Equal (this.value, value) == false))
				{
					if (this.type == null)
					{
						throw new System.InvalidOperationException ("Value cannot be changed; no type is defined.");
					}
					
					this.value          = value;
					this.is_value_valid = true;
					this.OnChanged ();
				}
			}
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
		
		public bool								IsValueValid
		{
			get
			{
				return this.is_value_valid;
			}
		}
		
		public object ReadValue()
		{
			object value = this.Value;
			
			if ((value != null) &&
				(this.type != null) &&
				(value.GetType () != this.type.SystemType))
			{
				if (value is string)
				{
					if (Common.Types.Converter.Convert (value, this.type.SystemType, out value))
					{
						return value;
					}
				}
				
				throw new System.InvalidOperationException ("Value is not of required type.");
			}
			
			return value;
		}

		public void WriteValue(object value)
		{
			if (this.type != null)
			{
				if (Common.Types.Converter.Convert (value, this.type.SystemType, out value))
				{
					this.Value = value;
				}
				else
				{
					throw new System.InvalidOperationException ("Value cannot be mapped to required type.");
				}
			}
			else
			{
				this.Value = value;
			}
		}
		
		public void NotifyInvalidData()
		{
			//	Donn�e pas valide...
			
			this.is_value_valid = false;
			this.OnChanged ();
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
		
		#region IChangedSource Members
		public event Support.EventHandler		Changed;
		#endregion
		
		public static Field CreateFromValue(Types.IDataValue value)
		{
			Field field = new Field (value.Name, null, value.DataType, value.DataConstraint);
			
			field.DefineCaption (value.Caption);
			field.DefineDescription (value.Description);
			
			return field;
		}
		
		
		protected virtual void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}
		
		
		private string							name;
		private string							caption;
		private string							description;
		
		private Types.INamedType				type;
		private Types.IDataConstraint			constraint;
		
		private object							value;
		private bool							is_value_valid;
	}
}
