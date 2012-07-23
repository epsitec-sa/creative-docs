Ext.define('Epsitec.cresus.webcore.EntityListItem', {
  extend: 'Ext.data.Model',
  alternateClassName: ['Epsitec.EntityListItem'],
  fields: [
    {
      name: 'name',
      type: 'string'
    },
    {
      name: 'uniqueId',
      type: 'string'
    }
  ]
});
