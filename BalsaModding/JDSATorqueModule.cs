using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BalsaCore;
using Modules;

namespace JDSATorqueModule
{
    public class GyroAddForce : PartModule
    {

        public float rotTorque = 1000;
        public Quaternion StartQuarternion;
        public Vector3 axis;
        public float dampenFactor = 0.8f;
        public float adjustFactor = 1000f;
        float angle;
        string StartUpString = "Starting JDSA Torque Systems";

        public override void OnModuleSpawn()
        {
            StartQuarternion = transform.rotation;
            Debug.Log(StartUpString);
            Debug.Log($"{Time.realtimeSinceStartup} [GyroTorquer] {StartUpString}");
        }

        void FixedUpdate()
        {
            if (!this.part.spawned || this.Rb == null || (Object)this.vehicle == (Object)null || !PartModuleUtil.CheckCanApplyForces((PartModule)this) || !this.vehicle.IsAuthorityOrBot)
                return;
            float vert = Input.GetAxis("Vertical");
            float horiz = Input.GetAxis("Horizontal");
            if (vert > 0)
            {
                transform.rotation = Quaternion.AngleAxis(rotTorque, Vector3.up);
            }
            if (vert < 0)
            {
                transform.rotation = Quaternion.AngleAxis(rotTorque, Vector3.down);
            }

            if (horiz < 0)
            {
                transform.rotation = Quaternion.AngleAxis(rotTorque, Vector3.left);
            }
            if (horiz > 0)
            {
                transform.rotation = Quaternion.AngleAxis(rotTorque, Vector3.right);
            }


            Quaternion deltaQuat = Quaternion.FromToRotation(Rb.rb.transform.up, Vector3.up);
            deltaQuat.ToAngleAxis(out angle, out axis);
            Rb.AddTorque(-Rb.angularVelocity * dampenFactor * Time.deltaTime, ForceMode.Acceleration);
            Rb.AddTorque(axis.normalized * angle * adjustFactor * Time.deltaTime, ForceMode.Acceleration);

            Quaternion deltaQuatYaw = Quaternion.FromToRotation(Rb.rb.transform.right, Vector3.right);
            deltaQuatYaw.ToAngleAxis(out angle, out axis);
            Rb.AddTorque(-Rb.angularVelocity * dampenFactor * Time.deltaTime, ForceMode.Acceleration);
            Rb.AddTorque(axis.normalized * angle * adjustFactor * Time.deltaTime, ForceMode.Acceleration);
        }

    }
}
