#start SQL Server, start the script to create the DB and initial data
echo 'starting database setup'
./setup-database.sh & /opt/mssql/bin/sqlservr