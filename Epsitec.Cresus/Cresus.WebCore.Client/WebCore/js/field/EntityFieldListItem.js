// This class is the object model that is used by the EntityFieldList class. It
// represents an entity, with its id and summary.

Ext.define('Epsitec.cresus.webcore.field.EntityFieldListItem', {
  extend: 'Ext.data.Model',
  alternateClassName: ['Epsitec.EntityFieldListItem'],

  /* Configuration */

  fields: [
    {
      name: 'id',
      type: 'string'
    },
    {
      name: 'summary',
      type: 'string'
    }
  ],

  /* Methods */

  toItem: function() {
    return {
      id: this.get('id'),
      summary: this.get('summary')
    };
  }
});
