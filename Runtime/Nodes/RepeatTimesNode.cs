using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Burst;

namespace EntitiesBT.Nodes
{
    [BurstCompile]
    [BehaviorNode("76E27039-91C1-4DEF-AFEF-1EDDBAAE8CCE", BehaviorNodeType.Decorate)]
    public class RepeatTimesNode
    {
        public struct Data : INodeData
        {
            public int TargetTimes;
            public int CurrentTimes;
            public NodeState BreakStates;
        }

        [BurstCompile]
        public static void Reset(int index, ref NodeBlobRef blob, ref CustomBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            data.CurrentTimes = 0;
        }

        [BurstCompile]
        public static NodeState Tick(int index, ref NodeBlobRef blob, ref CustomBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            var childIndex = index + 1;
            var endIndex = blob.GetEndIndex(index);
            if (childIndex < endIndex)
            {
                NodeState childState;
                try
                {
                    childState = VirtualMachine.Tick(childIndex, ref blob, ref blackboard);
                } catch (IndexOutOfRangeException)
                {
                    // TODO: reset ticked children only?
                    for (var i = index + 1; i < endIndex; i++) VirtualMachine.Reset(i, ref blob, ref blackboard);
                    childState = VirtualMachine.Tick(childIndex, ref blob, ref blackboard);
                }
                if (data.BreakStates.HasFlag(childState)) return childState;
            }
            data.CurrentTimes++;
            if (data.CurrentTimes == data.TargetTimes) return NodeState.Success;
            return NodeState.Running;
        }
    }
}
