net stop MySQL80
sc delete MySQL80
set a=%~d0
%a%"\VS projects\Traffic_Lights\MySQL Server 8.0\bin\mysqld" --install MySQL80 --defaults-file=%a%"\VS projects\Traffic_Lights\MySQL Server 8.0\data\my.ini"
net start MySQL80