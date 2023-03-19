// This class represents a paginated list of items of type T
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurfBoardApp
{
    //Used for the surf board pages
    public class PaginatedList<T> : List<T>
    {
        // The current page index
        public int PageIndex { get; private set; }

        // The total number of pages
        public int TotalPages { get; private set; }

        // Constructor to create a new instance of PaginatedList
        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            // Set the current page index
            PageIndex = pageNumber;

            // Calculate the total number of pages
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            // Add the items to the list
            AddRange(items);
        }

        // Indicates if there is a previous page
        public bool HasPreviousPage
        {
            get { return PageIndex > 1; }
        }

        // Indicates if there is a next page
        public bool HasNextPage
        {
            get { return PageIndex < TotalPages; }
        }

        // Creates a new instance of PaginatedList asynchronously
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            // Count the total number of items in the source
            var count = await source.CountAsync();

            // Get the items for the current page
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            // Return a new instance of PaginatedList
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}