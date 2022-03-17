using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using CGALDotNet;
using CGALDotNetGeometry.Numerics;
using CGALDotNet.Polyhedra;

using Common.Unity.Drawing;

namespace CGALDotNetUnity.Polyhedra
{

    public class PolyhedronExample : MonoBehaviour
    {
        public Material material;

        public bool allowQuads = false;

        public bool drawSegments = false;

        public Color lineColor = Color.black;

        private GameObject m_cube;

        private GameObject m_uvsphere;

        private GameObject m_normalizedCube;

        private GameObject m_icosahedron;

        private GameObject m_plane;

        private GameObject m_torus;

        private GameObject m_tetrahedron;

        private GameObject m_octohedron;

        private GameObject m_dodecahedron;

        private GameObject m_cylinder;

        private GameObject m_cone;

        private GameObject m_capsule;

        private GameObject m_dual;

        private SegmentRenderer m_wireframeRender;

        private Color[] m_colorTable = new Color[]
        {
            new Color(1, 0, 0, 1),
            new Color(1, 0, 0, 1),
            new Color(1, 0, 0, 1),
            new Color(1, 0, 0, 1),
            new Color(1, 0, 0, 1)
        };

        private void Start()
        {
            m_wireframeRender = new SegmentRenderer();
            m_wireframeRender.DefaultColor = lineColor;

            m_cube = CreateCube(new Vector3(3, 0.5f, 0));

            m_uvsphere = CreateUVSphere(new Vector3(1, 0.5f, 0));

            m_normalizedCube = CreateNormalizedCube(new Vector3(-1, 0.5f, 0));

            m_icosahedron = CreateIcosahedron(new Vector3(-3, 0.5f, 0));

            m_plane = CreatePlane(new Vector3(3, 0, 3));

            m_torus = CreateTorus(new Vector3(1, 0, 3));

            m_tetrahedron = CreateTetrahedron(new Vector3(-1, 0, 3));

            m_octohedron = CreateOctohedron(new Vector3(-3, 0, 3));

            m_dodecahedron = CreateDodecahedron(new Vector3(3, 0, 6));

            m_cylinder = CreateCylinder(new Vector3(1, 0, 6));

            m_cone = CreateCone(new Vector3(-1, 0, 6));

            //m_capsule = CreateCapsule(new Vector3(-3, 0, 6));

            m_dual = CreateDual(new Vector3(-3, 0, 6));

        }

        private void OnRenderObject()
        {
            if (drawSegments)
            {
                m_wireframeRender.SetColor(lineColor);
                m_wireframeRender.Draw();
            }
        }

        private GameObject CreateCube(Vector3 translation)
        {
            var poly = PolyhedronFactory<EIK>.CreateCube(1, allowQuads);
            poly.Translate(translation.ToCGALPoint3d());

            if (drawSegments)
                DrawSegments(poly);

            return poly.ToUnityMesh("Cube", material, true);
        }

        private GameObject CreateUVSphere(Vector3 translation)
        {
            var param = UVSphereParams.Default;
            var poly = PolyhedronFactory<EIK>.CreateUVSphere(param, allowQuads);
            poly.Translate(translation.ToCGALPoint3d());

            if (drawSegments)
                DrawSegments(poly);

            return poly.ToUnityMesh("UV sphere", material, true);
        }

        private GameObject CreateNormalizedCube(Vector3 translation)
        {
            var param = NormalizedCubeParams.Default;
            var poly = PolyhedronFactory<EIK>.CreateNormalizedCube(param, allowQuads);
            poly.Translate(translation.ToCGALPoint3d());

            if (drawSegments)
                DrawSegments(poly);

            return poly.ToUnityMesh("Normalized cube", material, true);
        }

        private GameObject CreateTetrahedron(Vector3 translation)
        {
            var poly = PolyhedronFactory<EIK>.CreateTetrahedron();
            poly.Translate(translation.ToCGALPoint3d());

            if (drawSegments)
                DrawSegments(poly);

            return poly.ToUnityMesh("Tetrahedron", material, true);
        }

        private GameObject CreateIcosahedron(Vector3 translation)
        {
            var poly = PolyhedronFactory<EIK>.CreateIcosahedron();
            poly.Translate(translation.ToCGALPoint3d());

            if (drawSegments)
                DrawSegments(poly);

            return poly.ToUnityMesh("Icosahedron",  material, true);
        }

        private GameObject CreateOctohedron(Vector3 translation)
        {
            var poly = PolyhedronFactory<EIK>.CreateOctahedron();
            poly.Translate(translation.ToCGALPoint3d());

            if (drawSegments)
                DrawSegments(poly);

            return poly.ToUnityMesh("Octahedron", material, true);
        }

        private GameObject CreateDodecahedron(Vector3 translation)
        {
            var poly = PolyhedronFactory<EIK>.CreateDodecahedron(1, true);
            poly.Translate(translation.ToCGALPoint3d());

            if (drawSegments)
                DrawSegments(poly);

            return poly.ToUnityMesh("Dodecahedron", material, true);
        }

        private GameObject CreatePlane(Vector3 translation)
        {
            var param = PlaneParams.Default;
            var poly = PolyhedronFactory<EIK>.CreatePlane(param, allowQuads);
            poly.Translate(translation.ToCGALPoint3d());

            if (drawSegments)
                DrawSegments(poly);

            return poly.ToUnityMesh("Plane", material, true);
        }

        private GameObject CreateTorus(Vector3 translation)
        {
            var param = TorusParams.Default;
            var poly = PolyhedronFactory<EIK>.CreateTorus(param, allowQuads);
            poly.Translate(translation.ToCGALPoint3d());

            if (drawSegments)
                DrawSegments(poly);

            return poly.ToUnityMesh("Torus", material, true);
        }

        private GameObject CreateCylinder(Vector3 translation)
        {
            var param = CylinderParams.Default;
            var poly = PolyhedronFactory<EIK>.CreateCylinder(param, allowQuads);
            poly.Translate(translation.ToCGALPoint3d());

            if (drawSegments)
                DrawSegments(poly);

            return poly.ToUnityMesh("Cylinder", material, true);
        }

        private GameObject CreateCone(Vector3 translation)
        {
            var param = ConeParams.Default;
            var poly = PolyhedronFactory<EIK>.CreateCone(param, allowQuads);
            poly.Translate(translation.ToCGALPoint3d());

            if (drawSegments)
                DrawSegments(poly);

            return poly.ToUnityMesh("Cone", material, true);
        }

        private GameObject CreateCapsule(Vector3 translation)
        {
            var param = CapsuleParams.Default;
            var poly = PolyhedronFactory<EIK>.CreateCapsule(param, allowQuads);
            poly.Translate(translation.ToCGALPoint3d());

            if (drawSegments)
                DrawSegments(poly);

            return poly.ToUnityMesh("Capsule", material, true);
        }

        private GameObject CreateDual(Vector3 translation)
        {
            var poly = PolyhedronFactory<EIK>.CreateIcosahedron();
            poly.Translate(translation.ToCGALPoint3d());
            poly.Subdivide(2);

            var dual = poly.Dual();

            if (drawSegments)
                DrawSegments(dual);

            return dual.ToUnityMesh("Dual", material, true);
        }

        private void DrawSegments(Polyhedron3 poly)
        {
            m_wireframeRender = RendererBuilder.CreateWireframeRenderer(poly, lineColor, 0, m_wireframeRender);
        }

    }

}
