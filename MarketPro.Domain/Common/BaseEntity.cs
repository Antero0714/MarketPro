using MarketPro.Domain.Common.Interfaces;

namespace MarketPro.Domain.Common
{
    public abstract class BaseEntity : IEntity
    {
        /// <summary>
        /// Уникальный идентификатор поста на странице пользователя
        /// </summary>
        public int Id { get; set; }
    }
}
