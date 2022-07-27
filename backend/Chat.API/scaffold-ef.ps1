$env:PGPASSWORD = '12345'
$env:PGUSER = 'chat-backend'
dotnet ef dbcontext scaffold Name=ChatApp Npgsql.EntityFrameworkCore.PostgreSQL -o ./DataAccess -c ChatContext --force