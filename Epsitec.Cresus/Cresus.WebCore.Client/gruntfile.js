module.exports = function(grunt) {
  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    clean: {
      src: [
        'WebCore/webcore.css',
        'WebCore/webcore.min.css',
        'WebCore/webcore.js',
        'WebCore/webcore.min.js'
      ]
    },
    cssmin: {
      combine: {
        files: {
          'WebCore/webcore.css': [
            'WebCore/css/style.css',
            'WebCore/css/icons.css',
            'WebCore/css/toastr.css',
            'WebCore/css/entitybag.css',
            'WebCore/css/chatjs.css',
            'WebCore/css/jquery.sidr.custom.css'
          ]
        }
      },
      minify: {
        expand: true,
        cwd: 'WebCore/',
        src: ['*.css', '!*.min.css'],
        dest: 'WebCore/',
        ext: '.min.css'
      }
    },
    concat: {
      dist: {
        src: [
          'WebCore/lib/jquery/jquery-1.10.2.min.js', 
          'WebCore/lib/sidr/jquery.sidr.min.js',
          'WebCore/lib/spin/spin.min.js',
          'WebCore/lib/toastr/toastr.js'
        ],
        dest: 'WebCore/webcore.js'
      }
    },
    uglify: {
      dist: {
        src: '<%= concat.dist.dest %>',
        dest: 'WebCore/webcore.min.js'
      }
    },
    jshint: {
        all: ['Gruntfile.js','WebCore/app.js', 'WebCore/js/**/*.js']
    }
  });
 
  // Defining a custom task to delete previously generated file before generate them
  grunt.registerTask('clean', 'Clean the previously generated files', function() {
    var files = grunt.config('clean').src;
 
    for (var i = 0; i < files.length; i++) {
      grunt.file.delete(files[i]);
    }
  });
 
  // loading the required tasks
  grunt.loadNpmTasks('grunt-contrib-concat');
  grunt.loadNpmTasks('grunt-contrib-uglify');
  grunt.loadNpmTasks('grunt-contrib-cssmin');
  grunt.loadNpmTasks('grunt-contrib-jshint');
  grunt.registerTask('default', ['clean', 'cssmin', 'concat', 'uglify','jshint']);
};