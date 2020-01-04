using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using EntitiesBT.Entities;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.Scripting;

namespace EntitiesBT.Core
{
    
    [BurstCompile]
    public static class VirtualMachine
    {
        public delegate void ResetFunc(int index, ref NodeBlobRef blob, ref CustomBlackboard bb);
        public delegate NodeState TickFunc(int index, ref NodeBlobRef blob, ref CustomBlackboard bb);
        
        private static readonly NativeHashMap<int, FunctionPointer<ResetFunc>> _resets;
        private static readonly NativeHashMap<int, FunctionPointer<TickFunc>> _ticks;

        static VirtualMachine()
        {
            _resets = new NativeHashMap<int, FunctionPointer<ResetFunc>>(128, Allocator.Persistent);
            _ticks = new NativeHashMap<int, FunctionPointer<TickFunc>>(128, Allocator.Persistent);
            
            AppDomain.CurrentDomain.DomainUnload += (sender, args) =>
            {
                _resets.Dispose();
                _ticks.Dispose();
            };

            // try
            // {
                foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes()))
                {
                    var attribute = type.GetCustomAttribute<BehaviorNodeAttribute>();
                    if (attribute == null) continue;
                    var resetFunc = type.GetMethod(attribute.ResetFunc).GetResetFunc();
                    var tickFunc = type.GetMethod(attribute.TickFunc).GetTickFunc();
                    var id = attribute.Id;

                    if (_resets.ContainsKey(id)) throw new DuplicateNameException($"Reset function {id} already registered");
                    if (_ticks.ContainsKey(id)) throw new DuplicateNameException($"Tick function {id} already registered");
                    _resets[id] = BurstCompiler.CompileFunctionPointer(resetFunc);
                    _ticks[id] = BurstCompiler.CompileFunctionPointer(tickFunc);
                }
            // }
            // catch (Exception _)
            // {
            //     _resets.Dispose();
            //     _ticks.Dispose();
            // }
            // finally
            // {
            // }
        }
        
        [Preserve, BurstCompile]
        static void DefaultReset(int index, ref NodeBlobRef blob, ref CustomBlackboard bb) {}
        
        [Preserve, BurstCompile]
        static NodeState DefaultTick(int index, ref NodeBlobRef blob, ref CustomBlackboard bb) => NodeState.None;

        static ResetFunc GetResetFunc(this MethodInfo methodInfo)
        {
            return methodInfo == null
                ? DefaultReset
                : (ResetFunc)methodInfo.CreateDelegate(typeof(ResetFunc))
            ;
        }

        static TickFunc GetTickFunc(this MethodInfo methodInfo)
        {
            return methodInfo == null
                ? DefaultTick
                : (TickFunc)methodInfo.CreateDelegate(typeof(TickFunc))
            ;
        }

        // static void Register(int id, ResetFunc reset, TickFunc tick)
        // {
        //     if (_resets.ContainsKey(id)) throw new DuplicateNameException($"Reset function {id} already registered");
        //     if (_ticks.ContainsKey(id)) throw new DuplicateNameException($"Tick function {id} already registered");
        //
        //     _resets[id] = reset;
        //     _ticks[id] = tick;
        // }
        
        [BurstCompile]
        public static NodeState Tick(ref NodeBlobRef blob, ref CustomBlackboard bb)
        {
            return Tick(0, ref blob, ref bb);
        }
        
        [BurstCompile]
        public static NodeState Tick(int index, ref NodeBlobRef blob, ref CustomBlackboard bb)
        {
            var typeId = blob.GetTypeId(index);
            var state = _ticks[typeId].Invoke(index, ref blob, ref bb);
            // var state = _ticks[typeId](index, blob, bb);
            // Debug.Log($"[BT] tick: {index}-{node.GetType().Name}-{state}");
            return state;
        }

        [BurstCompile]
        public static void Reset(int index, ref NodeBlobRef blob, ref CustomBlackboard bb)
        {
            var typeId = blob.GetTypeId(index);
            _resets[typeId].Invoke(index, ref blob, ref bb);
            // _resets[typeId](index, blob, bb);
            // Debug.Log($"[BT] tick: {index}-{node.GetType().Name}-{state}");
        }

        [BurstCompile]
        public static void Reset(int fromIndex, int count, ref NodeBlobRef blob, ref CustomBlackboard bb)
        {
            for (var i = fromIndex; i < fromIndex + count; i++)
                Reset(i, ref blob, ref bb);
        }

        [BurstCompile]
        public static void Reset(ref NodeBlobRef blob, ref CustomBlackboard bb)
        {
            var count = blob.GetEndIndex(0);
            Reset(0, count, ref blob, ref bb);
        }
    }
}