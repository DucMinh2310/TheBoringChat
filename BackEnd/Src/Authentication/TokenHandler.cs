namespace TheBoringChat.Authentication;

public class TokenHandler(Audience audience)
{
    private readonly Audience _audience = audience;

    public string GenerateAccessToken(Claim[] claims)
    {
        var now = DateTime.UtcNow;
        var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_audience.Secret));

        var jwt = new JwtSecurityToken(
            issuer: _audience.Iss,
            audience: _audience.Aud,
            claims: claims,
            notBefore: now,
            expires: now.Add(TimeSpan.FromSeconds(_audience.Expires)),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512) //HmacSha256 HmacSha512
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        var encodedJwt = tokenHandler.WriteToken(jwt);
        return encodedJwt;
    }

    public string? GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_audience.Secret)),
            ValidateIssuer = true,
            ValidIssuer = _audience.Iss,
            ValidateAudience = true,
            ValidAudience = _audience.Aud,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }
}
