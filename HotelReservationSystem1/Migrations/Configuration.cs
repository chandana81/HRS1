using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using HotelReservationSystem1.Models;
using Microsoft.AspNet.Identity;
using HotelReservationSystem1.Models.Hotel;

namespace HotelReservationSystem1.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        private const string InitialUserName = "Admin";
        private const string InitialUserFirstName = "AdminFirstName";
        private const string InitialUserLastName = "AdminLastName";
        private const string InitialUserEmail = "admin@hotel.com";
        private const string InitialUserPassword = "Hotel@123";

        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        private readonly string[] _groupAdminRoleNames = { "CanEditUser", "CanEditGroup", "User", 
                                                             "Is_Update", "Is_View", "Is_Access", "Is_Save" };
        private readonly IdentityManager _idManager = new IdentityManager();

        private readonly string[] _initialGroupNames = { "SuperAdmins", "GroupAdmins", "UserAdmins", "Users", "Hotel_General_Manager",
                                                       "Hotel_IT_Manager", "Hotel_IT_Staff", "Hotel_Front_Desk_Manager",
                                                       "Hotel_Front_Desk_Staff", "Hotel_Butler"};


        private readonly string[] _superAdminRoleNames = { "Admin", "CanEditUser", "CanEditGroup", "CanEditRole", "User"
                                                             , "Is_Update", "Is_View", "Is_Access", "Is_Save" };
        private readonly string[] _userAdminRoleNames = { "CanEditUser", "User", "Is_Update", "Is_View", "Is_Access", "Is_Save" };
        private readonly string[] _userRoleNames = { "User", "Is_View", "Is_Access" };

        private readonly string[] _hotelGeneralManagerRoleNames = { "Admin", "User", "Is_Update", "Is_View", "Is_Access", "Is_Save" };
        private readonly string[] _hotelITManagerRoleNames = { "Admin", "CanEditUser", "CanEditGroup", "CanEditRole", "User"
                                                             , "Is_Update", "Is_View", "Is_Access", "Is_Save" };
        private readonly string[] _hotelITStaffRoleNames = { "Admin", "CanEditUser", "User"
                                                             , "Is_Update", "Is_View", "Is_Access", "Is_Save" };
        private readonly string[] _hotelFDManagerRoleNames = { "User", "Is_Update", "Is_View", "Is_Access", "Is_Save" };
        private readonly string[] _hotelFDStaffRoleNames = { "User", "Is_Update", "Is_View", "Is_Access" };
        private readonly string[] _hotelButlerRoleNames = { "User", "Is_View", "Is_Access" };

        private readonly string[] _hotelRoomTypes = { "Single", "Double", "Deluxe" };

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            AddGroups();
            AddRoles();
            AddUsers();
            AddRolesToGroups();
            AddUsersToGroups();

            // Hotel Seed Data: Initial Room Types.
            AddRoomTypes(context);
        }

        private void AddRoomTypes(ApplicationDbContext context)
        {
            _hotelRoomTypes.ToList().ForEach(r => context.RoomTypes.Add(new RoomType { Name = r }));
            context.SaveChanges();
        }

        public void AddGroups()
        {
            foreach (var groupName in _initialGroupNames)
            {

                try
                {
                    _idManager.CreateGroup(groupName);
                }
                catch (GroupExistsException)
                {
                    // intentionally catched for seeding
                }
            }
        }

        private void AddRoles()
        {
            // Some example initial roles. These COULD BE much more granular:
            _idManager.CreateRole("Admin", "Global Access");
            _idManager.CreateRole("CanEditUser", "Add, modify, and delete Users");
            _idManager.CreateRole("CanEditGroup", "Add, modify, and delete Groups");
            _idManager.CreateRole("CanEditRole", "Add, modify, and delete roles");
            _idManager.CreateRole("User", "Restricted to business domain activity");

            _idManager.CreateRole("Is_Update", "User can update data");
            _idManager.CreateRole("Is_View", "User can view data.");
            _idManager.CreateRole("Is_Access", "User can upaccessdate data.");
            _idManager.CreateRole("Is_Save", "User can save data.");
        }

        private void AddRolesToGroups()
        {
            // Add the Super-Admin Roles to the Super-Admin Group:
            IDbSet<Group> allGroups = _db.Groups;
            Group superAdmins = allGroups.First(g => g.Name == "SuperAdmins");
            foreach (string name in _superAdminRoleNames)
            {
                _idManager.AddRoleToGroup(superAdmins.Id, name);
            }

            // Add the Group-Admin Roles to the Group-Admin Group:
            Group groupAdmins = _db.Groups.First(g => g.Name == "GroupAdmins");
            foreach (string name in _groupAdminRoleNames)
            {
                _idManager.AddRoleToGroup(groupAdmins.Id, name);
            }

            // Add the User-Admin Roles to the User-Admin Group:
            Group userAdmins = _db.Groups.First(g => g.Name == "UserAdmins");
            foreach (string name in _userAdminRoleNames)
            {
                _idManager.AddRoleToGroup(userAdmins.Id, name);
            }

            // Add the User Roles to the Users Group:
            Group users = _db.Groups.First(g => g.Name == "Users");
            foreach (string name in _userRoleNames)
            {
                _idManager.AddRoleToGroup(users.Id, name);
            }

            Group Hotel_General_Manager = allGroups.First(g => g.Name == "Hotel_General_Manager");
            foreach (string name in _hotelGeneralManagerRoleNames)
            {
                _idManager.AddRoleToGroup(Hotel_General_Manager.Id, name);
            }

            Group Hotel_IT_Manager = allGroups.First(g => g.Name == "Hotel_IT_Manager");
            foreach (string name in _hotelITManagerRoleNames)
            {
                _idManager.AddRoleToGroup(Hotel_IT_Manager.Id, name);
            }

            Group Hotel_IT_Staff = allGroups.First(g => g.Name == "Hotel_IT_Staff");
            foreach (string name in _hotelITStaffRoleNames)
            {
                _idManager.AddRoleToGroup(Hotel_IT_Staff.Id, name);
            }

            Group Hotel_Front_Desk_Manager = allGroups.First(g => g.Name == "Hotel_Front_Desk_Manager");
            foreach (string name in _hotelFDManagerRoleNames)
            {
                _idManager.AddRoleToGroup(Hotel_Front_Desk_Manager.Id, name);
            }

            Group Hotel_Front_Desk_Staff = allGroups.First(g => g.Name == "Hotel_Front_Desk_Staff");
            foreach (string name in _hotelFDStaffRoleNames)
            {
                _idManager.AddRoleToGroup(Hotel_Front_Desk_Staff.Id, name);
            }

            Group Hotel_Butler = allGroups.First(g => g.Name == "Hotel_Butler");
            foreach (string name in _hotelButlerRoleNames)
            {
                _idManager.AddRoleToGroup(Hotel_Butler.Id, name);
            }
        }

        // Change these to your own:

        private void AddUsers()
        {
            var newUser = new ApplicationUser
            {
                UserName = InitialUserName,
                FirstName = InitialUserFirstName,
                LastName = InitialUserLastName,
                Email = InitialUserEmail
            };

            // Be careful here - you  will need to use a password which will 
            // be valid under the password rules for the application, 
            // or the process will abort:
            var userCreationResult = _idManager.CreateUser(newUser, InitialUserPassword);
            if (!userCreationResult.Succeeded)
            {
                // warn the user that it's seeding went wrong
                throw new DbEntityValidationException("Could not create InitialUser because: " + String.Join(", ", userCreationResult.Errors));
            }

        }

        // Configure the initial Super-Admin user:
        private void AddUsersToGroups()
        {
            Console.WriteLine(String.Join(", ", _db.Users.Select(u => u.Email)));
            ApplicationUser user = _db.Users.First(u => u.UserName == InitialUserName);
            IDbSet<Group> allGroups = _db.Groups;
            foreach (Group group in allGroups)
            {
                _idManager.AddUserToGroup(user.Id, group.Id);
            }

        }
    }
}