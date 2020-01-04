using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Burst;

namespace EntitiesBT.Nodes
{
    [BurstCompile]
    [BehaviorNode("BD4C1D8F-BA8E-4D74-9039-7D1E6010B058", BehaviorNodeType.Composite)]
    public class SelectorNode
    {
        [BurstCompile]
        public static void Reset(int index, ref NodeBlobRef blob, ref CustomBlackboard blackboard)
        {
            SuccessionNode.Reset(index, ref blob, ref blackboard);
        }

        [BurstCompile]
        public static NodeState Tick(int index, ref NodeBlobRef blob, ref CustomBlackboard blackboard)
        {
            return SuccessionNode.Tick(NodeState.Failure, index, ref blob, ref blackboard);
        }
    }
}