//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Types;

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	/// <summary>
	/// The <c>SummaryAiderPersonViewController</c> class implements the base summary view controller
	/// of a <see cref="AiderPersonEntity"/>. It displays a compact summary of the person, the list
	/// of the relations, the list of the groups to which the person belongs, and comments.
	/// </summary>
	public sealed class SummaryAiderPersonViewController : SummaryViewController<AiderPersonEntity>
	{
		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			wall.AddBrick ()
				.Icon (this.Entity.GetIconName ("Data"))
				.Title (x => TextFormatter.FormatText (x.CallName, x.eCH_Person.PersonOfficialName, "(~", x.OriginalName, "~)"))
				.Text (x => TextFormatter.FormatText (TextFormatter.FormatText (x.Parish.Group.Name).ApplyBold(), "", "\n", TextFormatter.Command.IfElseEmpty, x.Household1.Address.GetPostalAddress ()))
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.SpecialController1);

			wall.AddBrick ()
				.Icon ("Data.AiderGroup.People")
				.Title (p => p.GetGroupTitle ())
				.Text (p => p.GetGroupText ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.SpecialController3);

			//if (this.Entity.Warnings.Any () || true)
			//{
			//    wall.AddBrick (x => x.Warnings)
			//        .Title ("Attention")
			//        .Icon ("manifest:Epsitec.Aider.Images.Data.AiderWarning.icon")
			//        .Template ()
			//        .End ();
			//}

			//wall.AddBrick (x => x.Relationships)
			//    .Attribute (BrickMode.DefaultToSummarySubView)
			//    .Attribute (BrickMode.SpecialController2);

			wall.AddBrick (x => x.Comment)
				.Attribute (BrickMode.AutoCreateNullEntity);
		}
	}
}
