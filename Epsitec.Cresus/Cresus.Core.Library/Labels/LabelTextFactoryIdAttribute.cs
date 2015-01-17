//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Labels
{
	[System.AttributeUsage (System.AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
	public sealed class LabelTextFactoryIdAttribute : System.Attribute
	{
		public LabelTextFactoryIdAttribute()
			: this (0)
		{
		}

		public LabelTextFactoryIdAttribute(int id)
		{
			this.id = id;
		}

		public int Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly int id;
	}
}
