echo off
cls
echo *******************************************************************
echo *                                                                 *
echo * Сборка дистрибутива "Форинт-С: Диспетчер POS-устройств" 2.0     *
echo * (c) ERP Service / Формула Юг, 2007                              *
echo *                                                                 *
echo *******************************************************************
pause
"%ProgramFiles(x86)%\Ethalone\Ghost Installer\Bin\GIBuild.exe" POS_Device_Manager-2.0.gpr
pause
echo on