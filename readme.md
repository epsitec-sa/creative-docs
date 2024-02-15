# Crésus Core

Crésus Core is used as a base for several projects.

## General

### Build instructions (using signed assemblies)

- Make sure that project `Epsitec.CresusToolkit` can be compiled, by providing the signing
  password required for file `CresusToolkit.pfx` (it is set to `Smaky400`).

### Firebird setup

- Select Firebird 2.5 x64 Superserver when installing the server; all code has been
  tested with version 2.5 only and the database used with project AIDER uses the 2.5
  data structure.

- Database files are expected in `C:\ProgramData\Epsitec\Firebird Databases`

- Edit the `aliases.conf` file in `C:\Program Files\Firebird\Firebird_2_5` and add
  following alias:

  ```.conf
  aider = C:\ProgramData\Epsitec\Firebird Databases\AIDER.FIREBIRD
  ```
