//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.CollectionType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>CollectionType</c> describes a collection of similarly typed items.
	/// </summary>
	public sealed class CollectionType : AbstractType, ICollectionType
	{
		public CollectionType()
			: base ("Collection")
		{
		}

		public CollectionType(Caption caption)
			: base (caption)
		{
		}


		/// <summary>
		/// Gets the type code for the type.
		/// </summary>
		/// <value>The type code.</value>
		public override TypeCode TypeCode
		{
			get
			{
				return TypeCode.Collection;
			}
		}



		#region ISystemType Members

		/// <summary>
		/// Gets the system type described by this object.
		/// </summary>
		/// <value>The system type described by this object.</value>
		public override System.Type				SystemType
		{
			get
			{
				return typeof (System.Collections.IEnumerable);
			}
		}

		#endregion

		#region ICollectionType Members

		/// <summary>
		/// Gets the type used by the items in the collection.
		/// </summary>
		/// <value>The type used by the items in the collection.</value>
		public INamedType						ItemType
		{
			get
			{
				return (INamedType) this.Caption.GetValue (CollectionType.ItemTypeProperty);
			}
		}

		#endregion

		public override bool IsValidValue(object value)
		{
			if (this.IsNullValue (value))
			{
				return this.IsNullable;
			}

			System.Type     valueType  = value.GetType ();
			INamedType      itemType   = this.ItemType;
			IDataConstraint constraint = itemType as IDataConstraint;

			if (TypeRosetta.DoesTypeImplementInterface (valueType, typeof (System.Collections.IEnumerable)))
			{
				if (constraint != null)
				{
					foreach (object item in (System.Collections.IEnumerable) value)
					{
						if (!constraint.IsValidValue (item))
						{
							return false;
						}
					}
				}
				
				return true;
			}
			
			return false;
		}

		public void DefineItemType(INamedType itemType)
		{
			if (itemType == null)
			{
				this.Caption.ClearValue (CollectionType.ItemTypeProperty);
			}
			else
			{
				this.Caption.SetValue (CollectionType.ItemTypeProperty, itemType);
			}
		}


		static CollectionType()
		{
			CollectionType.ItemTypeProperty.DefineSerializationConverter (new NamedTypeSerializationConverter ());
		}
		
		public static readonly DependencyProperty ItemTypeProperty = DependencyProperty.RegisterAttached ("ItemType", typeof (INamedType), typeof (CollectionType));
	}
}
