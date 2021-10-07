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

        private Quaternion lockedHeading = new Quaternion();
        private bool active = true;
        private float overrideThreshold = 0.05f;
        float kP = 5;

        public override void OnModuleSpawn()
        {
            StartQuarternion = transform.rotation;
            Debug.Log(StartUpString);
            Debug.Log($"{Time.realtimeSinceStartup} [GyroTorquer] {StartUpString}");
        }

        protected string GetModuleName() => "JDSATorqueModule.JDSATorqueModule";

        void FixedUpdate()
        {
            if ( !active || !this.part.spawned || this.Rb == null || (Object)this.vehicle == (Object)null || !PartModuleUtil.CheckCanApplyForces((PartModule)this) || !this.vehicle.IsAuthorityOrBot || inputState == null)
                return;


            if (System.Math.Abs(inputState.roll) > overrideThreshold || System.Math.Abs(inputState.pitch) > overrideThreshold || System.Math.Abs(inputState.yaw) > overrideThreshold)
            {
                Vector3 torqueVec = new Vector3(-inputState.pitch, inputState.yaw, -inputState.roll);
                Rb.AddRelativeTorque(torqueVec * adjustFactor * dampenFactor * Time.deltaTime, ForceMode.Force);
                lockedHeading = vehicle.Physics.transform.rotation;
            } else
            {
                Quaternion diff = lockedHeading * transform.rotation.Inverse();

                Vector3 diffEuler = diff.eulerAngles;
                diffEuler.x = diffEuler.x > 180 ? 360 - diffEuler.x : -diffEuler.x;
                diffEuler.y = diffEuler.y > 180 ? diffEuler.y - 360 : diffEuler.y;
                diffEuler.z = diffEuler.z > 180 ? 360 - diffEuler.z : -diffEuler.z;


                Rb.AddRelativeTorque(diffEuler * kP * Time.deltaTime, ForceMode.Force);
            }
        }

        public void OnReceiveCtrlState(FSInputState data)
        {
            inputState = data;
        }

    }
}
