using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN211FinalProj.Model;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace PRN211FinalProj.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string PhoneNumber { get; set; }

        [BindProperty]
        public string Otp { get; set; }

        public string Msg { get; set; }
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(ILogger<LoginModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostSendOtp(string returnUrl = null)
        {
            SendOtp();

            // all  done
            return Page();
        }
        public async Task<IActionResult> OnPostRegister(string returnUrl = null)
        {
            return RedirectToPage("/Register");
        }
        public string GenerateOtp()
        {
            VIDLContext context = new VIDLContext();
            Console.WriteLine(PhoneNumber);
            var u = context.Users.FirstOrDefault(u => u.PhoneNum == PhoneNumber);

            Random rng = new();
            string otp = rng.Next(0, 1000000).ToString("D6");


            u.Otp = BCrypt.Net.BCrypt.HashPassword(otp);
            Console.WriteLine(otp);
            context.SaveChanges();
            return otp;
        }

        public void SendOtp()
        {
            try
            {
                string accountSid = "ACdb5cb630bc0d972abcc437d4c3d5c161";
                string authToken = "632f27936a29f412f8fd3f34c39f6820";
                string phoneNum = "+84" + PhoneNumber.Remove(0, 1);
                TwilioClient.Init(accountSid, authToken);
                var message = MessageResource.Create(
                    body: "Your OTP is " + GenerateOtp(),
                    from: new Twilio.Types.PhoneNumber("+12567279723"),
                    to: new Twilio.Types.PhoneNumber(phoneNum)
                );

                Console.WriteLine(message.Sid + "\n" + message.Body + "\n" + PhoneNumber);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public IActionResult OnPost()
        {
            VIDLContext context = new VIDLContext();
            User u = context.Users.FirstOrDefault(u => u.PhoneNum == PhoneNumber);
            if(u == null)
            {
                return BadRequest("User does not exist, please register");
            }
            if (BCrypt.Net.BCrypt.Verify(Otp, u.Otp) == true)
            {
                HttpContext.Session.SetString("phonenumber", PhoneNumber);
                Console.WriteLine(HttpContext.Session.GetString("phonenumber"));
                u.Otp = null;
                context.Entry<User>(u).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToPage("Index");
            }
            else
            {
                Msg = "Invalid";
                u.Otp = null;
                context.Entry<User>(u).State = EntityState.Modified;
                context.SaveChanges();
                return Page();
            }
        }
    }
}
