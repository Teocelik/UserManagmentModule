using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagmentModule.Models;
using UserManagmentModule.ViewModels;

namespace UserManagmentModule.Controllers
{
    /*Bu controller, Normal bir kullanıcın giriş yapması, kayıt olması, şifre değiştirmesi vs gibi kendi
     profilini yönetmesi işlevlerini barındırır.*/
    public class UserController : Controller
    {
        /*Kullanıcı oluşturma, güncelleme, parola sıfırlama vs işlemleri yapmak için Identity 
         * kütüphanesinin sunduğu UserManager class’ını kullanacağım(filed olarak)*/
        private readonly UserManager<IdentityUser> _userManager;

        /*Kullanıcıların giriş işlemlerini yönetmek, örn/ Giriş yapma, çıkış yapma, 
         iki faktörlü kimlik doğrulama gibi işlemleri için Identity kütüphanesinin
          sunduğu SignInManager class’ını kullanacağım(filed olarak)*/
        private readonly SignInManager<IdentityUser> _signInManager;

        /*_userStore ve _emailStore ile kullanıcıya ait temel bilgilerin (kullanıcı adı ve e-posta) ayarlanmasını sağlar*/
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _userEmailStore;

        //
        private readonly ILogger<RegisterViewModel> _logger;

        //Constructor
        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IUserStore<IdentityUser> userStore, IUserEmailStore<IdentityUser> userEmailStore, ILogger<RegisterViewModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _userEmailStore = userEmailStore;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Kullanıcının kayıt olması için kayıt formunu görüntüler.
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //Kullanıcının girdiği bilgileri alır ve işler.
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            //Gelen veri formatı doğru değilse..
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            //ViewModel'dan gelen veriler doğru formatta ise..
            //yeni bir IdentityUser(kullanıcı) nesnesi oluştur
            var user = new IdentityUser();

            //kullanıcı adı ve e-posta bilgilerini ata.
            await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
            await _userEmailStore.SetEmailAsync(user, model.Email, CancellationToken.None);

            //Kullanıcıyı oluşturalım
            var result = await _userManager.CreateAsync(user, model.Password);

            //eğer kullanıcı başarılı oluşturulmuşsa..
            if(result.Succeeded)
            {
                _logger.LogInformation("Kullanıcı başarıyla oluşturuldu!");

                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Index", "Home");
            }
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
    }
}
