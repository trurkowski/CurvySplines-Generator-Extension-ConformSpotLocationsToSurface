# CurvySplines-Generator-Extension-ConformSpotLocationsToSurface

 Creates a node which conforms spot locations to a surface - similar to path conforming module. The conformation is done after the object offset randomization is complete, so is more accurate per-object.

 Curvysplines has an object placement assigner with randomization or sequence settings which places a customizable list of objects offset along a spline.
 It uses a node graph editor, and you can feed spline and object data through it and modify the data in several ways. 
 You can 
   - Place prefabs
   - Place meshes
   - Deform meshes
   - Generate new meshes procedurally with a crossection and cap system
   - Control/animate objects along a path

 For example, if you want to create a slightly randomized line of trees alongside a path in a forest setting, transposed relative to the spline path, this allows you to do that with control over several parameters and randomization options.

 it is very easily extendable when you gain some understanding of how it manages data for optimization purposes.
 The tool developer is responsive to requests and questions - documentation is decent, and the code is fairly well commented.
 
 Use case: Align randomized object positions on uneven surfaces, allowing for changes to that surface after the fact (i.e. when destructively editing a terrain)
---
 * In the context of this tool. Mesh or Prefab object placement positions, scales and rotations are called "Spots". 
 * They are assigned a quaternion or euler rotation and worldspace vector3 location in a different node.
---
 * This extension takes input Object Placement data from a "Spot" and conforms it to a surface using a directable raycast. 
 * There is a similar node within this tool that conformas a path to a surface, however path-conforming causes errors, as it calculates conformed positions before the randomization of object/offsets occur.
 
 * This Conform Spots node handles offsets after the objects are already randomized and assigned positions, so it does not cause errors in final object placement.
