@ECHO OFF 
@TITLE 卸载WINDOWS服务
ECHO==============================================================
ECHO=
ECHO          WINDOWS服务卸载
ECHO=
ECHO==============================================================
@ECHO OFF 
@SET serviceName=
@NET STOP %serviceName%  
@SC DELETE %serviceName% 
PAUSE