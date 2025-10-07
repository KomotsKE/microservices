namespace CoreLib.Config;

/// <summary>
/// Настройки JwtBearer
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Издатель токена
    /// </summary>
    public string Issuer { get; set; } = string.Empty;
    /// <summary>
    /// Аудитория токена
    /// </summary>
    public string Audience { get; set; } = string.Empty;
    /// <summary>
    /// Секретный ключ для подписи
    /// </summary>
    public string Key { get; set; } = string.Empty;
    /// <summary>
    /// Время жизни access-token в минутах
    /// </summary>
    public int AccessTokenLifetimeMinutes { get; set; }
    /// <summary>
    /// Время жизни refresh-token в днях
    /// </summary>
    public int RefreshTokenLifetimeDays { get; set; }
}