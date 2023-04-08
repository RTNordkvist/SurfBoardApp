using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SurfBoardApp.Data.Models;

namespace SurfBoardApp.Data
{
    public class SeedData
    {
        //Specifies the location of the Datasheet excel file
        private const string FILE_PATH = "Data/Datasheet.xlsx";

        // This method retrieves data from an Excel file and adds it to the database
        public static void GetDataFromExcel(IServiceProvider serviceProvider)
        {
            // Create a new list to hold Board objects
            var boards = new List<Board>();

            // Create a FileInfo object for the Excel file
            FileInfo existingFile = new FileInfo(FILE_PATH);

            // Open the Excel file using the EPPlus library
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                // Get the first worksheet in the workbook
                ExcelWorksheet boardWorksheet = package.Workbook.Worksheets[1];

                // Get the number of rows in the worksheet
                int rowCount = boardWorksheet.Dimension.End.Row;

                // Loop through each row in the worksheet, starting from row 2 (header row is excluded)
                for (int row = 2; row <= rowCount; row++)
                {
                    // Retrieve values from each column in the current row
                    var name = boardWorksheet.Cells[row, 1].Value?.ToString().Trim();
                    var length = boardWorksheet.Cells[row, 2].Value?.ToString().Trim();
                    var width = boardWorksheet.Cells[row, 3].Value?.ToString().Trim();
                    var thickness = boardWorksheet.Cells[row, 4].Value?.ToString().Trim();
                    var volume = boardWorksheet.Cells[row, 5].Value?.ToString().Trim();
                    var type = boardWorksheet.Cells[row, 6].Value?.ToString().Trim();
                    var price = boardWorksheet.Cells[row, 7].Value?.ToString().Trim();
                    var equipment = boardWorksheet.Cells[row, 8].Value?.ToString().Trim();

                    // Create a new Board object with the retrieved values
                    var board = new Board
                    {
                        Name = name,
                        Length = double.TryParse(length, out double resultLength) ? resultLength : null,
                        Width = double.TryParse(width, out double resultWidth) ? resultWidth : null,
                        Thickness = double.TryParse(thickness, out double resultThickness) ? resultThickness : null,
                        Volume = double.TryParse(volume, out double resultVolume) ? resultVolume : null,
                        Type = type,
                        Price = decimal.TryParse(price, out decimal resultPrice) ? resultPrice : 0,
                        Equipment = equipment
                    };

                    // Add the new Board object to the list
                    boards.Add(board);
                }
            }

            // Save the retrieved Board objects to the database
            using (var context = new SurfBoardAppContext(serviceProvider.GetRequiredService<DbContextOptions<SurfBoardAppContext>>()))
            {
                // If the database is not empty, return
                if (context.Board.Any()) { return; }

                // Add each Board object to the context and save changes to the database
                foreach (var board in boards)
                {
                    context.Add(board);
                }
                context.SaveChanges();
            }
        }


        public static async Task CreateRolesAndAdministrator(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { "Admin" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            //Here you could create a super user who will maintain the web app
            var adminUser = new ApplicationUser
            {
                Email = "admin@surf.com",
                EmailConfirmed = true,
                FirstName = "Ulla",
                LastName = "Knudsen"
            };
            adminUser.UserName = adminUser.Email;

            //Ensure you have these values in your appsettings.json file
            string userPWD = "Admin123!";

            var user = await userManager.FindByEmailAsync(adminUser.Email);
            if (user == null)
            {
                var createAdminUser = await userManager.CreateAsync(adminUser, userPWD);
                if(createAdminUser.Succeeded)
                {
                    var addRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                    if (!addRoleResult.Succeeded)
                    {
                        throw new Exception("Error admin to Admin role");
                    }
                }
                else
                {
                    throw new Exception("Error creating admin user");
                }
            }
        }
    }
}
