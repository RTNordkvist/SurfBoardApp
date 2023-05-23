using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfBoardApp.Blazor.Shared.ViewModels.ApplicationUserViewModels
{
    public class UserVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}
