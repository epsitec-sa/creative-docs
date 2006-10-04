using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public class TypeEditorEnum : AbstractTypeEditor
	{
		public TypeEditorEnum()
		{
			this.list = new ScrollList(this);
			this.list.Dock = DockStyle.StackBegin;
			this.list.PreferredHeight = 200;
		}

		public TypeEditorEnum(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			EnumType type = this.type as EnumType;

			this.ignoreChange = true;
			this.ignoreChange = false;
		}


		void HandleTextFieldRealChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			EnumType type = this.type as EnumType;


			this.OnContentChanged();
		}
		

		protected ScrollList						list;
	}
}
