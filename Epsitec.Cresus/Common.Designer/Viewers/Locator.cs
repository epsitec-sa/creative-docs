using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de localiser une ressource dans un Viewer.
	/// </summary>
	public class Locator : System.IEquatable<Locator>
	{
		public Locator(string moduleName, ResourceAccess.Type viewerType, int subView, Druid resource, Widget widgetFocused, int lineSelected)
		{
			this.moduleName = moduleName;
			this.viewerType = viewerType;
			this.subView = subView;
			this.resource = resource;
			this.widgetFocused = widgetFocused;
			this.lineSelected = lineSelected;
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

		public int SubView
		{
			get
			{
				return this.subView;
			}
		}

		public Druid Resource
		{
			get
			{
				return this.resource;
			}
		}

		public Widget WidgetFocused
		{
			get
			{
				return this.widgetFocused;
			}
		}

		public int LineSelected
		{
			get
			{
				return this.lineSelected;
			}
		}


		public static bool operator ==(Locator a, Locator b)
		{
			return (a.moduleName == b.moduleName) &&
				   (a.viewerType == b.viewerType) &&
				   (a.subView == b.subView) &&
				   (a.resource == b.resource);
		}
		
		public static bool operator !=(Locator a, Locator b)
		{
			return (a.moduleName != b.moduleName) ||
				   (a.viewerType != b.viewerType) ||
				   (a.subView != b.subView) ||
				   (a.resource != b.resource);
		}

		public override string ToString()
		{
			return System.String.Format("Module={0}, Type={1}, Sous-vue={2}, Resource={3}", this.moduleName, this.viewerType, this.subView, this.resource);
		}
		
		public override bool Equals(object obj)
		{
			return (obj is Locator) && (this == (Locator) obj);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		

		#region IEquatable<Locator> Members
		public bool Equals(Locator other)
		{
			//	Two locators can only be equal if they are represented by the same
			//	instance in memory :
			return object.ReferenceEquals(this, other);
		}
		#endregion


		protected string							moduleName;
		protected ResourceAccess.Type				viewerType;
		protected int								subView;
		protected Druid								resource;
		protected Widget							widgetFocused;
		protected int								lineSelected;
	}
}
