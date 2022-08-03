# Unfrozen-networking-test

## Launch
First install dotnet tools
```sh
dotnet tool restore
```

Start database
```sh
docker run --name chat-db -e POSTGRES_PASSWORD=123 -d -p 5551:5432 postgres:latest
```

Start web api
```sh
dotnet tye run --port 8088 --tags db 
// or start with logging
dotnet tye run --port 8088 --tags db --debug
```

Then you may connect to db at `localhost:5551`, view tye dashboard at `http://127.0.0.1:8088` and swagger at `http://localhost:8081/swagger/index.html`
