﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using UserManagmentModule.Models;
using UserManagmentModule.ViewModels;

namespace UserManagmentModule.Controllers
{
    /*Bu controller, Normal bir kullanıcın giriş yapması, kayıt olması, şifre değiştirmesi vs gibi kendi
     profilini yönetmesi işlevlerini barındırır.*/
    public class AccountController : Controller
    {
        /*Kullanıcı oluşturma, güncelleme, parola sıfırlama vs işlemleri yapmak için Identity 
         * kütüphanesinin sunduğu UserManager class’ını kullanacağım(filed olarak)*/
        private readonly UserManager<User> _userManager;

        /*Kullanıcıların giriş işlemlerini yönetmek, örn/ Giriş yapma, çıkış yapma, 
         iki faktörlü kimlik doğrulama gibi işlemleri için Identity kütüphanesinin
          sunduğu SignInManager class’ını kullanacağım(filed olarak)*/
        private readonly SignInManager<User> _signInManager;

        /*_userStore ve _emailStore ile kullanıcıya ait temel bilgilerin (kullanıcı adı ve e-posta) ayarlanmasını sağlar*/
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _userEmailStore;

        //
        private readonly ILogger<UserRegisterViewModel> _logger;

        //Constructor
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            IUserStore<User> userStore, IUserEmailStore<User> userEmailStore, ILogger<UserRegisterViewModel> logger)
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
        public IActionResult Register()
        {
            return View();
        }

     //-------------------------------------------------------------

        //Kullanıcının girdiği bilgileri alır ve işler.
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            //Gelen veri formatı doğru değilse..
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            //ViewModel'dan gelen veriler doğru formatta ise..
            //yeni bir User(kullanıcı) nesnesi oluştur
            var user = new User();

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

        // Form açar
        [HttpGet]
        public IActionResult ChangePassword() => View();



        //Form'dan gelen verileri alır ve işler.
        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserChangePasswordViewModel model)
        {
            //Model'dan gelen veri formatı doğru mu
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            //Oturumdaki kullanıcıyı getir(yükle)
            var user = await _userManager.GetUserAsync(User);

            //kullanıcı bulundu mu
            if(user == null)
            {
                return NotFound("Kullanıcı bulunamadı!");
            }

            //kullanıcı bulunduysa şifreyi değiştir
            var changePassword = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            //şifre değiştirme işlemi başarılı mı 
            if(!changePassword.Succeeded)
            {
                foreach(var error in changePassword.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            //işlem başarılı ise oturumu yenile..
            await _signInManager.RefreshSignInAsync(user);

            _logger.LogInformation($"kullanıcı Şifresini başarı ile değiştirdi!");
            return RedirectToAction("Index", "Home");
        }

        //------------------------------------
        //Şifre unutulması halinde, şifre değiştirme işlemleri

        //Verify Email Form'u açar!
        [HttpGet]
        public IActionResult VerifyEmail() => View();
        

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(UserVerifyEmailViewModel model)
        {
            //Kullanıcıdan gelen veri formatı doğruysa...
            if(ModelState.IsValid)
            {
                //kullanıcıyı bul
                var user = await _userManager.FindByNameAsync(model.Email);

                if(user == null)
                {
                    ModelState.AddModelError("", "Kullanıcı bulunamadı!");
                    return View(model);
                }
                else
                {
                    // Taşınacak Route değeri: (Email)
                    return RedirectToAction("ResetPassword", "User", new { userName = model.Email});
                }
            }
            return View(model);
        }


        //Şifre sıfırlama Form'unu açmak için get isteği gönderir!
        [HttpGet]
        public IActionResult ResetPassword(string userName)
        {
            if(string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("VerifyEmail", "User");
            }
            return View(new UserResetPasswordViewModel { Email = userName});
        }



        [HttpPost]
        public async Task<IActionResult> ResetPassword(UserResetPasswordViewModel model)
        {
            //Gelen veri formatını kontrol et!
            if(ModelState.IsValid)
            {
                //kullanıcıyı bul!
                var user = await _userManager.FindByNameAsync(model.Email);

                if(user != null)
                {
                    //mevcut şifreyi sil
                    var result = await _userManager.RemovePasswordAsync(user);

                    if(result.Succeeded)
                    {
                        //yeni şifreyi belirle!
                        result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                        _logger.LogInformation($"Kullanıcı, başarılı bir şekilde şifresini değiştirdi!");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach(var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı bulunamadı!");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Bir hata oluştu!");
                return View(model);
            }
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}