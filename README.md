# Auth
##### _The easiest way to have Identity in your project_

### Instalation

- **Step 0: Install NuGet**
```bash
  Auth.Infraestructure.Identity
  ```
- **Step 1: Install EntityFrameworkCore.Tools NuGet**
```bash
  Microsoft.EntityFrameworkCore.Tools
  ```
- **Step 2: Add this two lines of code in Program.cs**
```csharp
builder.Services.AddAuthentication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);
  ```
  
- **Step 3: Add this lines of code in Program.cs**
```csharp
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await services.AddIdentityRolesAsync();
} //
  ```
- **Step 4: That's All :p**
