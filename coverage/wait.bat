@echo off
echo Waiting...
PING -n %1 -w 1000 127.1 > NUL
echo Waited %1 second(s).
