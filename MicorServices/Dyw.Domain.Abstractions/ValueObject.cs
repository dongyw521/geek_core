using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dyw.Domain.Abstractions
{
    /// <summary>
    /// 值对象基类
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// 获取原子值
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<object> GetAtomicValues();

        public override bool Equals(object obj)
        {
            if(obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var otherOV = (ValueObject)obj;
            var thisValues = this.GetAtomicValues().GetEnumerator();
            var otherValues = otherOV.GetAtomicValues().GetEnumerator();
            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                if (ReferenceEquals(thisValues.Current, null) ^ ReferenceEquals(otherValues.Current, null))
                {
                    return false;
                }

                if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current))
                {
                    return false;
                }

            }
            return !thisValues.MoveNext() & !otherValues.MoveNext();
        }

        /// <summary>
        /// 静态方法，绑定到类型上
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        /// <summary>
        /// 静态方法，绑定到类型上
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        protected static bool NotEqualOperator(ValueObject left,ValueObject right)
        {
            return !EqualOperator(left, right);
        }

        /// <summary>
        /// 重写HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            //没太看懂这个方法
            return GetAtomicValues().Select(x => x != null ? x.GetHashCode() : 0).Aggregate((x, y) => x ^ y);
        }
    }
}
