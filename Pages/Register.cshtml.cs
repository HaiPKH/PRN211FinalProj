using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN211FinalProj.Model;

namespace PRN211FinalProj.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string PhoneNumber { get; set; }
        public IActionResult OnPost()
        {
            VIDLContext context = new VIDLContext();
            User u = new User();
            u.PhoneNum = PhoneNumber;
            u.GuestFlag = true;
            context.Add(u);
            context.SaveChanges();
            return RedirectToPage("/Login");
        }
    }
}
