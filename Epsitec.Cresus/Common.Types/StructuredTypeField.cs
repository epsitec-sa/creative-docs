//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[SerializationConverter (typeof (StructuredTypeField.SerializationConverter))]
	public class StructuredTypeField
	{
		public StructuredTypeField()
		{
		}

		public StructuredTypeField(string name, INamedType type)
		{
			this.Name = name;
			this.Type = type;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
				this.HandleNameOrTypeChanged ();
			}
		}

		public INamedType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
				this.HandleNameOrTypeChanged ();
			}
		}

		internal bool IsFullyDefined
		{
			get
			{
				return (this.name != null) && (this.type != null);
			}
		}

		internal void DefineContainer(StructuredTypeFieldCollection container)
		{
			this.container = container;
		}

		private void HandleNameOrTypeChanged()
		{
			if ((this.container != null) &&
				(this.IsFullyDefined))
			{
				this.container.NotifyFieldFullyDefined (this);
			}
		}

		#region SerializationConverter Class

		public class SerializationConverter : ISerializationConverter
		{
			#region ISerializationConverter Members

			public string ConvertToString(object value, IContextResolver context)
			{
				StructuredTypeField field = (StructuredTypeField) value;
				return string.Format ("{0};{1}", field.Name, field.Type.CaptionId);
			}

			public object ConvertFromString(string value, IContextResolver context)
			{
				string[] args = value.Split (';');
				StructuredTypeField field = new StructuredTypeField ();

				field.Name = args[0];
				//	TODO: ...

				return field;
			}

			#endregion
		}

		#endregion
		
		private StructuredTypeFieldCollection container;
		private string name;
		private INamedType type;
	}
}
