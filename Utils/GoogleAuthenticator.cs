using System;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;

namespace GoogleCalenderDemo.Utils
{
    public class GoogleAuthenticator
    {
        private readonly OAuth2Authenticator<NativeApplicationClient> _authenticator;

        public GoogleAuthenticator(OAuth2Authenticator<NativeApplicationClient> authenticator)
        {
            _authenticator = authenticator;
        }

        internal IAuthenticator Authenticator => _authenticator;

        public bool IsValid =>
            _authenticator != null &&
            DateTime.Compare(DateTime.Now.ToUniversalTime(), _authenticator.State.AccessTokenExpirationUtc.Value) < 0;

        public string RefreshToken => _authenticator.State.RefreshToken;
    }
}