using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace Shipstone.Authenticator.Api.Infrastructure.AuthenticationTest.Mocks;

internal sealed class MockJwtSecurityTokenHandler : JwtSecurityTokenHandler
{
    internal Func<SecurityTokenDescriptor, SecurityToken> _createTokenFunc;
    internal Func<String, TokenValidationParameters, (ClaimsPrincipal, SecurityToken)> _validateTokenFunc;
    internal Func<SecurityToken, String> _writeTokenFunc;

    public sealed override bool CanValidateToken =>
        throw new NotImplementedException();

    public sealed override bool CanWriteToken =>
        throw new NotImplementedException();

    public sealed override int MaximumTokenSizeInBytes
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public sealed override Type TokenType =>
        throw new NotImplementedException();

    public MockJwtSecurityTokenHandler()
    {
        this._createTokenFunc = _ => throw new NotImplementedException();

        this._validateTokenFunc = (_, _) =>
            throw new NotImplementedException();

        this._writeTokenFunc = _ => throw new NotImplementedException();
    }

    public sealed override bool CanReadToken(XmlReader reader) =>
        throw new NotImplementedException();

    public sealed override bool CanReadToken(String token) =>
        throw new NotImplementedException();

    protected sealed override String CreateActorValue(ClaimsIdentity actor) =>
        throw new NotImplementedException();

    protected sealed override ClaimsIdentity CreateClaimsIdentity(
        JwtSecurityToken jwtToken,
        String issuer,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    public sealed override String CreateEncodedJwt(SecurityTokenDescriptor tokenDescriptor) =>
        throw new NotImplementedException();

    public sealed override String CreateEncodedJwt(
        String issuer,
        String audience,
        ClaimsIdentity subject,
        Nullable<DateTime> notBefore,
        Nullable<DateTime> expires,
        Nullable<DateTime> issuedAt,
        SigningCredentials signingCredentials
    ) =>
        throw new NotImplementedException();

    public sealed override String CreateEncodedJwt(
        String issuer,
        String audience,
        ClaimsIdentity subject,
        Nullable<DateTime> notBefore,
        Nullable<DateTime> expires,
        Nullable<DateTime> issuedAt,
        SigningCredentials signingCredentials,
        EncryptingCredentials encryptingCredentials
    ) =>
        throw new NotImplementedException();

    public sealed override String CreateEncodedJwt(
        String issuer,
        String audience,
        ClaimsIdentity subject,
        Nullable<DateTime> notBefore,
        Nullable<DateTime> expires,
        Nullable<DateTime> issuedAt,
        SigningCredentials signingCredentials,
        EncryptingCredentials encryptingCredentials,
        IDictionary<String, Object> claimCollection
    ) =>
        throw new NotImplementedException();

    public sealed override JwtSecurityToken CreateJwtSecurityToken(SecurityTokenDescriptor tokenDescriptor) =>
        throw new NotImplementedException();

    public sealed override JwtSecurityToken CreateJwtSecurityToken(
        String? issuer = null,
        String? audience = null,
        ClaimsIdentity? subject = null,
        Nullable<DateTime> notBefore = null,
        Nullable<DateTime> expires = null,
        Nullable<DateTime> issuedAt = null,
        SigningCredentials? signingCredentials = null
    ) =>
        throw new NotImplementedException();

    public sealed override JwtSecurityToken CreateJwtSecurityToken(
        String issuer,
        String audience,
        ClaimsIdentity subject,
        Nullable<DateTime> notBefore,
        Nullable<DateTime> expires,
        Nullable<DateTime> issuedAt,
        SigningCredentials signingCredentials,
        EncryptingCredentials encryptingCredentials
    ) =>
        throw new NotImplementedException();

    public sealed override JwtSecurityToken CreateJwtSecurityToken(
        String issuer,
        String audience,
        ClaimsIdentity subject,
        Nullable<DateTime> notBefore,
        Nullable<DateTime> expires,
        Nullable<DateTime> issuedAt,
        SigningCredentials signingCredentials,
        EncryptingCredentials encryptingCredentials,
        IDictionary<String, Object> claimCollection
    ) =>
        throw new NotImplementedException();

    public sealed override SecurityKeyIdentifierClause CreateSecurityTokenReference(
        SecurityToken token,
        bool attached
    ) =>
        throw new NotImplementedException();

    public sealed override SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor) =>
        this._createTokenFunc(tokenDescriptor);

    public sealed override bool Equals(Object? obj) =>
        throw new NotImplementedException();

    public sealed override int GetHashCode() =>
        throw new NotImplementedException();

    public sealed override SecurityToken ReadToken(XmlReader reader) =>
        throw new NotImplementedException();

    public sealed override SecurityToken ReadToken(
        XmlReader reader,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    public sealed override SecurityToken ReadToken(String token) =>
        throw new NotImplementedException();

    protected sealed override SecurityKey ResolveIssuerSigningKey(
        String token,
        JwtSecurityToken jwtToken,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    protected sealed override SecurityKey ResolveTokenDecryptionKey(
        String token,
        JwtSecurityToken jwtToken,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    public sealed override String? ToString() =>
        throw new NotImplementedException();

    protected sealed override void ValidateAudience(
        IEnumerable<String> audiences,
        JwtSecurityToken jwtToken,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    protected sealed override String ValidateIssuer(
        String issuer,
        JwtSecurityToken jwtToken,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    protected sealed override void ValidateIssuerSecurityKey(
        SecurityKey key,
        JwtSecurityToken securityToken,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    protected sealed override void ValidateLifetime(
        Nullable<DateTime> notBefore,
        Nullable<DateTime> expires,
        JwtSecurityToken jwtToken,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    protected sealed override JwtSecurityToken ValidateSignature(
        String token,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    public sealed override ClaimsPrincipal ValidateToken(
        XmlReader reader,
        TokenValidationParameters validationParameters,
        out SecurityToken validatedToken
    ) =>
        throw new NotImplementedException();

    public sealed override ClaimsPrincipal ValidateToken(
        String token,
        TokenValidationParameters validationParameters,
        out SecurityToken validatedToken
    )
    {
        (ClaimsPrincipal, SecurityToken) result =
            this._validateTokenFunc(token, validationParameters);

        validatedToken = result.Item2;
        return result.Item1;
    }

    public sealed override Task<TokenValidationResult> ValidateTokenAsync(
        SecurityToken token,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    public sealed override Task<TokenValidationResult> ValidateTokenAsync(
        String token,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    protected sealed override void ValidateTokenReplay(
        Nullable<DateTime> expires,
        String securityToken,
        TokenValidationParameters validationParameters
    ) =>
        throw new NotImplementedException();

    public sealed override String WriteToken(SecurityToken token) =>
        this._writeTokenFunc(token);

    public sealed override void WriteToken(
        XmlWriter writer,
        SecurityToken token
    ) =>
        throw new NotImplementedException();
}
