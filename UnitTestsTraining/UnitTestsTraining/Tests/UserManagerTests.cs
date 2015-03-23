using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using UnitTestsTraining.Class;

namespace UnitTestsTraining.Tests
{
    [TestFixture]
    public class UserManagerTests
    {
	//Dependecies
        private IUserRepository _userRepository;
        private IMailSender _mailSender;
        private IPasswordCryptography _passwordCryptography;
        private IPasswordGenerator _passwordGenerator;
        private ILog _log;
	//Tested class
        private UserManager _userManager;

	[SetUp]
	public void PrepareTest()
	{
	    _userRepository = MockRepository.GenerateStub<IUserRepository>();
	    _mailSender = MockRepository.GenerateStub<IMailSender>();
	    _log = MockRepository.GenerateStub<ILog>();
	    _passwordGenerator = MockRepository.GenerateStub<IPasswordGenerator>();
	    _passwordCryptography = MockRepository.GenerateStub<IPasswordCryptography>();
	    _userManager = new UserManager(_userRepository, _mailSender, _log, _passwordCryptography, _passwordGenerator);
	}


	//Expectations old syntax 
	[Test]
	public void ShouldSendEmail_WhenPasswordIsReset_oldSyntax()
	{
	    //preparations
	    var mockRepo = new MockRepository();
	    var userRepository = mockRepo.Stub<IUserRepository>();
	    var passwordCypto = mockRepo.Stub<IPasswordCryptography>();
	    var passwordGen = mockRepo.Stub<IPasswordGenerator>();
	    var log = mockRepo.Stub<ILog>();
	    var mailSender = mockRepo.DynamicMock<IMailSender>();
	    const string email = "mail@mail.com";
	    const string newPassword = "newPass";
	    const string passwordHash = "hash";
	    var user = new User() {Email = email};
	    passwordGen.Stub(x => x.GenerateRandomPassword()).Return(newPassword);
	    passwordCypto.Stub(x => x.GenerateHash(newPassword)).Return(passwordHash);

	    //expectations
	    Expect.Call(() => mailSender.SendEmail(email, "Your new password is: newPass", "New Password"));
	    //end of expectations
	    mockRepo.ReplayAll();

	    //action
	    var userManager = new UserManager(userRepository, mailSender, log, passwordCypto, passwordGen);
	    userManager.ResetPassword(user);

	    //verification
	    mockRepo.VerifyAll();
	}

	//Expectations new syntax (Record-Reply)
        [Test]
        public void ShouldSendEmail_WhenPasswordIsReset_newSyntax()
        {
            //preparations
            var mockRepo = new MockRepository();
            var userRepository = mockRepo.Stub<IUserRepository>();
            var passwordCypto = mockRepo.Stub<IPasswordCryptography>();
            var passwordGen = mockRepo.Stub<IPasswordGenerator>();
            var log = mockRepo.Stub<ILog>();
            var mailSender = mockRepo.DynamicMock<IMailSender>();
            const string email = "mail@mail.com";
            const string newPassword = "newPass";
            const string passwordHash = "hash";
            var user = new User() {Email = email};
            passwordGen.Stub(x => x.GenerateRandomPassword()).Return(newPassword);
            passwordCypto.Stub(x => x.GenerateHash(newPassword)).Return(passwordHash);

            //expectations
            using (mockRepo.Record())
            {
                Expect.Call(() => mailSender.SendEmail(email, "Your new password is: newPass", "New Password"));
            }

            //action / verificationr
            using (mockRepo.Playback())
            {
                var userManager = new UserManager(userRepository, mailSender, log, passwordCypto, passwordGen);
                userManager.ResetPassword(user);
            }
        }

	//AAA syntax 
	//Bad assertions
        [Test]
        public void ShouldSendEmailToCorrectEmailAndWithCorrectMessageAndWithCorrectTitle_WhenPasswordIsReset_AAAsyntax()
        {
	    //Arrange
            const string email = "mail@mail.com";
            const string newPassword = "newPass";
            const string passwordHash = "hash";
            var user = new User() { Email = email };
            _passwordGenerator.Stub(x => x.GenerateRandomPassword()).Return(newPassword);
            _passwordCryptography.Stub(x => x.GenerateHash(newPassword)).Return(passwordHash);
	    //Act
	    _userManager.ResetPassword(user);
	    //Assert
            _mailSender.AssertWasCalled(x => x.SendEmail(
                email, 
                "Your new password is: newPass", 
                "New Password"));
        }

	//Better but still there are two assertions in a single test
        [Test]
        public void ShouldSendEmailToCorrectEmailAndSendOneEmailOnlyOnce_WhenPasswordIsReset()
        {
            //Arranger
            const string email = "mail@mail.com";
            const string newPassword = "newPass";
            const string passwordHash = "hash";
            var user = new User() { Email = email };
            _passwordGenerator.Stub(x => x.GenerateRandomPassword()).Return(newPassword);
            _passwordCryptography.Stub(x => x.GenerateHash(newPassword)).Return(passwordHash);
            //Act
            _userManager.ResetPassword(user);
            //Assert
            _mailSender.AssertWasCalled(
                x => x.SendEmail(Arg<string>.Matches(a => a == email),
			         Arg<string>.Is.Anything, 
			         Arg<string>.Is.Anything), o => o.Repeat.Once());
        }

	//A proper UnitTest :)
        [Test]
        public void ShouldSendEmailWithNewPassword_WhenPasswordIsReset_AssertEmailText()
        {
            //Arranger
            const string email = "mail@mail.com";
            const string newPassword = "newPass";
            const string passwordHash = "hash";
            var user = new User() { Email = email };
            _passwordGenerator.Stub(x => x.GenerateRandomPassword()).Return(newPassword);
            _passwordCryptography.Stub(x => x.GenerateHash(newPassword)).Return(passwordHash);
            //Act
            _userManager.ResetPassword(user);
            //Assert
            _mailSender.AssertWasCalled(
                x => x.SendEmail(Arg<string>.Is.Anything,
                     Arg<string>.Matches(a => a.Contains(newPassword)),
                     Arg<string>.Is.Anything));
        }

        [Test]
        public void RestPasswordShouldThrowArgumentExceptionWhen_WhenUserIsNull()
        {
            //Arranger
            const string newPassword = "newPass";
            const string passwordHash = "hash";
            _passwordGenerator.Stub(x => x.GenerateRandomPassword()).Return(newPassword);
            _passwordCryptography.Stub(x => x.GenerateHash(newPassword)).Return(passwordHash);
            //Act/Assert
            Assert.That(() => _userManager.ResetPassword(null), 
                Throws.Exception.TypeOf<ArgumentException>());

        }

    }
}
