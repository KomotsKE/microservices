namespace CoreLib.Entities
{
    /// <summary>
    /// Представляет refresh-токен, используемый для обновления access-токена пользователя.
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// Уникальный идентификатор refresh-токена.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя, которому принадлежит данный токен.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с сущностью <see cref="User"/>.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Уникальное строковое значение токена.
        /// </summary>
        public string Token { get; set; } = null!;

        /// <summary>
        /// Дата и время (в UTC), когда срок действия токена истекает.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Флаг, показывающий, был ли токен отозван (аннулирован).
        /// </summary>
        public bool IsRevoked { get; set; } = false;
    }
}
