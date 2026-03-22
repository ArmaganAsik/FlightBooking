using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace FlightBooking.ViewComponents.DefaultViewComponents
{
    public class _DefaultFooterComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
