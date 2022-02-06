/*
*   Copyright (C) 2020 University of Central Florida, created by Dr. Ryan P. McMahan.
*
*   This program is free software: you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation, either version 3 of the License, or
*   (at your option) any later version.
*
*   This program is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*
*   Primary Author Contact:  Dr. Ryan P. McMahan <rpm@ucf.edu>
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.Interaction.Toolkit
{
    // The BubbleCursorProvider works with the XRDirectInteractor to provide a 3D bubble cursor.
    [AddComponentMenu("XRST/Interaction/BubbleCursorProvider")]
    public class BubbleCursorProvider : MonoBehaviour
    {
        // The SphereCollider that the XRDirectInteractor uses for triggering. 
        [SerializeField]
        [Tooltip("The SphereCollider that the XRDirectInteractor uses for triggering.")]
        SphereCollider m_Collider;
        public SphereCollider Collider { get { return m_Collider; } set { m_Collider = value; } }

        // The GameObject that the XRDirectInteractor uses to represent the 3D cursor. 
        [SerializeField]
        [Tooltip("The GameObject that the XRDirectInteractor uses to represent the 3D cursor.")]
        GameObject m_Cursor;
        public GameObject Cursor { get { return m_Cursor; } set { m_Cursor = value; } }

        // The minimum radius that the cursor will shrink to. 
        [SerializeField]
        [Tooltip("The minimum radius that the cursor will shrink to.")]
        float m_MinRadius = 0.1f;
        public float MinRadius { get { return m_MinRadius; } set { m_MinRadius = value; } }

        // The maximum radius that the cursor will grow to. 
        [SerializeField]
        [Tooltip("The maximum radius that the cursor will grow to.")]
        float m_MaxRadius = 0.5f;
        public float MaxRadius { get { return m_MaxRadius; } set { m_MaxRadius = value; } }

        // Update is called once per frame.
        void Update()
        {
            // If the collider and cursor are valid.
            if (Collider != null && Cursor != null)
            {
                // Get a list of currently valid interactables. 
                List<XRGrabInteractable> interactables = new List<XRGrabInteractable>(FindObjectsOfType<XRGrabInteractable>());

                // Determine the closest interactable.
                int closest = -1;
                float closestDistance = Mathf.Infinity;

                // For each interactable.
                for (int i = 0; i < interactables.Count; i++)
                {
                    // Fetch the interactable's collider.
                    Collider interactableCollider = interactables[i].GetComponent<Collider>();

                    // Ensure the collider is valid.
                    if (interactableCollider != null)
                    {
                        // Fetch the collider's bounds.
                        Bounds interactableBounds = interactableCollider.bounds;

                        // Determine its closest point to our collider.
                        Vector3 closestPoint = interactableBounds.ClosestPoint(Collider.transform.position);

                        // Determine its distance to our collider.
                        float distance = Vector3.Distance(closestPoint, Collider.transform.position);

                        // Buffer the distance to avoid non-collisions.
                        distance += 0.01f;

                        // Keep track of the closest distance and interactable.
                        if (distance < closestDistance)
                        {
                            closest = i;
                            closestDistance = distance;
                        }
                    }
                }

                // If a closest interactable was found and within the maximum radius.
                if (closest > -1 && closestDistance <= MaxRadius)
                {
                    // If the closest distance is smaller than the minimum radius, reset it.
                    if (closestDistance < MinRadius)
                    {
                        closestDistance = MinRadius;
                    }

                    // Set the collider and cursor to match the closest distance.
                    Collider.radius = closestDistance;
                    Cursor.transform.localScale = new Vector3(closestDistance * 2.0f, closestDistance * 2.0f, closestDistance * 2.0f);
                }
                // Otherwise, set the collider and cursor to the minimum radius.
                else
                {
                    Collider.radius = MinRadius;
                    Cursor.transform.localScale = new Vector3(MinRadius * 2.0f, MinRadius * 2.0f, MinRadius * 2.0f);
                }
            }
        }
    }
}
