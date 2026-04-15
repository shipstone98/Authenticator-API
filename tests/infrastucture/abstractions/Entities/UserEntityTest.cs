using System;
using Xunit;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Infrastructure.AbstractionsTest.Entities;

public sealed class UserEntityTest
{
    [Fact]
    public void TestEmailAddress_Set_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                new UserEntity
                {
                    EmailAddress = null!
                });

        // Assert
        Assert.Equal("value", ex.ParamName);
    }

    [InlineData("")]
    [InlineData(" ")]
    [InlineData("john.doe@contoso.com")]
    [Theory]
    public void TestEmailAddress_Set_Valid(String emailAddress)
    {
        // Act
        UserEntity user = new UserEntity
        {
            EmailAddress = emailAddress
        };

        // Assert
        Assert.Equal(emailAddress, user.EmailAddress);
    }

    [Fact]
    public void TestForename_Set_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                new UserEntity
                {
                    Forename = null!
                });

        // Assert
        Assert.Equal("value", ex.ParamName);
    }

    [InlineData("")]
    [InlineData(" ")]
    [InlineData("John")]
    [Theory]
    public void TestForename_Set_Valid(String forename)
    {
        // Act
        UserEntity user = new UserEntity
        {
            Forename = forename
        };

        // Assert
        Assert.Equal(forename, user.Forename);
    }

    [Fact]
    public void TestOtp_Set_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentException>(() =>
                new UserEntity
                {
                    Otp = new('0', Constants.UserOtpMaxLength + 1)
                });

        // Assert
        Assert.Equal("value", ex.ParamName);
    }

    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123456")]
    [Theory]
    public void TestOtp_Set_Valid(String? otp)
    {
        // Act
        UserEntity user = new UserEntity
        {
            Otp = otp
        };

        // Assert
        Assert.Equal(otp, user.Otp);
    }

    [Fact]
    public void TestSurname_Set_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                new UserEntity
                {
                    Surname = null!
                });

        // Assert
        Assert.Equal("value", ex.ParamName);
    }

    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Doe")]
    [Theory]
    public void TestSurname_Set_Valid(String surname)
    {
        // Act
        UserEntity user = new UserEntity
        {
            Surname = surname
        };

        // Assert
        Assert.Equal(surname, user.Surname);
    }

    [Fact]
    public void TestConstructor()
    {
        // Act
        UserEntity user = new();

        // Assert
        Assert.NotNull(user.EmailAddress);
        Assert.NotNull(user.Forename);

        Assert.True(
            user.Otp is null
            || user.Otp.Length <= Constants.UserOtpMaxLength
        );

        Assert.NotNull(user.Surname);
    }
}
