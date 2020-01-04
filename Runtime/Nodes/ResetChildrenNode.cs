using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Burst;

namespace EntitiesBT.Nodes
{
    [BurstCompile]
    [BehaviorNode("3F494113-5404-49D6-ABCC-8BB285B730F8", BehaviorNodeType.Decorate)]
    public class ResetChildrenNode
    {
        [BurstCompile]
        public static NodeState Tick(int index, ref NodeBlobRef blob, ref CustomBlackboard blackboard)
        {
            var endIndex = blob.GetEndIndex(index);
            for (var childIndex = index + 1; childIndex < endIndex; childIndex = blob.GetEndIndex(childIndex))
                VirtualMachine.Reset(childIndex, ref blob, ref blackboard);
            return NodeState.Success;
        }
    }
}