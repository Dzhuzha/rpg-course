using System;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;

namespace RPG.CloudServices
{
    public class UnityAuthenticator : MonoBehaviour
    {
        public event Action AuthSucceed;
        
        public async void Start()
        {
            await UnityServices.InitializeAsync();
            SignIn();
        }

        private async void SignIn()
        {
            await SignInAnonymous();
        }

        private async Task SignInAnonymous()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Sign in via {AuthenticationService.Instance.PlayerId} successful!");
                AuthSucceed?.Invoke();
            }
            catch (AuthenticationException e)
            {
                Debug.Log($"Signing failed! {e}");
            }
       
        }
    }
}