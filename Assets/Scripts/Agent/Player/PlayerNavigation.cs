using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Events;

namespace Complete
{
    public class PlayerNavigation : NavigationController
    {
        private Rigidbody rigidbody;              // Reference used to move the tank.

        private float movementInputValue;         // The current value of the movement input.
        private float turnInputValue;             // The current value of the turn input.

        
        private new void Awake ()
        {
            base.Awake();
            rigidbody = GetComponent<Rigidbody> ();
        }
        private void OnEnable()
        {
            // When the tank is turned on, make sure it's not kinematic.
            rigidbody.isKinematic = false;

            // Also reset the input values.
            movementInputValue = 0f;
            turnInputValue = 0f;

        }
        private void OnDisable()
        {
            // When the tank is turned off, set it to kinematic so it stops moving.
            rigidbody.isKinematic = true;
        }

        private void FixedUpdate ()
        {
            // Adjust the rigidbodies position and orientation in FixedUpdate.
            Move();
            Turn();
        }

        public void UpdateMovement(float axisValue)
        {
            OnMovement(IsMoving());
            movementInputValue = axisValue;
        }
        public void UpdateTurning(float axisValue)
        {
            OnMovement(IsMoving());
            turnInputValue = axisValue;
        }

        private bool IsMoving()
        {
            // Player is moving if either of the input values
            // is different then zero.
            return movementInputValue != 0 || turnInputValue != 0;
        }
        private void Move ()
        {
            // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
            Vector3 movement = transform.forward * movementInputValue * currentSpeed * Time.deltaTime;

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

        
    }
}