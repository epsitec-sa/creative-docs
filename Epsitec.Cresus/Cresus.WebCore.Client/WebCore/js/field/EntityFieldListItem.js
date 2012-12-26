Ext.define('Epsitec.cresus.webcore.field.EntityFieldListItem', {
  extend: 'Ext.data.Model',
  alternateClassName: ['Epsitec.EntityFieldListItem'],

  /* Config */

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

  /* Additional methods */

  toItem: function() {
    return {
      id: this.get('id'),
      summary: this.get('summary')
    };
  }
});
