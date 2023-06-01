using SurfBoardApp.Blazor.Shared.ViewModels;
using SurfBoardApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfBoardApp.Test
{
    [TestClass]
    public class PaginatedListTests
    {
        [TestMethod]
        public async Task PageSizeTest()
        {
            // Arrange
            var queryable = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }.AsQueryable();
            int pageSize = 5;
            int pageIndex = 1;

            // Act
            var paginatedList = await PaginatedList<int>.Create(queryable, pageIndex, pageSize);

            // Assert
            Assert.AreEqual(5, paginatedList.Count);
        }

        [TestMethod]
        public async Task NullListTest()
        {
            // Arrange
            int pageSize = 5;
            int pageIndex = 1;

            // Act
            try
            {
                var paginatedList = await PaginatedList<int>.Create(null, pageIndex, pageSize);
            } 
            catch (Exception ex)
            {
                // Assert
                Assert.AreEqual(typeof(ArgumentNullException), ex.GetType());
            }
        }

        [TestMethod]
        public async Task EmptyListTest()
        {
            // Arrange
            var queryable = new int[0].AsQueryable();
            int pageSize = 5;
            int pageIndex = 1;

            // Act
            var paginatedList = await PaginatedList<int>.Create(queryable, pageIndex, pageSize);

            // Assert
            Assert.AreEqual(0, paginatedList.Count);
            Assert.AreEqual(0, paginatedList.TotalPages);
        }

        [TestMethod]
        public async Task TotalPagesTest()
        {
            // Arrange
            var queryable = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }.AsQueryable();
            int pageSize = 11;
            int pageIndex = 1;

            // Act
            var paginatedList = await PaginatedList<int>.Create(queryable, pageIndex, pageSize);

            // Assert
            Assert.AreEqual(2, paginatedList.TotalPages);
        }

        [TestMethod]
        public async Task PagesIndexTest()
        {
            // Arrange
            var queryable = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }.AsQueryable();
            int pageSize = 12;
            int pageIndex = 1;

            // Act
            var paginatedList = await PaginatedList<int>.Create(queryable, pageIndex, pageSize);

            // Assert
            Assert.AreEqual(1, paginatedList.TotalPages);
        }

        [TestMethod]
        public async Task HasNextPageTest()
        {
            // Arrange
            var queryable = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }.AsQueryable();
            int pageSize = 7;
            int pageIndex = 1;

            // Act
            var paginatedList = await PaginatedList<int>.Create(queryable, pageIndex, pageSize);

            // Assert
            Assert.AreEqual(true, paginatedList.HasNextPage);
        }

        [TestMethod]
        public async Task HasNoNextPageTest()
        {
            // Arrange
            var queryable = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }.AsQueryable();
            int pageSize = 7;
            int pageIndex = 2;

            // Act
            var paginatedList = await PaginatedList<int>.Create(queryable, pageIndex, pageSize);

            // Assert
            Assert.AreEqual(false, paginatedList.HasNextPage);
        }

        [TestMethod]
        public async Task HasPreviousPageTest()
        {
            // Arrange
            var queryable = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }.AsQueryable();
            int pageSize = 5;
            int pageIndex = 3;

            // Act
            var paginatedList = await PaginatedList<int>.Create(queryable, pageIndex, pageSize);

            // Assert
            Assert.AreEqual(true, paginatedList.HasPreviousPage);
        }

        [TestMethod]
        public async Task HasNoPreviousPageTest()
        {
            // Arrange
            var queryable = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }.AsQueryable();
            int pageSize = 5;
            int pageIndex = 1;

            // Act
            var paginatedList = await PaginatedList<int>.Create(queryable, pageIndex, pageSize);

            // Assert
            Assert.AreEqual(false, paginatedList.HasPreviousPage);
        }
    }
}
