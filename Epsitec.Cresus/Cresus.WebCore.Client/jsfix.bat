@ECHO ==================== fixjsstype ====================
@fixjsstyle --nojsdoc --strict "%~dp0\WebCore\app.js"
@fixjsstyle --nojsdoc --strict -r "%~dp0\WebCore\js"
