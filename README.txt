ELECTRICAL STORE - .NET 8 BLAZOR WEB APP

1. Existing database:
   ElectronicStoreDB

2. IMPORTANT:
   This project uses the existing SQL Server database.
   Do NOT run Add-Migration or Update-Database for this project.

3. Connection string:
   Open appsettings.json.
   Current example:
   Server=.\SQLEXPRESS

   If your SSMS server is different, replace it with your actual Server name.

   LocalDB example:
   Server=(localdb)\MSSQLLocalDB;Database=ElectronicStoreDB;Trusted_Connection=True;TrustServerCertificate=True;

4. Visual Studio:
   - Open ElectricalStore.csproj
   - Restore NuGet packages
   - Build
   - Run

5. Pages:
   /                 Dashboard
   /categories       Categories
   /products         Products
   /stock            Stock In
   /inventory        Current Inventory
   /sales            Sales
   /reports          Reports

6. Flow:
   Category -> Product -> Stock In -> Sale -> Inventory -> Dashboard/Reports

7. Stock calculation:
   Total Stock = SUM(StockIn.Quantity)
   Total Sold = SUM(SaleItems.Quantity)
   Available = Total Stock - Total Sold

8. Sales:
   Sale header is saved in Sales.
   Products sold are saved in SaleItems.
   Stock is NOT manually reduced; it is calculated from stock-in minus sold quantity.
