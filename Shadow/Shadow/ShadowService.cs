//#define OFFLINE_SYNC_ENABLED

using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Net.Http;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices.Sync;
//using UIKit;
using Shadow.Model;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

namespace Shadow
{
    public static class ShadowService
    {
        public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

        private static Account account;
        private static Boolean isAuthenticated;
        private static MobileServiceClient Client;
        private static string countryCode;


#if OFFLINE_SYNC_ENABLED
        private static IMobileServiceSyncTable<Account> AccountTable;
        private static IMobileServiceSyncTable<Contact> ContactsTable;
        private static IMobileServiceSyncTable<Audit> AuditTable;
#else
        private static IMobileServiceTable<Account> AccountTable;
        private static IMobileServiceTable<Audit> AuditTable;
        private static IMobileServiceTable<Contact> ContactsTable;
#endif

        static ShadowService()
        {

        
            Client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore("localstore.db");
            store.DefineTable<Account>();
            store.DefineTable<Contact>();
            store.DefineTable<Audit>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            Client.SyncContext.InitializeAsync(store);

            AccountTable = Client.GetSyncTable<Account>();
            ContactsTable = Client.GetSyncTable<Contact>();
            AuditTable = Client.GetSyncTable<Audit>();
#else
            AccountTable = Client.GetTable<Account>();
            ContactsTable = Client.GetTable<Contact>();
            AuditTable = Client.GetTable<Audit>();
#endif

        }

        private static async Task<Account> Authenticate(MobileServiceAuthenticationProvider provider)
        {
            try
            {
                isAuthenticated = false;
#if __IOS__
                var user = await Client.LoginAsync(UIKit.UIApplication.SharedApplication.KeyWindow.RootViewController, provider);
#elif WINDOWS_PHONE
                var user = await Client.CurrentUser = await Client.LoginAsync(provider);
#else
                var user = await Client.LoginAsync(Xamarin.Forms.Forms.Context, provider);
#endif
                IMobileServiceTable<Account> table = Client.GetTable<Account>();
                Client.CurrentUser = null;
                Client.CurrentUser = await AuthenticateSocialAsync(user.UserId, user.MobileServiceAuthenticationToken);

                isAuthenticated = true;
                IMobileServiceTableQuery<Account> userquery = table.Where(t => t.socialid == user.UserId);
                var userres = await userquery.ToListAsync();
                if (userres.Count > 0)
                {
                    var _account = userres.Find(t => t.socialid == user.UserId);
                    account = _account;
                    var res = await LoadContacts(account);
                    SaveLoggedinUser();
                    return _account;
                }
                throw new Exception(String.Format("User logged in but no account found for sociaId: {0}", user.UserId));
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                throw ex;
            }
        }

        private static async Task<Boolean> LoadContacts(Account account)
        {
            IMobileServiceTableQuery<Contact> contactquery = ContactsTable.Where(t => t.userId == account.Id);
            var contactsres = await contactquery.ToListAsync();
            foreach (Contact contact in contactsres)
            {
                account.addEmergencyContact(contact);
                contact.Changed = false;
            }
            return true;
        }

        private static async Task SaveTaskAsync(Account item)
        {
            if (item.Id == null)
            {
                await AccountTable.InsertAsync(item);
            }
            else
            {
                await AccountTable.UpdateAsync(item);
            }
            foreach (Contact contact in item.ContactList)
            {
                if (contact.Id == null)
                {
                    await ContactsTable.InsertAsync(contact);
                }
                else
                {
                    await ContactsTable.UpdateAsync(contact);
                }
            }
        }

        public static async Task<Account> AuthenticateGoogle()
        {
            return await Authenticate(MobileServiceAuthenticationProvider.Google);
        }

        public static async Task<Account> AuthenticateTwitter()
        {
           return await Authenticate(MobileServiceAuthenticationProvider.Twitter);
        }

        public static async Task<Account> AuthenticateFacebook()
        {
            return await Authenticate(MobileServiceAuthenticationProvider.Facebook);
        }

        public static void SaveLoggedinUser()
        {
            if (isAuthenticated && (CurrentUser != null))
            {
                Application.Current.Properties.Clear();
                Application.Current.Properties.Add("UserId", Client.CurrentUser.UserId);
                Application.Current.Properties.Add("token", Client.CurrentUser.MobileServiceAuthenticationToken);
                Application.Current.Properties.Add("Id", CurrentUser.Id);
#if __IOS__
                Application.Current.SavePropertiesAsync();
#endif
            }
        }

        public static async Task<Account> AuthenticateUser(string email, string password)
        {
            try
            {
                isAuthenticated = false;
                account = null;
                Client.CurrentUser = await AuthenticateAsync(email, password);
                IMobileServiceTableQuery<Account> userquery = AccountTable.Where(t => t.email == email);
                var userres = await userquery.ToListAsync();
                if (userres.Count > 0)
                {
                    var _account = userres.Find(t => t.email == email);
                    account = _account;
                    var res = await LoadContacts(account);
                    isAuthenticated = true;
                    SaveLoggedinUser();
                    return _account;
                }
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                throw ex;
            }
            return null;
        }

        public static async Task<Account> GetLoggedinUser()
        {
            try
            {
                if (account != null)
                {
                    return account;
                }
                if (Application.Current.Properties.ContainsKey("UserId"))
                {
                    var userid = Application.Current.Properties["UserId"] as string;
                    var token = Application.Current.Properties["token"] as string;
                    var Id = Application.Current.Properties["Id"] as string;

                    if (Client.CurrentUser == null)
                    {
                        Client.CurrentUser = new MobileServiceUser(userid);
                        Client.CurrentUser.MobileServiceAuthenticationToken = token;
                    }
                    IMobileServiceTableQuery<Account> userquery = AccountTable.Where(t => t.Id == Id);
                    var userres = await userquery.ToListAsync();
                    if (userres.Count > 0)
                    {
                        var _account = userres.Find(t => t.Id == Id);
                        account = _account;
                        var res = await LoadContacts(account);
                        isAuthenticated = true;
                        RaiseOnSavedUserLoaded(account);
                        return _account;
                    }
                   
                }
                else
                {
                    return null;
                }
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                Logout();
                throw ex;
            }
            return null;
        }

        public static Account CurrentUser
        {
            get
            {
                if (isAuthenticated)
                {
                    return account;
                }
                else
                {
                    throw new System.InvalidOperationException("User not logged in");
                }

            }
        }

        public static async Task SaveCurrentUser()
        {
            await AccountTable.UpdateAsync(CurrentUser);
            if (CurrentUser.Id == null)
            {
                await AccountTable.InsertAsync(CurrentUser);
            }
            else
            {
                foreach (Contact contact in CurrentUser.ContactList)
                {
                    if (contact.Id == null)
                    {
                        await ContactsTable.InsertAsync(contact);
                    }
                    else
                    {
                        if (contact.deleted)
                        {
                            await ContactsTable.DeleteAsync(contact);
                        }
                        else
                        {
                            if (contact.Changed)
                            {
                                await ContactsTable.UpdateAsync(contact);
                            }
                        }
                    }
                }
            }
        }

        public static async Task<Boolean> sendSMS(string phoneno, string message)
        {
            Boolean Delivered = false;
            try
            {
                Delivered = await SendSms(phoneno, message);
                if (Delivered)
                {
                    await ShadowService.Addlog(0, "Delivery to [" + phoneno + "] succeeded", "SMS");
                    RaiseOnSMSDelivered(phoneno);
                    return true;
                }
                else
                {
                    await ShadowService.Addlog(-1, "Delivery to [" + phoneno + "] failed", "SMS");
                    RaiseonSMSFailed(phoneno);
                    return false;
                }
            }
            catch (Exception ex)
            {
                await ShadowService.Addlog(-2, "Delivery to [" + phoneno + "] failed", "SMS");
                RaiseonSMSFailed(phoneno);
                return false;
            }



        }

        public static async Task Addlog(int eventstatus, string eventdescription, string eventtype)
        {
            Audit logentry = new Audit();

            logentry.eventStatus = eventstatus;
            logentry.eventDescription = eventdescription;
            logentry.eventType = eventtype;
            logentry.timeStamp = DateTime.Now.ToUniversalTime();
            logentry.UserId = account.Id;

            await AuditTable.InsertAsync(logentry);
        }

#if OFFLINE_SYNC_ENABLED
        public static async Task SyncData()
        {
            await Client.SyncContext.PushAsync();
        }
#endif

        /*Event handlers*/
        public static event EventHandler onSMSDelivered;

        public static event EventHandler onSMSFailed;

        public static event EventHandler OnSavedUserLoaded;

        private static void RaiseOnSMSDelivered(string phoneno)
        {
            var handler = onSMSDelivered;
            if (handler != null)
                handler(typeof(ShadowService), EventArgs.Empty);
        }

        private static void RaiseonSMSFailed(string phoneno)
        {
            var handler = onSMSFailed;
            if (handler != null)
                handler(typeof(ShadowService), EventArgs.Empty);
        }

        private static void RaiseOnSavedUserLoaded(Account account)
        {
            var handler = OnSavedUserLoaded;
            if (handler != null)
                handler(typeof(ShadowService), EventArgs.Empty);
        }

        private static async Task<Boolean> SendSms(string phoneno, string smsMessage)
        {
            //method 1 - RouteSMS
            return RouteSMS.SendSMS(phoneno, countryCode,  smsMessage);

            //method 2 - Twilio
            //var accountSid = "ACffd7aeddc478c222a68b1ac151662c7b"; // Your Account SID from twilio.com/console
            //var authToken = "3c3c7dbf39ec3b28a116d06f69d4aa60";   // Your Auth Token from twilio.com/console

            //TwilioClient.Init(accountSid, authToken);
            //var twilio = new TwilioRestClient(accountSid, authToken);
            //PhoneNumber toNum = new PhoneNumber(phoneno);
            //    //"++27828213175"

            //var message = await
            //  new MessageCreator(
            //    accountSid,
            //    toNum,  // To number
            //    new PhoneNumber("+14129619401"),  // Twilio From number +14129619401
            //    smsMessage
            //  ).ExecuteAsync(twilio);
            // Console.WriteLine("Message sent");
            //return true;
        }

        public static async Task<Account> RegisterAccount(string email, string password)
        {
            try
            {
                isAuthenticated = false;
                account = null;
                RegistrationResponse response = await RegisterAsync(email, password);
                if (response.email == email)
                {
                    return await AuthenticateUser(email, password);
                }
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                throw ex;
            }
            return null;
        }

        private static async Task<MobileServiceUser> AuthenticateAsync(string username, string password)
        {
            // Call the CustomLogin API and set the returned MobileServiceUser as the current user.
            var user = await Client
                .InvokeApiAsync<LoginRequest, MobileServiceUser>(
                "CustomLogin", new LoginRequest()
                {
                    email = username,
                    password = password
                });

            return user;
        }

        private static async Task<MobileServiceUser> AuthenticateSocialAsync(string userid, string token)
        {
            // Call the CustomLogin API and set the returned MobileServiceUser as the current user.
            var user = await Client
                .InvokeApiAsync<SocialLoginRequest, MobileServiceUser>(
                "CustomSocialReset", new SocialLoginRequest()
                {
                    SocialId = userid,
                    token = token
                });

            return user;
        }

        private static async Task<RegistrationResponse> RegisterAsync(string username, string password)
        {
            // Call the CustomLogin API and set the returned MobileServiceUser as the current user.
            var user = await Client
                .InvokeApiAsync<RegistrationRequest, RegistrationResponse>(
                "CustomRegistration", new RegistrationRequest()
                {
                    email = username,
                    password = password
                });
            return user;
        }

        public static async Task<String> ResetPassword(string emailAddress)
        {
            // Call the CustomLogin API and set the returned MobileServiceUser as the current user.
            var response = await Client
                .InvokeApiAsync<ResetRequest, String>(
                "ForgotPassword", new ResetRequest()
                {
                    email = emailAddress
                });
            return response;
        }

        public static void Logout()
        {
            // Call the CustomLogin API and set the returned MobileServiceUser as the current user.
            Application.Current.Properties.Clear();
            Client.CurrentUser = null;
            account = null;
            isAuthenticated = false;
        }

        public static void CountryCode(string countrycode)
        {
            countryCode = countrycode;        }
    }

}