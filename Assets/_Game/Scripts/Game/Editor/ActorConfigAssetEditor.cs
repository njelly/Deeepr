using UnityEditor;

namespace Tofunaut.Deeepr.Game
{
    [CustomEditor(typeof(ActorConfigAsset))]
    [CanEditMultipleObjects]
    public class ActorConfigAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ActorConfigAsset asset = target as ActorConfigAsset;
            if(asset.data == null)
            {
                asset.data = new ActorConfig();
            }

            asset.data.moveSpeed = EditorGUILayout.FloatField("Move Speed", asset.data.moveSpeed);
            asset.data.viewPath = EditorGUILayout.TextField("View Path", asset.data.viewPath);
            asset.data.collisionInfo.solidLayer = (Collision.ELayer)EditorGUILayout.EnumFlagsField("Solid Layer", asset.data.collisionInfo.solidLayer);
            asset.data.collisionInfo.collsionLayer = (Collision.ELayer)EditorGUILayout.EnumFlagsField("Collision Layer", asset.data.collisionInfo.collsionLayer);
        }
    }
}