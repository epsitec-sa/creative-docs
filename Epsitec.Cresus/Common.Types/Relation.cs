//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public enum Relation : byte
	{
		None = 0,

		Reference=1,
		Bijective=2,
		Collection=3,
		Cluster=4,

		OneToOne=Bijective,
		OneToMany=Collection,
		ManyToMany=Cluster,
	}
}
