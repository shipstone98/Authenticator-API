using System;
using Xunit;

using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Infrastructure.AbstractionsTest.Entities;

public sealed class UserRefreshTokenEntityTest
{
    [Fact]
    public void TestValue_Set_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                new UserRefreshTokenEntity
                {
                    Value = null!
                });

        // Assert
        Assert.Equal("value", ex.ParamName);
    }

    [InlineData("")]
    [InlineData(" ")]
    [InlineData("My refresh tokewn")]
    [Theory]
    public void TestValue_Set_Valid(String val)
    {
        // Act
        UserRefreshTokenEntity userRefreshToken = new UserRefreshTokenEntity
        {
            Value = val
        };

        // Assert
        Assert.Equal(val, userRefreshToken.Value);
    }

    [Fact]
    public void TestConstructor()
    {
        // Act
        UserRefreshTokenEntity userRefreshToken = new();

        // Assert
        Assert.NotNull(userRefreshToken.Value);
    }
}
