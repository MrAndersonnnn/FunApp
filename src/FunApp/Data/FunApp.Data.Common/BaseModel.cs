using System;

namespace FunApp.Data.Common
{
    public abstract class BaseModel<T>
    {
        public T Id { get; set; }
    }
}
