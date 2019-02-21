using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class SpinePanel : MonoBehaviour {
    private SkeletonGraphic skeleton;
    Slot slot;

    void Awake() {
        skeleton = GetComponent<SkeletonGraphic>();
        slot = skeleton.Skeleton.FindSlot("Slot 1");

        Debug.Log(slot);
        Debug.Log((slot.Attachment as PointAttachment).X);
    }
    
    void Update() {
        Debug.Log(this.transform.worldToLocalMatrix * (slot.Attachment as PointAttachment).GetWorldPosition(skeleton.Skeleton.FindBone("Volet Gauche"), this.transform));
        
    }
}
