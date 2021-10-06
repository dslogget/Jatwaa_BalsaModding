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
        public float adjustFactor = 40f;
        [SerializeField]
        [CfgField(CfgContext.Config, null, false, null)]
        float angle;
        [SerializeField]
        [CfgField(CfgContext.Config, null, false, null)]
        string StartUpString = "Starting JDSA Torque Systems";

        private float vertAxis = 0;
        private float horiAxis = 0;

        public override void OnModuleSpawn()
        {
            StartQuarternion = transform.rotation;
            Debug.Log(StartUpString);
            Debug.Log($"{Time.realtimeSinceStartup} [GyroTorquer] {StartUpString}");
        }

        protected string GetModuleName() => "JDSATorqueModule.JDSATorqueModule";

        void FixedUpdate()
        {
            if (!this.part.spawned || this.Rb == null || (Object)this.vehicle == (Object)null || !PartModuleUtil.CheckCanApplyForces((PartModule)this) || !this.vehicle.IsAuthorityOrBot)
                return;
            Quaternion applied = Quaternion.AngleAxis(-vertAxis * rotTorque, Vector3.right) * Quaternion.AngleAxis(horiAxis * rotTorque, Vector3.up);
            transform.localRotation = applied;


            Rb.AddRelativeTorque( applied.eulerAngles * adjustFactor * dampenFactor * Time.deltaTime, ForceMode.Force );
        }

        public void OnReceiveCtrlState(FSInputState data)
        {
            vertAxis = data.pitch;
            horiAxis = data.roll;
        }

    }
}
