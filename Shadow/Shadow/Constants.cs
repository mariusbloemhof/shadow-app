using System;

namespace Shadow
{
    public enum AccountResult
    {
        error,
        emailExists,
        passwordTooShort,
        accountCreated,
        loginSuccess,
        loginInvalidPassword,
        loginInvalidUser,
        socialLoginFailed
    }

    public static class Constants
    {
        // Replace strings with your mobile services and gateway URLs.
        //public static string ApplicationURL = @"https://test-shadow-mobapp.azurewebsites.net";
        public static string ApplicationURL = @"https://shadow-backend.azurewebsites.net/";
        //public static string ApplicationURL = @"http://10.100.109.47/shadow-test/";
        public static string RegisterURL = @"https://test-shadow-mobapp.azurewebsites.net/api/register";
        public static string LoginURL = @"https://localhost:44306/Account/ExternalLogin";
        //public static string LoginURL = @"https://test-shadow-mobapp.azurewebsites.net/api/login";
    }
}

