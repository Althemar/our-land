using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

[ExecuteInEditMode]
[AddComponentMenu("Spine/Point Follower UI")]
public class PointFollowerUI : MonoBehaviour {

    [SerializeField] public SkeletonGraphic skeletonRenderer;
    public SkeletonGraphic SkeletonRenderer { get { return this.skeletonRenderer; } }
    public ISkeletonComponent SkeletonComponent { get { return skeletonRenderer as ISkeletonComponent; } }

    [SpineSlot(dataField: "skeletonRenderer", includeNone: true)]
    public string slotName;

    [SpineAttachment(slotField: "slotName", dataField: "skeletonRenderer", fallbackToTextField: true, includeNone: true)]
    public string pointAttachmentName;

    public bool followRotation = true;
    public bool followSkeletonFlip = true;
    public bool followSkeletonZPosition = false;

    Transform skeletonTransform;
    bool skeletonTransformIsParent;
    PointAttachment point;
    Bone bone;
    bool valid;
    public bool IsValid { get { return valid; } }

    public void Initialize() {
        valid = skeletonRenderer != null;
        if (!valid)
            return;

        UpdateReferences();

#if UNITY_EDITOR
        if (Application.isEditor) LateUpdate();
#endif
    }

    void UpdateReferences() {
        skeletonTransform = skeletonRenderer.transform;
        skeletonTransformIsParent = Transform.ReferenceEquals(skeletonTransform, transform.parent);

        bone = null;
        point = null;
        if (!string.IsNullOrEmpty(pointAttachmentName)) {
            var skeleton = skeletonRenderer.Skeleton;

            int slotIndex = skeleton.FindSlotIndex(slotName);
            if (slotIndex >= 0) {
                var slot = skeleton.Slots.Items[slotIndex];
                bone = slot.Bone;
                point = skeleton.GetAttachment(slotIndex, pointAttachmentName) as PointAttachment;
            }
        }
    }

    public void LateUpdate() {
#if UNITY_EDITOR
        if (!Application.isPlaying) skeletonTransformIsParent = Transform.ReferenceEquals(skeletonTransform, transform.parent);
#endif

        if (point == null) {
            if (string.IsNullOrEmpty(pointAttachmentName)) return;
            UpdateReferences();
            if (point == null) return;
        }

        Vector2 worldPos;
        point.ComputeWorldPosition(bone, out worldPos.x, out worldPos.y);
        float rotation = point.ComputeWorldRotation(bone);

        Transform thisTransform = this.transform;
        if (skeletonTransformIsParent) {
            // Recommended setup: Use local transform properties if Spine GameObject is the immediate parent
            thisTransform.localPosition = new Vector3(worldPos.x, worldPos.y, followSkeletonZPosition ? 0f : thisTransform.localPosition.z);
            if (followRotation) {
                float halfRotation = rotation * 0.5f * Mathf.Deg2Rad;

                var q = default(Quaternion);
                q.z = Mathf.Sin(halfRotation);
                q.w = Mathf.Cos(halfRotation);
                thisTransform.localRotation = q;
            }
        }
        else {
            // For special cases: Use transform world properties if transform relationship is complicated
            Vector3 targetWorldPosition = skeletonTransform.TransformPoint(new Vector3(worldPos.x, worldPos.y, 0f));
            if (!followSkeletonZPosition)
                targetWorldPosition.z = thisTransform.position.z;

            Transform transformParent = thisTransform.parent;
            if (transformParent != null) {
                Matrix4x4 m = transformParent.localToWorldMatrix;
                if (m.m00 * m.m11 - m.m01 * m.m10 < 0) // Determinant2D is negative
                    rotation = -rotation;
            }

            if (followRotation) {
                Vector3 transformWorldRotation = skeletonTransform.rotation.eulerAngles;
                thisTransform.SetPositionAndRotation(targetWorldPosition, Quaternion.Euler(transformWorldRotation.x, transformWorldRotation.y, transformWorldRotation.z + rotation));
            }
            else {
                thisTransform.position = targetWorldPosition;
            }
        }

        if (followSkeletonFlip) {
            Vector3 localScale = thisTransform.localScale;
            localScale.y = Mathf.Abs(localScale.y) * Mathf.Sign(bone.Skeleton.ScaleX * bone.Skeleton.ScaleY);
            thisTransform.localScale = localScale;
        }
    }
}
