@echo off

gacutil /i CresusDoc.exe
gacutil /i Common.Dialogs.dll
gacutil /i Common.Document.dll
gacutil /i Common.Drawing.dll
gacutil /i Common.Drawing.Platform.dll
gacutil /i Common.IO.dll
gacutil /i Common.Printing.dll
gacutil /i Common.Script.dll
gacutil /i Common.Script.Glue.dll
REM gacutil /i Common.Support.dll
gacutil /i Common.Support.Events.dll
gacutil /i Common.Support.Implementation.dll
gacutil /i Common.Types.dll
gacutil /i Common.UI.dll
gacutil /i Common.Widgets.dll
gacutil /i Cresus.Database.dll
gacutil /i Cresus.Database.Implementation.dll
gacutil /i FirebirdSql.Data.Firebird.dll
gacutil /i ICSharpCode.SharpZipLib.dll
gacutil /i System.Extension.dll

ngen CresusDoc.exe
ngen Common.Dialogs.dll
ngen Common.Document.dll
ngen Common.Drawing.dll
ngen Common.Drawing.Platform.dll
ngen Common.IO.dll
ngen Common.Printing.dll
ngen Common.Script.dll
ngen Common.Script.Glue.dll
REM ngen Common.Support.dll
ngen Common.Support.Events.dll
ngen Common.Support.Implementation.dll
ngen Common.Types.dll
ngen Common.UI.dll
ngen Common.Widgets.dll
ngen Cresus.Database.dll
ngen Cresus.Database.Implementation.dll
ngen FirebirdSql.Data.Firebird.dll
ngen ICSharpCode.SharpZipLib.dll
ngen System.Extension.dll
