using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Auth.Api;
using Firebase.Auth;
using Firebase;
using Android.Content;
using System;
using Android.Gms.Tasks;
using Java.Lang;
using Android.Media;

namespace LoginWithGoogle
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnSuccessListener, IOnFailureListener
    {
        Button signinButton;
        TextView displayNameText;
        TextView emailText;
        TextView photourlText;
        TextView phoneText;

        GoogleSignInOptions gso;
        GoogleApiClient googleApiClient;

        FirebaseAuth firebaseAuth;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            signinButton = (Button)FindViewById(Resource.Id.signinButton);
            displayNameText = (TextView)FindViewById(Resource.Id.displayNameText);
            emailText = (TextView)FindViewById(Resource.Id.emailText);
            photourlText = (TextView)FindViewById(Resource.Id.photourlText);
            phoneText = (TextView)FindViewById(Resource.Id.phoneText);
            signinButton.Click += SigninButton_Click;

            gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestIdToken("148409925853-g729sb67d8e9uh72pn8ocvl3n93bocat.apps.googleusercontent.com")
                .RequestEmail()
                .Build();

            googleApiClient = new GoogleApiClient.Builder(this)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, gso).Build();
            googleApiClient.Connect();

            firebaseAuth = GetFirebaseAuth();
            UpdateUI();
        }

        private FirebaseAuth GetFirebaseAuth()
        {
            var app = FirebaseApp.InitializeApp(this);
            FirebaseAuth mAuth;

            if (app == null)
            {
                //var options = new FirebaseOptions.Builder()
                //    .SetProjectId("loginwith-79490")
                //    .SetApplicationId("loginwith-79490")
                //    .SetApiKey("AIzaSyA22eZ56ISG1j3NSFNpUBa-as7FhIi_OSg")
                //    .SetDatabaseUrl("https://loginwith-79490.firebaseio.com")
                //    .SetStorageBucket("loginwith-79490.appspot.com")
                //    .Build();

                var options = new FirebaseOptions.Builder()
                   .SetProjectId("fir-auth-mtwdm-ff4f2")
                   .SetApplicationId("fir-auth-mtwdm-ff4f2")
                   .SetApiKey("AIzaSyACKqie_2Z9VlPkyVfa8avBh4772G16EMo")
                   .SetDatabaseUrl("https://fir-auth-mtwdm-ff4f2-bb102.firebaseio.com/")
                   .SetStorageBucket("fir-auth-mtwdm-ff4f2.appspot.com")
                   .Build();

                app = FirebaseApp.InitializeApp(this, options);
                mAuth = FirebaseAuth.Instance;
            }
            else
            {
                mAuth = FirebaseAuth.Instance;
            }
            return mAuth;
        }

        private void SigninButton_Click(object sender, System.EventArgs e)
        {
            UpdateUI();
            if(firebaseAuth.CurrentUser == null)
            {
                var intent = Auth.GoogleSignInApi.GetSignInIntent(googleApiClient);
                StartActivityForResult(intent, 1);
            }
            else
            {
                firebaseAuth.SignOut();
                UpdateUI();
            }
           
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if(requestCode == 1)
            {
                GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                if (result.IsSuccess)
                {
                    GoogleSignInAccount account = result.SignInAccount;
                    LoginWithFirebase(account);
                }
            }
        }

        private void LoginWithFirebase(GoogleSignInAccount account)
        {
            var credentials = GoogleAuthProvider.GetCredential(account.IdToken, null);
            firebaseAuth.SignInWithCredential(credentials).AddOnSuccessListener(this)
                .AddOnFailureListener(this);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            displayNameText.Text = "Display Name: " + firebaseAuth.CurrentUser.DisplayName;
            emailText.Text = "Email: " + firebaseAuth.CurrentUser.Email;
            photourlText.Text = "Photo URL: " + firebaseAuth.CurrentUser.PhotoUrl.Path;
            phoneText.Text = firebaseAuth.CurrentUser.PhoneNumber;

            Toast.MakeText(this, "Login successful", ToastLength.Short).Show();
            UpdateUI();
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(this, "Login Failed", ToastLength.Short).Show();
            UpdateUI();
        }

        void UpdateUI()
        {
            if(firebaseAuth.CurrentUser != null)
            {
                signinButton.Text = "Sign Out";
            }
            else
            {
                signinButton.Text = "Sign In With Google";
            }
        }
    }
}