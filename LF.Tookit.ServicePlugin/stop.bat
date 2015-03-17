@ECHO OFF 
@TITLE 关闭WINDOWS服务
ECHO==============================================================
ECHO=
ECHO         WINDOWS服务程序关闭
ECHO=
ECHO==============================================================
@ECHO OFF
@SET serviceName=
@NET STOP %serviceName%  
PAUSE