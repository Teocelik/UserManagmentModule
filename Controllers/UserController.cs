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
        private readonly ILogger<UserSingInViewModel> _logger;

        //Constructor
        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IUserStore<IdentityUser> userStore, IUserEmailStore<IdentityUser> userEmailStore, ILogger<UserSingInViewModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _userEmailStore = userEmailStore;
            _logger = logger;
        }

        //Kullanıcının başarılı bir girişi sonrası açılacak olan sayfa
        public IActionResult Index()
        {
            return View();
        }

        //Kullanıcının kayıt olması için kayıt formunu görüntüler.
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

     //-------------------------------------------------------------

        //Kullanıcının girdiği bilgileri alır ve işler.
        [HttpPost]
        public async Task<IActionResult> SignIn(UserSingInViewModel model)
        {
            //Gelen veri formatı doğru değilse..
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            //ViewModel'dan gelen veriler doğru formatta ise..
            //yeni bir User(kullanıcı) nesnesi oluştur
            var user = new IdentityUser();

            //Kullanıcı adı ve eMail set et.
            await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
            await _userEmailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
           

            //Kullanıcıyı oluşturalım
            var result = await _userManager.CreateAsync(user, model.Password);

            //eğer kullanıcı başarılı oluşturulmuşsa..
            if(result.Succeeded)
            {
                _logger.LogInformation("Kullanıcı başarıyla oluşturuldu!");

                //Oturum aç!
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("LoginSucceed");
            }
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        //------------------------------------------------------

        //Kullanıcının giriş yapması için form'u açar.
        [HttpGet]
        public IActionResult Login() => View();
        

        //Form'dan gelen verileri alır ve işler
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel model)
        {
            //Kullanıcıdan gelen veri'nin formatını kontrol et.
            if(ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Kullanıcı giriş yaptı!");
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Giriş yapılamadı!");
                }
            }
            return View(model);
        }

        //------------------------------------------------------
        //şifre değiştirme işlemleri...

        //şifre değişikliği için Form açar
        [HttpGet]
        public IActionResult ChangePassword() => View();

        //Form'dan gelen verileri alır ve işler.
        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserChangePasswordViewModel model)
        {
            //kullanıcıdan gelen veri formatını doğrula
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            //Oturumumdaki kullanıcıyı getir(yükle)
            var user = await _userManager.GetUserAsync(User);

            
            if(user == null)
            {
                return NotFound($"Kullanıcı bulunamadı!");
            }

            //şifreyi değiştir!
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            //işlem başarılı mı
            if(!changePasswordResult.Succeeded)
            {
                foreach(var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            //Eğer işlem başarılı ise oturumu yenile..
            await _signInManager.RefreshSignInAsync(user);

            _logger.LogInformation("Kullanıcı, şifresini başarılı bir şekilde değiştirdi!");

            return RedirectToAction("Index", "Home");
        }

    }
}
