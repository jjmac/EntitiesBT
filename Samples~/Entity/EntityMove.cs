using EntitiesBT.Core;
using EntitiesBT.Components;
using EntitiesBT.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class EntityMove : BTNode<EntityMoveNode, EntityMoveNode.Data>
    {
        public Vector3 Velocity;

        public override unsafe void Build(void* dataPtr)
        {
            var ptr = (EntityMoveNode.Data*) dataPtr;
            ptr->Velocity = Velocity;
        }
    }
    
    [BurstCompile]
    [BehaviorNode("F5C2EE7E-690A-4B5C-9489-FB362C949192")]
    public class EntityMoveNode
    {
        public struct Data : INodeData
        {
            public float3 Velocity;
        }

        [BurstCompile]
        public static NodeState Tick(int index, ref NodeBlobRef blob, ref CustomBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            ref var translation = ref blackboard.GetData<Translation>();
            ref var deltaTime = ref blackboard.GetData<TickDeltaTime>();
            translation.Value += data.Velocity * (float)deltaTime.Value.TotalSeconds;
            return NodeState.Running;
        }
    }
}
