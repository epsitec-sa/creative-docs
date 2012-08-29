Ext.define('Epsitec.cresus.webcore.BooleanNullableColumn', {
  extend: 'Ext.grid.column.Boolean',
  alias: ['widget.booleannullablecolumn'],
  alternateClassName: ['Epsitec.BooleanNullableColumn'],

  /* Config */

  nullText: '&#160;',

  /* Additional methods */

  defaultRenderer: function(value) {
    if (value === null) {
      return this.nullText;
    }
    return this.callParent(arguments);
  }
});
