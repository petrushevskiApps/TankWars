using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Complete
{
    public class PlayerMovement : MonoBehaviour
    {
        public float speed = 12f;                 // How fast the tank moves forward and back.
        public float turnSpeed = 180f;            // How fast the tank turns in degrees per second.
        
        
        private Rigidbody rigidbody;              // Reference used to move the tank.
        private float movementInputValue;         // The current value of the movement input.
        private float turnInputValue;             // The current value of the turn input.

        [HideInInspector]
        public UnityEvent OnAgentMoving = new UnityEvent();

        [HideInInspector]
        public UnityEvent OnAgentIdling = new UnityEvent();

        private void Awake ()
        {
            rigidbody = GetComponent<Rigidbody> ();
        }
        private void OnEnable()
        {
            RegisterListeners();
            // When the tank is turned on, make sure it's not kinematic.
            rigidbody.isKinematic = false;

            // Also reset the input values.
            movementInputValue = 0f;
            turnInputValue = 0f;

        }
        private void OnDisable()
        {
            UnregisterListeners();
            // When the tank is turned off, set it to kinematic so it stops moving.
            rigidbody.isKinematic = true;
        }

        private void RegisterListeners()
        {
            InputController.OnMovementAxis.AddListener(UpdateMovement);
            InputController.OnTurningAxis.AddListener(UpdateTurning);
        }
        private void UnregisterListeners()
        {
            InputController.OnMovementAxis.RemoveListener(UpdateMovement);
            InputController.OnTurningAxis.RemoveListener(UpdateTurning);
        }

        


        private void FixedUpdate ()
        {
            // Adjust the rigidbodies position and orientation in FixedUpdate.
            Move();
            Turn();
        }

        private void UpdateMovement(float axisValue)
        {
            movementInputValue = axisValue;
            EngineAudio();
        }
        private void UpdateTurning(float axisValue)
        {
            turnInputValue = axisValue;
            EngineAudio();
        }

        private void Move ()
        {
            // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
            Vector3 movement = transform.forward * movementInputValue * speed * Time.deltaTime;

            // Apply this movement to the rigidbody's position.
            rigidbody.MovePosition(rigidbody.position + movement);
        }


        private void Turn ()
        {
            // Determine the number of degrees to be turned based on the input, speed and time between frames.
            float turn = turnInputValue * turnSpeed * Time.deltaTime;

            // Make this into a rotation in the y axis.
            Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

            // Apply this rotation to the rigidbody's rotation.
            rigidbody.MoveRotation (rigidbody.rotation * turnRotation);
        }


        private void EngineAudio()
        {
            // If there is no input (the tank is stationary)...
            if (Mathf.Abs(movementInputValue) < 0.1f && Mathf.Abs(turnInputValue) < 0.1f)
            {
                OnAgentIdling.Invoke();
            }
            else
            {
                OnAgentMoving.Invoke();
            }
        }
        

        
    }
}