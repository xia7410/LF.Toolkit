@ECHO OFF 
@TITLE 安装WINDOWS服务
ECHO==============================================================
ECHO=
ECHO         WINDOWS服务程序安装
ECHO=
ECHO==============================================================
@ECHO OFF
@SET serviceName=
@SET targetEXE=
@SET serviceDescription=
@SC CREATE %serviceName% binPath= "%~dp0ServicePlugin.exe %targetEXE%" START= AUTO
@SC DESCRIPTION %serviceName% %serviceDescription%
@NET START %serviceName%  
PAUSE