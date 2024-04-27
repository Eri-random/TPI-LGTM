# APP_NAME
Bievenido a ... 🚀 Acá podrás encontrar información para correr la aplicación localmente


## Backend

Para correr la API localmente necesitas compilar la solución y luego correrla usando Visual Studio o dotnet cli.

```
cd .\backend\backend.api
dotnet restore
dotnet run
```

Mas información de la API en el [README del backend](backend/README.md).


## Base de Datos
Para usar la base de datos localmente se necesita PostgreSQL 15 y correr scripts para crear tablas y agregar data de ejemplo.

Mas información de la DB en el [README de la DB](db/README.md).


## Frontend
Para levantar el frontend localmente se tiene correr los siguientes comandos

```
cd .\frontend
npm install
npm run start
```

Mas información de la UI en el [README del frontend](frontend/README.md).
