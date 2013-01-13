@rem Goes to the script directory so we can launch it from anywhere.
cd /d %~dp0

@rem Goes the to nginx directory
cd Nginx

@rem Stops nginx
nginx.exe -s stop

@rem Goes back to the script directory
cd /d %~dp0