echo 'Please wait while SQL Server 2017 warms up'
sleep 20s

echo 'Initializing database after 10 seconds of wait'
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P thisisalongpassword123! -d master -i initialise-database.sql

echo 'Finished initializing the database'