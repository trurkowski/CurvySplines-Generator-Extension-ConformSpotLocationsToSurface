using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using FluffyUnderware.Curvy.Generator;
using FluffyUnderware.Curvy.Generator.Modules;
using FluffyUnderware.DevToolsEditor;
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.DevToolsEditor.Extensions;
using UnityEditorInternal;
using FluffyUnderware.Curvy;

namespace FluffyUnderware.CurvyEditor.Generator.Modules
{
    [CustomEditor(typeof(ModifierConformSpots))]
    public class ModifierConformSpotsEditor : CGModuleEditor<ModifierConformSpots>
    {

        //Scene View GUI - Called only if the module is initialized and configured
        //public override void OnModuleSceneGUI() {}

        protected override void OnEnable()
        {
            base.OnEnable();
            HasDebugVisuals = true;
        }

        public override void OnModuleSceneDebugGUI()
        {
            CGSpots data = Target.SimulatedSpots;
            if (data)
            {
                Handles.matrix = Target.Generator.transform.localToWorldMatrix;
                for (int i = 0; i < data.Spots.Count; i++)
                {
                    Quaternion Q = data.Spots.Array[i].Rotation * Quaternion.Euler(-90, 0, 0);
                    
                    Handles.ArrowHandleCap(0, data.Spots.Array[i].Position, Q, 2, EventType.Repaint);

                    Handles.Label(data.Spots.Array[i].Position, data.Spots.Array[i].Index.ToString(), EditorStyles.whiteBoldLabel);
                }
                Handles.matrix = Matrix4x4.identity;
            }
        }
        
        // Inspector Debug GUI - Called only when Show Debug Values is activated
        //public override void OnModuleDebugGUI() {}
        
    }
   
}