﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.Gms.Extensions;
using Firebase.Auth;
using ToDoApp.Authentication;
using ToDoApp.Models;

namespace ToDoApp.Droid.Authentication
{
    public class FirebaseAuthentication : IFirebaseAuthentication
    {
        public async Task<UserModel> LoginWithEmailAndPassword(string email, string password)
        {
            try
            {
                var firebaseUser = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
                var token = await firebaseUser.User.GetIdToken(false).AsAsync<GetTokenResult>();
                var user = new UserModel()
                {
                    DisplayName = firebaseUser.User.DisplayName,
                    Email = firebaseUser.User.Email,
                    Token = token.Token
                };
                return user;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task<bool> RegisterWithEmailAndPassword(string username, string email, string password)
        {
            try
            {
                var result = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
                var userProfileBuilder = new UserProfileChangeRequest.Builder();
                userProfileBuilder.SetDisplayName(username);
                await result.User.UpdateProfileAsync(userProfileBuilder.Build());
                return result.User != null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public Task<bool> ForgetPassword(string email)
        {
            throw new NotImplementedException();
        }

        public string GetUsername()
        {
            var user = FirebaseAuth.Instance.CurrentUser;
            return user?.DisplayName;
        }

        public string GetUserId()
        {
            var user = FirebaseAuth.Instance.CurrentUser;
            return user?.Uid;
        }


        public bool IsLoggedIn()
        {
            var user = FirebaseAuth.Instance.CurrentUser;
            return user != null;
        }

        public bool LogOut()
        {
            try
            {
                FirebaseAuth.Instance.SignOut();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}