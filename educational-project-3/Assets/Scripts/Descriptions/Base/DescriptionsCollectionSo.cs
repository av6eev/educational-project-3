using UnityEngine;

namespace Descriptions.Base
{
    [CreateAssetMenu(menuName = "Create Descriptions Collection/New Description Collection", fileName = "NewCollection", order = 51)]
    public class DescriptionsCollectionSo : ScriptableObject
    {
        public DescriptionsCollection Collection;
    }
}