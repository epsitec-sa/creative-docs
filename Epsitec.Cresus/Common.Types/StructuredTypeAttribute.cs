//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>StructuredTypeAttribute</c> attribute is used to decorate a class
	/// which implements a known <see cref="IStructuredType"/> interface.
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
	public sealed class StructuredTypeAttribute : System.Attribute
	{
		public StructuredTypeAttribute(string id)
		{
			this.structuredTypeId = Support.Druid.Parse (id);
		}

		public StructuredTypeAttribute(long id)
		{
			this.structuredTypeId = Support.Druid.FromLong (id);
		}

		public StructuredTypeAttribute(int module, int dev, int local)
		{
			this.structuredTypeId = new Support.Druid (module, dev, local);
		}


		public Support.Druid StructuredTypeId
		{
			get
			{
				return this.structuredTypeId;
			}
		}

		private Support.Druid structuredTypeId;
	}
}
