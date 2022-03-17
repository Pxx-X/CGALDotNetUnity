using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CGALDotNet;
using CGALDotNetGeometry.Numerics;
using CGALDotNet.Polyhedra;
using CGALDotNet.Processing;
using Common.Unity.Drawing;

namespace CGALDotNetUnity.Polyhedra
{

    public class PolyhedronBooleanExample : MonoBehaviour
    {
        public Material material;

        public Color wireFrameColor = Color.black;

        private GameObject m_mesh;

        private SegmentRenderer m_wireframeRender;

        private void Start()
        {
            var box1 = PolyhedronFactory<EEK>.CreateCube();
            box1.Translate(new Point3d(0.5));

            var box2 = PolyhedronFactory<EEK>.CreateCube();

            var boolean = MeshProcessingBoolean<EEK>.Instance;
            var op = POLYHEDRA_BOOLEAN.UNION;

            if (boolean.Op(op, box1, box2, out Polyhedron3<EEK> poly))
            {
                m_mesh = poly.ToUnityMesh("Mesh", material, true);
                m_mesh.transform.position = new Vector3(0, 0.5f, 0);

                m_wireframeRender = RendererBuilder.CreateWireframeRenderer(poly, wireFrameColor, 0);
            }

        }

        private void OnRenderObject()
        {
            m_wireframeRender.LocalToWorld = m_mesh.transform.localToWorldMatrix;
            m_wireframeRender.Draw();
        }

    }

}
