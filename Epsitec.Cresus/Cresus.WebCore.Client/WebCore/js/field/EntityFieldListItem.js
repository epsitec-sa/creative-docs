Ext.define('Epsitec.cresus.webcore.field.EntityFieldListItem', {
  extend: 'Ext.data.Model',
  alternateClassName: ['Epsitec.EntityFieldListItem'],

  /* Config */

  fields: [
    {
      name: 'submitted',
      type: 'string'
    },
    {
      name: 'displayed',
      type: 'string'
    }
  ],

  /* Additional methods */

  toItem: function() {
    return {
      submitted: this.get('submitted'),
      displayed: this.get('displayed')
    };
  }
});
