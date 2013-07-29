// This class is the object model used by SortWindow to represent the sorters
// in its two grid panels.

Ext.define('Epsitec.cresus.webcore.ui.SortItem', {
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
    },
    {
      name: 'sortDirection',
      type: 'string'
    }
  ]
});
