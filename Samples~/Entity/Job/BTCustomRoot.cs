using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;
using NodeBlobRef = EntitiesBT.Entities.NodeBlobRef;

namespace EntitiesBT.Sample
{
    [DisallowMultipleComponent]
    public class BTCustomRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private BTNode RootNode;
        
        private void Reset()
        {
            RootNode = GetComponentInChildren<BTNode>();
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var blobRef = new NodeBlobRef(RootNode.ToBlob());
            var bb = new CustomBlackboard();
            VirtualMachine.Reset(ref blobRef, ref bb);
            
            dstManager.AddComponentData(entity, blobRef);
            dstManager.AddComponentData(entity, bb);
            dstManager.AddComponentData(entity, new TickDeltaTime());
        }
    }
}
