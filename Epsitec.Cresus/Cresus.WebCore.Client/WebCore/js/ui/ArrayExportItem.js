// This class is the model of objects that are used within the stores of the
// ArrayExportWindow class, to represents columns that can be exported.

Ext.define('Epsitec.cresus.webcore.ui.ArrayExportItem', {
  extend: 'Ext.data.Model',

  /* Configuration */

  fields: [
    {
      name: 'title',
      type: 'string'
    },
    {
      name: 'name',
      type: 'string'
    }
  ]
});
