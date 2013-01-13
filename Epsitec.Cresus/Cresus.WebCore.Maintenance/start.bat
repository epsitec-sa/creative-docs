@rem Goes to the script directory so we can launch it from anywhere.
cd /d %~dp0

@rem Goes the to nginx directory
cd Nginx

@rem Launches nginx
nginx.exe

@rem Goes back to the script directory
cd /d %~dp0