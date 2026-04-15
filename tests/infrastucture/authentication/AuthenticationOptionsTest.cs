using System;
using Xunit;

using Shipstone.Authenticator.Api.Infrastructure.Authentication;

namespace Shipstone.Authenticator.Api.Infrastructure.AuthenticationTest;

public sealed class AuthenticationOptionsTest
{
    [Fact]
    public void TestAudience_Set_Invalid()
    {
        // Arrange
        AuthenticationOptions options = new();
        String audience = options.Audience;

        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                options.Audience = null!);

        // Assert
        Assert.Equal("value", ex.ParamName);
        Assert.Equal(audience, options.Audience);
    }

    [InlineData("")]
    [InlineData(" ")]
    [InlineData("My audience")]
    [Theory]
    public void TestAudience_Set_Valid(String audience)
    {
        // Arrange
        AuthenticationOptions options = new();

        // Act
        options.Audience = audience;

        // Assert
        Assert.Equal(audience, options.Audience);
    }

    [Fact]
    public void TestIssuer_Set_Invalid()
    {
        // Arrange
        AuthenticationOptions options = new();
        String issuer = options.Issuer;

        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() => options.Issuer = null!);

        // Assert
        Assert.Equal("value", ex.ParamName);
        Assert.Equal(issuer, options.Issuer);
    }

    [InlineData("")]
    [InlineData(" ")]
    [InlineData("My issuer")]
    [Theory]
    public void TestIssuer_Set_Valid(String issuer)
    {
        // Arrange
        AuthenticationOptions options = new();

        // Act
        options.Issuer = issuer;

        // Assert
        Assert.Equal(issuer, options.Issuer);
    }

    [InlineData(Int32.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [Theory]
    public void TestOtpExpiryMinutes_Set_Invalid(int newOtpExpiryMinutes)
    {
        // Arrange
        AuthenticationOptions options = new();
        int otpExpiryMinutes = options.OtpExpiryMinutes;

        // Act
        ArgumentOutOfRangeException ex =
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                options.OtpExpiryMinutes = newOtpExpiryMinutes);

        // Assert
        Assert.Equal(newOtpExpiryMinutes, ex.ActualValue);
        Assert.Equal("value", ex.ParamName);
        Assert.Equal(otpExpiryMinutes, options.OtpExpiryMinutes);
    }

    [InlineData(1)]
    [InlineData(Int32.MaxValue)]
    [Theory]
    public void TestOtpExpiryMinutes_Set_Valid(int otpExpiryMinutes)
    {
        // Arrange
        AuthenticationOptions options = new();

        // Act
        options.OtpExpiryMinutes = otpExpiryMinutes;

        // Assert
        Assert.Equal(otpExpiryMinutes, options.OtpExpiryMinutes);
    }

    [Fact]
    public void TestConstructor()
    {
        // Act
        AuthenticationOptions options = new();

        // Assert
        Assert.False(options.AccessTokenExpiryMinutes <= 0);
        Assert.NotNull(options.AccessTokenSigningKey);
        Assert.NotNull(options.Audience);
        Assert.NotNull(options.Issuer);
        Assert.False(options.OtpExpiryMinutes <= 0);
        Assert.False(options.RefreshTokenExpiryMinutes <= 0);
        Assert.NotNull(options.RefreshTokenSigningKey);
    }
}
