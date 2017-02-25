@echo off
cls
@echo POS DEVICE MANAGER SOURCE CODE CLEANUP.

rmdir /S /Q .\Bin
rmdir /S /Q .\Source\.vs
rmdir /S /Q .\Source\packages
rmdir /S /Q .\Source\ipch
del /S *.log
del /S *.local
del /S *.sdf
del /S *.identcache
del /S *.stat
del /S *.user
del /S _*.*
del /S dlldata.c

for /d /r . %%d in (obj) do @if exist "%%d" echo "%%d" && rd /s/q "%%d" 

@echo CLEANUP FINISHED.
@echo on

