using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using CGALDotNet;
using CGALDotNetGeometry.Numerics;
using CGALDotNetGeometry.Shapes;
using CGALDotNet.Polyhedra;
using CGALDotNet.Processing;
using CGALDotNet.Polylines;

using Common.Unity.Drawing;
using UnityEngine.Rendering;

namespace CGALDotNetUnity.Processing
{

    public enum SELECT_MODE
    {
        FACE, VERTEX, EDGE
    }

    public class ProcessingExample : MonoBehaviour
    {

        public Color lineColor = Color.black;

        public Color vertexNormalColor = Color.red;

        public Color faceNormalColor = Color.blue;

        public Material material;

        private Polyhedron3<EIK> m_mesh;

        private GameObject m_object;

        private SegmentRenderer  m_wireframe, m_featureRenderer;

        private NormalRenderer m_vertNormalRenderer, m_faceNormalRenderer;

        private VertexRenderer m_pointRenderer;

        private string m_info;

        private double m_refineFactor = 3;

        private double m_featureAngle = 60;

        private double m_targetEdgeLen = 0.05;

        private double m_simplifyAmount = 0.5;

        private SELECT_MODE m_selectionMode = SELECT_MODE.FACE;

        private MeshFace3? m_hitFace;

        private MeshVertex3? m_hitVertex;

        private MeshHalfedge3? m_hitEdge;

        private float wireframeOffset = 0.002f;

        private void Start()
        {

        }

        private GameObject CreateGameobject(string name, Polyhedron3<EIK> poly)
        {
            var go = poly.ToUnityMesh(name, material, false);
            return go;
        }

        private void RenderEdgeFeature(List<int> edges)
        {
            ClearLast();
            m_featureRenderer = new SegmentRenderer();
            m_featureRenderer.DefaultColor = Color.red;

            foreach(var edge in edges)
            {
                Segment3d seg;
                if(m_mesh.GetSegment(edge, out seg))
                {
                    var a = seg.A.ToUnityVector3();
                    var b = seg.B.ToUnityVector3();
                    m_featureRenderer.Load(a, b);
                }
            }
        }

        private void RenderEdgeFeature(int a, int b)
        {
            ClearLast();
            m_featureRenderer = new SegmentRenderer();
            m_featureRenderer.DefaultColor = Color.red;

            var A = m_mesh.GetPoint(a);
            var B = m_mesh.GetPoint(b);

            m_featureRenderer.Load(A.ToUnityVector3(), B.ToUnityVector3());
        }

        private void RenderFaceFeature(List<Point3d> points)
        {
            ClearLast();
            m_featureRenderer = new SegmentRenderer();
            m_featureRenderer.DefaultColor = Color.red;

            int count = points.Count;
            for (int i = 0; i < count; i++)
            {
                var a = points[MathUtil.Wrap(i + 0, count)].ToUnityVector3();
                var b = points[MathUtil.Wrap(i + 1, count)].ToUnityVector3();
                m_featureRenderer.Load(a, b);
            }
        }

        private void RenderVertexFeature(Point3d point)
        {
            ClearLast();
            m_pointRenderer = new VertexRenderer(0.005f);
            m_pointRenderer.DefaultColor = Color.red;
            m_pointRenderer.Load(point.ToUnityVector3());
        }

        private void CreateWireFrameObj()
        {
            bool enabled = m_wireframe != null ? m_wireframe.Enabled : true;
            m_wireframe = RendererBuilder.CreateWireframeRenderer(m_mesh, lineColor, wireframeOffset);
            m_wireframe.Enabled = enabled;
        }

        private void ToggleWireFrame()
        {
            if (m_wireframe == null)
            {
                m_wireframe = RendererBuilder.CreateWireframeRenderer(m_mesh, lineColor, wireframeOffset);
            }
            else if(m_wireframe != null)
            {
                m_wireframe.Enabled = !m_wireframe.Enabled;
            }
        }

        private void ToggleVertexNormals()
        {
            if (m_vertNormalRenderer == null)
            {
                m_vertNormalRenderer = RendererBuilder.CreateVertexNormalRenderer(m_mesh, vertexNormalColor, 0.01f);
            }
            else if (m_vertNormalRenderer != null)
            {
                m_vertNormalRenderer.Enabled = !m_vertNormalRenderer.Enabled;
            }
        }

        private void ToggleFaceNormals()
        {
            if (m_faceNormalRenderer == null)
            {
                m_faceNormalRenderer = RendererBuilder.CreateFaceNormalRenderer(m_mesh, faceNormalColor, 0.01f);
            }
            else if (m_faceNormalRenderer != null)
            {
                m_faceNormalRenderer.Enabled = !m_faceNormalRenderer.Enabled;
            }
        }

        private void LoadMesh(string file)
        {
            string filename = Application.dataPath + "/Examples/Data/" + file;

            var split = filename.Split('/', '.');
            int i = split.Length - 2;
            var name = i > 0 ? split[i] : "Mesh";

            m_mesh = new Polyhedron3<EIK>();
            m_mesh.ReadOFF(filename);

        }

        private void OnRenderObject()
        {
            if(m_object != null && m_wireframe != null && m_wireframe.Enabled)
            {
                m_wireframe.SetColor(lineColor);
                //m_wireframe.LocalToWorld = m_object.transform.localToWorldMatrix;
                m_wireframe.Draw();
            }

            if (m_featureRenderer != null && m_featureRenderer.Enabled)
            {
                //m_featureRenderer.LocalToWorld = m_object.transform.localToWorldMatrix;
                m_featureRenderer.Draw();
            }

            if (m_vertNormalRenderer != null && m_vertNormalRenderer.Enabled)
            {
                m_vertNormalRenderer.SetColor(vertexNormalColor);
                //m_vertNormalRenderer.LocalToWorld = m_object.transform.localToWorldMatrix;
                m_vertNormalRenderer.Draw();
            }

            if (m_faceNormalRenderer != null && m_faceNormalRenderer.Enabled)
            {
                m_faceNormalRenderer.SetColor(faceNormalColor);
                //m_faceNormalRenderer.LocalToWorld = m_object.transform.localToWorldMatrix;
                m_faceNormalRenderer.Draw();
            }

            if (m_pointRenderer != null && m_pointRenderer.Enabled)
            {
                //m_pointRenderer.LocalToWorld = m_object.transform.localToWorldMatrix;
                m_pointRenderer.Draw();
            }

        }

        private void OnLeftClick()
        {

            if (m_mesh != null && Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (m_selectionMode == SELECT_MODE.FACE)
                {
                    if (m_mesh.LocateFace(ray.ToCGALRay3d(), out MeshFace3 face))
                    {
                        m_hitFace = face;
                        m_info = "Hit Face = " + m_hitFace.ToString();

                        List<Point3d> points = new List<Point3d>();
                        foreach(var v in face.EnumerateVertices(m_mesh))
                            points.Add(v.Point);

                        RenderFaceFeature(points);
                    }
                    else
                    {
                        m_hitFace = null;
                    }
                }
                else if (m_selectionMode == SELECT_MODE.VERTEX)
                {
                    if (m_mesh.LocateVertex(ray.ToCGALRay3d(), 0.01, out MeshVertex3 vertex))
                    {
                        m_hitVertex = vertex;
                        m_hitVertex.Value.Point.Round(4);
                        m_info = "Hit Vertex = " + m_hitVertex.ToString();

                        RenderVertexFeature(m_hitVertex.Value.Point);
                    }
                    else
                    {
                        m_hitVertex = null;
                    }
                }
                else if (m_selectionMode == SELECT_MODE.EDGE)
                {
                    if (m_mesh.LocateHalfedge(ray.ToCGALRay3d(), 0.01, out MeshHalfedge3 edge))
                    {
                        m_hitEdge = edge;
                        m_info = "Hit Halfedge = " + m_hitEdge.ToString();

                        RenderEdgeFeature(edge.Source, edge.Target);
                    }
                    else
                    {
                        m_hitEdge = null;
                    }
                }
            }

        }

        private void ClearScene()
        {
            if (m_object != null)
            {
                DestroyImmediate(m_object);
                m_object = null;
            }

            m_mesh = null;
            m_wireframe = null;
            m_featureRenderer = null;
            m_vertNormalRenderer = null;
            m_faceNormalRenderer = null;
            m_info = "";
        }

        private void ClearLast()
        {
            m_featureRenderer = null;
            m_vertNormalRenderer = null;
            m_faceNormalRenderer = null;
            m_pointRenderer = null;
            m_info = "";
        }

        private void DestroyObject(GameObject go)
        {
            if (go == null) return;

            go.SetActive(false);
            DestroyImmediate(go);
        }

        private void Update()
        {

            OnLeftClick();

            if(m_mesh != null && Input.GetKeyDown(KeyCode.Space))
            {
                ClearScene();
            }

            if (m_mesh == null)
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    ClearScene();

                    var pos = new Vector3(0, 0, 0);
                    var rot = Quaternion.Euler(0, 180, 0);
                    var scale = Vector3.one;

                    LoadMesh("bunny00.off");

                    m_mesh.Transform(pos.ToCGALPoint3d(), rot.ToCGALQuaternion3d(), scale.ToCGALPoint3d());
                    m_object = CreateGameobject("Bunny", m_mesh);

                    CreateWireFrameObj();
                }
                else if (Input.GetKeyDown(KeyCode.F2))
                {
                    ClearScene();

                    var pos = new Vector3(0, 0, 0);
                    var rot = Quaternion.identity;
                    var scale = Vector3.one;

                    LoadMesh("elephant.off");

                    m_mesh.Transform(pos.ToCGALPoint3d(), rot.ToCGALQuaternion3d(), scale.ToCGALPoint3d());
                    m_object = CreateGameobject("elephant", m_mesh);

                    CreateWireFrameObj();
  
                }
                else if (Input.GetKeyDown(KeyCode.F3))
                {
                    ClearScene();

                    var pos = new Vector3(0, 0, 4);
                    var rot = Quaternion.Euler(-90, 0, 180);
                    var scale = new Vector3(0.1f, 0.1f, 0.1f);

                    LoadMesh("mannequin-devil.off");

                    m_mesh.Transform(pos.ToCGALPoint3d(), rot.ToCGALQuaternion3d(), scale.ToCGALPoint3d());
                    m_object = CreateGameobject("mannequin", m_mesh);

                    CreateWireFrameObj();
                }
                else if (Input.GetKeyDown(KeyCode.F4))
                {
                    ClearScene();

                    var pos = new Vector3(0, 0, 0);
                    var rot = Quaternion.Euler(180, 90, 0);
                    var scale = Vector3.one;

                    LoadMesh("fandisk.off");

                    m_mesh.Transform(pos.ToCGALPoint3d(), rot.ToCGALQuaternion3d(), scale.ToCGALPoint3d());
                    m_object = CreateGameobject("fandisk", m_mesh);

                    CreateWireFrameObj();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    m_selectionMode = m_selectionMode.Next();
                }
                else if (Input.GetKeyDown(KeyCode.F1))
                {
                    ToggleWireFrame();
                }
                else if (Input.GetKeyDown(KeyCode.F2))
                {
                    ToggleVertexNormals();
                    
                }
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    ToggleFaceNormals();
                }
                else if (Input.GetKeyDown(KeyCode.F4))
                {
                    ClearLast();

                    int new_verts = m_mesh.Refine(m_refineFactor);
                    m_info = "New vertices added " + new_verts;

                    var name = m_object.name;

                    DestroyObject(m_object);
                    m_object = CreateGameobject(name, m_mesh);

                    CreateWireFrameObj();
                }
                else if (Input.GetKeyDown(KeyCode.F5))
                {
                    ClearLast();

                    var processor = MeshProcessingMeshing<EIK>.Instance;

                    var minmax = m_mesh.FindMinMaxAvgEdgeLength();
                    m_targetEdgeLen = Math.Round(minmax.Average, 4);

                    int new_verts = processor.IsotropicRemeshing(m_mesh, m_targetEdgeLen, 1);
                    m_info = "New vertices added " + new_verts;

                    var name = m_object.name;

                    DestroyObject(m_object);
                    m_object = CreateGameobject(name, m_mesh);
                    CreateWireFrameObj();
                }
                else if (Input.GetKeyDown(KeyCode.F6))
                {
                    var processor = MeshProcessingFeatures<EIK>.Instance;

                    var edges = new List<int>();
                    processor.DetectSharpEdges(m_mesh, new Degree(m_featureAngle), edges);
                    m_info = "Feature edges " + edges.Count;
                    RenderEdgeFeature(edges);
                }
                else if (Input.GetKeyDown(KeyCode.F7))
                {
                    var processor = SubdivisionSurface<EIK>.Instance;

                    processor.Subdivide(SUBDIVISION_METHOD.LOOP, m_mesh, 1);

                    var name = m_object.name;
         
                    DestroyObject(m_object);
                    m_object = CreateGameobject(name, m_mesh);
                    CreateWireFrameObj();
                }
                else if (Input.GetKeyDown(KeyCode.F8))
                {
                    var processor = SurfaceSimplification<EIK>.Instance;

                    processor.Simplify(m_mesh, m_simplifyAmount);

                    var name = m_object.name;

                    DestroyObject(m_object);
                    m_object = CreateGameobject(name, m_mesh);
                    CreateWireFrameObj();
                }

            }
        }

        protected void OnGUI()
        {
            int textLen = 1000;
            int textHeight = 1000;
            GUI.color = Color.black;

            if (m_mesh == null)
            {
                GUI.Label(new Rect(10, 10, textLen, textHeight), "Tab to toggle wireframe.");
                GUI.Label(new Rect(10, 30, textLen, textHeight), "F1 to load bunny.");
                GUI.Label(new Rect(10, 50, textLen, textHeight), "F2 to load elephant.");
                GUI.Label(new Rect(10, 70, textLen, textHeight), "F3 to load mannequin.");
                GUI.Label(new Rect(10, 90, textLen, textHeight), "F4 to load fan disk.");
                GUI.Label(new Rect(10, 110, textLen, textHeight), "Space to clear mesh.");
            }
            else
            {
                GUI.Label(new Rect(10, 10, textLen, textHeight), "Tab to toggle selection mode.");
                GUI.Label(new Rect(10, 30, textLen, textHeight), string.Format("Left click to select {0}.", m_selectionMode));

                if(m_hitVertex != null)
                    GUI.Label(new Rect(10, 50, textLen, textHeight), string.Format("Hit vertex {0}.", m_hitVertex.Value));

                else if (m_hitFace != null)
                    GUI.Label(new Rect(10, 50, textLen, textHeight), string.Format("Hit face {0}.", m_hitFace.Value));

                else if (m_hitEdge != null)
                    GUI.Label(new Rect(10, 50, textLen, textHeight), string.Format("Hit edge {0}.", m_hitEdge.Value));
                else
                    GUI.Label(new Rect(10, 50, textLen, textHeight), string.Format("No selection."));

                GUI.Label(new Rect(10, 90, textLen, textHeight), string.Format("F1 to toggle wireframe."));
                GUI.Label(new Rect(10, 110, textLen, textHeight), string.Format("F2 to toggle vertex normals."));
                GUI.Label(new Rect(10, 130, textLen, textHeight), string.Format("F3 to toggle face normals."));
                GUI.Label(new Rect(10, 150, textLen, textHeight), string.Format("F4 to refine with factor {0}.", m_refineFactor));
                GUI.Label(new Rect(10, 170, textLen, textHeight), string.Format("F5 perform isotropic remeshing with target edge length {0}.", m_targetEdgeLen));
                GUI.Label(new Rect(10, 190, textLen, textHeight), string.Format("F6 to dected sharp edges with angle {0}.", m_featureAngle));
                GUI.Label(new Rect(10, 210, textLen, textHeight), string.Format("F7 to subdivide once."));
                GUI.Label(new Rect(10, 230, textLen, textHeight), string.Format("F8 to simplify {0}.", m_simplifyAmount));
                GUI.Label(new Rect(10, 250, textLen, textHeight), m_info);
            }

        }

    }

}
