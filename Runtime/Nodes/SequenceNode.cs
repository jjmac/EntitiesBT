using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Burst;

namespace EntitiesBT.Nodes
{
    [BurstCompile]
    [BehaviorNode("8A3B18AE-C5E9-4F34-BCB7-BD645C5017A5", BehaviorNodeType.Composite)]
    public class SequenceNode
    {
        [BurstCompile]
        public static void Reset(int index, ref NodeBlobRef blob, ref CustomBlackboard blackboard)
        {
            SuccessionNode.Reset(index, ref blob, ref blackboard);
        }

        [BurstCompile]
        public static NodeState Tick(int index, ref NodeBlobRef blob, ref CustomBlackboard blackboard)
        {
            return SuccessionNode.Tick(NodeState.Success, index, ref blob, ref blackboard);
        }
    }
}