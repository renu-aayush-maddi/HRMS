dotnet ef dbcontext scaffold "Host=localhost;Port=5469;Database=HRMS;Username=postgres;Password=aayush05" Npgsql.EntityFrameworkCore.PostgreSQL -o Models/Entities --context-dir Data -c AppDbContext -f

dotnet run --urls="http://0.0.0.0:5237"
ng serve --host 0.0.0.0 --port 4200

--hr cannot accept his own leave
