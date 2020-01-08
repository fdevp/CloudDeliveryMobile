using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Widget;
using CloudDeliveryMobile.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;
using System;
using static Android.Gms.Common.Apis.GoogleApiClient;

namespace CloudDeliveryMobile.Android.Activities
{
    [Activity(Label = "SigninActivity")]
    class SigninActivity : MvxAppCompatActivity<SignInViewModel>, IOnConnectionFailedListener
    {
        private GoogleApiClient googleApiClient;
        const int RC_SIGN_IN = 9001;

        public void OnConnectionFailed(ConnectionResult result)
        {
            throw new NotImplementedException();
        }

        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.Theme_AppCompat_DayNight_NoActionBar);

            base.OnCreate(bundle);

            var googleAuthOptions = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestEmail()
                .RequestServerAuthCode(ViewModel.ClientId)
                .Build();

            googleApiClient = new GoogleApiClient.Builder(this)
                .EnableAutoManage(this, this)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, googleAuthOptions)
                .Build();
            
            SetContentView(Resource.Layout.main_signin);

            var button = FindViewById<SignInButton>(Resource.Id.google_auth_button);
            button.Click += googleSignIn;
        }

        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
            if (requestCode == RC_SIGN_IN)
            {
                var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                await ViewModel.GoogleSignIn(result.SignInAccount.ServerAuthCode);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void googleSignIn(object sender, EventArgs e)
        {
            var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(googleApiClient);
            StartActivityForResult(signInIntent, RC_SIGN_IN);
        }
    }
}