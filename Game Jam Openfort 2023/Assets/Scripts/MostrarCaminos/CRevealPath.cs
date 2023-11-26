using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class CRevealPath : MonoBehaviour
{
    const KeyCode mClickKeycode = KeyCode.Mouse0;
    
    [Header("Generation")]
        
    [SerializeField]
    Material mMaskMaterial;

    [SerializeField]
    LayerMask mRevealObjectMask;
    
    [SerializeField]
    LayerMask mFloorMask;
    
    [SerializeField]
    int mDrawMaskBufferLayer;

    [SerializeField]
    ObjectPooler mMaskObjectPooler;

    [Header("Validation")]
    [SerializeField]
    Transform mHUDWindowTopLeftCorner;

    [SerializeField]
    Transform mHUDWindowBotRightCorner;


    [Header("Preview Mask")]

    [SerializeField]
    Material mPreviewMaterial;
    
    [SerializeField]
    Color mPreviewValidColor;

    [SerializeField]
    Color mPreviewInvalidColor;
    
    // Drag positions from camera to world
    CSpellScriptableObject mActiveSpellInfo;
    Vector2 mCornerTLWorldPos;
    Vector2 mCornerBRWorldPos;
    Vector3 mInitShadowWorldPoint;
    Vector3 mCurrentShadowWorldPoint;
    Vector2 mMaskDimensions;
    GameObject mPreviewObject;
    Mesh mPreviewQuadMesh;
    bool mIsValidDrag;
    public Action OnMaskCreated;

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

    public void Start()
    {
        mInitShadowWorldPoint = Vector2.zero;

        mMaskObjectPooler.AddConstructor(CreateMaskGameobject);

        Ray CornerTLRay = Camera.main.ScreenPointToRay(mHUDWindowTopLeftCorner.position);
        Ray CornerBRRay = Camera.main.ScreenPointToRay(mHUDWindowBotRightCorner.position);

        if(Physics.Raycast(CornerTLRay, out RaycastHit HitInfoTL, Mathf.Infinity, mRevealObjectMask))
        {
            mCornerTLWorldPos = HitInfoTL.point;
        }

        if(Physics.Raycast(CornerBRRay, out RaycastHit HitInfoBR, Mathf.Infinity, mRevealObjectMask))
        {
            mCornerBRWorldPos = HitInfoBR.point;
        }
    }

    private void OnEnable() 
    {
        if(mPreviewObject && Input.GetKey(mClickKeycode))
        {
            mPreviewObject.SetActive(true);    
        }
    }

    private void OnDisable()
    {
        if(mPreviewObject)
        {
            mPreviewObject.SetActive(false);    
        }
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
                mInitShadowWorldPoint = HitInfo.point;
                
                UpdatePreviewMesh();
                mPreviewObject.SetActive(true);
            }
            else
            {
                Debug.LogError("There should be an object to mask a reveal path");
            }
        }

        if(Input.GetKey(mClickKeycode))
        {
            Ray DragRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            ValidateDrag();

            if(Physics.Raycast(DragRay, out RaycastHit HitInfo, Mathf.Infinity, mRevealObjectMask))
            {
                mMaskDimensions = new Vector2()
                {
                    x = Mathf.Abs(mInitShadowWorldPoint.x - mCurrentShadowWorldPoint.x) ,
                    y = Mathf.Abs(mInitShadowWorldPoint.y - mCurrentShadowWorldPoint.y)
                };
                
                UpdatePreviewMesh();

                mCurrentShadowWorldPoint = HitInfo.point;
            }
            else
            {
                UpdatePreviewMesh();
            }
        }


        if(Input.GetKeyUp(mClickKeycode))
        {
            ValidateDrag();
            
            if(mIsValidDrag)
            {
                Mesh NewMesh = new Mesh();
                NewMesh.name = "RevealQuad";

                SetBoundMesh(ref NewMesh, mInitShadowWorldPoint, mCurrentShadowWorldPoint);
                
                var NewMaskObject = mMaskObjectPooler.GetPooledObject();
                NewMaskObject.GetComponent<MeshFilter>().mesh = NewMesh;

                NewMaskObject.GetComponent<CSpellRevealMask>().ConfigSpell(mActiveSpellInfo, mMaskDimensions);

                Vector3 LightShadowPos = new Vector3()
                {
                    x = mInitShadowWorldPoint.x < mCurrentShadowWorldPoint.x ? 
                        Mathf.Lerp(mInitShadowWorldPoint.x, mCurrentShadowWorldPoint.x, mActiveSpellInfo.mLightPosition01.x) :
                        Mathf.Lerp(mCurrentShadowWorldPoint.x, mInitShadowWorldPoint.x, mActiveSpellInfo.mLightPosition01.x) ,
                    
                    y = mInitShadowWorldPoint.y < mCurrentShadowWorldPoint.y ? 
                        Mathf.Lerp(mInitShadowWorldPoint.y, mCurrentShadowWorldPoint.y, mActiveSpellInfo.mLightPosition01.y) :
                        Mathf.Lerp(mCurrentShadowWorldPoint.y, mInitShadowWorldPoint.y, mActiveSpellInfo.mLightPosition01.y) ,
                    
                    z = Mathf.Lerp(mInitShadowWorldPoint.z, mCurrentShadowWorldPoint.z, 0.5f)
                };
                
                if(Physics.Raycast(LightShadowPos, LightShadowPos - Camera.main.transform.position, out RaycastHit LightHit, Mathf.Infinity, mFloorMask))
                {
                    float LightSize = (mMaskDimensions.x + mMaskDimensions.y) / 2;

                    NewMaskObject.GetComponent<CSpellRevealMask>().ConfigLight(LightHit.point - Vector3.forward * LightSize / 4.0f, LightSize);
                }
                
                NewMaskObject.SetActive(true);

                OnMaskCreated();
            }

            mPreviewObject.SetActive(false);

            mInitShadowWorldPoint = Vector2.zero;
        }
    }

    void ValidateDrag()
    {
        bool IsInitPosInsideBounds = mInitShadowWorldPoint.x > mCornerTLWorldPos.x && mInitShadowWorldPoint.x < mCornerBRWorldPos.x
                                    && mInitShadowWorldPoint.y < mCornerTLWorldPos.y && mInitShadowWorldPoint.y > mCornerBRWorldPos.y;
        
        bool IsFinalPosInsideBounds = mCurrentShadowWorldPoint.x > mCornerTLWorldPos.x && mCurrentShadowWorldPoint.x < mCornerBRWorldPos.x
                                    && mCurrentShadowWorldPoint.y < mCornerTLWorldPos.y && mCurrentShadowWorldPoint.y > mCornerBRWorldPos.y;

        mIsValidDrag = IsInitPosInsideBounds && IsFinalPosInsideBounds;
    }

    void ClampPositionToTopLeftCorner()
    {
        mCurrentShadowWorldPoint.x = Math.Max(mCurrentShadowWorldPoint.x, mInitShadowWorldPoint.x);
        mCurrentShadowWorldPoint.y = Math.Max(mCurrentShadowWorldPoint.y, mInitShadowWorldPoint.y);
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

        SetBoundMesh(ref mPreviewQuadMesh, mInitShadowWorldPoint, mCurrentShadowWorldPoint);

        PREVIEW_MPB.SetTexture("_MainTex", mActiveSpellInfo.mStencilTexture);
        PREVIEW_MPB.SetFloat("_MaskWidth", mMaskDimensions.x);
        PREVIEW_MPB.SetFloat("_MaskHeight", mMaskDimensions.y);
        PREVIEW_MPB.SetColor("_HighlightColor", mIsValidDrag ? mPreviewValidColor : mPreviewInvalidColor);
        mPreviewObject.GetComponent<MeshRenderer>().SetPropertyBlock(PREVIEW_MPB);
    }

    List<Vector3> GetBoundsCorners()
    {
        Vector3 TopLeft = mInitShadowWorldPoint;
        Vector3 BotRight = mCurrentShadowWorldPoint;
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

    GameObject CreateMaskGameobject()
    {
        var NewMaskObject = new GameObject("RevealMask");
        NewMaskObject.transform.SetParent(this.transform);

        NewMaskObject.layer = mDrawMaskBufferLayer;

        var MeshFilterComponent = NewMaskObject.AddComponent<MeshFilter>();

        var RendererComponent = NewMaskObject.AddComponent<MeshRenderer>();
        RendererComponent.material = mMaskMaterial;

        var SpellMaskComponent = NewMaskObject.AddComponent<CSpellRevealMask>();

        return NewMaskObject;
    }

    public void SetActiveSpell(CSpellScriptableObject aSpell)
    {
        mActiveSpellInfo = aSpell;
    }
}
