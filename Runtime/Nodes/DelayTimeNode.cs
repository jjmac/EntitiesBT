using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Burst;

namespace EntitiesBT.Nodes
{
    [BurstCompile]
    [BehaviorNode("2F6009D3-1314-42E6-8E52-4AEB7CDDB4CD")]
    public class DelayTimerNode
    {
        public struct Data : INodeData
        {
            public TimeSpan Target;
            public TimeSpan Current;
        }

        [BurstCompile]
        public static void Reset(int index, ref NodeBlobRef blob, ref CustomBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            data.Current = TimeSpan.Zero;
        }

        [BurstCompile]
        public static NodeState Tick(int index, ref NodeBlobRef blob, ref CustomBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            if (data.Current >= data.Target)
                return NodeState.Success;
            
            data.Current += blackboard.GetData<TickDeltaTime>().Value;
            return NodeState.Running;
        }
    }
}
