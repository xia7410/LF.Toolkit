@echo off

set msbuild="C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe"

%msbuild% LF.Toolkit.Data.Dapper\LF.Toolkit.Data.Dapper.csproj /p:Configuration=Release /t:Clean;Rebuild /p:OutputPath=..\build\LF.Toolkit.Data.Dapper
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

%msbuild% LF.Toolkit.Data\LF.Toolkit.Data.csproj /p:Configuration=Release /t:Clean;Rebuild /p:OutputPath=..\build\LF.Toolkit.Data
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

%msbuild% LF.Toolkit.Data.NET45\LF.Toolkit.Data.NET45.csproj /p:Configuration=Release /t:Clean;Rebuild /p:OutputPath=..\build\LF.Toolkit.Data.NET45
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

%msbuild% LF.Toolkit.MongoDB\LF.Toolkit.MongoDB.csproj /p:Configuration=Release /t:Clean;Rebuild /p:OutputPath=..\build\LF.Toolkit.MongoDB
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

%msbuild% LF.Toolkit.IOC\LF.Toolkit.IOC.csproj /p:Configuration=Release /t:Clean;Rebuild /p:OutputPath=..\build\LF.Toolkit.IOC
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

%msbuild% LF.Toolkit.Common\LF.Toolkit.Common.csproj /p:Configuration=Release /t:Clean;Rebuild /p:OutputPath=..\build\LF.Toolkit.Common
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

pause