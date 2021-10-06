using BalsaCore;
using BalsaCore.FX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Modules;
using CfgFields;

namespace JDSATorqueModule
{
    public class JDSATorqueModule : PartModule, ICtrlReceiver
    {
        [SerializeField]
        [CfgField(CfgContext.Config, null, false, null)]
        public float rotTorque;
        public Quaternion StartQuarternion;
        [SerializeField]
        [CfgField(CfgContext.Config, null, false, null)]
        public Vector3 axis;
        [SerializeField]
        [CfgField(CfgContext.Config, null, false, null)]
        public float dampenFactor = 0.8f;
        [SerializeField]
        [CfgField(CfgContext.Config, null, false, null)]
        public float adjustFactor = 50f;
        [SerializeField]
        [CfgField(CfgContext.Config, null, false, null)]
        float angle;
        [SerializeField]
        [CfgField(CfgContext.Config, null, false, null)]
        string StartUpString = "Starting JDSA Torque Systems";

        private FSInputState inputState;

        public override void OnModuleSpawn()
        {
            StartQuarternion = transform.rotation;
            Debug.Log(StartUpString);
            Debug.Log($"{Time.realtimeSinceStartup} [GyroTorquer] {StartUpString}");
        }

        protected string GetModuleName() => "JDSATorqueModule.JDSATorqueModule";

        void FixedUpdate()
        {
            if (!this.part.spawned || this.Rb == null || (Object)this.vehicle == (Object)null || !PartModuleUtil.CheckCanApplyForces((PartModule)this) || !this.vehicle.IsAuthorityOrBot || inputState == null)
                return;
            Quaternion applied = Quaternion.AngleAxis(-inputState.pitch * rotTorque, Vector3.right) 
                                 * Quaternion.AngleAxis(inputState.roll * rotTorque, Vector3.up)
                                 * Quaternion.AngleAxis(inputState.yaw * rotTorque, Vector3.back);
            transform.localRotation = applied;

            Vector3 torqueVec = new Vector3( -inputState.pitch, inputState.yaw, -inputState.roll );
            Rb.AddRelativeTorque( torqueVec * adjustFactor * dampenFactor * Time.deltaTime, ForceMode.Force );
        }

        public void OnReceiveCtrlState(FSInputState data)
        {
            inputState = data;
        }

    }
}
