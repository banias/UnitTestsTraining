using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTestsTraining.Tests
{
    [TestFixture]
    public class PasswordCryptographyTests
    {
        private PasswordCryptography _passwordCryptography;

	[SetUp]
	public void PrepareTest()
	{
	    _passwordCryptography = new PasswordCryptography();
	}

	[Test]
	public void IsPasswordHashReturnsFalse_WhenPasswordStartsWithPrefixAndShorterThan50Chars()
	{
	    //Arrange
	    const string hash = "#¶µ•#‡123";
	    //Act
	    var result = _passwordCryptography.IsPasswordHash(hash);
	    //Assert
	    Assert.That(result, Is.False);
	}

	[Test]
	public void IsPasswordHashReturnsTrue_WhenPasswordStartsWithPrefixAndIs50CharsLong()
	{
	    //Arrange
	    const string hash = "#¶µ•#‡j37t9qlc9oYSDguVsOUF/czZ8r62ZXkYLATckeCXKCc=";
	    //Act
	    var result = _passwordCryptography.IsPasswordHash(hash);
	    //Assert
	    Assert.That(result, Is.True);
	}

	//DRY
        [TestCase("#¶µ•#‡123")]
        [TestCase("#¶µ•#‡12333333333333333333333333333333333333333333333333333333333333333333333")]
        public void IsPasswordHashReturnsFalse_WhenPasswordStartsWithPrefixButIsNot50charLong(string hash)
        {
            //Act
            var result = _passwordCryptography.IsPasswordHash(hash);
            //Assert
            Assert.That(result, Is.False);
        }

	[Test]
	public void GenerateHash_generatesCorrectHash()
	{
	    //Arrange
	    const string password = "alamakota";
	    //Act
	    var result = _passwordCryptography.GenerateHash(password);
	    //Assert
	    Assert.That(result, Is.EqualTo("#¶µ•#‡j37t9qlc9oYSDguVsOUF/czZ8r62ZXkYLATckeCXKCc="));
	}

	//Extreme cases
        [TestCase("år", Result = "#¶µ•#‡5Lq2FRH3ijRxl9iFeLKvnf7AOjrGNtnulDCZ7X3G7w4=")]
        [TestCase("年", Result = "#¶µ•#‡AyC/7C4uetrPKDREF1rtJx5lWNeakvoMI6kJWfJqRDE=")]
        [TestCase("Lorem ipsum dolor sit amet, consectetur adipisicing elit. Proin nibh augue, suscipit a, scelerisque sed, lacinia in, mi. Cras vel lorem. Etiam pellentesque aliquet tellus. Phasellus pharetra nulla ac ", 
            Result = "#¶µ•#‡z/BsYCbTPGDo2lHdW1yVX4zj1nv3J/8p1RBCQGxprU0=")]
        public string GenerateHash_generatesCorrectHash(string password)
        {
            //Act
            var result = _passwordCryptography.GenerateHash(password);
            //Assert
            return result;
        }




    }
}
