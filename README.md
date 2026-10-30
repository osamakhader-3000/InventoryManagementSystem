1. Inventory Management System – API

      A web API for managing IT department assets, tracking devices and accessories,
      who they're assigned to, and their current status.
  
      Built for an internal technical support team. The front end lives in
      [InventoryManagementSystemClient](https://github.com/osamakhader-3000/InventoryManagementSystemClient).
   

3. Features
      - Register devices and accessories with details such as serial number and type
      - Assign assets to employees and track their history
      - Track asset status (in use, in stock, under repair, retired)
        

4. Tech Stack
      - C# / ASP.NET Core
      - Entity Framework Core
      - PostgreSQL
      - Hosted on IIS
        

1. Getting Started
   
      a. Install the .NET SDK and PostgreSQL.

      b. Set your database connection string in `appsettings.json`.
  
      c. Apply migrations: `dotnet ef database update`
  
      d. Run the API: `dotnet run`
