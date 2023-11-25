using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class CRevealPath : MonoBehaviour
{
    [SerializeField]
    Material mMaskMaterial;

    [SerializeField]
    Material mPreviewMaterial;
    
    [SerializeField]
    CSpellScriptableObject mSpellInfo;

    [SerializeField]
    LayerMask mRevealObjectMask;
    
    [SerializeField]
    int mDrawMaskBufferLayer;
    
    // Drag positions from camera to world
    Vector3 mInitWorldPos;
    Vector3 mCurrentWorldPos;

    GameObject mPreviewObject;
    Mesh mPreviewQuadMesh;

    bool mIsValidDrag;

    readonly KeyCode mClickKeycode;

    MaterialPropertyBlock mPreviewMaterialPropertyBlock;
    
    MaterialPropertyBlock PREVIEW_MPB
    {
        get
        {
            if(mPreviewMaterialPropertyBlock == null)
                mPreviewMaterialPropertyBlock = new MaterialPropertyBlock();

            return mPreviewMaterialPropertyBlock;
        }
    }


    public CRevealPath()
    {
        mInitWorldPos = Vector2.zero;
        mClickKeycode = KeyCode.Mouse0;
    }
    
    private void Update() 
    {
        ClickAndDrag();
    }

    void ClickAndDrag()
    {
        if(Input.GetKeyDown(mClickKeycode))
        {
            Ray ClickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(ClickRay, out RaycastHit HitInfo, Mathf.Infinity, mRevealObjectMask))
            {
                mInitWorldPos = HitInfo.point;
                mIsValidDrag = true;
            }
            else
            {
                Debug.LogError("There should be an object to mask a reveal path");
                mIsValidDrag = false;
            }
        }

        if(mIsValidDrag && Input.GetKey(mClickKeycode))
        {
            Ray DragRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(DragRay, out RaycastHit HitInfo, Mathf.Infinity, mRevealObjectMask))
            {
                UpdatePreviewMesh();

                mPreviewObject.SetActive(true);

                mCurrentWorldPos = HitInfo.point;
                mIsValidDrag = true;
            }
        }

        if(Input.GetKeyUp(mClickKeycode))
        {
            if(mIsValidDrag)
            {
                Mesh NewMesh = new Mesh();
                NewMesh.name = "RevealQuad";

                SetBoundMesh(ref NewMesh, mInitWorldPos, mCurrentWorldPos);
                
                var NewMaskObject = CreateMaskGameobject(NewMesh);
                NewMaskObject.GetComponent<CSpellRevealMask>().Config(mSpellInfo);
            }

            mPreviewObject.SetActive(false);

            mInitWorldPos = Vector2.zero;
        }
    }

    void ClampPositionToTopLeftCorner()
    {
        mCurrentWorldPos.x = Math.Max(mCurrentWorldPos.x, mInitWorldPos.x);
        mCurrentWorldPos.y = Math.Max(mCurrentWorldPos.y, mInitWorldPos.y);
    }

    void SetBoundMesh(ref Mesh aMesh, Vector3 aPointA, Vector3 aPointB)
    {
        List<Vector3> Vertices = GetBoundsCorners();

        List<int> TriangleIndices = new List<int>()
        {
            0,1,2,
            2,1,3
        };

        List<Vector2> UVs = new List<Vector2>()
        {
            Vector2.up,
            Vector2.one,
            Vector2.zero,
            Vector2.right
        };

        aMesh.SetVertices(Vertices);
        aMesh.SetUVs(0, UVs);
        aMesh.SetTriangles(TriangleIndices, 0);
    }

    void UpdatePreviewMesh()
    {
        if(mPreviewObject == null)
        {
            mPreviewObject = new GameObject("PreviewWindow");
            mPreviewObject.transform.SetParent(this.transform);
            mPreviewObject.transform.SetLocalPositionAndRotation(-Vector3.forward * 0.1f, Quaternion.identity);

            mPreviewQuadMesh = new Mesh();
            mPreviewQuadMesh.name = "PreviewQuad";

            var FilterComponent = mPreviewObject.AddComponent<MeshFilter>();
            var RendererComponent = mPreviewObject.AddComponent<MeshRenderer>();
            RendererComponent.material = mPreviewMaterial;
            
            FilterComponent.mesh = mPreviewQuadMesh;
        }

        SetBoundMesh(ref mPreviewQuadMesh, mInitWorldPos, mCurrentWorldPos);

        PREVIEW_MPB.SetTexture("_MainTex", mSpellInfo.mStencilTexture);
        mPreviewObject.GetComponent<MeshRenderer>().SetPropertyBlock(PREVIEW_MPB);
    }

    List<Vector3> GetBoundsCorners()
    {
        Vector3 TopLeft = mInitWorldPos;
        Vector3 BotRight = mCurrentWorldPos;
        Vector3 TopRight = new Vector3(BotRight.x, TopLeft.y, TopLeft.z);
        Vector3 BotLeft = new Vector3(TopLeft.x, BotRight.y, BotRight.z);

        ReorderAnchorsClockwise(ref TopLeft, ref TopRight, ref BotLeft, ref BotRight);

        return new List<Vector3>()
        {
            TopLeft, TopRight, BotLeft, BotRight
        };
    }

    void ReorderAnchorsClockwise(ref Vector3 aTopLeft, ref Vector3 aTopRight, ref Vector3 aBotLeft, ref Vector3 aBotRight)
    {
        if(aTopLeft.y > aBotRight.y)
        {
            if(aTopLeft.x > aBotRight.x)
            {
                (aBotLeft, aBotRight) = (aBotRight, aBotLeft);
                (aTopLeft, aTopRight) = (aTopRight, aTopLeft);
            }
        }
        else
        {
            (aTopRight, aBotRight) = (aBotRight, aTopRight);
            (aTopLeft, aBotLeft) = (aBotLeft, aTopLeft);

            if(aTopLeft.x > aBotRight.x)
            {
                (aTopLeft, aTopRight) = (aTopRight, aTopLeft);
                (aBotLeft, aBotRight) = (aBotRight, aBotLeft);
            }
        }
    }

    GameObject CreateMaskGameobject(Mesh aMaskMesh)
    {
        var NewMaskObject = new GameObject("RevealMask");
        NewMaskObject.transform.SetParent(this.transform);

        NewMaskObject.layer = mDrawMaskBufferLayer;

        var MeshFilterComponent = NewMaskObject.AddComponent<MeshFilter>();
        MeshFilterComponent.mesh = aMaskMesh;

        var RendererComponent = NewMaskObject.AddComponent<MeshRenderer>();
        RendererComponent.material = mMaskMaterial;

        var SpellMaskComponent = NewMaskObject.AddComponent<CSpellRevealMask>();

        return NewMaskObject;
    }
}
