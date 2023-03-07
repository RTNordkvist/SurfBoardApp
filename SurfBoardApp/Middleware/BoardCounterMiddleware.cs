using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using SurfBoardApp.Data;

namespace SurfBoardApp.Middleware
{
    public class BoardCounterMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Call the next middleware in the pipeline
            await next(context);

            // Check if the request is for the surfboard details page
            if (context.Request.Path.StartsWithSegments("/Boards/Details"))
            {
                // Get the surfboard ID from the URL
                var id = context.Request.RouteValues["id"];

                // Increment the surfboard counter in the database
                var optionsBuilder = new DbContextOptionsBuilder<SurfBoardAppContext>();
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SurfBoardApp.Data;Trusted_Connection=True;MultipleActiveResultSets=true");
                using (var db = new SurfBoardAppContext(optionsBuilder.Options))
                {
                    string surfboardIdString = (string)id; //casting id to a string
                    int surfboardId = int.Parse(surfboardIdString); 
                    var surfboard = await db.Board.FindAsync(surfboardId);
                    if (surfboard.ClickCount == null) //Null handling. 
                    {
                        surfboard.ClickCount = 0; //If null set ClickCount to 0
                    }
                    surfboard.ClickCount++; //increment ClickCount by 1
                    await db.SaveChangesAsync(); // Save changes to Database
                }
            }
        }
    }
}

