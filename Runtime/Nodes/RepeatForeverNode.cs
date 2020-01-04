using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Burst;

namespace EntitiesBT.Nodes
{
    [BurstCompile]
    [BehaviorNode("A13666BD-48E3-414A-BD13-5C696F2EA87E", BehaviorNodeType.Decorate)]
    public class RepeatForeverNode
    {
        public struct Data : INodeData
        {
            public NodeState BreakStates;
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
                    // TODO: reset ticked node only?
                    for (var i = index + 1; i < endIndex; i++) VirtualMachine.Reset(i, ref blob, ref blackboard);
                    childState = VirtualMachine.Tick(childIndex, ref blob, ref blackboard);
                }
                if (data.BreakStates.HasFlag(childState))
                    return childState;
            }
            return NodeState.Running;
        }
    }
}
