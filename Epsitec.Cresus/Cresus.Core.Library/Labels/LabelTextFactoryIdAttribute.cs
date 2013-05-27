using System;

namespace Epsitec.Cresus.Core.Labels
{
	[AttributeUsage (AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
	public sealed class LabelTextFactoryIdAttribute : Attribute
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
