using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfBoardApp.Blazor.Shared.ViewModels
{
    public class RequiredConfirmationVM<T> where T : class
    {
        public bool RequiresConfirmation { get; set; }
        public T? Data { get; set; }
    }
}
