Hotel Reservation System
======================================================

This is a project to manage and maintain hotel reservations along with group and role based permissions. Users belong to Groups, and Groups have sets of authorization permissions to execute code within the application (using [Authorize]). 

You may need to enable Nuget Package Restore in Visual Studio in order to download and restore Nuget packages during build.

You will also need to run Entity Framework Migrations `Update-Database`. The migration files are included in the repo, so you will NOT need to `Enable-Migrations` or run `Add-Migration Init`. 



## Getting Started 
After you've cloned the project, go ahead and restore the NuGet Packages from the solution or console. Then run the project. 
You'll be able to log in and administer the groups and roles as well as users with the "admin@hotel.com" user.

    Username: admin
    Password: Hotel@123