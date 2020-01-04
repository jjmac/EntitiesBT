using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Burst;

namespace EntitiesBT.Nodes
{
    [BurstCompile]
    [BehaviorNode("A316D182-7D8C-4075-A46D-FEE08CAEEEAF", BehaviorNodeType.Composite)]
    public class ParallelNode
    {
        public static int DataSize(int childCount) => SimpleBlobArray<NodeState>.Size(childCount);
        
        [BurstCompile]
        public static unsafe void Reset(int index, ref NodeBlobRef blob, ref CustomBlackboard blackboard)
        {
            var childrenStates = new SimpleBlobArray<NodeState>(blob.GetNodeDataPtr(index));
            for (var i = 0; i < childrenStates.Length; i++) childrenStates[i] = NodeState.Running;
        }

        [BurstCompile]
        public static unsafe NodeState Tick(int index, ref NodeBlobRef blob, ref CustomBlackboard blackboard)
        {
            var childrenStates = new SimpleBlobArray<NodeState>(blob.GetNodeDataPtr(index));
            var hasAnyRunningChild = false;
            var state = NodeState.Success;
            var localChildIndex = 0;
            var childIndex = index + 1;
            while (childIndex < blob.GetEndIndex(index))
            {
                var childState = childrenStates[localChildIndex];
                if (childState == NodeState.Running)
                {
                    childState = VirtualMachine.Tick(childIndex, ref blob, ref blackboard);
                    childrenStates[localChildIndex] = childState;
                    hasAnyRunningChild = true;
                }

                childIndex = blob.GetEndIndex(childIndex);
                localChildIndex++;
                
                if (state == NodeState.Running) continue;
                if (childState != NodeState.Success) state = childState;
            }
            
            if (hasAnyRunningChild) return state;
            throw new IndexOutOfRangeException();
        }
    }
}