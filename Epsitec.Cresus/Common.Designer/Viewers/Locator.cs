using System.Collections.Generic;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de localiser une ressource dans un Viewer.
	/// </summary>
	public class Locator
	{
		public Locator(string moduleName, ResourceAccess.Type viewerType, Druid resource)
		{
			this.moduleName = moduleName;
			this.viewerType = viewerType;
			this.resource = resource;
		}

		public string ModuleName
		{
			get
			{
				return this.moduleName;
			}
		}

		public ResourceAccess.Type ViewerType
		{
			get
			{
				return this.viewerType;
			}
		}

		public Druid Resource
		{
			get
			{
				return this.resource;
			}
		}


		public static bool operator ==(Locator a, Locator b)
		{
			return (a.moduleName == b.moduleName) && (a.viewerType == b.viewerType) && (a.resource == b.resource);
		}
		
		public static bool operator !=(Locator a, Locator b)
		{
			return (a.moduleName != b.moduleName) || (a.viewerType != b.viewerType) || (a.resource != b.resource);
		}

		public override string ToString()
		{
			return System.String.Format("Module={0}, Type={1}, Resource={1}", this.moduleName, this.viewerType, this.resource);
		}
		
		public override bool Equals(object obj)
		{
			return (obj is Locator) && (this == (Locator) obj);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		

		protected string							moduleName;
		protected ResourceAccess.Type				viewerType;
		protected Druid								resource;						
	}
}
