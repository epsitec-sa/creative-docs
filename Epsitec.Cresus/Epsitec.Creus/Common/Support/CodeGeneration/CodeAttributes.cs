//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeGeneration
{
	/// <summary>
	/// The <c>CodeAttributes</c> structure describes the accessibility, visibility
	/// and other method and property related attributes.
	/// </summary>
	public struct CodeAttributes
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodeAttributes"/> struct.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		public CodeAttributes(CodeAttributes attributes)
		{
			this.accessibility = attributes.accessibility;
			this.visibility = attributes.visibility;
			this.isReadonly = attributes.isReadonly;
			this.isConst = attributes.isConst;
			this.isNew = attributes.isNew;
			this.isPartial = attributes.isPartial;
			this.isPartialDefinition = attributes.isPartialDefinition;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeAttributes"/> struct.
		/// </summary>
		/// <param name="accessibility">The code accessibility.</param>
		public CodeAttributes(CodeAccessibility accessibility)
		{
			this.accessibility = accessibility;
			this.visibility = CodeVisibility.Public;
			this.isReadonly = false;
			this.isConst = false;
			this.isNew = false;
			this.isPartial = false;
			this.isPartialDefinition = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeAttributes"/> struct.
		/// </summary>
		/// <param name="visibility">The code visibility.</param>
		public CodeAttributes(CodeVisibility visibility)
		{
			this.accessibility = CodeAccessibility.Default;
			this.visibility = visibility;
			this.isReadonly = false;
			this.isConst = false;
			this.isNew = false;
			this.isPartial = false;
			this.isPartialDefinition = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeAttributes"/> struct.
		/// </summary>
		/// <param name="visibility">The code visibility.</param>
		/// <param name="accessibility">The code accessibility.</param>
		public CodeAttributes(CodeVisibility visibility, CodeAccessibility accessibility)
		{
			this.accessibility = accessibility;
			this.visibility = visibility;
			this.isReadonly = false;
			this.isConst = false;
			this.isNew = false;
			this.isPartial = false;
			this.isPartialDefinition = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeAttributes"/> struct.
		/// </summary>
		/// <param name="visibility">The code visibility.</param>
		/// <param name="attributes">One or more attributes (such as <see cref="CodeAttributes.ReadOnlyAttribute"/> or <see cref="CodeAttributes.NewAttribute"/>).</param>
		public CodeAttributes(CodeVisibility visibility, params object[] attributes)
			: this (visibility, CodeAccessibility.Default, attributes)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeAttributes"/> struct.
		/// </summary>
		/// <param name="visibility">The code visibility.</param>
		/// <param name="accessibility">The code accessibility.</param>
		/// <param name="attributes">One or more attributes (such as <see cref="CodeAttributes.ReadOnlyAttribute"/> or <see cref="CodeAttributes.NewAttribute"/>).</param>
		public CodeAttributes(CodeVisibility visibility, CodeAccessibility accessibility, params object[] attributes)
		{
			this.accessibility = accessibility;
			this.visibility = visibility;
			this.isReadonly = false;
			this.isConst = false;
			this.isNew = false;
			this.isPartial = false;
			this.isPartialDefinition = false;

			foreach (object attribute in attributes)
			{
				if (attribute == CodeAttributes.ReadOnlyAttribute)
				{
					this.isReadonly = true;
				}
				else if (attribute == CodeAttributes.ConstAttribute)
				{
					this.isConst = true;
				}
				else if (attribute == CodeAttributes.NewAttribute)
				{
					this.isNew = true;
				}
				else if (attribute == CodeAttributes.PartialAttribute)
				{
					this.isPartial = true;
				}
				else if (attribute == CodeAttributes.PartialDefinitionAttribute)
				{
					this.isPartial = true;
					this.isPartialDefinition = true;
				}
			}
		}

		/// <summary>
		/// Gets the code accessibility.
		/// </summary>
		/// <value>The code accessibility.</value>
		public CodeAccessibility				Accessibility
		{
			get
			{
				return this.accessibility;
			}
		}

		/// <summary>
		/// Gets the code visibility.
		/// </summary>
		/// <value>The code visibility.</value>
		public CodeVisibility					Visibility
		{
			get
			{
				return this.visibility;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the access is read only and should specify
		/// the <c>readonly</c> keyword.
		/// </summary>
		/// <value><c>true</c> if the access is read only; otherwise, <c>false</c>.</value>
		public bool								IsReadonly
		{
			get
			{
				return this.isReadonly;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the access is constant and should specify
		/// the <c>const</c> keyword.
		/// </summary>
		/// <value><c>true</c> if the access is const; otherwise, <c>false</c>.</value>
		public bool IsConst
		{
			get
			{
				return this.isConst;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the attribute should specify the <c>new</c>
		/// keyword.
		/// </summary>
		/// <value><c>true</c> if the attribute should specify the <c>new</c> keyword;
		/// otherwise, <c>false</c>.</value>
		public bool								IsNew
		{
			get
			{
				return this.isNew;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the attribute should specify the <c>partial</c>
		/// keyword.
		/// </summary>
		/// <value><c>true</c> if the attribute should specify the <c>partial</c> keyword;
		/// otherwise, <c>false</c>.</value>
		public bool								IsPartial
		{
			get
			{
				return this.isPartial;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the attribute should .
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is partial definition; otherwise, <c>false</c>.
		/// </value>
		public bool								IsPartialDefinition
		{
			get
			{
				return this.isPartialDefinition;
			}
		}

		/// <summary>
		/// Gets the attributes used to initialize this instance.
		/// </summary>
		/// <value>The attributes.</value>
		public object[]							Attributes
		{
			get
			{
				List<object> attributes = new List<object> ();

				if (this.IsReadonly)
				{
					attributes.Add (CodeAttributes.ReadOnlyAttribute);
				}
				if (this.IsConst)
				{
					attributes.Add (CodeAttributes.ConstAttribute);
				}
				if (this.IsNew)
				{
					attributes.Add (CodeAttributes.NewAttribute);
				}
				
				if (this.IsPartialDefinition)
				{
					attributes.Add (CodeAttributes.PartialDefinitionAttribute);
				}
				else if (this.IsPartial)
				{
					attributes.Add (CodeAttributes.PartialAttribute);
				}

				return attributes.ToArray ();
			}
		}


		/// <summary>
		/// Gets the <c>readonly</c> attribute constant which can be passed to the
		/// <see cref="CodeAttributes"/> constructor.
		/// </summary>
		public static readonly object			ReadOnlyAttribute = new object ();

		/// <summary>
		/// Gets the <c>const</c> attribute constant which can be passed to the
		/// <see cref="CodeAttributes"/> constructor.
		/// </summary>
		public static readonly object			ConstAttribute = new object ();

		/// <summary>
		/// Gets the <c>new</c> attribute constant which can be passed to the
		/// <see cref="CodeAttributes"/> constructor.
		/// </summary>
		public static readonly object			NewAttribute = new object ();

		/// <summary>
		/// Gets the <c>partial</c> attribute constant which can be passed to the
		/// <see cref="CodeAttributes"/> constructor.
		/// </summary>
		public static readonly object			PartialAttribute = new object ();

		/// <summary>
		/// Gets the <c>partial</c> attribute constant which can be passed to the
		/// <see cref="CodeAttributes"/> constructor. This is similar to the
		/// <see cref="PartialAttribute"/> constant and can be used with methods
		/// which have no body.
		/// </summary>
		public static readonly object			PartialDefinitionAttribute = new object ();

		/// <summary>
		/// Gets the default <see cref="CodeAttributes"/> instance.
		/// </summary>
		public static readonly CodeAttributes Default = new CodeAttributes ();

		/// <summary>
		/// Performs an implicit conversion from <see cref="CodeAttributes"/> to <see cref="CodeAccessibility"/>.
		/// </summary>
		/// <param name="attributes">The code attributes.</param>
		/// <returns>The result of the conversion.</returns>
		public static implicit operator CodeAccessibility(CodeAttributes attributes)
		{
			return attributes.Accessibility;
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="CodeAttributes"/> to <see cref="CodeVisibility"/>.
		/// </summary>
		/// <param name="attributes">The code attributes.</param>
		/// <returns>The result of the conversion.</returns>
		public static implicit operator CodeVisibility(CodeAttributes attributes)
		{
			return attributes.Visibility;
		}

		/// <summary>
		/// Returns the C# representation of the code attributes defined by this
		/// instance.
		/// </summary>
		/// <returns>
		/// A <see cref="string"/> containing the C# representation of the code
		/// attributes defined by this instance.
		/// </returns>
		public override string ToString()
		{
			List<string> tokens = new List<string> ();

			switch (this.visibility)
			{
				case CodeVisibility.None:
					break;

				case CodeVisibility.Internal:
					tokens.Add (CodeFormatter.Strings.Keywords.Internal);
					break;
				
				case CodeVisibility.Private:
					tokens.Add (CodeFormatter.Strings.Keywords.Private);
					break;

				case CodeVisibility.Protected:
					tokens.Add (CodeFormatter.Strings.Keywords.Protected);
					break;
				
				case CodeVisibility.Public:
					tokens.Add (CodeFormatter.Strings.Keywords.Public);
					break;

				default:
					throw new System.NotSupportedException (string.Format ("CodeVisibility.{0} not supported here", this.visibility));
			}

			switch (this.accessibility)
			{
				case CodeAccessibility.Abstract:
					tokens.Add (CodeFormatter.Strings.Keywords.Abstract);
					break;
				
				case CodeAccessibility.Constant:
					tokens.Add (CodeFormatter.Strings.Keywords.Const);
					break;

				case CodeAccessibility.Default:
				case CodeAccessibility.Final:
					break;
				
				case CodeAccessibility.Override:
					tokens.Add (CodeFormatter.Strings.Keywords.Override);
					break;

				case CodeAccessibility.Sealed:
					tokens.Add (CodeFormatter.Strings.Keywords.Sealed);
					break;

				case CodeAccessibility.Static:
					tokens.Add (CodeFormatter.Strings.Keywords.Static);
					break;
				
				case CodeAccessibility.Virtual:
					tokens.Add (CodeFormatter.Strings.Keywords.Virtual);
					break;

				default:
					throw new System.NotSupportedException (string.Format ("CodeAccess.{0} not supported here", this.accessibility));
			}

			if (this.isReadonly)
			{
				tokens.Add (CodeFormatter.Strings.Keywords.Readonly);
			}
			if (this.isConst)
			{
				tokens.Add (CodeFormatter.Strings.Keywords.Const);
			}

			if (this.isNew)
			{
				tokens.Add (CodeFormatter.Strings.Keywords.New);
			}

			if (this.isPartial)
			{
				tokens.Add (CodeFormatter.Strings.Keywords.Partial);
			}

			return string.Join (" ", tokens.ToArray ());
		}

		private CodeAccessibility				accessibility;
		private CodeVisibility					visibility;
		
		private bool							isReadonly;
		private bool							isConst;
		private bool							isNew;
		private bool							isPartial;
		private bool							isPartialDefinition;
	}
}
