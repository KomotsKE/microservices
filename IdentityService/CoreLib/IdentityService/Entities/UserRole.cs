namespace CoreLib.Entities
{
    /// <summary>
    /// Представляет связь между пользователем и ролью.
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// Уникальный идентификатор пользователя.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Пользователь, которому принадлежит эта роль.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Уникальный идентификатор роли.
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Роль, назначенная пользователю.
        /// </summary>
        public Role Role { get; set; } = null!;
    }
}