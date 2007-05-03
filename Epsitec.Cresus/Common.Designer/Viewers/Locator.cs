using System.Collections.Generic;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de localiser une ressource dans un Viewer.
	/// </summary>
	public class Locator
	{
		public Locator(ResourceAccess.Type viewerType, Druid resource)
		{
			this.viewerType = viewerType;
			this.resource = resource;
		}

		public ResourceAccess.Type ViewerType
		{
			get
			{
				return this.viewerType;
			}
			set
			{
				this.viewerType = value;
			}
		}

		public Druid Resource
		{
			get
			{
				return this.resource;
			}
			set
			{
				this.resource = value;
			}
		}


		public static bool operator ==(Locator a, Locator b)
		{
			return (a.viewerType == b.viewerType) && (a.resource == b.resource);
		}
		
		public static bool operator !=(Locator a, Locator b)
		{
			return (a.viewerType != b.viewerType) || (a.resource != b.resource);
		}

		public override string ToString()
		{
			return System.String.Format("Type={0}, Resource={1}", this.viewerType, this.resource);
		}
		
		public override bool Equals(object obj)
		{
			return (obj is Locator) && (this == (Locator) obj);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		

		protected ResourceAccess.Type				viewerType;
		protected Druid								resource;						
	}
}
