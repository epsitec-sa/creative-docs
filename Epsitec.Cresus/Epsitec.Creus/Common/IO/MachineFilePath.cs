//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.IO
{
	/// <summary>
	/// The <c>MachineFilePath</c> class represents a file path, including the name of the
	/// machine and of the user who accesses it.
	/// </summary>
	public sealed class MachineFilePath : System.IEquatable<MachineFilePath>
	{
		public MachineFilePath(System.IO.FileInfo fileInfo)
			: this (fileInfo.FullName)
		{
		}

		public MachineFilePath(string path)
			: this (System.IO.Path.GetFullPath (path), System.Environment.MachineName, System.Environment.UserName)
		{
		}

		public MachineFilePath(MachineFilePath path)
			: this (path.FullPath, path.MachineName, path.UserName)
		{
		}

		private MachineFilePath(string fullPath, string machineName, string userName)
		{
			this.fullPath = fullPath;
			this.machineName = machineName;
			this.userName = userName;

			this.AssertValidNames ();
		}


		public string							FullPath
		{
			get
			{
				return this.fullPath;
			}
		}

		public string							MachineName
		{
			get
			{
				return this.machineName;
			}
		}

		public string							UserName
		{
			get
			{
				return this.userName;
			}
		}


		#region IEquatable<MachineFilePath> Members

		public bool Equals(MachineFilePath other)
		{
			if (other == null)
			{
				return false;
			}

			return string.Equals (this.fullPath, other.fullPath, System.StringComparison.OrdinalIgnoreCase)
				&& string.Equals (this.machineName, other.machineName, System.StringComparison.OrdinalIgnoreCase)
				&& string.Equals (this.userName, other.userName, System.StringComparison.OrdinalIgnoreCase);
		}

		#endregion

		public override string ToString()
		{
			return string.Concat (this.fullPath, ";", this.userName, "@", this.machineName);
		}

		public override bool Equals(object obj)
		{
			return this.Equals (obj as MachineFilePath);
		}

		public override int GetHashCode()
		{
			return this.fullPath.GetHashCode () ^ this.machineName.GetHashCode () ^ this.userName.GetHashCode ();
		}

		public static MachineFilePath Parse(string text)
		{
			string[] args = text.Split (';');

			if (args.Length != 2)
			{
				throw new System.FormatException ("Invalid machine file path (1)");
			}

			string[] machineUser = args[1].Split ('@');

			if (machineUser.Length != 2)
            {
				throw new System.FormatException ("Invalid machine file path (2)");
            }

			return new MachineFilePath (fullPath: args[0], machineName: machineUser[1], userName: machineUser[0]);
		}

		private void AssertValidNames()
		{
			if (this.machineName == null)
			{
				throw new System.ArgumentNullException ("machineName");
			}
			if (this.userName == null)
			{
				throw new System.ArgumentNullException ("userName");
			}

			if ((this.machineName.Contains (";")) ||
				(this.machineName.Contains ("@")))
			{
				throw new System.ArgumentException ("Invalid machine name");
			}

			if ((this.userName.Contains (";")) ||
				(this.userName.Contains ("@")))
			{
				throw new System.ArgumentException ("Invalid user name");
			}
		}

		private readonly string fullPath;
		private readonly string machineName;
		private readonly string userName;
	}
}
