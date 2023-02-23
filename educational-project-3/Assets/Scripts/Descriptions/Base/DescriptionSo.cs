using UnityEngine;

namespace Descriptions.Base
{
    public class DescriptionSo<T> : ScriptableObject where T : IDescription
    {
        public T Description;
    }
}