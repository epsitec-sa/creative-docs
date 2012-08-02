Ext.define('Epsitec.cresus.webcore.EntityListItem', {
  extend: 'Ext.data.Model',
  alternateClassName: ['Epsitec.EntityListItem'],

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

  toItem: function() {
    return {
      id: this.get('id'),
      summary: this.get('summary')
    };
  }
});
