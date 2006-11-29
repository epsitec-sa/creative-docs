//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>Relation</c> enumeration defines what relation binds two
	/// structures (or tables, when speaking the database lingua). See
	/// <see cref="StructuredTypeField"/>.
	/// </summary>
	public enum Relation : byte
	{
		/// <summary>
		/// There is no relation defined for this field.
		/// </summary>
		None=0,

		/// <summary>
		/// The field defines a reference (pointer) to another structure.
		/// </summary>
		Reference=1,
		
		/// <summary>
		/// The field defines a bijective reference (bidirectional pointer)
		/// with another structure. This a synonym for <c>OneToOne</c>.
		/// </summary>
		Bijective=2,
		
		/// <summary>
		/// The field defines a collection of structures. This is a synonym
		/// for <c>OneToMany</c>.
		/// </summary>
		Collection=3,
		
		/// <summary>
		/// The field defines a bijective cluster reference; this means that
		/// several source structures reference several target structures,
		/// and the other way round too. This is a synonym for <c>ManyToMany</c>.
		/// </summary>
		Cluster=4,

		/// <summary>
		/// The field defines a one-to-one relation. This is a synonym for
		/// <c>Bijective</c>.
		/// </summary>
		OneToOne=Bijective,

		/// <summary>
		/// The field defines a one-to-many relation. This is a synonym for
		/// <c>Collection</c>.
		/// </summary>
		OneToMany=Collection,
		
		/// <summary>
		/// The field defines a many-to-many relation. This is a synonym for
		/// <c>Cluster</c>.
		/// </summary>
		ManyToMany=Cluster,
	}
}
