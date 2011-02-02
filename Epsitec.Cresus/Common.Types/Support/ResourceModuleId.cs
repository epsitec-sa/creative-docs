//	Copyright © 2006-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceModuleId</c> structure stores the name and the identifier
	/// of a resource module.
	/// </summary>

	[System.ComponentModel.TypeConverter (typeof (ResourceModuleId.Converter))]
	
	public struct ResourceModuleId : System.IEquatable<ResourceModuleId>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceModuleId"/> structure.
		/// </summary>
		/// <param name="name">The name of the module.</param>
		/// <param name="id">The id of the module.</param>
		public ResourceModuleId(string name, int id)
		{
			this.name = string.IsNullOrEmpty (name) ? null : name;
			this.path = null;
			this.id = id+1;
			this.layer = ResourceModuleLayer.Undefined;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceModuleId"/> structure.
		/// </summary>
		/// <param name="name">The name of the module.</param>
		/// <param name="path">The path to the module.</param>
		/// <param name="id">The id of the module.</param>
		public ResourceModuleId(string name, string path, int id)
		{
			this.name = string.IsNullOrEmpty (name) ? null : name;
			this.path = string.IsNullOrEmpty (path) ? null : path;
			this.id = id+1;
			this.layer = ResourceModuleLayer.Undefined;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceModuleId"/> structure.
		/// </summary>
		/// <param name="name">The name of the module.</param>
		/// <param name="path">The path to the module.</param>
		/// <param name="id">The id of the module.</param>
		/// <param name="layer">The application layer of the module.</param>
		public ResourceModuleId(string name, string path, int id, ResourceModuleLayer layer)
		{
			this.name = string.IsNullOrEmpty (name) ? null : name;
			this.path = string.IsNullOrEmpty (path) ? null : path;
			this.id = id+1;
			this.layer = layer;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceModuleId"/> structure.
		/// </summary>
		/// <param name="name">The name of the module.</param>
		public ResourceModuleId(string name)
		{
			this.name = string.IsNullOrEmpty (name) ? null : name;
			this.path = null;
			this.id = 0;
			this.layer = ResourceModuleLayer.Undefined;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceModuleId"/> structure.
		/// </summary>
		/// <param name="id">The id of the module.</param>
		public ResourceModuleId(int id)
		{
			this.name = null;
			this.path = null;
			this.id = id+1;
			this.layer = ResourceModuleLayer.Undefined;
		}

		/// <summary>
		/// Gets the name of the module.
		/// </summary>
		/// <value>The name of the module.</value>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Gets the path of the module.
		/// </summary>
		/// <value>The path of the module.</value>
		public string Path
		{
			get
			{
				return this.path;
			}
		}

		/// <summary>
		/// Gets the id of the module.
		/// </summary>
		/// <value>The id of the module.</value>
		public int Id
		{
			get
			{
				return this.id-1;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is empty.
		/// </summary>
		/// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
		public bool IsEmpty
		{
			get
			{
				return this.id == 0;
			}
		}

		/// <summary>
		/// Gets the layer of the module.
		/// </summary>
		/// <value>The layer of the module.</value>
		public ResourceModuleLayer Layer
		{
			get
			{
				return this.layer;
			}
		}


		public static readonly ResourceModuleId Empty = new ResourceModuleId ();

		/// <summary>
		/// Returns the module id as a string representation.
		/// </summary>
		/// <returns>
		/// The module id if it is valid; otherwise, <c>null</c>.
		/// </returns>
		public override string ToString()
		{
			int id = this.Id;

			if (id < 0)
			{
				return null;
			}
			else
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}", id);
			}
		}

		/// <summary>
		/// Parses the specified string, which can be either the module name or a
		/// module id.
		/// </summary>
		/// <param name="module">Name or id of the module.</param>
		/// <returns>The module information structure.</returns>
		public static ResourceModuleId Parse(string module)
		{
			int moduleId;

			if (int.TryParse (module, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out moduleId))
			{
				return new ResourceModuleId (moduleId);
			}
			else
			{
				return new ResourceModuleId (module);
			}
		}

		/// <summary>
		/// Converts the layer to a string prefix (e.g. maps <c>Application</c>
		/// to <c>"A"</c>).
		/// </summary>
		/// <param name="layer">The layer.</param>
		/// <returns>The string prefix.</returns>
		public static string ConvertLayerToPrefix(ResourceModuleLayer layer)
		{
			switch (layer)
			{
				case ResourceModuleLayer.Application:
					return "A";

				case ResourceModuleLayer.Customization1:
					return "C";

				case ResourceModuleLayer.Customization2:
					return "D";

				case ResourceModuleLayer.Customization3:
					return "E";

				case ResourceModuleLayer.User:
					return "U";

				case ResourceModuleLayer.System:
					return "s";

				default:
					throw new System.ArgumentOutOfRangeException ("layer");
			}
		}

		/// <summary>
		/// Converts the string prefix  to a layer (e.g. maps <c>"A"</c> to
		/// <c>Application</c>).
		/// </summary>
		/// <param name="prefix">The string prefix.</param>
		/// <returns>The layer.</returns>
		public static ResourceModuleLayer ConvertPrefixToLayer(string prefix)
		{
			switch (prefix)
			{
				case "A":
					return ResourceModuleLayer.Application;
				
				case "C":
					return ResourceModuleLayer.Customization1;
				
				case "D":
					return ResourceModuleLayer.Customization2;
				
				case "E":
					return ResourceModuleLayer.Customization3;
				
				case "U":
					return ResourceModuleLayer.User;

				case "s":
					return ResourceModuleLayer.System;

				default:
					throw new System.ArgumentException ();
			}
		}

		#region IEquatable<ResourceModuleId> Members

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// <c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.
		/// </returns>
		public bool Equals(ResourceModuleId other)
		{
			return (this.id == other.id) && (this.name == other.name) && (this.path == other.path);
		}

		#endregion

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>
		/// <c>true</c> if obj and this instance are the same type and represent the same value; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj is ResourceModuleId)
			{
				return this.Equals ((ResourceModuleId) obj);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		public override int GetHashCode()
		{
			if (this.name == null)
			{
				return this.id;
			}
			else if (this.id == 0)
			{
				return this.name.GetHashCode ();
			}
			else
			{
				return this.id;
			}
		}

		#region Private Fields
		
		private string							name;			//	null or name
		private string							path;			//	module path or null
		private int								id;				//	0 or module id+1
		private ResourceModuleLayer				layer;			//	module layer
		
		#endregion
		
		#region Converter Class

		public class Converter : Types.AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				string[] args = value.Split (';');

				string name = args[0];
				int id = int.Parse (args[1], System.Globalization.NumberStyles.Integer, culture);
				string path = args[2];
				ResourceModuleLayer layer = ResourceModuleId.ConvertPrefixToLayer (args[3]);

				return new ResourceModuleId (name, path, id, layer);
			}

			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				ResourceModuleId moduleId = (ResourceModuleId) value;
				string[] args = new string[4];

				args[0] = moduleId.Name ?? "";
				args[1] = moduleId.Id.ToString (System.Globalization.CultureInfo.InvariantCulture);
				args[2] = moduleId.Path ?? "";
				args[3] = ResourceModuleId.ConvertLayerToPrefix (moduleId.Layer);
				
				return string.Join (";", args);
			}
		}
		#endregion

	}
}
