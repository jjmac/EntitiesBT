using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Burst;

namespace EntitiesBT.Nodes
{
    [BurstCompile]
    public static class SuccessionNode
    {
        public struct Data : INodeData
        {
            public int ChildIndex;
        }

        [BurstCompile]
        public static void Reset(int index, ref NodeBlobRef blob, ref CustomBlackboard bb)
        {
            blob.GetNodeData<Data>(index).ChildIndex = index + 1;
        }

        [BurstCompile]
        public static NodeState Tick(NodeState continueState, int index, ref NodeBlobRef blob, ref CustomBlackboard bb)
        {
            ref var childIndex = ref blob.GetNodeData<Data>(index).ChildIndex;
            var endIndex = blob.GetEndIndex(index);
            if (childIndex >= endIndex) throw new IndexOutOfRangeException();

            while (childIndex < endIndex)
            {
                var childState = VirtualMachine.Tick(childIndex, ref blob, ref bb);

                if (childState == NodeState.Running) return childState;

                if (childState != continueState)
                {
                    childIndex = endIndex;
                    return childState;
                }

                childIndex = blob.GetEndIndex(childIndex);
            }
            return continueState;
        }
    }
}