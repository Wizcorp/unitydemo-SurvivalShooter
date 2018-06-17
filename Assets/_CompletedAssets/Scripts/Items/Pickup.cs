using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
    public class Pickup : MonoBehaviour
    {

        //inventory values
        public GameObject pickupWeapon;
        public Sprite inviUIIcon;
        public int pickupAmount = 15;
        public float rotation = 15f;

        public float timeoutTime = 20;

        //values for passive bobbing and spinning
        float floatDeltaTime;
        float baseYPos;
        public float bounceSize = 1f;
        public float bounceSpeed = 1f;

        //initialize the starting y position to keep the sine curve around that point
        private void Awake()
        {
            baseYPos = transform.position.y;
        }

        //NOTE: temporarily diabled for testing
        /**
        private void Start()
        {
            WaitAndDestroy();
        }
        **/

        // Make the pickup spin and bounce
        void Update()
        {

            floatDeltaTime += Time.deltaTime;

            //simple rotation
            transform.Rotate(new Vector3(0, rotation, 0) * Time.deltaTime);
            //translate on the y-axis based on a sine curve
            transform.localPosition = new Vector3(transform.position.x, baseYPos + Mathf.Abs(bounceSize * Mathf.Sin(bounceSpeed * floatDeltaTime)), transform.position.z);

        }

        //destroy self after waiting for some time
        private void WaitAndDestroy()
        {
            Object.Destroy(gameObject, timeoutTime);
        }

        //add to player inventory and remove current instance
        private void OnCollisionEnter(Collision collision)
        {
            GameObject otherObject = collision.gameObject;

            //verify other object is player, otherwise ignore
            if (otherObject.CompareTag("Player"))
            {
                PlayerInventory playerInventory = otherObject.GetComponent<PlayerInventory>();

                //verify that player has inventory
                if (playerInventory)
                {

                    bool addItemSuccess = playerInventory.PickupItem(this);

                    if (addItemSuccess) { Destroy(gameObject); }
                    else { print("Inventory full"); }
                }
                else { Debug.Log("<color=red>Fatal error:</color> Inventory script not properly added to player character or object is improperly tagged"); }
            }
        }
    }
}
