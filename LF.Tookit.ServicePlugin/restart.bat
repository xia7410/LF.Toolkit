@ECHO OFF 
@TITLE 启动WINDOWS服务
ECHO==============================================================
ECHO=
ECHO         WINDOWS服务程序启动
ECHO=
ECHO==============================================================
@ECHO OFF
@SET serviceName=
@NET STOP %serviceName%  
@NET START %serviceName%  
PAUSE