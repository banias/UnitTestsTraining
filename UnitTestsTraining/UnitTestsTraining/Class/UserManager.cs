using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsTraining.Class
{
    public class UserManager
    {
        private IUserRepository _userRepository;
        private IPasswordCryptography _passwordCryptography;
        private IPasswordGenerator _passwordGenerator;
        private IMailSender _mailSender;
        private ILog _log;

        public UserManager(IUserRepository userRepository, IMailSender mailSender, ILog log)
            : this(userRepository, mailSender, log, new PasswordCryptography(), new PasswordGenerator())
        {
        }

        public UserManager(IUserRepository userRepository, IMailSender mailSender, ILog log, IPasswordCryptography passwordCryptography, IPasswordGenerator passwordGenerator)
        {
            _userRepository = userRepository;
            _log = log;
            _mailSender = mailSender;
            _passwordCryptography = passwordCryptography;
            _passwordGenerator = passwordGenerator;
        }
	public void ResetPassword(User user)
	{
	    try
	    {
	        _userRepository.StartTransaction();
	        var newPassword = _passwordGenerator.GenerateRandomPassword();
	        user.PasswordHash = _passwordCryptography.GenerateHash(newPassword);
	        _userRepository.UpdateUser(user);
	        _userRepository.Commit();
	        _mailSender.SendEmail(user.Email, CreateNewPasswordBody(newPassword), "New Password");
	    }
	    catch (Exception e)
	    {
	        _userRepository.Rollback();
	        _log.LogError("Error in password reset", e);
	    }
	}

        private string CreateNewPasswordBody(string newPassword)
        {
            return string.Format("Your new password is: {0}", newPassword);
        }
    }

    public interface ILog
    {
        void LogError(string message, Exception e);
    }

    public class User
    {
        public string PasswordHash { get; set; }

        public string Email { get; set; }
    }

    public interface IMailSender
    {
        void SendEmail(string email, string createNewPasswordBody, string newPassword);
    }

    public interface IPasswordGenerator
    {
        string GenerateRandomPassword();
    }

    public class PasswordGenerator : IPasswordGenerator
    {
        public string GenerateRandomPassword()
        {
            return "4242424242422424";
        }
    }

    public interface IUserRepository
    {
        void StartTransaction();
        void Commit();
        void UpdateUser(User user);
        void Rollback();
    }
}
