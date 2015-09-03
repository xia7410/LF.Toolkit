@echo off

set fdir=%WINDIR%\Microsoft.NET\Framework64

if not exist %fdir% (
	set fdir=%WINDIR%\Microsoft.NET\Framework
)

set msbuild=%fdir%\v4.0.30319\msbuild.exe

%msbuild% LF.Toolkit.Data\LF.Toolkit.Data.csproj /p:Configuration=Release /t:Clean;Rebuild /p:OutputPath=..\build\LF.Toolkit.Data
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

%msbuild% LF.Toolkit.Data.NET45\LF.Toolkit.Data.NET45.csproj /p:Configuration=Release /t:Clean;Rebuild /p:OutputPath=..\build\LF.Toolkit.Data.NET45
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

%msbuild% LF.Toolkit.MongoDB.NET45\LF.Toolkit.MongoDB.NET45.csproj /p:Configuration=Release /t:Clean;Rebuild /p:OutputPath=..\build\LF.Toolkit.MongoDB.NET45
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

%msbuild% LF.Toolkit.Common\LF.Toolkit.Common.csproj /p:Configuration=Release /t:Clean;Rebuild /p:OutputPath=..\build\LF.Toolkit.Common
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

pause