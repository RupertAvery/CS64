using System;

namespace CS64.Core
{
    public class UnsupportedMapperException : Exception
    {
        private readonly int _mapperId;

        public UnsupportedMapperException(int mapperId) : base($"Unsupported Mapper {mapperId}")
        {
            _mapperId = mapperId;
        }
    }
}