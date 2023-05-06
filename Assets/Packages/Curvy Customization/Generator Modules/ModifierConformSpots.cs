using UnityEngine;
using FluffyUnderware.DevTools;
using System.Collections.Generic;

/* Custom Tool Extension Description
 * use case: Align randomized object positions on uneven surfaces, allowing for changes to that surface after the fact (i.e. when destructively editing a terrain)
 * 
 * This extension moves spawned gameobjects to a surface. 
---
 * In the context of this tool. Mesh or Prefab object placement positions, scales and rotations are called "Spots". 
 * They are assigned a quaternion or euler rotation and worldspace vector3 location in a different node.
---
 * This extension takes input Object Placement data from a "Spot" and conforms it to a surface using a directable raycast. 
 * There is a similar node within this tool that conformas a path to a surface, however path-conforming causes errors, as it calculates conformed positions before the randomization of object/offsets occur.
 
 * This Conform Spots node handles offsets after the objects are already randomized and assigned positions, so it does not cause errors in final object placement.
*/

namespace FluffyUnderware.Curvy.Generator.Modules
{
    [ModuleInfo("Modifier/Conform Spots", ModuleName = "Conform Spots", Description = "Snap Spot to ground below final placement position")]
    public class ModifierConformSpots : CGModule
    {

        [HideInInspector]
        [InputSlotInfo(typeof(CGSpots),Array = true, Name = "Spots", ModifiesData = true)]
        public CGModuleInputSlot InSpots = new CGModuleInputSlot();

        [HideInInspector]
        [OutputSlotInfo(typeof(CGSpots), Array = true, Name = "Spots")]
        public CGModuleOutputSlot OutSpots = new CGModuleOutputSlot();

        #region ### Serialized Fields ###

        [SerializeField]
        [VectorEx]
        [Tooltip("Raycast Direction, Worldspace")]
        private Vector3 m_Direction = new Vector3(0, -1, 0);
        [SerializeField]
        [Tooltip("The maximum raycast distance in units")]
        private float m_MaxDistance = 200;
        [SerializeField]
        [Tooltip("Offset of placed object from hit point, in relation to raycast direction. -1 while casting vector 3 0 ,-1, 0  results in resulting placement being 1 unit above hit surface.")]
        private float m_Offset;
        [SerializeField]
        [Tooltip("Layers you want to raycast against")]
        private LayerMask m_LayerMask;

        #endregion

        #region ### Public Properties ###

        /// <summary>
        /// Raycast Direction, Worldspace
        /// </summary>
        public Vector3 Direction
        {
            get
            {
                return m_Direction;
            }
            set
            {
                if (m_Direction != value)
                    m_Direction = value;
                Dirty = true;
            }
        }

        /// <summary>
        /// The maximum raycast distance in units
        /// </summary>
        public float MaxDistance
        {
            get { return m_MaxDistance; }
            set
            {
                if (m_MaxDistance != value)
                    m_MaxDistance = value;
                Dirty = true;
            }
        }

        /// <summary>
        /// Offset of placed object from hit point, in relation to raycast direction. -1 while casting vector 3 0 ,-1, 0  results in resulting placement being 1 unit above hit surface.
        /// </summary>
        public float Offset
        {
            get { return m_Offset; }
            set
            {
                if (m_Offset != value)
                    m_Offset = value;
                Dirty = true;
            }
        }

        /// <summary>
        /// Layers you want to raycast against
        /// </summary>
        public LayerMask LayerMask
        {
            get { return m_LayerMask; }
            set
            {
                if (m_LayerMask != value)
                    m_LayerMask = value;
                Dirty = true;
            }
        }

        public int Count { get; private set; }

        //Simulated spots are used by the scene view GUI to draw gizmos which indicate new position and orientation of adjusted spots.
        //I am using this because it is reflected in other modules, and I do not have a perfect understanding of how data is being handled. It works.
        public CGSpots SimulatedSpots;


        #endregion

        #region ### Unity Callbacks ###
        /*! \cond UNITY */

        protected override void OnEnable()
        {
            base.OnEnable();
            Properties.LabelWidth = 80;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Direction = m_Direction;
            MaxDistance = m_MaxDistance;
            Offset = m_Offset;
            LayerMask = m_LayerMask;
        }
#endif

        public override void Reset()
        {
            base.Reset();
            Direction = new Vector3(0, -1, 0);
            MaxDistance = 200;
            Offset = 0;
            LayerMask = 1;
        }

        #endregion

        #region ### Module Overrides ###

        public void Clear()
        {
            Count = 0;
            SimulatedSpots = new CGSpots();
            OutSpots.SetData(SimulatedSpots);
        }


        public override void Refresh()
        {
            /// <summary>
            /// Conforms a spot by projecting it on top of objects (with a collider) of a specific layer
            /// </summary>
            /// <param name="position"></param>
            /// <param name="spot"></param>
            /// <param name="layers"></param>
            /// <param name="projectionDirection"></param>
            /// <param name="offset"></param>
            /// <param name="rayLength"></param>
            base.Refresh();
            Vector3 projectionDirection = Direction;
            float rayLength = MaxDistance;
            CGSpots spots = InSpots.GetData<CGSpots>();
            List<CGSpot> outSpots = new List<CGSpot>();
            int spotCount = spots.Count;
            LayerMask layers = LayerMask;
            float offset = Offset;

            // Iterate through "spots" in the array, and raycast in direction. Reassign spot positions per hit.
            // If no hit, the object will remain in its original position - normally in the air.

            // I am planning to extend this further to optionally remove invalid spots and provide options to filter out objects according to surface normals.

            if (projectionDirection != Vector3.zero && rayLength > 0 && spotCount > 0)
            {
                RaycastHit raycastHit;

                for (int i = 0; i < spotCount; i++)
                {
                    CGSpot spot = spots.Spots.Array[i];
                    if (Physics.Raycast(spot.Position, projectionDirection, out raycastHit, rayLength, layers))
                    {
                        spot.Position += projectionDirection * (raycastHit.distance + offset);
                        outSpots.Add(spot);
                    }
                }
            }

            //Send the new data to the next node.
            OutSpots.SetData(new CGSpots(outSpots));
            SimulatedSpots = new CGSpots(outSpots);
        }

        #endregion
    }
}
