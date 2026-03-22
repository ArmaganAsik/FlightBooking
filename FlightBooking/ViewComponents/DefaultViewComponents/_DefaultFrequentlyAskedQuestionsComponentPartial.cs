using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.ViewComponents.DefaultViewComponents
{
    public class _DefaultFrequentlyAskedQuestionsComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
