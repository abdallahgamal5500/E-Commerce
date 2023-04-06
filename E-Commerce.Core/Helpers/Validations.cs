using E_Commerce.Core.Helpers.Constants;
using E_Commerce.Core.Models.Database;
using E_Commerce.Core.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Helpers
{
    public class Validations
    {
        private static Validations _validations;
        private Validations() { }
        public static Validations GetInstance()
        {
            if (_validations == null)
                _validations = new Validations();
            return _validations;
        }
        public bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
        public bool IsValidPassword(String password)
        {

            // for checking if password length
            // is between 8 and 15
            if (!((password.Length >= 8)
                && (password.Length <= 15)))
            {
                return false;
            }

            // to check space
            if (password.Contains(" "))
            {
                return false;
            }
            if (true)
            {
                int count = 0;

                // check digits from 0 to 9
                for (int i = 0; i <= 9; i++)
                {

                    // to convert int to string
                    String str1 = i.ToString();

                    if (password.Contains(str1))
                    {
                        count = 1;
                    }
                }
                if (count == 0)
                {
                    return false;
                }
            }

            // for special characters
            if (!(password.Contains("@") || password.Contains("#")
                || password.Contains("!") || password.Contains("~")
                || password.Contains("$") || password.Contains("%")
                || password.Contains("^") || password.Contains("&")
                || password.Contains("*") || password.Contains("(")
                || password.Contains(")") || password.Contains("-")
                || password.Contains("+") || password.Contains("/")
                || password.Contains(":") || password.Contains(".")
                || password.Contains(", ") || password.Contains("<")
                || password.Contains(">") || password.Contains("?")
                || password.Contains("|")))
            {
                return false;
            }

            if (true)
            {
                int count = 0;

                // checking capital letters
                for (int i = 65; i <= 90; i++)
                {

                    // type casting
                    char c = (char)i;

                    String str1 = c.ToString();
                    if (password.Contains(str1))
                    {
                        count = 1;
                    }
                }
                if (count == 0)
                {
                    return false;
                }
            }

            if (true)
            {
                int count = 0;

                // checking small letters
                for (int i = 97; i <= 122; i++)
                {

                    // type casting
                    char c = (char)i;
                    String str1 = c.ToString();

                    if (password.Contains(str1))
                    {
                        count = 1;
                    }
                }
                if (count == 0)
                {
                    return false;
                }
            }

            // if all conditions fails
            return true;
        }
        public async Task<ValidatingUserTokenDto> ValidateUserToken(string userToken, IUnitOfWork unitOfWork, string role = null)
        {
            var validatingUserToken = new ValidatingUserTokenDto();
            validatingUserToken.StatusCode = 200;
            
            string[] splitedToken = userToken.Split(" ");
            if (splitedToken.Length <= 1 || splitedToken.Length > 2 || !splitedToken[0].Equals("Bearer"))
            {
                validatingUserToken.StatusCode = 401;
                return validatingUserToken;
            }

            DecodedTokenDto decodedToken = null;
            try
            {
                decodedToken = await Encryptions.DecodeToken(splitedToken[1]);
            }
            catch
            {
                validatingUserToken.StatusCode = 401;
                return validatingUserToken;
            }

            User user = await unitOfWork.Users.GetById(decodedToken.UserId);
            if (user == null)
            {
                validatingUserToken.StatusCode = 401;
                return validatingUserToken;
            }

            if (role != null && !user.Role.Equals(role))
            {
                validatingUserToken.StatusCode = 401;
                return validatingUserToken;
            }

            validatingUserToken.User = user;
            validatingUserToken.Token = splitedToken[1];
            validatingUserToken.IsTokenChanged = false;

            if (!decodedToken.IsVaild)
            {
                validatingUserToken.Token = await Encryptions.GenerateToken(user);
                validatingUserToken.IsTokenChanged = true;
            }
            
            return validatingUserToken;
        }
    }
}
