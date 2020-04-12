using DatingWebApp.Contract;
using DatingWebApp.Data;
using DatingWebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DatingWebApp.Services
{
    public class AuthServiceLogin : IAuthRepository
    {
        public AuthServiceLogin(AppDbContext context)
        {
            this._context = context;
        }
        private const int _saltSize = 128;
        private const int _hashSize = 128;
        private const int _IterationCount = 1000;
        private readonly AppDbContext _context;

        async Task<User> IAuthRepository.Login(string userName, string password)
        {
            var user=_context.Users.FirstOrDefault(x => x.UserName == userName);

            if (user == null) return null;

            if (!VerifyPassword(user, password)) return null;

            return user;
        }

        async Task<User> IAuthRepository.Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;

            CreatePasswordParameters(password, out passwordSalt, out passwordHash);

            user.passwordHash = passwordHash;
            user.salt = passwordSalt;

            await _context.AddAsync(user);

            await _context.SaveChangesAsync();

            return user;
        }


        async Task<bool> IAuthRepository.UserExists(string userName)
        {
            if (_context.Users.Any(x => x.UserName == userName)) return true;

            return false;
        }

        private void CreatePasswordParameters(string password, out byte[] passwordSalt, out byte[] passwordHash)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            passwordSalt = new byte[_saltSize];
            rng.GetBytes(passwordSalt);

            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, passwordSalt, _IterationCount);
            passwordHash = rfc2898DeriveBytes.GetBytes(_hashSize);

            string s = Convert.ToBase64String(passwordHash);
        }

        private bool VerifyPassword(User user,string password)
        {
            byte[] passwordHash, passwordSalt;
            passwordHash = user.passwordHash;
            passwordSalt = user.salt;

            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, passwordSalt, _IterationCount);
            passwordHash = rfc2898DeriveBytes.GetBytes(_hashSize);

            return SlowEquals(user.passwordHash, passwordHash);
        }
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}
