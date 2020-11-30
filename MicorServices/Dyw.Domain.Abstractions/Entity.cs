using System;
using System.Collections.Generic;
using System.Text;

namespace Dyw.Domain.Abstractions
{
    /// <summary>
    /// 无主键
    /// </summary>
    public abstract class Entity : IEntity
    {
        public abstract object[] GetKeys();

        public override string ToString()
        {
            return $"[Entity:{GetType().Name}] Keys={string.Join(",", GetKeys())}";
        }

        #region 领域事件相关操作

        private List<IDomainEvent> _domainEvents;

        /// <summary>
        /// 暴露给外部的领域事件集合
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents = _domainEvents ?? new List<IDomainEvent>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvent()
        {
            _domainEvents?.Clear();
        }

        #endregion
    }

    /// <summary>
    /// 有主键
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public abstract class Entity<TPrimaryKey> : Entity, IEntity<TPrimaryKey>
    {
        int? _requestedHashCode;//没看懂这个

        public virtual TPrimaryKey Id { get; protected set; }

        public override object[] GetKeys()
        {
            return new object[] { Id };
        }

        public override string ToString()
        {
            return $"[Entity:{GetType().Name}] Id={Id}";
        }

        /// <summary>
        /// 是否是全新创建尚未持久化的对象
        /// </summary>
        /// <returns></returns>
        public bool IsTransient()
        {
            return EqualityComparer<TPrimaryKey>.Default.Equals(Id, default);
        }

        /// <summary>
        /// 重写对象Equals方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Entity<TPrimaryKey>))
                return false;
            if (this.GetType() != obj.GetType())
                return false;
            if (Object.ReferenceEquals(obj, this))
                return true;
            Entity<TPrimaryKey> item = (Entity<TPrimaryKey>)obj;
            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31;
                return _requestedHashCode.Value;
            }
            else
            {
                return base.GetHashCode();
            }
        }

        /// <summary>
        /// 自定义对象运算符
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Entity<TPrimaryKey> left, Entity<TPrimaryKey> right)
        {
            if (Equals(left, null))
                return Equals(right, null) ? true : false;
            return left.Equals(right);
        }

        /// <summary>
        /// 自定义对象运算符
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Entity<TPrimaryKey> left, Entity<TPrimaryKey> right)
        {
            return !(left == right);
        }
    }
}
