using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Burst;

namespace EntitiesBT.Nodes
{
    [BurstCompile]
    [BehaviorNode("46540F67-6145-4433-9A3A-E470992B952E", BehaviorNodeType.Decorate)]
    public class TimerNode
    {
        public struct Data : INodeData
        {
            public TimeSpan Target;
            public TimeSpan Current;
            public NodeState ChildState;
            public NodeState BreakReturnState;
        }

        [BurstCompile]
        public static void Reset(int index, ref NodeBlobRef blob, ref CustomBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            data.Current = TimeSpan.Zero;
            data.ChildState = NodeState.Running;
        }

        [BurstCompile]
        public static NodeState Tick(int index, ref NodeBlobRef blob, ref CustomBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);

            if (data.Current >= data.Target)
                return data.ChildState == NodeState.Running ? data.BreakReturnState : data.ChildState;
            
            var childIndex = index + 1;
            if (data.ChildState == NodeState.Running && childIndex < blob.GetEndIndex(index))
            {
                var childState = VirtualMachine.Tick(childIndex, ref blob, ref blackboard);
                data.ChildState = childState;
            }

            ref var deltaTime = ref blackboard.GetData<TickDeltaTime>();
            data.Current += deltaTime.Value;
            return NodeState.Running;
        }
    }
}
