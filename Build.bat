@echo off
cls
@echo POS DEVICE MANAGER BATCH BUILD.

if "%1" == "Debug" goto build
if "%1" == "Release" goto build

@echo Unknown build configuration.
goto seereadme

:build
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild" .\Source\POSDeviceManager.sln /t:rebuild /p:Configuration=%1 /verbosity:minimal /fileLogger /fileLoggerParameters:LogFile=Build.log;Verbosity=detailed;Append=true
if not %errorlevel% == 0 goto :buildfailed

@echo --------------------------------------------------------
@echo BUILD SUCCEEDED.
exit /b

:seereadme
@echo Type "Build Debug" or "Build Release".
exit /b 1

:buildfailed
@echo --------------------------------------------------------
@echo BUILD FAILED. SEE Build.log FOR DETAILS.
@echo on